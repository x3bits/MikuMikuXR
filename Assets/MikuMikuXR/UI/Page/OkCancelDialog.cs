using MikuMikuXR.DTO;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace MikuMikuXR.UI.Page
{
    public class OkCancelDialog : HideOtherPage
    {
        private Text _titleText;
        private Text _tipText;

        public class Context
        {
            public string Title { get; set; }
            public string Tip { get; set; }
            public UnityAction OnOk { get; set; }
            public UnityAction OnCancel { get; set; }
        }

        public OkCancelDialog()
        {
            uiPath = PrefabPaths.OkCancelDialogPath;
        }

        public override void Awake(GameObject go)
        {
            _titleText = transform.Find("Title").gameObject.GetComponent<Text>();
            _tipText = transform.Find("Content/Tip").gameObject.GetComponent<Text>();
        }

        public override void Refresh()
        {
            base.Refresh();
            var context = (Context) data;
            _titleText.text = context.Title;
            _tipText.text = context.Tip;
            SetButtonListener("Bottom/BtnOk", () =>
            {
                ClosePage();
                context.OnOk.Invoke();
            });
            SetButtonListener("Bottom/BtnCancel", () =>
            {
                ClosePage();
                context.OnCancel.Invoke();
            });
        }
    }
}