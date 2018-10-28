using System.Collections;
using MikuMikuXR.SceneController;
using UnityEngine;
using UnityEngine.UI;

namespace MikuMikuXR.UI.Page
{
    public class VrCountdownPage : HideOtherPage
    {
        private Text _leftText;
        private Text _rightText;
        private int _countDownValue;
        
        public VrCountdownPage()
        {
            uiPath = PrefabPaths.VrCountdownPath;
        }

        public override void Awake(GameObject go)
        {
            _leftText = transform.Find("Left/Text").GetComponent<Text>();
            _rightText = transform.Find("Right/Text").GetComponent<Text>();
            SetButtonListener("Left/BtnLess", LessTime);
            SetButtonListener("Left/BtnMore", MoreTime);
            SetButtonListener("Right/BtnLess", LessTime);
            SetButtonListener("Right/BtnMore", MoreTime);
        }
        
        public override void Active()
        {
            base.Active();
            AdjustHalfScreenSize("Left");
            AdjustHalfScreenSize("Right");
            MainSceneController.Instance.StartCoroutine(CountDown());
        }

        private void LessTime()
        {
            _countDownValue -= 5;
            if (_countDownValue < 1)
            {
                _countDownValue = 1;
            }
            RefreshUi();
        }

        private void MoreTime()
        {
            _countDownValue += 5;
            RefreshUi();
        }

        private void AdjustHalfScreenSize(string objName)
        {
            var leftRect = transform.Find(objName).GetComponent<RectTransform>();
            var halfScreenSize = leftRect.rect.height / Screen.height * Screen.width / 2.0f;
            leftRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, halfScreenSize);
        }

        private IEnumerator CountDown()
        {
            _countDownValue = 10;
            while (true) 
            {
                RefreshUi();
                yield return new WaitForSeconds(1);
                _countDownValue--;
                if (_countDownValue <= 0)
                {
                    break;
                }
            }
            ClosePage();
            MainSceneController.Instance.SwitchPlayPause(true);
        }

        private void RefreshUi()
        {
            _leftText.text = _countDownValue.ToString();
            _rightText.text = _countDownValue.ToString();
        }
    }
}