// smidgens @ github

namespace Smidgenomics.Unity.Console
{
	using UnityEngine;
	using System;
	using Conditional = System.Diagnostics.ConditionalAttribute;

	[Conditional("UNITY_EDITOR")]
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
	internal class BoolPopupAttribute : PropertyAttribute
	{
		public BoolPopupAttribute() { }
		public BoolPopupAttribute(string l0, string l1) => Labels = new string[] { l0, l1 };
		internal string[] Labels { get; } = { "False", "True" };
	}
}


#if UNITY_EDITOR
namespace Smidgenomics.Unity.Console
{
	using UnityEngine;
	using UnityEditor;

	[CustomPropertyDrawer(typeof(BoolPopupAttribute))]
	internal class _BoolPopup : PropertyDrawer
	{
		public override void OnGUI(Rect pos, SerializedProperty prop, GUIContent l)
		{
			if(fieldInfo.FieldType != typeof(bool)){ return; }
			var a = attribute as BoolPopupAttribute;
			
			using(new EditorGUI.PropertyScope(pos, l, prop))
			{
				if (l != GUIContent.none && !fieldInfo.FieldType.IsArray)
				{
					pos = EditorGUI.PrefixLabel(pos, l);
				}
				var i = prop.boolValue ? 1 : 0;
				var ni = EditorGUI.Popup(pos, i, a.Labels);
				if (i != ni) { prop.boolValue = ni == 0 ? false : true; }
			}
		}
	}
}

#endif