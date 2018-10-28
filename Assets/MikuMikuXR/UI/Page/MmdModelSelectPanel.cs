using System.Collections.Generic;
using System.Linq;
using MikuMikuXR.SceneController;
using MikuMikuXR.UI.ListItem;
using NSUListView;
using UnityEngine;

namespace MikuMikuXR.UI.Page
{
    public class MmdModelSelectPanel : HideOtherPage
    {
        private USimpleListView _modelListView;

        
        public MmdModelSelectPanel()
        {
            uiPath = PrefabPaths.MmdModelSelectPanelPath;
        }

        public override void Awake(GameObject go)
        {
            _modelListView = FindCompoment<USimpleListView>("Viewport/Content/ModelList");
            SetButtonListener("Bottom/BtnClose", ClosePage);
        }

        public override void Active()
        {
            base.Active();
            MainSceneController.Instance.ShowSelectedMark(MainSceneController.Instance.GetModelCount() > 0);
            _modelListView.SetData(ConvertDataForUi());
        }

        private static List<object> ConvertDataForUi()
        {
            var modelNames = MainSceneController.Instance.GetModelNames();
            return modelNames.Select((modelName, i) => new MmdModelListItemData
                {
                    Name = modelName.Replace(' ', '\u00A0'),
                    OnClick = () =>
                    {
                        MainSceneController.Instance.SelectModel(i);
                        ClosePage<MmdModelSelectPanel>();
                        ShowPage<MmdModelPanel>();
                    }
                })
                .Cast<object>()
                .ToList();
        }
        
    }
}