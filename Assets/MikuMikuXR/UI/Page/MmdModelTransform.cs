using System;
using MikuMikuXR.SceneController;
using UnityEngine;

namespace MikuMikuXR.UI.Page
{
    public class MmdModelTransform : HideOtherPage
    {
        private const float PositionMoveFactor = 5.0f;
        private const float RotationFactor = 90.0f;
        private const double ZoomFactor = 2.0;

        private GameObject _positionPanel;
        private GameObject _rotationPanel;
        private GameObject _sizePanel;

        public MmdModelTransform()
        {
            uiPath = PrefabPaths.MmdModelTransformPath;
        }

        public override void Awake(GameObject go)
        {
            _positionPanel = transform.Find("MMDModelPosition").gameObject;
            _rotationPanel = transform.Find("MMDModelRotation").gameObject;
            _sizePanel = transform.Find("MMDModelSize").gameObject;
            SetButtonListener("BtnPosition", () => ShowPanel(_positionPanel));
            SetButtonListener("BtnRotation", () => ShowPanel(_rotationPanel));
            SetButtonListener("BtnSize", () => ShowPanel(_sizePanel));
            SetButtonListener("BtnReset", ResetPosition);
            SetButtonListener("BtnBack", ClosePage);
            InitDeltaSlider("MMDModelPosition/SliderX", value => MoveModel(value, 0));
            InitDeltaSlider("MMDModelPosition/SliderY", value => MoveModel(value, 1));
            InitDeltaSlider("MMDModelPosition/SliderZ", value => MoveModel(value, 2));
            InitDeltaSlider("MMDModelRotation/SliderRX", value => RotateModel(value, 0));
            InitDeltaSlider("MMDModelRotation/SliderRY", value => RotateModel(value, 1));
            InitDeltaSlider("MMDModelRotation/SliderRZ", value => RotateModel(value, 2));
            InitDeltaSlider("MMDModelSize/SliderSize", ZoomModel);
        }

        public override void Active()
        {
            base.Active();
            ShowPanel(_positionPanel);
            MainSceneController.Instance.ShowSelectedMark(MainSceneController.Instance.GetModelCount() > 0);
        }

        private void ShowPanel(GameObject panel)
        {
            _positionPanel.SetActive(false);
            _rotationPanel.SetActive(false);
            _sizePanel.SetActive(false);
            panel.SetActive(true);
        }

        private static void ResetPosition()
        {
            var modelTransform = MainSceneController.Instance.GetCurrentModelTransform();
            if (modelTransform == null)
            {
                return;
            }
            modelTransform.localPosition = Vector3.zero;
            modelTransform.localRotation = Quaternion.identity;
        }

        private static void MoveModel(float delta, int axisIndex)
        {
            var modelTransform = MainSceneController.Instance.GetCurrentModelTransform();
            if (modelTransform == null)
            {
                return;
            }
            var localPosition = modelTransform.localPosition;
            localPosition[axisIndex] += delta * PositionMoveFactor;
            modelTransform.localPosition = localPosition;
        }

        private static void RotateModel(float delta, int axisIndex)
        {
            var modelTransform = MainSceneController.Instance.GetCurrentModelTransform();
            if (modelTransform == null)
            {
                return;
            }
            var localRotation = modelTransform.localRotation;
            var rotation = Vector3.zero;
            rotation[axisIndex] = delta * RotationFactor;
            modelTransform.localRotation = localRotation * Quaternion.Euler(rotation);
        }

        private static void ZoomModel(float delta)
        {
            var modelTransform = MainSceneController.Instance.GetCurrentModelTransform();
            if (modelTransform == null)
            {
                return;
            }
            modelTransform.localScale = modelTransform.localScale * (float)Math.Pow(ZoomFactor, delta);
        }

    }
}