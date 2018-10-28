using System;
using System.IO;
using LibMMD.Unity3D;
using MikuMikuXR.SceneController;
using MikuMikuXR.Utils;
using UnityEngine;

namespace MikuMikuXR.UI.Page
{
    public class MmdModelPanel : HideOtherPage
    {
        public MmdModelPanel()
        {
            uiPath = PrefabPaths.MmdModelPanelPath;
        }

        public override void Awake(GameObject go)
        {
            SetButtonListener("BtnSelectMotion", () =>
            {
                ShowPage<MmdFileSelector>(new MmdFileSelector.Context
                {
                    Type = MmdFileSelector.FileType.Motion,
                    Title = "选择动作",
                    OnFileSelect = filePath =>
                    {
                        TtuiUtils.RunWithLoadingUI<LoadingDialog>(MainSceneController.Instance, () =>
                        {
                            try
                            {
                                MainSceneController.Instance.ChangeCurrentMotion(filePath);
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
            SetButtonListener("BtnSelectModel", () =>
            {
                ShowPage<MmdFileSelector>(new MmdFileSelector.Context
                {
                    Type = MmdFileSelector.FileType.Model,
                    Title = "替换模型",
                    OnFileSelect = filePath =>
                    {
                        TtuiUtils.RunWithLoadingUI<LoadingDialog>(MainSceneController.Instance, () =>
                        {
                            try
                            {
                                MainSceneController.Instance.ChangeCurrentModel(filePath);
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
            SetButtonListener("BtnPosition", ShowPage<MmdModelTransform>);
            SetButtonListener("BtnDelete", () => { MainSceneController.Instance.DeleteCurrentModel(); });
            SetButtonListener("BtnBack", ClosePage);
            SetButtonListener("BtnBonePoseCalculate", () =>
            {
                var modelPath = MainSceneController.Instance.GetCurrentModelPath();
                var motionPath = MainSceneController.Instance.GetCurrentMotionPath();
                if (string.IsNullOrEmpty(modelPath))
                {
                    return;
                }
                if (string.IsNullOrEmpty(motionPath))
                {
                    ShowPage<OkDialog>(new OkDialog.Context
                    {
                        Title = "提示",
                        Tip = "还没有选择动作。"
                    } );
                    return;
                }
                ShowPage<PhysicsCalculateSave>(new PhysicsCalculateSave.Context
                {
                    ModelPath = modelPath,
                    MotionPath = motionPath,
                    DefaultFileName = GenerateDefaultBonePoseFileName(modelPath, motionPath)
                });
            });
        }

        private static string GenerateDefaultBonePoseFileName(string modelPath, string motionPath)
        {
            var modelFileName = Path.GetFileNameWithoutExtension(new FileInfo(modelPath).Name);
            var motionFileName = Path.GetFileNameWithoutExtension(new FileInfo(motionPath).Name);
            return modelFileName + "_" + motionFileName;
        }

        public override void Active()
        {
            base.Active();
            MainSceneController.Instance.ShowSelectedMark(MainSceneController.Instance.GetModelCount() > 0);
        }
    }
}