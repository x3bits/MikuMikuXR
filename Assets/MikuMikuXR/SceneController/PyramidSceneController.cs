using System.Collections;
using LibMMD.Unity3D;
using MikuMikuXR.Components;
using MikuMikuXR.UI.Page;
using MikuMikuXR.Utils;
using TinyTeam.UI;
using UnityEngine;
using UnityEngine.Events;

namespace MikuMikuXR.SceneController
{
    public class PyramidSceneController : MonoBehaviour
    {
        private const float PyramidCameraSize = 12.0f;

        private GameObject currentControlledMmd;

        private AudioSource _audioSource;

        private AudioClip _audioClip;

        private bool _playing;

        private TouchEventDetector _touchEventDetector;

        private GameObject _pyramidCamerasRoot;

        public static PyramidSceneController Instance;

        private void Start()
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            InitScene();
            TTUIPage.ShowPage<MmdStagePanel>();
            Instance = this;
            InitUiLayer();
        }

        private void InitUiLayer()
        {
            var uiCam = GameObject.Find("UICamera").GetComponent<Camera>();
            uiCam.depth = 5.0f;
            uiCam.allowMSAA = false;
        }

        private void InitScene()
        {
            CreatePyramidCameras();
            currentControlledMmd = MmdGameObject.CreateGameObject();
            _audioSource = gameObject.AddComponent(typeof(AudioSource)) as AudioSource;
            InitTouchScreenEvent();
        }

        private void InitTouchScreenEvent()
        {
            var touchDetector = gameObject.AddComponent(typeof(TouchEventDetector)) as TouchEventDetector;
            if (touchDetector == null) return;
            touchDetector.Enabled = false;
            touchDetector.OnShortClick = new UnityEvent();
            touchDetector.OnShortClick.AddListener(() =>
            {
                SwitchPlayPause();
            });
            _touchEventDetector = touchDetector;
        }

        public void ChangeMusic(string path)
        {
            StartCoroutine(LoadMusic(path));
        }

        public void ChangeCurrentModel(string path)
        {
            currentControlledMmd.GetComponent<MmdGameObject>().LoadModel(path);
            ResetAll();
        }

        public void ChangeCurrentMotion(string path)
        {
            var mmdGameObject = currentControlledMmd.GetComponent<MmdGameObject>();
            mmdGameObject.LoadMotion(path);
            ResetAll();
        }

        public bool SwitchPlayPause()
        {
            _playing = !_playing;
            var mmdGameObject = currentControlledMmd.GetComponent<MmdGameObject>();
            if (_playing)
            {
                mmdGameObject.Playing = true;
                _audioSource.Play();
                TTUIPage.ClosePage<MmdStagePanel>();
                UnityUtils.DelayCall(this, 0.5f, () => { _touchEventDetector.Enabled = true; });
            }
            else
            {
                mmdGameObject.Playing = false;
                _audioSource.Pause();
                TTUIPage.ShowPage<MmdStagePanel>();
                _touchEventDetector.Enabled = false;
            }

            return _playing;
        }

        private void ResetAll()
        {
            var mmdGameObject = currentControlledMmd.GetComponent<MmdGameObject>();
            mmdGameObject.ResetMotion();
            mmdGameObject.Playing = false;
            _audioSource.Pause();
            _audioSource.time = 0.0f;
        }

        private IEnumerator LoadMusic(string path)
        {
            WWW www = new WWW("file://" + path);
            yield return www;
            if (_audioClip != null)
            {
                Destroy(_audioClip);
            }
            _audioClip = www.GetAudioClip(true, true);
            _audioSource.clip = _audioClip;
            ResetAll();
        }

        private void CreatePyramidCameras()
        {
            var pyramidCameraContainer = new GameObject("PyramidCamerasRoot");
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

        private void ScaleCamera(Camera camera, Vector3 scale)
        {
            var mat = camera.projectionMatrix;
            mat *= Matrix4x4.Scale(scale);
            camera.projectionMatrix = mat;
        }

        private void InitPyramidCameraPos(GameObject[] cameras)
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
        
    }
}