using System;
using MikuMikuXR.SceneController;
using MikuMikuXR.Utils;
using MikuMikuXR.XR;
using UnityEngine;
using UnityEngine.UI;
//using Vuforia;

namespace MikuMikuXR.UI.Page
{
    public class MmdStagePanel : HideOtherPage
    {
        private GameObject _cameraFilePanel;
        private GameObject _arUserDefinedPanel;
        private GameObject _arUserDefinedInitPanel;
        private GameObject _arUserDefinedResetPanel;

        public MmdStagePanel()
        {
            uiPath = PrefabPaths.MmdStagePanelPath;
        }

        public override void Awake(GameObject go)
        {
            _cameraFilePanel = transform.Find("MmdCamera").gameObject;
            _arUserDefinedPanel = transform.Find("ArUserDefined").gameObject;
            _arUserDefinedInitPanel = transform.Find("ArUserDefined/Init").gameObject;
            _arUserDefinedResetPanel = transform.Find("ArUserDefined/Reset").gameObject;
            MainSceneController.Instance.OnXrTypeChanged.AddListener(xrType =>
            {
                _cameraFilePanel.SetActive(xrType == XrType.CameraFile);
                _arUserDefinedPanel.SetActive(xrType == XrType.ArUserDefined);
                if (xrType == XrType.ArUserDefined)
                {
                    _arUserDefinedInitPanel.SetActive(true);
                    _arUserDefinedResetPanel.SetActive(false);
                }
            });

            SetButtonListener("Functions/BtnAddModel", () =>
            {
                ShowPage<MmdFileSelector>(new MmdFileSelector.Context
                {
                    Type = MmdFileSelector.FileType.Model,
                    Title = "添加模型",
                    OnFileSelect = filePath =>
                    {
                        TtuiUtils.RunWithLoadingUI<LoadingDialog>(MainSceneController.Instance, () =>
                        {
                            try
                            {
                                if (!MainSceneController.Instance.AddModel(filePath))
                                {
                                    ShowAddModelFailTip();
                                    return;
                                }
                            }
                            catch (Exception e)
                            {
                                Debug.LogException(e);
                                ShowAddModelFailTip();
                                return;
                            }
                            ShowPage<MmdModelPanel>();
                        });
                    }
                });
                OnceTipPage.ShowOnceTip(TipNames.ExportCustomMmdFiles);
            });
            SetButtonListener("Functions/BtnSelectModel", () =>
            {
                if (MainSceneController.Instance.GetModelCount() > 0)
                {
                    ShowPage<MmdModelSelectPanel>();
                }
            });
            SetButtonListener("Functions/BtnSelectMusic", () =>
            {
                ShowPage<MmdFileSelector>(new MmdFileSelector.Context
                {
                    Type = MmdFileSelector.FileType.Music,
                    Title = "选择音乐",
                    OnFileSelect = filePath =>
                    {
                        TtuiUtils.RunWithLoadingUI<LoadingDialog>(MainSceneController.Instance,
                            () =>
                            {
                                try
                                {
                                    MainSceneController.Instance.ChangeMusic(filePath);
                                }
                                catch (Exception e)
                                {
                                    Debug.LogException(e);
                                }
                            });
                    }
                });
                OnceTipPage.ShowOnceTip(TipNames.ExportCustomMmdFiles);
            });
            SetButtonListener("Functions/BtnPlay", () =>
            {
                if (MainSceneController.Instance.GetXrType().Equals(XrType.VrGlass))
                {
                    ShowPage<VrCountdownPage>();
                }
                else
                {
                    MainSceneController.Instance.SwitchPlayPause(true);
                }
            });
            SetButtonListener("Functions/BtnXR", ShowPage<XrSelecror>);
            SetButtonListener("Bottom/BtnQuit", ShowPage<ConfirmReturnTitlePage>);
            SetButtonListener("Bottom/BtnSave", ShowPage<SaveSceneDialog>);
            SetButtonListener("Bottom/BtnLoad", ShowPage<LoadSceneDilog>);
            SetButtonListener("MmdCamera/BtnSelectCamera", () =>
            {
                ShowPage<MmdFileSelector>(new MmdFileSelector.Context
                {
                    Type = MmdFileSelector.FileType.Motion,
                    Title = "相机数据",
                    OnFileSelect = filePath =>
                    {
                        TtuiUtils.RunWithLoadingUI<LoadingDialog>(MainSceneController.Instance, () =>
                        {
                            try
                            {
                                var xrController = MainSceneController.Instance.GetXrController();
                                var cameraFileController = xrController as CameraFileController;
                                if (cameraFileController == null)
                                {
                                    return;
                                }
                                MainSceneController.Instance.SwitchPlayPause(false);
                                MainSceneController.Instance.ResetAll();
                                MainSceneController.Instance.CameraFilePath = filePath;
                                if (!cameraFileController.CameraObject.LoadCameraMotion(filePath))
                                {
                                    TtuiUtils.ShowPageAfterLoadingUI<OkDialog>(MainSceneController.Instance,
                                        new OkDialog.Context
                                        {
                                            Tip = "载入的文件中不含镜头数据。",
                                            Title = "提示"
                                        });
                                }
                            }
                            catch (Exception e)
                            {
                                Debug.LogException(e);
                                TtuiUtils.ShowPageAfterLoadingUI<OkDialog>(MainSceneController.Instance,
                                    new OkDialog.Context
                                    {
                                        Tip = "载入镜头数据失败。",
                                        Title = "提示"
                                    });
                            }
                        });
                    }
                });
                OnceTipPage.ShowOnceTip(TipNames.ExportCustomMmdFiles);
            });
            SetButtonListener("ArUserDefined/Init/BtnStartAR", () =>
            {
                var arController = MainSceneController.Instance.GetXrController() as ArUserDefinedController;
                if (arController == null) return;
                _arUserDefinedInitPanel.SetActive(false);
                _arUserDefinedResetPanel.SetActive(true);
            });
            SetButtonListener("ArUserDefined/Reset/BtnResetAR", () =>
            {
                var arController = MainSceneController.Instance.GetXrController() as ArUserDefinedController;
                if (arController == null) return;
                _arUserDefinedInitPanel.SetActive(true);
                _arUserDefinedResetPanel.SetActive(false);
            });
        }

        private static void ShowAddModelFailTip()
        {
            TtuiUtils.ShowPageAfterLoadingUI<OkDialog>(MainSceneController.Instance,
                new OkDialog.Context
                {
                    Title = "提示",
                    Tip = "载入模型失败。请确认模型存在且为正确的MikuMikuDance模型。"
                });
        }

        public override void Active()
        {
            base.Active();
            var mainSceneController = MainSceneController.Instance;
            if (mainSceneController == null)
            {
                return;
            }
            mainSceneController.ShowSelectedMark(false);
        }
    }
}