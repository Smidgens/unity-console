// smidgens @ github

namespace Smidgenomics.Unity.Console
{
	using System;
	using UnityEngine;
	using Conditional = System.Diagnostics.ConditionalAttribute;

	// display type fields expanded
	[AttributeUsage(AttributeTargets.Field)]
	[Conditional("UNITY_EDITOR")]
	internal sealed class ExpandAttribute : PropertyAttribute
	{
		public ExpandAttribute(bool innerOnly = false)
		{
			this.innerOnly = innerOnly;
		}
		internal bool innerOnly { get; }
	}
}


#if UNITY_EDITOR

namespace Smidgenomics.Unity.Console.Editor
{
	using UnityEditor;
	using UnityEngine;
	using System.Reflection;
	using System.Collections.Generic;

	[CustomPropertyDrawer(typeof(ExpandAttribute))]
	internal sealed class _Expand : PropertyDrawer
	{
		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			EnsureInit();

			var h = 0f;
			
			var attr =  (attribute as ExpandAttribute)!;

			if (!attr.innerOnly)
			{
				h += EditorStyles.label.CalcSize(GUIContent.none).y;
				h += EditorGUIUtility.standardVerticalSpacing;
			}

			h += Mathf.Max(_fields.Count - 1, 0f) * EditorGUIUtility.standardVerticalSpacing;
			foreach (var f in _fields)
			{
				var prop = property.FindPropertyRelative(f.Name);
				h += EditorGUI.GetPropertyHeight(prop);
			}
			return h;
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			var attr =  (attribute as ExpandAttribute)!;
			EditorGUI.BeginProperty(position, label, property);

			if (!attr.innerOnly)
			{
				var lHeight = EditorStyles.label.CalcSize(GUIContent.none).y;
				var lrow = position.SliceTop(lHeight);
				position.SliceTop(EditorGUIUtility.standardVerticalSpacing);
				EditorGUI.LabelField(lrow, label);
			}

			var extraIndent = attr.innerOnly ? 0 : 1;

			EditorGUI.indentLevel += extraIndent;

			int i = -1;
			foreach (var f in _fields)
			{
				i++;
				var prop = property.FindPropertyRelative(f.Name);
				var frow = position.SliceTop(EditorGUI.GetPropertyHeight(prop));
				EditorGUI.PropertyField(frow, prop);
				if (i < _fields.Count - 1)
				{
					position.SliceTop(EditorGUIUtility.standardVerticalSpacing);
				}
			}
			EditorGUI.indentLevel -= extraIndent;
			EditorGUI.EndProperty();
		}

		private IReadOnlyList<FieldInfo> _fields;
		private bool _init;

		private void EnsureInit()
		{
			if (_init)
			{
				return;
			}
			_fields = fieldInfo.FieldType.FindInspectorFields<object>();
		}
	}

}

#endif