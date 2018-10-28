using TinyTeam.UI;
using UnityEngine.UI;

namespace MikuMikuXR.UI.Page
{
	public class StartBackground : BasePage
	{
		private Button _startButton;
		
		public StartBackground() : base(UIType.Normal, UIMode.DoNothing, UICollider.None)
		{
			uiPath = PrefabPaths.StartBackgroundPath;
		}
	}
}