using UnityEngine;

namespace MikuMikuXR.GameCompoment
{
	public class DeviceOrientationAdaptor : MonoBehaviour
	{

		public float LandscapeScale = 1.0f;
		public float PortraitScale = 1.0f;

		private RectTransform _rectTransform;

		private void Awake()
		{
			_rectTransform = GetComponent<RectTransform>();
		}
	
		private void Update ()
		{
			switch (Screen.orientation)
			{
				case ScreenOrientation.LandscapeLeft:
				case ScreenOrientation.LandscapeRight:
					_rectTransform.localScale = Vector3.one * LandscapeScale;
					break;
				case ScreenOrientation.Portrait:
				case ScreenOrientation.PortraitUpsideDown:
					_rectTransform.localScale = Vector3.one * PortraitScale;
					break;
				default:
					_rectTransform.localScale = Vector3.one;
					break;
			}
		}
	}
}