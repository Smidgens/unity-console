// smidgens @ github

namespace Smidgenomics.Unity.Console.Editor
{
	using UnityEngine;
	using UnityEditor;
	using System.Linq;
	using System.Collections.Generic;
	using System;
	using System.Reflection;
	using SP = UnityEditor.SerializedProperty;

	[CustomPropertyDrawer(typeof(InlinedAttribute))]
	internal class _Inlined : PropertyDrawer
	{
		public override void OnGUI(Rect pos, SP prop, GUIContent l)
		{
			var a = (InlinedAttribute)attribute;
			var ftype = fieldInfo.FieldType;

			if (ftype.IsArray)
			{
				ftype = fieldInfo.FieldType.GetElementType();
			}


			if (a.FieldsFrom != null && ftype != a.FieldsFrom)
			{
				var r = pos;
				var c = r.center;
				r.height = EditorGUIUtility.singleLineHeight;
				r.center = c;
				EditorGUI.DrawRect(pos, Color.red * 0.3f);
				EditorGUI.LabelField(r, "Incompatible field type", EditorStyles.centeredGreyMiniLabel);
				return;
			}

			var cattributes = fieldInfo.GetCustomAttributes<FieldSizeAttribute>().ToArray();

			var fields = a.Fields;

			if (fields.Length == 0)
			{
				fields = InlinedAttribute.FindSerializedFields(ftype).ToArray();
			}

			Dictionary<string, float> psizes = null;

			if (cattributes.Length > 0)
			{
				fields = fields
				.OrderBy(f => {
					var ci = Array.FindIndex(cattributes, x => x.Name == f);
					return ci > -1 ? cattributes[ci].Order : 0;
				})
				.ToArray();
				psizes = new Dictionary<string, float>();
				foreach (var fs in cattributes) { psizes[fs.Name] = fs.Width; }
			}

			var sizes = InlinedAttribute.ComputeWidths(fields, a.Widths, psizes);
			var isArrayItem = prop.propertyPath.Contains("[");

			var tindent = EditorGUI.indentLevel;

			EditorGUI.indentLevel = 0;
			EditorGUI.BeginProperty(pos, l, prop);

			if (a.ShowArrayLabel || (!isArrayItem && l != GUIContent.none))
			{
				pos = EditorGUI.PrefixLabel(pos, l);
			}

			var cols = pos.SplitHorizontally((double)a.Padding, sizes);

			var props = FindProps(fields, prop);
			for (var i = 0; i < props.Length; i++)
			{
				if (props[i] == null)
				{
					EditorGUI.HelpBox(cols[i], "?", MessageType.Error);
					continue;
				}
				EditorGUI.PropertyField(cols[i], props[i], GUIContent.none);
			}
			EditorGUI.EndProperty();
			EditorGUI.indentLevel = tindent;
		}

		private static SP[] FindProps(string[] fields, SP p)
		{
			return fields.Select(x => p.FindPropertyRelative(x)).ToArray();
		}
	}

}