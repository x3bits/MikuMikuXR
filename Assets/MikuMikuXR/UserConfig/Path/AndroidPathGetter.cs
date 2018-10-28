using UnityEngine;

namespace MikuMikuXR.UserConfig.Path
{
    public class AndroidPathGetter:BasePathGetter
    {
        public override string Home()
        {
            return AndroidSdPath + "/MikuMikuAR";
        }
        
        private static string _androidSdPath = null;
		
        private static string AndroidSdPath {
            get {
                if (_androidSdPath != null) {
                    return _androidSdPath;
                }
                var utilClass = new AndroidJavaClass("com.x3bits.mikumikuar.common.Utils");
                _androidSdPath = utilClass.CallStatic<string> ("getSdPath");
                return _androidSdPath;
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Init() //防止非UI线程调用AndroidSdPath导致报错
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                Debug.Log("Android SD Path: " + AndroidSdPath);
            }
        }

    }
}