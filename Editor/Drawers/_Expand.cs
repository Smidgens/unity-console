// smidgens @ github

namespace Smidgenomics.Unity.Console.Editor
{
	using System;
	using UnityEditor;
	using UnityEngine;
	using System.Reflection;
	using System.Collections.Generic;

	[CustomPropertyDrawer(typeof(ExpandAttribute))]
	internal class _Expand : PropertyDrawer
	{
		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			OnBeforeGUI();
			return _rows * EditorGUIUtility.singleLineHeight;
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUI.BeginProperty(position, label, property);

			var rowHeight = position.height / (_rows);

			EditorGUI.LabelField(GetRow(position, 0, rowHeight), label);
			EditorGUI.indentLevel++;

			var labelOffset = 1;

			for (var i = 0; i < _fields.Count; i++)
			{
				var r = GetRow(position, i + labelOffset, rowHeight);
				var prop = property.FindPropertyRelative(_fields[i]);
				EditorGUI.PropertyField(r, prop);
			}
			EditorGUI.indentLevel--;
			EditorGUI.EndProperty();
		}

		private int _rows = 1;
		private List<string> _fields = null;
		private Action<_Expand> _beforeGUI = Init;

		private Rect GetRow(in Rect pos, int i, float h)
		{
			var r = pos;
			r.position += new Vector2(0f, h * i);
			r.height = h;
			return r;
		}

		private void OnBeforeGUI()
		{
			_beforeGUI.Invoke(this);
		}

		private static void Init(_Expand a)
		{
			a._fields = FindFields(a.fieldInfo);
			a._rows = a._fields.Count + 1;
			a._beforeGUI = NoOp.Action.A1;
		}

		private static List<string> FindFields(FieldInfo fi)
		{
			var l = new List<string>();

			foreach (var f in fi.FieldType.GetFields(RFlags.ANY_INSTANCE_MEMBER))
			{
				if (!IsSerialized(f)) { continue; }
				l.Add(f.Name);
			}
			return l;
		}

		private static bool IsSerialized(FieldInfo m)
		{
			if (m.IsNotSerialized) { return false; }
			if (m.GetCustomAttribute<HideInInspector>() != null) { return false; }

			if (!m.IsPublic)
			{
				if (m.GetCustomAttribute<SerializeField>() == null) { return false; }
			}
			if (m.GetCustomAttribute<NonSerializedAttribute>() != null) { return false; }
			return true;
		}
	}

}