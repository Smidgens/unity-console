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
		public const byte MARGIN_Y = 2;
		public static readonly float LINE_HEIGHT = EditorGUIUtility.singleLineHeight;

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			OnBeforeGUI();
			if(_totalHeight == 0f)
			{
				for(var i = 0; i < _fields.Count; i++)
				{
					var item = _fields[i];
					var prop = property.FindPropertyRelative(item.Item1);
					if(prop == null) { continue; }
					var height = EditorGUI.GetPropertyHeight(prop, true);
					item.Item2 = height;
					_fields[i] = item;

					_totalHeight += height + MARGIN_Y;
				}

			}
			return
			(LINE_HEIGHT + MARGIN_Y)
			+ _totalHeight;
			//return _rows * EditorGUIUtility.singleLineHeight;
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUI.BeginProperty(position, label, property);

			var rowHeight = position.height / (_rows);

			if(!string.IsNullOrEmpty(_attribute.label))
			{
				label.text = _attribute.label;
			}

			var lrow = SliceField(ref position, LINE_HEIGHT);

			EditorGUI.LabelField(lrow, label);

			EditorGUI.indentLevel++;
			for (var i = 0; i < _fields.Count; i++)
			{
				var (name, height) = _fields[i];

				var frow = SliceField(ref position, height);
				var prop = property.FindPropertyRelative(_fields[i].Item1);
				EditorGUI.PropertyField(frow, prop);
			}
			EditorGUI.indentLevel--;
			EditorGUI.EndProperty();
		}

		private int _rows = 1;
		private List<(string, float)> _fields = null;
		private Action<_Expand> _beforeGUI = Init;
		private ExpandAttribute _attribute = null;

		private float _totalHeight = 0f;

		private static Rect SliceField(ref Rect pos, in float h)
		{
			var r = pos.SliceTop(h);
			pos.SliceTop(MARGIN_Y);
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
			a._attribute = a.attribute as ExpandAttribute;
			a._beforeGUI = NoOp.Action.A1;
		}

		private static List<(string, float)> FindFields(FieldInfo fi)
		{
			var l = new List<(string, float)>();

			foreach (var f in fi.FieldType.GetFields(RFlags.ANY_INSTANCE_MEMBER))
			{
				if (!IsSerialized(f)) { continue; }

				var item = (f.Name, 0f);

				l.Add(item);
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