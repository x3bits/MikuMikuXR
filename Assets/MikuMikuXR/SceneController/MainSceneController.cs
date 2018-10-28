using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using LibMMD.Unity3D;
using MikuMikuXR.Components;
using MikuMikuXR.GameCompoment;
using MikuMikuXR.UI.Page;
using MikuMikuXR.UserConfig;
using MikuMikuXR.UserConfig.Path;
using MikuMikuXR.UserConfig.Scene;
using MikuMikuXR.Utils;
using MikuMikuXR.XR;
using Newtonsoft.Json;
using TinyTeam.UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Vuforia;
using Object = UnityEngine.Object;

namespace MikuMikuXR.SceneController
{
    public class MainSceneController : MonoBehaviour
    {
        public class XrTypeChangeEvent : UnityEvent<XrType>
        {
        }

        public class PlayPauseEvent : UnityEvent<bool>
        {
        }

        public class ResetAllEvent : UnityEvent
        {
        }

        public class ArFrameQualityChangedEvent : UnityEvent<ImageTargetBuilder.FrameQuality>
        {
        }

        public DelayInactive DotMark;

        public readonly XrTypeChangeEvent OnXrTypeChanged = new XrTypeChangeEvent();

        public readonly PlayPauseEvent OnPlayPause = new PlayPauseEvent();

        public readonly ResetAllEvent OnResetAll = new ResetAllEvent();

        public readonly ArFrameQualityChangedEvent OnArFrameQualityChanged = new ArFrameQualityChangedEvent();

        public string CameraFilePath { get; set; }

        private readonly IList<GameObject> _mmdObjects = new List<GameObject>();

        private int _selectedMmdIndex = 0;

        private AudioSource _audioSource;

        private AudioClip _audioClip;

        private bool _playing;

        private string _musicPath;

        private GameObject _pyramidCamerasRoot;

        private GameObject _currentModelMark;

        private GameObject _groundGrid;

        private GameObject _cameraManualPos;

        private Dictionary<XrType, IXrController> _xrControllers;

        private IXrController _currentXrController;

        public static MainSceneController Instance;

        private GameObject GetCurrentControlledMmd()
        {
            return _mmdObjects.Count <= 0 ? null : _mmdObjects[_selectedMmdIndex];
        }

        private void Start()
        {
            Instance = this;
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            InitScene();
            TTUIPage.ShowPage<MmdStagePanel>();
            OnceTipPage.ShowOnceTip(TipNames.HowToStart);
            InitUiLayer();
            InitXrTypes();
            ChangeXrType(XrType.VrManual);
        }

        private void OnDestroy()
        {
            TTUIPage.ClearNodes();
        }

        private static void InitUiLayer()
        {
            var uiCam = GameObject.Find("UICamera").GetComponent<Camera>();
            uiCam.depth = 5.0f;
            uiCam.allowMSAA = false;
            var canvasScaler = GameObject.Find("UIRoot").GetComponent<CanvasScaler>();
            canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            canvasScaler.matchWidthOrHeight = 100.0f;
            canvasScaler.referenceResolution = new Vector2(720.0f, 1280.0f);
        }

        private void InitScene()
        {
            _audioSource = gameObject.AddComponent(typeof(AudioSource)) as AudioSource;
            var modelMark = Resources.Load<GameObject>("Model/CurrentModelMark");
            _currentModelMark = Instantiate(modelMark);
            _currentModelMark.SetActive(false);
            var groundGrid = Resources.Load<GameObject>("Model/GroundGrid");
            _groundGrid = Instantiate(groundGrid);
            _groundGrid.SetActive(false);
            _cameraManualPos = new GameObject("CameraManualPos");
            _cameraManualPos.transform.localPosition = new Vector3(0, 18, -40);
            _cameraManualPos.AddComponent<CameraTransformGesture>();
        }

        private void RefreshCurrentModelMarkParant()
        {
            if (_mmdObjects.Count <= 0)
            {
                return;
            }
            _currentModelMark.transform.SetParent(_mmdObjects[_selectedMmdIndex].transform, false);
            _currentModelMark.transform.localPosition = Vector3.zero;
        }

