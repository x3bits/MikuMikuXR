using UnityEditor;
using UnityEngine;

namespace Editor.Scripts
{
	public class MenuItems
	{
		[MenuItem("Tools/Clear PlayerPrefs")]
		private static void NewMenuOption()
		{
			PlayerPrefs.DeleteAll();
		}
	}
}
