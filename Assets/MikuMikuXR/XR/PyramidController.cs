using System.Collections;
using MikuMikuXR.Components;
using MikuMikuXR.SceneController;
using UnityEngine;

namespace MikuMikuXR.XR
{
    public class PyramidController : IXrController
    {
        private const float PyramidCameraSize = 12.0f;

        private GameObject _pyramidCamerasRoot;

        private bool _active = false;

        public void Create()
        {
            _active = true;
            if (Screen.orientation != ScreenOrientation.Portrait)
            {
                Screen.orientation = ScreenOrientation.Portrait;
                MainSceneController.Instance.StartCoroutine(WaitPortraitScreenAndCreatePyramidCameras());
            }
            else
            {
                Screen.orientation = ScreenOrientation.Portrait;
                CreatePyramidCameras();
            }
        }

        public void Destroy()
        {
            _active = false;
            Object.Destroy(_pyramidCamerasRoot);
        }

        public new XrType GetType()
        {
            return XrType.Pyramid;
        }

        public bool EnableGesture()
        {
            return false;
        }

        private IEnumerator WaitPortraitScreenAndCreatePyramidCameras()
        {
            while (Screen.width > Screen.height && _active)
            {
                yield return null;
            }
            CreatePyramidCameras();
        }

        private void CreatePyramidCameras()
        {
            var pyramidCameraContainer = new GameObject("PyramidCamerasRoot");
            var backgroundCamera = pyramidCameraContainer.AddComponent<Camera>();
            backgroundCamera.clearFlags = CameraClearFlags.Color;
            backgroundCamera.backgroundColor = Color.black;
            backgroundCamera.cullingMask = 0;
            var cameras = new GameObject[4];
            for (var i = 0; i < 4; i++)
            {
                var cameraObj = new GameObject("PyramidCamera" + i);
                var cam = cameraObj.AddComponent(typeof(Camera)) as Camera;
                cameraObj.transform.SetParent(pyramidCameraContainer.transform, true);
                cameras[i] = cameraObj;
                if (cam == null) continue;
                cam.depth = i + 1;
                cam.orthographic = true;
                cam.orthographicSize = PyramidCameraSize;
                cam.nearClipPlane = -PyramidCameraSize;
                cam.farClipPlane = PyramidCameraSize;
                cam.clearFlags = CameraClearFlags.SolidColor;
                cam.backgroundColor = Color.black;
                cam.gameObject.transform.position = new Vector3(0.0f, PyramidCameraSize, 0.0f);
                cam.gameObject.AddComponent<CameraInvertCulling>();
            }
            InitPyramidCameraPos(cameras);
            _pyramidCamerasRoot = pyramidCameraContainer;
        }

        private static void InitPyramidCameraPos(GameObject[] cameras)
        {
            var sw = Screen.width;
            var sh = Screen.height;
            float vw, vh;
            var camLeft = cameras[0].GetComponent<Camera>();
            var camRight = cameras[1].GetComponent<Camera>();
            var camTop = cameras[3].GetComponent<Camera>();
            var camBottom = cameras[2].GetComponent<Camera>();

            if (sw < sh)
            {
                vw = 1.0f / 3;
                vh = vw / sh * sw;
                camLeft.rect = new Rect(0.0f, 0.5f - vh / 2, vw, vh);
                camRight.rect = new Rect(1.0f - vw, 0.5f - vh / 2, vw, vh);
                camTop.rect = new Rect(0.5f - vw / 2, 0.5f + vh * 0.5f, vw, vh);
                camBottom.rect = new Rect(0.5f - vw / 2, 0.5f - vh * 1.5f, vw, vh);
            }
            else
            {
                vh = 1.0f / 3;
                vw = vh / sw * sh;
                camTop.rect = new Rect(0.5f - vw / 2, 0.0f, vw, vh);
                camBottom.rect = new Rect(0.5f - vw / 2, 1.0f - vh, vw, vh);
                camRight.rect = new Rect(0.5f + vw * 0.5f, 0.5f - vh / 2, vw, vh);
                camLeft.rect = new Rect(0.5f - vw * 1.5f, 0.5f - vh / 2, vw, vh);
            }

            camLeft.gameObject.transform.eulerAngles = new Vector3(0.0f, 0.0f, 90.0f);
            camRight.gameObject.transform.eulerAngles = new Vector3(0.0f, 180.0f, -90.0f);
            camTop.gameObject.transform.eulerAngles = new Vector3(0.0f, -90.0f, 0.0f);
            camBottom.gameObject.transform.eulerAngles = new Vector3(180.0f, -90.0f, 0.0f);
            ScaleCamera(camLeft, new Vector3(1, -1, 1));
            ScaleCamera(camRight, new Vector3(1, -1, 1));
            ScaleCamera(camTop, new Vector3(1, -1, 1));
            ScaleCamera(camBottom, new Vector3(1, -1, 1));
        }

        private static void ScaleCamera(Camera camera, Vector3 scale)
        {
            var mat = camera.projectionMatrix;
            mat *= Matrix4x4.Scale(scale);
            camera.projectionMatrix = mat;
        }
    }
}