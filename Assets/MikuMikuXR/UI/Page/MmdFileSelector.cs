using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MikuMikuXR.SceneController;
using MikuMikuXR.UI.ListItem;
using MikuMikuXR.UI.Resource;
using MikuMikuXR.UserConfig.Path;
using MikuMikuXR.UserConfig.Resource;
using MikuMikuXR.Utils;
using NSUListView;
using TinyTeam.UI;
using UnityEngine;
using UnityEngine.UI;

namespace MikuMikuXR.UI.Page
{
    public class MmdFileSelector : HideOtherPage
    {
        private USimpleListView _fileListView;
        private Text _title;
        private Context _context;
        private Text _scanTipText;
        
        public delegate void FileSelectHandler(string path);

        public class Context
        {
            public string Title { get; set; }
            public FileType Type { get; set; }
            public FileSelectHandler OnFileSelect { get; set; }
        }

        public enum FileType
        {
            Model,
            Motion,
            Music,
            BonePose
        }

        public MmdFileSelector()
        {
            uiPath = PrefabPaths.MmdFileSelectorPath;
        }

        public override void Awake(GameObject go)
        {
            SetButtonListener("Bottom/BtnClose", ClosePage);
            SetButtonListener("Bottom/BtnScan", ShowPage<ScanDialog>);
            _fileListView = FindCompoment<USimpleListView>("Viewport/Content/FileList");
            _title = FindCompoment<Text>("Title");
            _scanTipText = FindCompoment<Text>("Viewport/Content/ScanTip");
        }
        
        public override void Refresh()
        {
            var context = (Context) data;
            if (context != null)
            {
                _context = context;
            }
            if (_context == null)
            {
                throw new ArgumentException("null context");
            }
            _title.text = _context.Title;
            AllResources allResources;
            var listPath = Paths.Getter().ResourceList();
            try
            {
                allResources = ResourceListStore.Instance.Load(listPath);
            }
            catch (Exception e)
            {
                _scanTipText.gameObject.SetActive(true);
                Debug.LogWarning("load allResources exception. " + e);
                _fileListView.SetData(new List<object>());
                return;
            }
            _scanTipText.gameObject.SetActive(false);
            ResourceList resourceList;
            switch (_context.Type)
            {
                case FileType.Model:
                    resourceList = allResources.ModelList;
                    break;
                case FileType.Motion:
                    resourceList = allResources.MotionList;
                    break;
                case FileType.Music:
                    resourceList = allResources.MusicList;
                    break;
                case FileType.BonePose:
                    resourceList = allResources.BonePoseList;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            _fileListView.SetData(ConvertDataForUi(resourceList));
        }

        private List<object> ConvertDataForUi(ResourceList resourceList)
        {
            var list = resourceList.List;
            return list.Select(info => new MmdFileListItemData
                {
                    FileName = info.Title.Replace(' ', '\u00A0'), //防止换行
                    FilePath = info.FilePath.Replace(' ', '\u00A0'),
                    Icon =  GetIconByFileType(_context.Type),
                    OnClick = () =>
                    {
                        ClosePage();
                        _context.OnFileSelect(info.FilePath);
                    }
                })
                .Cast<object>()
                .ToList();
        }

        private static Sprite GetIconByFileType(FileType fileType)
        {
            switch (fileType)
            {
                case FileType.Model:
                    return Sprites.FileIconModel;
                case FileType.Motion:
                    return Sprites.FileIconMotion;
                case FileType.Music:
                    return Sprites.FileIconMusic;
                case FileType.BonePose:
                    return Sprites.FileIconMotion;
                default:
                    throw new ArgumentOutOfRangeException("fileType", fileType, null);
            }
        }
    }
}