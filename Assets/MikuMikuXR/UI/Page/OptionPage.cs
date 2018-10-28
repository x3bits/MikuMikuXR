using MikuMikuXR.UserConfig;
using UnityEngine;
using UnityEngine.UI;

namespace MikuMikuXR.UI.Page
{
    public class OptionPage : HideOtherPage
    {
        private Dropdown _physicsFpsDropdown;

        private readonly int[] _physicsFpsOptions = {240, 120, 60, 30, 15};

        public OptionPage()
        {
            uiPath = PrefabPaths.OptionPagePath;
        }

        public override void Awake(GameObject go)
        {
            InitPhysicsFpsDropdown();
            InitToggleSlowCalculation();
            InitToggleDrawShadow();
            SetButtonListener("Bottom/BtnClose", () =>
            {
                PlayerPrefs.Save();
                ClosePage();
            });
        }

        private void InitPhysicsFpsDropdown()
        {
            _physicsFpsDropdown = transform.Find("Content/Physics/PhysicsFps").GetComponent<Dropdown>();
            var physicsFps = PlayerPrefs.GetInt(UserConfigs.PhysicsFps, 120);
            SetPhysicsDropDown(physicsFps);
            _physicsFpsDropdown.onValueChanged.RemoveAllListeners();
            _physicsFpsDropdown.onValueChanged.AddListener(value =>
            {
                PlayerPrefs.SetInt(UserConfigs.PhysicsFps, _physicsFpsOptions[value]);
            });
        }

        private void InitToggleSlowCalculation()
        {
            var toggle = transform.Find("Content/Physics/ToggleShowSlowCalculation").GetComponent<Toggle>();
            toggle.isOn = PlayerPrefs.GetInt(UserConfigs.ShowSlowPhysics, 0) != 0;
            toggle.onValueChanged.RemoveAllListeners();
            toggle.onValueChanged.AddListener(
                isOn => { PlayerPrefs.SetInt(UserConfigs.ShowSlowPhysics, isOn ? 1 : 0); });
        }

        private void InitToggleDrawShadow()
        {
            var toggle = transform.Find("Content/Drawing/ToggleDrawShadow").GetComponent<Toggle>();
            toggle.isOn = PlayerPrefs.GetInt(UserConfigs.DrawShadow, 1) != 0;
            toggle.onValueChanged.RemoveAllListeners();
            toggle.onValueChanged.AddListener(
                isOn => { PlayerPrefs.SetInt(UserConfigs.DrawShadow, isOn ? 1 : 0); });
        }

        private void SetPhysicsDropDown(int fps)
        {
            for (var i = 0; i < _physicsFpsOptions.Length; i++)
            {
                if (fps < _physicsFpsOptions[i]) continue;
                _physicsFpsDropdown.value = i;
                return;
            }
            _physicsFpsDropdown.value = _physicsFpsOptions.Length - 1;
        }
    }
}