        private void SetSelectedModelIndex(int index)
        {
            _selectedMmdIndex = index;
            RefreshCurrentModelMarkParant();
        }

        public Transform GetCameraManualTransform()
        {
            return _cameraManualPos.transform;
        }

        public void ShowSelectedMark(bool show)
        {
            _currentModelMark.SetActive(show);
            _groundGrid.SetActive(show);
        }

        public bool AddModel(string path)
        {
            var added = MmdGameObject.CreateGameObject();
            var mmdGameObject = added.GetComponent<MmdGameObject>();
            mmdGameObject.PhysicsFps = PlayerPrefs.GetInt(UserConfigs.PhysicsFps, 120);
            mmdGameObject.OnMmdEvent = mmdEvent =>
            {
                if (mmdEvent == MmdGameObject.MmdEvent.SlowPoseCalculation && DotMark != null
                    && PlayerPrefs.GetInt(UserConfigs.ShowSlowPhysics) != 0)
                {
                    DotMark.Show();
                }
            };
            var config = mmdGameObject.GetConfig();
            config.EnableDrawSelfShadow = PlayerPrefs.GetInt(UserConfigs.DrawShadow, 1) == 0
                ? MmdConfigSwitch.ForceFalse
                : MmdConfigSwitch.AsConfig;
            config.EnableEdge = MmdConfigSwitch.ForceFalse;
            mmdGameObject.UpdateConfig(config);
            if (!added.GetComponent<MmdGameObject>().LoadModel(path))
            {
                Destroy(added);
                return false;
            }
            _mmdObjects.Add(added);
            ResetAll();
            SetSelectedModelIndex(_mmdObjects.Count - 1);
            return true;
        }

        public void ChangeCurrentModel(string path)
        {
            _currentModelMark.transform.SetParent(null, false); //否则重新Load模型时，它会被Destroy
            _mmdObjects[_selectedMmdIndex].GetComponent<MmdGameObject>().LoadModel(path);
            ResetAll();
            RefreshCurrentModelMarkParant();
        }

        public int GetModelCount()
        {
            return _mmdObjects.Count;
        }

        public void ChangeMusic(string path)
        {
            StartCoroutine(LoadMusic(path));
        }

        public string GetCurrentModelPath()
        {
            return _mmdObjects.Count == 0
                ? null
                : _mmdObjects[_selectedMmdIndex].GetComponent<MmdGameObject>().ModelPath;
        }

        public string GetCurrentMotionPath()
        {
            return _mmdObjects.Count == 0
                ? null
                : _mmdObjects[_selectedMmdIndex].GetComponent<MmdGameObject>().MotionPath;
        }

        public void ChangeCurrentMotion(string path)
        {
            var currentControlledMmd = GetCurrentControlledMmd();
            if (currentControlledMmd == null)
            {
                return;
            }
            var mmdGameObject = currentControlledMmd.GetComponent<MmdGameObject>();
            mmdGameObject.LoadMotion(path);
            ResetAll();
        }

        public void ChangeCurrentBonePoseFile(string path)
        {
            var currentControlledMmd = GetCurrentControlledMmd();
            if (currentControlledMmd == null)
            {
                return;
            }
            var mmdGameObject = currentControlledMmd.GetComponent<MmdGameObject>();
            mmdGameObject.LoadBonePoseFile(path);
            ResetAll();
        }

        public void DeleteCurrentModel()
        {
            if (_mmdObjects.Count <= 0)
            {
                return;
            }
            _currentModelMark.transform.SetParent(null, false);
            Destroy(_mmdObjects[_selectedMmdIndex]);
            _mmdObjects.RemoveAt(_selectedMmdIndex);
            SetSelectedModelIndex(_selectedMmdIndex > _mmdObjects.Count - 1 ? 0 : _selectedMmdIndex);
        }

