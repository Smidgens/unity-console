// smidgens @ github

namespace Smidgenomics.Unity.Console.Editor
{
	using UnityEngine;
	using UnityEditor;

	[CustomPropertyDrawer(typeof(BoolDropdownAttribute))]
	internal class _BoolDropdown : _BaseDrawer
	{
		protected override void OnDrawGUI(in Rect pos, SerializedProperty prop, GUIContent l)
		{
			if (fieldInfo.FieldType != typeof(bool)) { return; }
			var a = attribute as BoolDropdownAttribute;
			var i = prop.boolValue ? 1 : 0;
			var ni = EditorGUI.Popup(pos, i, a.Labels);
			if (i != ni) { prop.boolValue = ni == 0 ? false : true; }
		}
	}
}
