using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace MikuMikuXR.UI.Page
{
	public class StartPage : HideOtherPage
	{
		private Button _startButton;
		
		public StartPage()
		{
			uiPath = PrefabPaths.StartPagePath;
		}
		

		public override void Awake(GameObject go)
		{
			_startButton = transform.Find("Buttons/BtnStart").GetComponent<Button>();
			SetButtonListener("Buttons/BtnStart", () =>
			{
				ClearNodes();
				SceneManager.LoadSceneAsync("Main");
			});
			SetButtonListener("Buttons/BtnOption", ShowPage<OptionPage>);
			SetButtonListener("Buttons/BtnIntroduction", ShowPage<IntroductionPage>);
			SetButtonListener("Buttons/BtnAbout", ShowPage<AboutPage>);
			transform.Find("Version").GetComponent<Text>().text = "version: " + Application.version;
		}

		public void SetStartButtonEnable(bool enable)
		{
			_startButton.gameObject.SetActive(enable);
		}
	}
}