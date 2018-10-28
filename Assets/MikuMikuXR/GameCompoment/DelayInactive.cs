using UnityEngine;

namespace MikuMikuXR.GameCompoment
{
	public class DelayInactive : MonoBehaviour
	{

		public int ShowFrameLength = 2;

		private int _frameLeft = 0;

		public void Show()
		{
			gameObject.SetActive(true);
			_frameLeft = ShowFrameLength;
		}
	
		// Update is called once per frame
		private void Update ()
		{
			if (_frameLeft <= 0)
			{
				return;
			}
			_frameLeft--;
			if (_frameLeft == 0)
			{
				gameObject.SetActive(false);
			}
		}
	}
}
