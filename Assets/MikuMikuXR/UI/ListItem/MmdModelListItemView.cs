using NSUListView;
using UnityEngine.UI;

namespace MikuMikuXR.UI.ListItem
{
    public class MmdModelListItemView : IUListItemView
    {
        public override void SetData(object data)
        {
            var fileData = (MmdModelListItemData) data;
            transform.Find("Text").GetComponent<Text>().text = fileData.Name;
            var button = GetComponent<Button>();            
            UiUtils.SetButtonListener(button, fileData.OnClick);
        }
    }
}