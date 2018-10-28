using System;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace MikuMikuXR.UI.Page
{
    public class OnceTipPage : HideOtherPage
    {
        private Toggle _dontShowAgainToggle;
        private string _tipName = "";
        private GameObject _manualGameObject;
        private Transform _contentTransform;
        
        public class Context
        {
            public string TipName { get; set; }
            public string TipText { get; set; }
            public string ManualPath { get; set; }
        }
        
        public OnceTipPage()
        {
            uiPath = PrefabPaths.OnceTipPath;
        }

        public static void ShowOnceTip(string tipName)
        {
            if (UiUtils.IsDoNotShowAgain(tipName))
            {
                return;
            }
            string tipText;
            TipNames.TipNameToText.TryGetValue(tipName, out tipText);
            var manualPath = tipText != null ? PrefabPaths.TipTextPath : PrefabPaths.TipNameToPath[tipName];
            ShowPage<OnceTipPage>(new Context
            {
                TipName = tipName,
                ManualPath = manualPath,
                TipText = tipText
            });
        }

        public override void Awake(GameObject go)
        {
            _dontShowAgainToggle = transform.Find("Bottom/DontShowAgain").GetComponent<Toggle>();
            _contentTransform = transform.Find("Content");
            SetButtonListener("Bottom/BtnClose", () =>
            {
                if (_dontShowAgainToggle.isOn)
                {
                    UiUtils.SetDontShowAgain(_tipName, true);
                }
                ClosePage();
            });
            
        }

        public override void Active()
        {
            base.Active();
            var context = (Context) data;
            _tipName = context.TipName;
            var manualPrefab = Resources.Load(context.ManualPath) as GameObject;
            _manualGameObject = Object.Instantiate(manualPrefab);
            _manualGameObject.transform.SetParent(_contentTransform, false);
            _dontShowAgainToggle.isOn = true;
            if (context.TipText == null) return;
            try
            {
                _manualGameObject.GetComponent<Text>().text = context.TipText;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        public override void Hide()
        {
            base.Hide();
            if (_manualGameObject != null)
            {
                Object.Destroy(_manualGameObject);
            }
        }

    }
}