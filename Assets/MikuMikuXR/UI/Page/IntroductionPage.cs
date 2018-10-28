using UnityEngine;

namespace MikuMikuXR.UI.Page
{
    public class IntroductionPage : HideOtherPage
    {
        public IntroductionPage()
        {
            uiPath = PrefabPaths.IntroductionPagePath;
        }

        public override void Awake(GameObject go)
        {
            SetButtonListener("Bottom/BtnOK", ClosePage);
        }
    }
}