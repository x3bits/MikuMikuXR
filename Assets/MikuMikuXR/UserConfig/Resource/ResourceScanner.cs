using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using MikuMikuXR.Components;
using UnityEngine;

namespace MikuMikuXR.UserConfig.Resource
{
    public class ResourceScanner
    {
        public static readonly ResourceScanner Instance = new ResourceScanner();

        public interface IScanListener
        {
            void OnScanFile(string path);
            void OnFinish(AllResources allResources);
        }

        private ResourceScanner()
        {
        }

        public void ScanAsync(string path, IScanListener listener)
        {
            new Thread(() =>
            {
                Scan(path, listener);
            }).Start();
        }

        public AllResources Scan(string path, IScanListener scanListener)
        {
            var ret = new AllResources
            {
                ModelList = new ResourceList {List = new List<ResourceInfo>()},
                MotionList = new ResourceList {List = new List<ResourceInfo>()},
                MusicList = new ResourceList {List = new List<ResourceInfo>()},
                BonePoseList = new ResourceList {List = new List<ResourceInfo>()}
            };
            try
            {
                if (!new DirectoryInfo(path).Exists)
                {
                    return ret;
                }
                DoScan(path, ret, scanListener);
                //ret.ModelList.List.Sort(ResourceComparator.Instance);
                return ret;
            }
            finally
            {
                if (scanListener != null)
                {
                    scanListener.OnFinish(ret);
                }
            }
            
        }

        private class ResourceComparator : IComparer<ResourceInfo>
        {
            public static readonly ResourceComparator Instance = new ResourceComparator();

            private ResourceComparator()
            {
            }

            public int Compare(ResourceInfo x, ResourceInfo y)
            {
                if (x == null)
                {
                    if (y == null)
                    {
                        return 0;
                    }
                    return -1;
                }
                if (y == null)
                {
                    return 1;
                }
                return string.Compare(x.Title, y.Title, StringComparison.Ordinal);
            }
        }

        private static void DoScan(string dir, AllResources allResources, IScanListener scanListener)
        {
            var noMmdPath = dir + System.IO.Path.DirectorySeparatorChar + ".nommdresource";
            if (File.Exists(noMmdPath))
            {
                return;
            }
            foreach (var file in Directory.GetFiles(dir))
            {
                if (scanListener != null)
                {
                    scanListener.OnScanFile(file);
                }
                var ext = System.IO.Path.GetExtension(file);
                if (!string.IsNullOrEmpty(ext))
                {
                    ext = ext.ToLower();
                }
                var fileName = System.IO.Path.GetFileName(file);
                if (".pmd".Equals(ext) || ".pmx".Equals(ext))
                {
                    allResources.ModelList.List.Add(new ResourceInfo {FilePath = file, Title = fileName});
                }
                else if (".vmd".Equals(ext))
                {
                    allResources.MotionList.List.Add(new ResourceInfo {FilePath = file, Title = fileName});
                }
                else if (".mp3".Equals(ext) || ".m4a".Equals(ext) || ".ogg".Equals(ext) || ".wav".Equals(ext))
                {
                    allResources.MusicList.List.Add(new ResourceInfo {FilePath = file, Title = fileName});
                }
                else if (Constants.BonePoseFileExt.Equals(ext))
                {
                    allResources.BonePoseList.List.Add(new ResourceInfo {FilePath = file, Title = fileName});
                }
            }
            foreach (var directory in Directory.GetDirectories(dir))
            {
                if (scanListener != null)
                {
                    scanListener.OnScanFile(directory);
                }
                DoScan(directory, allResources, scanListener);
            }
        }
    }
}