        public void SwitchPlayPause(bool play)
        {
            if (_playing == play)
            {
                return;
            }
            _playing = play;
            if (_playing)
            {
                _audioSource.Play();
            }
            else
            {
                _audioSource.Pause();
            }
            foreach (var obj in _mmdObjects)
            {
                var mmdGameObject = obj.GetComponent<MmdGameObject>();
                mmdGameObject.Playing = _playing;
            }
            if (_playing)
            {
                if (_currentXrController.EnableGesture())
                {
                    TTUIPage.ShowPage<PlayingButtonPage>();
                }
                else
                {
                    TTUIPage.ShowPage<PlayingBlankPage>();
                }
            }
            OnPlayPause.Invoke(_playing);
        }

        public IList<string> GetModelNames()
        {
            return _mmdObjects.Select(mmdObject => mmdObject.GetComponent<MmdGameObject>().ModelName).ToList();
        }

        public void SelectModel(int index)
        {
            var modelCount = GetModelCount();
            if (modelCount <= 0)
            {
                return;
            }
            SetSelectedModelIndex(index);
        }

        public Transform GetCurrentModelTransform()
        {
            return _mmdObjects.Count <= 0 ? null : _mmdObjects[_selectedMmdIndex].transform;
        }

        public void ChangeXrType(XrType xrType)
        {
            if (_currentXrController != null)
            {
                if (xrType.Equals(_currentXrController.GetType()))
                {
                    return;
                }
                _currentXrController.Destroy();
            }
            _currentXrController = _xrControllers[xrType];
            _currentXrController.Create();
            OnXrTypeChanged.Invoke(xrType);
        }

        public XrType GetXrType()
        {
            return _currentXrController.GetType();
        }

        public IXrController GetXrController()
        {
            return _currentXrController;
        }

        public void SaveScene(string sceneName)
        {
            var sceneMeta = GetSceneMeta();
            SaveSceneMeta(sceneName, sceneMeta);
        }

        public static void SaveSceneMeta(string sceneName, SceneMeta sceneMeta)
        {
            Directory.CreateDirectory(Paths.Getter().SceneFolder());
            var sceneMetaSerialize = JsonConvert.SerializeObject(sceneMeta);
            File.WriteAllText(GetSceneFilePath(sceneName), sceneMetaSerialize, Encoding.UTF8);
        }

        private static string GetSceneFilePath(string name)
        {
            return Paths.Getter().SceneFolder() + "/" + name + ".mxrs";
        }

        public void LoadScene(string fileName)
        {
            var sceneMetaSerialize = File.ReadAllText(Paths.Getter().SceneFolder() + "/" + fileName + ".mxrs");
            var sceneMeta = JsonConvert.DeserializeObject<SceneMeta>(sceneMetaSerialize);
            ReloadScene(sceneMeta);
        }

        public IList<string> ListScenes()
        {
            FileInfo[] files;
            try
            {
                files = new DirectoryInfo(Paths.Getter().SceneFolder()).GetFiles();
            }
            catch (DirectoryNotFoundException e)
            {
                Debug.LogException(e);
                return new List<string>();
            }
            var ret = new List<string>();
            foreach (var file in files)
            {
                var fileName = file.Name;
                if (!fileName.ToLower().EndsWith(".mxrs"))
                {
                    continue;
                }
                ret.Add(fileName.Substring(0, fileName.Length - 5));
            }
            ret.Sort();
            return ret;
        }

        public void DeleteScene(string sceneName)
        {
            var filePath = GetSceneFilePath(sceneName);
            File.Delete(filePath);
        }

        public bool IsSceneExist(string sceneName)
        {
            var filePath = GetSceneFilePath(sceneName);
            return File.Exists(filePath);
        }

        public void ResetAll()
        {
            foreach (var obj in _mmdObjects)
            {
                var mmdGameObject = obj.GetComponent<MmdGameObject>();
                mmdGameObject.ResetMotion();
                mmdGameObject.Playing = false;
            }
            _audioSource.Pause();
            _audioSource.time = 0.0f;
            OnResetAll.Invoke();
        }

