using System;
using System.Collections.Generic;
using System.Linq;
using MikuMikuXR.SceneController;
using MikuMikuXR.UI.ListItem;
using MikuMikuXR.Utils;
using NSUListView;
using UnityEngine;

namespace MikuMikuXR.UI.Page
{
    public class LoadSceneDilog : HideOtherPage
    {
        private USimpleListView _modelListView;

        
        public LoadSceneDilog()
        {
            uiPath = PrefabPaths.LoadSceneDialogPath;
        }

        public override void Awake(GameObject go)
        {
            _modelListView = FindCompoment<USimpleListView>("Viewport/Content/SceneList");
            SetButtonListener("Bottom/BtnClose", ClosePage);
        }

        public override void Active()
        {
            base.Active();
            _modelListView.SetData(ConvertDataForUi());
        }

        private List<object> ConvertDataForUi()
        {
            var sceneNames = MainSceneController.Instance.ListScenes();
            return sceneNames.Select((sceneName, i) => new SceneListItemData
                {
                    Name = sceneName.Replace(' ', '\u00A0'),
                    OnClick = () =>
                    {
                        ClosePage();
                        try
                        {
                            TtuiUtils.RunWithLoadingUI<LoadingDialog>(MainSceneController.Instance, () =>
                            {
                                try
                                {
                                    MainSceneController.Instance.LoadScene(sceneName);
                                }
                                catch (Exception e)
                                {
                                    Debug.LogException(e);
                                }
                            });
                        }
                        catch (Exception e)
                        {
                            Debug.LogException(e);
                        }
                    },
                    OnDelete = () =>
                    {
                        ShowPage<OkCancelDialog>(new OkCancelDialog.Context
                        {
                            Title = "确认删除",
                            Tip = "确认要删除吗？",
                            OnOk = () =>
                            {
                                try
                                {
                                    MainSceneController.Instance.DeleteScene(sceneName);
                                    _modelListView.SetData(ConvertDataForUi());
                                } catch (Exception e)
                                {
                                    Debug.LogException(e);
                                }
                            },
                            OnCancel = () => { }
                        });
                    }
                })
                .Cast<object>()
                .ToList();
        }
        
    }
}