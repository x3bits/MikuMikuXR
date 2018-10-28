using UnityEngine;
using UnityEngine.Events;

namespace MikuMikuXR.UI.ListItem
{
    public class MmdFileListItemData
    {
        public string FileName;
        public string FilePath;
        public UnityAction OnClick;
        public Sprite Icon;
    }
}