        private IEnumerator LoadMusic(string path)
        {
            _musicPath = path;
            var www = new WWW("file://" + path);
            yield return www;
            if (_audioClip != null)
            {
                Destroy(_audioClip);
            }
            _audioClip = www.GetAudioClip(true, true);
            _audioSource.clip = _audioClip;
            ResetAll();
        }

        private void InitXrTypes()
        {
            var typeDic = new Dictionary<XrType, IXrController>();
            IXrController xrController = new VrManualController();
            typeDic[xrController.GetType()] = xrController;
            xrController = new VrGlassController();
            typeDic[xrController.GetType()] = xrController;
            xrController = new VrSingleController();
            typeDic[xrController.GetType()] = xrController;
            xrController = new PyramidController();
            typeDic[xrController.GetType()] = xrController;
            xrController = new CameraFileController();
            typeDic[xrController.GetType()] = xrController;
            xrController = new ArUserDefinedController();
            typeDic[xrController.GetType()] = xrController;
            _xrControllers = typeDic;
        }

        private SceneMeta GetSceneMeta()
        {
            var ret = new SceneMeta {MusicPath = _musicPath, CameraPath = CameraFilePath};
            var objectMetas = (from mmdObject in _mmdObjects
                let mmdGameObject = mmdObject.GetComponent<MmdGameObject>()
                select new MmdObjectMeta
                {
                    ModelPath = mmdGameObject.ModelPath,
                    MotionPath = mmdGameObject.MotionPath,
                    BonePosePath = mmdGameObject.BonePoseFilePath,
                    Position = UnityUtils.Vector3ToList(mmdObject.transform.localPosition),
                    Rotation = UnityUtils.QuaternionToList(mmdObject.transform.localRotation)
                }).ToList();
            ret.MmdObjects = objectMetas;
            return ret;
        }

        private void ReloadScene(SceneMeta meta)
        {
            ClearScene();
            LoadScene(meta);
        }

        private void ClearScene()
        {
            ClearModels();
            ClearAudioClip();
            CameraFilePath = null;
        }

        private void ClearAudioClip()
        {
            if (_audioClip != null)
            {
                Destroy(_audioClip);
            }
            _audioSource.clip = null;
        }

        private void ClearModels()
        {
            var modelCount = _mmdObjects.Count;
            for (var i = 0; i < modelCount; i++)
            {
                DeleteCurrentModel();
            }
        }

        private void LoadScene(SceneMeta meta)
        {
            foreach (var mmdObjectMeta in meta.MmdObjects)
            {
                if (string.IsNullOrEmpty(mmdObjectMeta.ModelPath))
                {
                    continue;
                }
                try
                {
                    AddModel(Paths.RelativeToHomePath(mmdObjectMeta.ModelPath));
                    var modelTransform = GetCurrentModelTransform();
                    modelTransform.localPosition = UnityUtils.ListToVector3(mmdObjectMeta.Position);
                    modelTransform.localRotation = UnityUtils.ListToQuaternion(mmdObjectMeta.Rotation);
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                    continue;
                }
                if (!string.IsNullOrEmpty(mmdObjectMeta.MotionPath))
                {
                    try
                    {
                        ChangeCurrentMotion(Paths.RelativeToHomePath(mmdObjectMeta.MotionPath));
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);
                    }
                }
                if (!string.IsNullOrEmpty(mmdObjectMeta.BonePosePath))
                {
                    try
                    {
                        ChangeCurrentBonePoseFile(Paths.RelativeToHomePath(mmdObjectMeta.BonePosePath));
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);
                    }
                }
            }
            if (!string.IsNullOrEmpty(meta.MusicPath))
            {
                ChangeMusic(Paths.RelativeToHomePath(meta.MusicPath));
            }
            CameraFilePath = meta.CameraPath;
            if (_currentXrController.GetType() == XrType.CameraFile)
            {
                var cameraFileController = _currentXrController as CameraFileController;
                if (cameraFileController != null)
                {
                    cameraFileController.CameraObject.LoadCameraMotion(Paths.RelativeToHomePath(CameraFilePath));
                }
            }
        }
    }
}