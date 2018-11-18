using System.Collections.Generic;

namespace MikuMikuXR.Components
{
    public class Constants
    {
        public const string BonePoseFileExt = ".vbp";
        public static readonly HashSet<string> ModelExts = new HashSet<string> {".pmd", ".pmx"};
        public static readonly HashSet<string> MotionExts = new HashSet<string> {".vmd"};
        public static readonly HashSet<string> MusicExts = new HashSet<string> {".mp3", ".m4a", ".ogg", ".wav"};
        public static readonly HashSet<string> BonePoseExts = new HashSet<string> {BonePoseFileExt};
    }
}