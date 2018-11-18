using UnityEngine;

namespace MikuMikuXR.UI.Resource
{
    public class Sprites
    {
        private static Sprite _fileIconModel;
        private static Sprite _fileIconMotion;
        private static Sprite _fileIconMusic;
        private static Sprite _fileIconDirectory;

        public static Sprite FileIconModel
        {
            get { return LoadSprite(ref _fileIconModel, "UI/Images/model_file"); }
        }

        public static Sprite FileIconMotion
        {
            get { return LoadSprite(ref _fileIconMotion, "UI/Images/motion_file"); }
        }

        public static Sprite FileIconMusic
        {
            get { return LoadSprite(ref _fileIconMusic, "UI/Images/music_file"); }
        }
        
        public static Sprite FileIconDirectory
        {
            get { return LoadSprite(ref _fileIconDirectory, "UI/Images/directory"); }
        }

        private static Sprite LoadSprite(ref Sprite sprite, string resourcePath)
        {
            if (sprite != null)
            {
                return sprite;
            }
            sprite = Resources.Load<Sprite>(resourcePath);
            return sprite;
        }
    }
}