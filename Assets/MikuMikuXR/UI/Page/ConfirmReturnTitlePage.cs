using UnityEngine;
using UnityEngine.SceneManagement;

namespace MikuMikuXR.UI.Page
{
    public class ConfirmReturnTitlePage : HideOtherPage
    {
        public ConfirmReturnTitlePage()
        {
            uiPath = PrefabPaths.ConfirmReturnTitlePath;
        }

        public override void Awake(GameObject go)
        {
            SetButtonListener("Bottom/BtnOK", () =>
            {
                SceneManager.LoadScene("Start");
            });
            SetButtonListener("Bottom/BtnClose", ClosePage);
        }
    }
}