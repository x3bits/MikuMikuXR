using System;
using UnityEngine;
//using Vuforia;
using Object = UnityEngine.Object;

namespace MikuMikuXR.XR
{
    public class ArUserDefinedController : IXrController
    {
        private Object _vuforiaPrefab;
        private GameObject _vuforiaObject;
        
        private GameObject _arCameraObject;
        private GameObject _targetBuilderObject;

        public ArUserDefinedController()
        {
            Tracking = false;
        }

        public bool Tracking { get; private set; }

        public void Create()
        {
            Screen.orientation = ScreenOrientation.Landscape;
            if (_vuforiaPrefab == null)
            {
                _vuforiaPrefab = Resources.Load("Prefabs/ArUserDefined");
            }
            _vuforiaObject =  Object.Instantiate(_vuforiaPrefab) as GameObject;
            if (_vuforiaObject == null)
            {
                throw new InvalidOperationException("[Bug] vuforia object is null");
            }
            _vuforiaObject.name = "ArUserDefined";
        }

        public void Destroy()
        {
            Object.Destroy(_vuforiaObject);
        }

        public XrType GetType()
        {
            return XrType.ArUserDefined;
        }

        public bool EnableGesture()
        {
            return false;
        }
    }
}