// smidgens @ github

namespace Smidgenomics.Unity.Console.Editor
{
	using UnityEngine;
	using UnityEditor;
	using System;

	using UObject = UnityEngine.Object;
	using SP = UnityEditor.SerializedProperty;

	[CustomEditor(typeof(ConsoleGUI))]
	internal class _ConsoleGUI : _Base
	{
		public override void OnInspectorGUI()
		{
			serializedObject.UpdateIfRequiredOrScript();
			foreach (var p in _defaultProps)
			{
				EditorGUILayout.PropertyField(p);
			}
			serializedObject.ApplyModifiedProperties();
		}

		private static readonly string[] _DEFAULT_PROPS =
		{
			"_console",
			"_themeOverride"
		};

		private SP[] _defaultProps = new SP[_DEFAULT_PROPS.Length];

		private void OnEnable()
		{
			Find(serializedObject, _DEFAULT_PROPS, ref _defaultProps);
		}

	}
}