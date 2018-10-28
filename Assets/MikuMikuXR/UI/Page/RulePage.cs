using MikuMikuXR.SceneController;
using MikuMikuXR.UserConfig;
using UnityEngine;

namespace MikuMikuXR.UI.Page
{
    public class RulePage : HideOtherPage
    {
        public RulePage()
        {
            uiPath = PrefabPaths.RulePagePath;
        }

        public override void Awake(GameObject go)
        {
            SetButtonListener("Bottom/BtnOK", () =>
            {
                StartSceneController.Instance.InitResource();
                AppConfig.RuleAgreed = true;
                ClosePage();
            });
            SetButtonListener("Bottom/BtnClose", Application.Quit);
        }
    }
}