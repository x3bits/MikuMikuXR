using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace MikuMikuXR.UI
{
    public class UiUtils
    {
        private const string DontShowAgainPrefix = "DontShowAgain.";

        public static void SetButtonListener(Button button, UnityAction unityAction)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(unityAction);
        }

        public static void SetSliderOnChangeListener(Slider slider, UnityAction<float> unityAction)
        {
            slider.onValueChanged.RemoveAllListeners();
            slider.onValueChanged.AddListener(unityAction);
        }

        public static bool IsDoNotShowAgain(string tipName)
        {
            return PlayerPrefs.GetInt(DontShowAgainPrefix + tipName, 0) != 0;
        }

        public static void SetDontShowAgain(string tipName, bool dontShowAgain)
        {
            PlayerPrefs.SetInt(DontShowAgainPrefix + tipName, dontShowAgain ? 1 : 0);
        }
    }
}