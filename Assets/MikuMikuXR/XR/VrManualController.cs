using MikuMikuXR.Components;
using MikuMikuXR.SceneController;
using MikuMikuXR.Utils;
using UnityEngine;

namespace MikuMikuXR.XR
{
    public class VrManualController : IXrController
    {
        private GameObject _cameraObj;
        
        public void Create()
        {
            Screen.orientation = ScreenOrientation.Portrait;
            var cameraObj = new GameObject("ManualVR");
            var camera = cameraObj.AddComponent<Camera>();
            camera.clearFlags = CameraClearFlags.Color;
            camera.backgroundColor = Color.black;
            camera.fieldOfView = 60.0f;
            camera.nearClipPlane = 0.3f;
            camera.farClipPlane = 5000.0f;
            var cameraManualTransform = MainSceneController.Instance.GetCameraManualTransform();
            camera.transform.SetParent(cameraManualTransform, false);
            var gesture = cameraManualTransform.gameObject.GetComponent<CameraTransformGesture>();
            gesture.EnableRotation = true;
            gesture.EnableTranslation = true;
            UnityUtils.ResetTransform(camera.transform);
            _cameraObj = cameraObj;
        }

        public void Destroy()
        {
            Object.Destroy(_cameraObj);
        }

        public new XrType GetType()
        {
            return XrType.VrManual;
        }

        public bool EnableGesture()
        {
            return true;
        }
    }
}