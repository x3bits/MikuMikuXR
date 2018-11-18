using System;
using LibMMD.Unity3D;
using MikuMikuXR.SceneController;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;

namespace MikuMikuXR.XR
{
    public class CameraFileController : IXrController
    {
        private UnityAction<bool> _playPauseListener;

        private UnityAction _resetAllListener;

        public void Create()
        {
            Screen.orientation = ScreenOrientation.Landscape;
            _playPauseListener = playing =>
            {
                CameraObject.Playing = playing;
            };
            _resetAllListener = () =>
            {
                CameraObject.Playing = false;
                CameraObject.SetPlayPos(0.0);
            };
            CameraObject = MmdCameraObject.CreateGameObject().GetComponent<MmdCameraObject>();
            CameraObject.transform.localPosition = new Vector3(0, 18, -40);
            CameraObject.transform.localRotation = Quaternion.identity;
            MainSceneController.Instance.SwitchPlayPause(false);
            MainSceneController.Instance.OnPlayPause.AddListener(_playPauseListener);
            MainSceneController.Instance.ResetAll();
            MainSceneController.Instance.OnResetAll.AddListener(_resetAllListener);
            try
            {
                CameraObject.LoadCameraMotion(MainSceneController.Instance.CameraFilePath);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        public void Destroy()
        {
            MainSceneController.Instance.OnPlayPause.RemoveListener(_playPauseListener);
            MainSceneController.Instance.OnResetAll.RemoveListener(_resetAllListener);
            Object.Destroy(CameraObject.gameObject);
        }

        public XrType GetType()
        {
            return XrType.CameraFile;
        }

        public bool EnableGesture()
        {
            return false;
        }

        public MmdCameraObject CameraObject { get; private set; }
    }
}