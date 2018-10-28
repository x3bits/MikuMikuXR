using MikuMikuXR.SceneController;
using NSUListView;
using UnityEngine.UI;

namespace MikuMikuXR.UI.ListItem
{
    public class SceneListItemView : IUListItemView
    {
        public override void SetData(object data)
        {
            var sceneData = (SceneListItemData) data;
            transform.Find("Text").GetComponent<Text>().text = sceneData.Name;
            var button = GetComponent<Button>();      
            UiUtils.SetButtonListener(button, sceneData.OnClick);
            var deleteButton = transform.Find("BtnDelete").GetComponent<Button>();
            UiUtils.SetButtonListener(deleteButton, sceneData.OnDelete);
        }
    }
}