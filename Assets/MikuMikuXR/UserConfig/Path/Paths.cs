using System;
using System.IO;
using UnityEngine;

namespace MikuMikuXR.UserConfig.Path
{
    public class Paths
    {
        private static readonly IPathGetter PathGetter;

        private static bool _directoryCreated;
        
        static Paths()
        {
            switch (Application.platform)
            {
                case RuntimePlatform.Android:
                    PathGetter = new AndroidPathGetter();
                    break;
                case RuntimePlatform.IPhonePlayer:
                    PathGetter = new IosPathGetter();
                    break;
                case RuntimePlatform.OSXEditor:
                    PathGetter = new MacEditorPathGetter();
                    break;
                default:
                    throw new NotImplementedException("unsupported platform " + Application.platform);
            }
        }

        public static IPathGetter Getter()
        {
            return PathGetter;
        }

        public static string RelativeToHomePath(string path)
        {
            if (System.IO.Path.IsPathRooted(path))
            {
                return path;
            }
            return Getter().Home() + "/" + path;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Init()
        {
            if (_directoryCreated)
            {
                return;
            }
            Directory.CreateDirectory(PathGetter.ConfigFolder());
            _directoryCreated = true;
        }

    }
}