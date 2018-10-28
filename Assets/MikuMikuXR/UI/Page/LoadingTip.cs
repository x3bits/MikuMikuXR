using UnityEngine;
using UnityEngine.UI;

namespace MikuMikuXR.UI.Page
{
    public class LoadingTip:StandalonePage
    {
        private Text _text;
        
        public LoadingTip()
        {
            uiPath = PrefabPaths.LoadingTipPath;
        }

        public override void Awake(GameObject go)
        {
            _text = gameObject.GetComponent<Text>();
        }

        public void SetText(string text)
        {
            _text.text = text;
        }
    }
}