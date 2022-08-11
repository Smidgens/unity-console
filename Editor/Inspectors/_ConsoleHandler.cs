// smidgens @ github

namespace Smidgenomics.Unity.Console.Editor
{
	using UnityEngine;
	using UnityEditor;
	using SP = UnityEditor.SerializedProperty;

	[CustomEditor(typeof(ConsoleHandler))]
	internal class _ConsoleHandler : _Base
	{
		public override void OnInspectorGUI()
		{

			serializedObject.UpdateIfRequiredOrScript();

			LayoutFields(_console);

			using(new GUILayout.HorizontalScope())
			{
				var pos = EditorGUILayout.GetControlRect();
				pos = EditorGUI.PrefixLabel(pos, new GUIContent(_keyword.displayName));

				var cols = pos.SplitHorizontally(2.0, 65f, 1f);

				EditorGUI.PropertyField(cols[0], _keywordMode, GUIContent.none);

				if (_keywordMode.enumValueIndex == 0)
				{
					EditorGUI.PropertyField(cols[1], _keyword, GUIContent.none);
				}
				else
				{
					var t = (ConsoleHandler)target;
					var kw = t.GetKeyword();
					EditorGUI.LabelField(cols[1], kw, EditorStyles.whiteMiniLabel);
				}
			}
			LayoutFields(_method);
			serializedObject.ApplyModifiedProperties();
		}


		private SP
		_console, _keyword, _keywordMode, _method;

		private void OnEnable()
		{
			Find(ConsoleHandler._Fields.CONSOLE, ref _console);
			Find(ConsoleHandler._Fields.KEYWORD, ref _keyword);
			Find(ConsoleHandler._Fields.METHOD, ref _method);
			Find(ConsoleHandler._Fields.KW_MODE, ref _keywordMode);
		}

	}
}