using UnityEngine;

namespace MikuMikuXR.UI.Page
{
    public class AboutPage : HideOtherPage
    {
        public AboutPage()
        {
            uiPath = PrefabPaths.AboutPagePath;
        }

        public override void Awake(GameObject go)
        {
            SetButtonListener("Bottom/BtnOK", ClosePage);
        }
    }
}