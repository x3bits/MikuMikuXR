using MikuMikuXR.Components;
using MikuMikuXR.GameCompoment;
using MikuMikuXR.SceneController;
using MikuMikuXR.Utils;
using UnityEngine;

namespace MikuMikuXR.XR
{
    public class VrSingleController : IXrController
    {
        private GameObject _gameObject;

        public void Create()
        {
            var gesture = MainSceneController.Instance.GetCameraManualTransform().gameObject.GetComponent<CameraTransformGesture>();
            gesture.EnableRotation = false;
            gesture.EnableTranslation = true;
            Screen.orientation = ScreenOrientation.Portrait;
            var vrCameraObj = new GameObject("VrSingle");
            vrCameraObj.transform.SetParent(MainSceneController.Instance.GetCameraManualTransform(), false);
            UnityUtils.ResetTransform(vrCameraObj.transform);
            vrCameraObj.AddComponent<GyroBehaviour>();
            AddCamera(vrCameraObj);
            _gameObject = vrCameraObj;
        }
        
        private static void AddCamera(GameObject vrCameraObj)
        {
            var camera = vrCameraObj.AddComponent<Camera>();
            camera.clearFlags = CameraClearFlags.Color;
            camera.backgroundColor = Color.black;
            camera.nearClipPlane = 0.3f;
            camera.farClipPlane = 5000.0f;
            camera.fieldOfView = 60.0f;
        }
        
        public void Destroy()
        {
            Object.Destroy(_gameObject);
        }

        public new XrType GetType()
        {
            return XrType.VrSingle;
        }
        
        public bool EnableGesture()
        {
            return false;
        }
    }
}