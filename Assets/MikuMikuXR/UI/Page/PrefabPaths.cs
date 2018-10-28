using System.Collections.Generic;

namespace MikuMikuXR.UI.Page
{
    public class PrefabPaths
    {
        private const string UiPrefabPath = "UI/Prefabs/";
        public const string MmdStagePanelPath = UiPrefabPath + "MMDStagePanel";
        public const string MmdFileSelectorPath = UiPrefabPath + "MMDFileSelector";
        public const string ScanDialogPath = UiPrefabPath + "ScanDialog";
        public const string StartPagePath = UiPrefabPath + "StartPage";
        public const string LoadingTipPath = UiPrefabPath + "LoadingTip";
        public const string RulePagePath = UiPrefabPath + "RulePage";
        public const string AboutPagePath = UiPrefabPath + "AboutPage";
        public const string LoadingDialogPath = UiPrefabPath + "LoadingDialog";
        public const string MmdModelSelectPanelPath = UiPrefabPath + "MMDModelSelectPanel";
        public const string LoadSceneDialogPath = UiPrefabPath + "LoadSceneDialog";
        public const string MmdModelPanelPath = UiPrefabPath + "MMDModelPanel";
        public const string MmdModelTransformPath = UiPrefabPath + "MMDModelTransform";
        public const string XrSelectorPath = UiPrefabPath + "ViewSelector";
        public const string PlayingBlankPath = UiPrefabPath + "PlayingBlank";
        public const string PlayingButtonPath = UiPrefabPath + "PlayingButton";
        public const string VrCountdownPath = UiPrefabPath + "VrCountdown";
        public const string OnceTipPath = UiPrefabPath + "OnceTip";
        public const string StartBackgroundPath = UiPrefabPath + "StartBackground";
        public const string ConfirmReturnTitlePath = UiPrefabPath + "ConfirmReturnTitle";
        public const string OptionPagePath = UiPrefabPath + "OptionPage";
        public const string SaveSceneDialogPath = UiPrefabPath + "SaveSceneDialog";
        public const string UpdateDialogPath = UiPrefabPath + "UpdateDialog";
        public const string OkCancelDialogPath = UiPrefabPath + "OkCancelDialog";
        public const string OkDialogPath = UiPrefabPath + "OkDialog";
        public const string PhysicsCalculateSavePath = UiPrefabPath + "PhysicsCalculateSave";
        public const string PhysicsCalculatingPath = UiPrefabPath + "PhysicsCalculating";
        public const string IntroductionPagePath = UiPrefabPath + "IntroductionPage";

        public const string ManualPrefabPath = "UI/Prefabs/Manual/";
        public const string TipTextPath = ManualPrefabPath + "TipText";
        public static readonly IDictionary<string, string> TipNameToPath = new Dictionary<string, string>
        {
            {TipNames.Pyramid, ManualPrefabPath + TipNames.Pyramid}
        };
    }
}