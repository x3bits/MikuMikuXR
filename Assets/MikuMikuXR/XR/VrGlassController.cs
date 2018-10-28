using MikuMikuXR.Components;
using MikuMikuXR.GameCompoment;
using MikuMikuXR.SceneController;
using MikuMikuXR.Utils;
using UnityEngine;

namespace MikuMikuXR.XR
{
    public class VrGlassController : IXrController
    {
        private GameObject _gameObject;

        public void Create()
        {
            var gesture = MainSceneController.Instance.GetCameraManualTransform().gameObject.GetComponent<CameraTransformGesture>();
            gesture.EnableRotation = false;
            gesture.EnableTranslation = true;
            Screen.orientation = ScreenOrientation.Landscape;
            var vrCameraObj = new GameObject("VrGlass");
            vrCameraObj.transform.SetParent(MainSceneController.Instance.GetCameraManualTransform(), false);
            UnityUtils.ResetTransform(vrCameraObj.transform);
            AddCamera(vrCameraObj, "LeftEye", new Rect(0.0f, 0.0f, 0.5f, 1), -0.5f, 0.0f);
            AddCamera(vrCameraObj, "RightEye", new Rect(0.5f, 0.0f, 0.5f, 1), 0.5f, 0.0f);
            vrCameraObj.AddComponent<GyroBehaviour>();
            _gameObject = vrCameraObj;
        }

        private static void AddCamera(GameObject vrCameraObj, string name, Rect cameraRect, float xPos, float rotationY)
        {
            var eyeObj = new GameObject(name);
            var camera = eyeObj.AddComponent<Camera>();
            camera.clearFlags = CameraClearFlags.Color;
            camera.backgroundColor = Color.black;
            camera.rect = cameraRect;
            camera.fieldOfView = 60.0f;
            camera.nearClipPlane = 0.3f;
            camera.farClipPlane = 5000.0f;
            eyeObj.transform.SetParent(vrCameraObj.transform, false);
            UnityUtils.ResetTransform(eyeObj.transform);
            eyeObj.transform.localPosition = new Vector3(xPos, 0.0f, 0.0f);
            eyeObj.transform.localRotation = Quaternion.Euler(0.0f, rotationY, 0.0f); 
        }
        
        public bool EnableGesture()
        {
            return false;
        }

        public void Destroy()
        {
            Object.Destroy(_gameObject);
        }

        public new XrType GetType()
        {
            return XrType.VrGlass;
        }
    }
}