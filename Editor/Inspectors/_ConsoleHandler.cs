// smidgens @ github

namespace Smidgenomics.Unity.Console.Editor
{
	using UnityEngine;
	using UnityEditor;
	using SP = UnityEditor.SerializedProperty;

	[CustomEditor(typeof(CCommandBehaviour))]
	internal class _ConsoleHandler : _Base
	{
		public override void OnInspectorGUI()
		{
			serializedObject.UpdateIfRequiredOrScript();
			EditorGUILayout.PropertyField(_console);
			KeywordField();
			EditorGUILayout.PropertyField(_method);
			serializedObject.ApplyModifiedProperties();
		}

		private SP
		_console, _keyword, _keywordMode, _method;

		private void OnEnable()
		{
			Find(CCommandBehaviour._Fields.CONSOLE, ref _console);
			Find(CCommandBehaviour._Fields.NAME, ref _keyword);
			Find(CCommandBehaviour._Fields.METHOD, ref _method);
			Find(CCommandBehaviour._Fields.NAME_MODE, ref _keywordMode);
		}

		private void KeywordField()
		{
			using (new GUILayout.HorizontalScope())
			{
				EditorGUILayout.PrefixLabel(_keyword.displayName);

				using (new GUILayout.VerticalScope())
				{
					EditorGUILayout.PropertyField(_keywordMode, GUIContent.none);

					if (_keywordMode.enumValueIndex == 0)
					{
						EditorGUILayout.PropertyField(_keyword, GUIContent.none);
					}
					else
					{
						var t = (CCommandBehaviour)target;
						var kw = t.GetKeyword();
						EditorGUILayout.LabelField(kw, EditorStyles.whiteMiniLabel);
					}

				}

			}
		}

	}
}