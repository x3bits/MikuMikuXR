using NSUListView;
using UnityEngine.UI;

namespace MikuMikuXR.UI.ListItem
{
    public class MmdFileListItemView : IUListItemView
    {
        public override void SetData(object data)
        {
            var fileData = (MmdFileListItemData) data;
            transform.Find("Text").GetComponent<Text>().text = fileData.FileName;
            transform.Find("Icon").GetComponent<Image>().sprite = fileData.Icon;
            var button = GetComponent<Button>();
            
            UiUtils.SetButtonListener(button, fileData.OnClick);
        }
    }
}