// smidgens @ github

namespace Smidgenomics.Unity.Console
{
	using System;
	using UnityEngine;

	[Serializable]
	public struct ConsoleReference
	{
		public bool IsSet => _asset != null;
		public IConsole Console => _asset;
		[SerializeField] private ConsoleAsset _asset;
	}
}


#if UNITY_EDITOR

namespace Smidgenomics.Unity.Console.Editor
{
	using UnityEngine;
	using UnityEditor;

	[CustomPropertyDrawer(typeof(ConsoleReference))]
	internal class _ConsoleReference : PropertyDrawer
	{
		public override void OnGUI(Rect pos, SerializedProperty prop, GUIContent l)
		{
			EditorGUI.BeginProperty(pos, l, prop);
			{
				if (l != GUIContent.none && !fieldInfo.FieldType.IsArray)
				{
					pos = EditorGUI.PrefixLabel(pos, l);
				}
				var cprop = prop.FindPropertyRelative("_asset");
				EditorGUI.PropertyField(pos, cprop, GUIContent.none);
			}
			EditorGUI.EndProperty();
		}
	}
}

#endif