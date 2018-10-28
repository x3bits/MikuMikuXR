using System;
using System.IO;
using LibMMD.Unity3D.BonePose;
using MikuMikuXR.Components;
using MikuMikuXR.SceneController;
using MikuMikuXR.UserConfig.Path;
using MikuMikuXR.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace MikuMikuXR.UI.Page
{
    public class PhysicsCalculateSave : HideOtherPage
    {
        private InputField _inputField;

        public class Context
        {
            public string ModelPath { get; set; }
            public string MotionPath { get; set; }
            public string DefaultFileName { get; set; }
        }

        public PhysicsCalculateSave()
        {
            uiPath = PrefabPaths.PhysicsCalculateSavePath;
        }

        public override void Awake(GameObject go)
        {
            _inputField = transform.Find("Content/InputField").gameObject.GetComponent<InputField>();
            SetButtonListener("Bottom/BtnClose", ClosePage);
            SetButtonListener("Bottom/BtnOK", () =>
            {
                var fileName = _inputField.text;
                Directory.CreateDirectory(Paths.Getter().BonePoseFolder());
                var context = (Context) data;
                var savePath = Paths.Getter().BonePoseFolder() + "/" + fileName + Constants.BonePoseFileExt;
                ClosePage();
                if (File.Exists(savePath))
                {
                    ShowPage<OkCancelDialog>(new OkCancelDialog.Context
                    {
                        Title = "确认保存",
                        Tip = "文件已存在，要覆盖吗？",
                        OnOk = () =>
                        {
                            OpenCalculatingDialog(context, savePath);
                        },
                        OnCancel = () => { }
                    });
                }
                else
                {
                    OpenCalculatingDialog(context, savePath);
                }
            });
            SetButtonListener("Bottom/BtnLoad", () =>
            {
                ClosePage();
                ShowPage<MmdFileSelector>(new MmdFileSelector.Context
                {
                    Type = MmdFileSelector.FileType.BonePose,
                    Title = "物理计算",
                    OnFileSelect = filePath =>
                    {
                        TtuiUtils.RunWithLoadingUI<LoadingDialog>(MainSceneController.Instance, () =>
                        {
                            try
                            {
                                MainSceneController.Instance.ChangeCurrentBonePoseFile(filePath);
                            }
                            catch (BonePoseNotSuitableException e)
                            {
                                TtuiUtils.ShowPageAfterLoadingUI<OkDialog>(MainSceneController.Instance,
                                    new OkDialog.Context
                                    {
                                        Title = "提示",
                                        Tip = "载入的物理计算结果不适用于当前模型"
                                    });
                            }
                            catch (Exception e)
                            {
                                Debug.LogException(e);
                            }
                        });
                    }
                });
            });
        }

        private static void OpenCalculatingDialog(Context context, string savePath)
        {
            ShowPage<PhysicsCalculating>(new PhysicsCalculating.Context
            {
                ModelPath = context.ModelPath,
                MotionPath = context.MotionPath,
                SavePath = savePath
            });
        }

        public override void Active()
        {
            base.Active();
            var context = (Context) data;
            _inputField.text = context.DefaultFileName;
        }
    }
}