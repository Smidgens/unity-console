// smidgens @ github

#if UNITY_EDITOR

namespace Smidgenomics.Unity.Console.Editor
{
	using UnityEditor;
	using System.Collections.Generic;
	using UnityEditorInternal;
	using UnityEngine;

	internal abstract class _Base<T> : _Base where T : Object
	{
		protected T Target => target as T;
	}

	internal abstract class _Base : Editor
	{
		public override void OnInspectorGUI()
		{
			if(_props == null)
			{
				_props = new();
				foreach (var f in target.GetType().FindInspectorFields<UnityEngine.Object>())
				{
					var prop = serializedObject.FindProperty(f.Name);
					if (prop != null)
					{
						var pi = new PropInfo
						{
							prop = prop,
						};
						if (prop.isArray)
						{
							pi.list = GetList(prop);
						}
						_props.Add(pi);
					}
				}
			}
			serializedObject.UpdateIfRequiredOrScript();
			foreach (var pi in _props)
			{
				if (pi.list != null)
				{
					pi.list.DoLayoutList();
				}
				else
				{
					EditorGUILayout.PropertyField(pi.prop);
				}
			}
			serializedObject.ApplyModifiedProperties();
		}

		protected override bool ShouldHideOpenButton()
		{
			return true;
		}

		private struct PropInfo
		{
			public SerializedProperty prop;
			public ReorderableList list;
		}

		private List<PropInfo> _props;

		private static ReorderableList GetList(SerializedProperty prop)
		{
			var list = new ReorderableList(prop.serializedObject, prop, true, true, true, true)
			{
				drawHeaderCallback = r =>
				{
					GUI.Label(r, prop.displayName);
				},
				elementHeightCallback = i =>
				{
					var it = prop.GetArrayElementAtIndex(i);
					return EditorGUI.GetPropertyHeight(it);
				},
				drawElementCallback = (r, i, a, f) =>
				{
					r.height = EditorGUIUtility.singleLineHeight;
					var it = prop.GetArrayElementAtIndex(i);
					EditorGUI.PropertyField(r, it, GUIContent.none);
				}
			};
			return list;
		}
	}
	
	
}

#endif