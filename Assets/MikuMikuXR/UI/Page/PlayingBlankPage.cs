using MikuMikuXR.SceneController;
using UnityEngine;
using UnityEngine.UI;

namespace MikuMikuXR.UI.Page
{
    public class PlayingBlankPage: HideOtherPage
    {
        public PlayingBlankPage()
        {
            uiPath = PrefabPaths.PlayingBlankPath;
        }

        public override void Awake(GameObject go)
        {
            var onClick = transform.GetComponent<Button>().onClick;
            onClick.RemoveAllListeners();
            onClick.AddListener(() =>
            {
                MainSceneController.Instance.SwitchPlayPause(false);
                ClosePage();
            });
        }
    }
}