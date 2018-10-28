using System;
using System.CodeDom.Compiler;
using MikuMikuXR.SceneController;
using MikuMikuXR.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace MikuMikuXR.UI.Page
{
    public class SaveSceneDialog : HideOtherPage
    {
        private InputField _inputField;

        public SaveSceneDialog()
        {
            uiPath = PrefabPaths.SaveSceneDialogPath;
        }

        public override void Awake(GameObject go)
        {
            _inputField = transform.Find("Content/InputField").gameObject.GetComponent<InputField>();
            SetButtonListener("Bottom/BtnClose", ClosePage);
            SetButtonListener("Bottom/BtnOK", () =>
            {
                var sceneName = _inputField.text;
                if (string.IsNullOrEmpty(sceneName))
                {
                    return;
                }
                if (MainSceneController.Instance.IsSceneExist(sceneName))
                {
                    ShowPage<OkCancelDialog>(new OkCancelDialog.Context
                    {
                        Title = "确认保存",
                        Tip = "Live已存在，要覆盖吗？",
                        OnOk = () =>
                        {
                            SaveScene(sceneName); 
                            ClosePage<SaveSceneDialog>();
                        },
                        OnCancel = () => { }
                    });
                }
                else
                {
                    SaveScene(sceneName);
                    ClosePage();
                }
            });
        }

        private static void SaveScene(string sceneName)
        {
            try
            {
                MainSceneController.Instance.SaveScene(sceneName);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        public override void Active()
        {
            base.Active();
            _inputField.text = AutoGenerateSceneName();
        }

        private static string AutoGenerateSceneName()
        {
            return "live" + DateTime.Now.ToString("yyyyMMddHHmmssfff");
        }
    }
}