using System.IO;
using UnityEngine;

namespace MikuMikuXR.UserConfig.Path
{
    public abstract class BasePathGetter : IPathGetter
    {
        public abstract string Home();
        
        public string ConfigFolder()
        {
            return Home() + "/xr/config";
        }

        public string ResourceList()
        {
            return ConfigFolder() + "/resource.json";
        }

        public string SceneFolder()
        {
            return Home() + "/xr/scenes";
        }

        public string BonePoseFolder()
        {
            return Home() + "/xr/physics-calculation";
        }
    }
}