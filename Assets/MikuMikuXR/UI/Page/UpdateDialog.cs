using System;
using System.CodeDom.Compiler;
using MikuMikuXR.DTO;
using MikuMikuXR.SceneController;
using UnityEngine;
using UnityEngine.UI;

namespace MikuMikuXR.UI.Page
{
    public class UpdateDialog : HideOtherPage
    {
        private InputField _inputField;
        private Text _versionNameText;
        private Text _descriptionText;
        private string _updateUrl;

        public UpdateDialog()
        {
            uiPath = PrefabPaths.UpdateDialogPath;
        }

        public override void Awake(GameObject go)
        {
            SetButtonListener("Bottom/BtnCancel", ClosePage);
            SetButtonListener("Bottom/BtnUpdate", () =>
            {
                if (string.IsNullOrEmpty(_updateUrl)) return;
                Application.OpenURL(_updateUrl);
                ClosePage();
            });
            _versionNameText = transform.Find("Content/VersionName").gameObject.GetComponent<Text>();
            _descriptionText = transform.Find("Content/Description").gameObject.GetComponent<Text>();
        }

        public override void Refresh()
        {
            base.Refresh();
            var updateVersion = (UpdateVersionDto) data;
            _versionNameText.text = updateVersion.VersionName;
            _descriptionText.text = updateVersion.Description;
            _updateUrl = updateVersion.Url;
        }
    }
}