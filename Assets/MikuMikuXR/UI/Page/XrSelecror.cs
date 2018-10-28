using MikuMikuXR.SceneController;
using MikuMikuXR.XR;
using UnityEngine;

namespace MikuMikuXR.UI.Page
{
    public class XrSelecror : HideOtherPage
    {
        public XrSelecror()
        {
            uiPath = PrefabPaths.XrSelectorPath;
        }

        public override void Awake(GameObject go)
        {
            SetButtonListener("BtnVRManual", () =>
            {
                ChangeXrType(XrType.VrManual);
            });
            SetButtonListener("BtnVRSingle", () =>
            {
                ChangeXrType(XrType.VrSingle);
            });
            SetButtonListener("BtnVRGlass", () =>
            {
                ChangeXrType(XrType.VrGlass);
            });
            SetButtonListener("BtnPyramid", () =>
            {
                ChangeXrType(XrType.Pyramid);
                OnceTipPage.ShowOnceTip(TipNames.Pyramid);
            });
            SetButtonListener("BtnCameraFile", () =>
            {
                ChangeXrType(XrType.CameraFile);
            });
            SetButtonListener("BtnArUserDefined", () =>
            {
                ChangeXrType(XrType.ArUserDefined);
            });
            SetButtonListener("BtnBack", ClosePage);
        }

        private static void ChangeXrType(XrType xrType)
        {
            MainSceneController.Instance.ChangeXrType(xrType);
            ClosePage();
        }
    }
}