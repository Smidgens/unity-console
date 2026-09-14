// smidgens @ github

namespace Smidgenomics.Unity.Console
{
	using System;
	using UnityEngine;
	using Conditional = System.Diagnostics.ConditionalAttribute;

	[Conditional("UNITY_EDITOR")]
	[AttributeUsage(AttributeTargets.Field)]
	internal sealed class ToggleEnumAttribute : PropertyAttribute
	{
		public ToggleEnumAttribute(string label = null)
		{
			this.label = label;
		}
		internal string label { get; }
	}
}


#if UNITY_EDITOR

namespace Smidgenomics.Unity.Console.Editor
{
	using UnityEngine;
	using UnityEditor;
	using System;
	using System.Collections.Generic;
	using System.Reflection;

	[CustomPropertyDrawer(typeof(ToggleEnumAttribute))]
	internal sealed class _ToggleEnum : _BaseDrawer
	{
		protected override float GetHeight(SerializedProperty property, GUIContent label)
		{
			return _values.Count * EditorGUIUtility.singleLineHeight;
		}

		protected override void OnInit(SerializedProperty first, GUIContent l)
		{
			Init();
		}

		protected override void OnDrawGUI(in Rect r, SerializedProperty prop, GUIContent l)
		{
			if (prop.propertyType != SerializedPropertyType.Enum)
			{
				return;
			}

			var pos = r;

			var evalue = prop.intValue;

			if (_isFlags)
			{
				foreach (var (label, v) in _values)
				{
					var active = (evalue & v) != 0;
					var row = pos.SliceTop(EditorGUIUtility.singleLineHeight);
					var na = EditorGUI.ToggleLeft(row, label, active);

					if (na != active)
					{
						if (!na)
						{
							evalue &= ~v;
						}
						else { evalue |= v; }
					}
				}
				
				
				prop.intValue = evalue;
			}
			else
			{
				foreach (var (label, v) in _values)
				{
					var active = prop.enumValueIndex == v;
					var row = pos.SliceTop(EditorGUIUtility.singleLineHeight);
					var na = EditorGUI.ToggleLeft(row, label, active);
					if (na != active)
					{
						prop.enumValueIndex = v;
					}
				}
			}
		}
		
		private List<(string, int)> _values;
		private bool _isFlags;

		private void Init()
		{
			var ftype = fieldInfo.FieldType;
			_isFlags = fieldInfo.FieldType.IsDefined(typeof(FlagsAttribute));
			var labels = Enum.GetNames(ftype);
			var values = Enum.GetValues(ftype) as int[];

			_values = new();

			int i = -1;
			foreach (var v in values!)
			{
				i++;
				if (_isFlags && (!Mathf.IsPowerOfTwo(v) || v == 0))
				{
					continue;
				}
				var dn = GetAttribute<InspectorNameAttribute>(ftype, v);
				_values.Add((dn?.displayName ?? labels[i].ToSentenceCase(), v));
			}
		}

		public static AT GetAttribute<AT>(Type enumType, int value) where AT : Attribute
		{
			var a = enumType.GetField(Enum.GetName(enumType, value)).GetCustomAttribute<AT>();
			return a;
		}

	}
}

#endif

