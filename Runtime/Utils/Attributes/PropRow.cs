// smidgens @ github

#pragma warning disable 0168 // var declared, unused
#pragma warning disable 0219 // var assigned, unused
#pragma warning disable 0414 // private var assigned, unused


namespace Smidgenomics.Unity.Console
{
	using UnityEngine;
	using System;
	using System.Linq;
	using System.Collections.Generic;
	using System.Reflection;

	[AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
	public class FieldColumnAttribute : Attribute
	{
		public string Name { get; }
		public float Width { get; } = -1f;
		public int Order { get; set; } = 0;
		public FieldColumnAttribute(string name, float w, int order = 0)
		{
			Name = name;
			Width = w;
			Order = order;
		}
	}

	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
	public class FieldRowAttribute : PropertyAttribute
	{
		public bool ShowArrayLabel { get; set; } = false;
		public string[] Fields { get; private set; } = { };
		public float Padding { get; set; } = 5f;
		public float[] Widths { get; set; } = { };
		public Type FieldsFrom { get; set; } = default;

		public FieldRowAttribute() { }

		public FieldRowAttribute(params float[] widths)
		{
			Widths = widths;
		}

		public FieldRowAttribute(params string[] fields)
		{
			Fields = fields;
		}

		public FieldRowAttribute(Type type, params string[] exclude)
		{
			Fields =
			FindSerializedFields(type)
			.Where(x => !exclude.Contains(x))
			.ToArray();
			FieldsFrom = type;
		}

		public static float[] ComputeWidths(
			string[] fields,
			float[] widths,
			Dictionary<string, float> keyedWidths = null
		)
		{
			var sl = fields.Select(x => -1f).ToList();
			if (widths != null)
			{
				for (var i = 0; i < Mathf.Min(fields.Length, widths.Length); i++)
				{
					sl[i] = widths[i];
				}
			}

			if (keyedWidths != null)
			{
				foreach (var v in keyedWidths)
				{
					var i = Array.FindIndex(fields, x => x == v.Key);
					if (i < 0) { continue; }
					sl[i] = v.Value;
				}
			}
			NormalizeSizes(sl);
			return sl.ToArray();
		}

		public static IEnumerable<string> FindSerializedFields(Type t)
		{
			var fields =
			t.GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public)
			.Where(x => {
				return !x.IsNotSerialized;
			})
			.Select(x => x.Name);
			return fields;
		}

		private static void NormalizeSizes(List<float> widths)
		{
			if (widths.Count == 0) { return; }
			float rtotal = 0f;
			var ratio = new List<int>();
			var flex = new List<int>();
			for (var i = 0; i < widths.Count; i++)
			{
				var w = widths[i];
				if (w > 1f) { continue; }
				if (w <= 0f) { flex.Add(i); continue; }
				rtotal += w;
				ratio.Add(i);
			}

			float flexRemainder = 1f - rtotal;

			if (flexRemainder > 0f && flex.Count > 0)
			{
				var fw = flexRemainder / flex.Count;
				foreach (var fi in flex) { widths[fi] = fw; }
				rtotal += flexRemainder;
			}
			foreach (var ri in ratio)
			{
				widths[ri] = widths[ri] / rtotal;
			}
		}

	}
}


#if UNITY_EDITOR
namespace Smidgenomics.Unity.Console
{
	using UnityEngine;
	using UnityEditor;
	using System.Linq;
	using System.Collections.Generic;
	using System;
	using System.Reflection;
	using SP = UnityEditor.SerializedProperty;

	[CustomPropertyDrawer(typeof(FieldRowAttribute))]
	class FieldRowAttribute_PD : PropertyDrawer
	{
		public override void OnGUI(Rect pos, SP prop, GUIContent l)
		{
			var a = (FieldRowAttribute)attribute;
			var ftype = fieldInfo.FieldType;

			if(ftype.IsArray)
			{
				ftype = fieldInfo.FieldType.GetElementType();
			}


			if(a.FieldsFrom != null && ftype != a.FieldsFrom)
			{
				var r = pos;
				var c = r.center;
				r.height = EditorGUIUtility.singleLineHeight;
				r.center = c;
				EditorGUI.DrawRect(pos, Color.red * 0.3f);
				EditorGUI.LabelField(r, "Incompatible field type", EditorStyles.centeredGreyMiniLabel);
				return;
			}

			var cattributes = fieldInfo.GetCustomAttributes<FieldColumnAttribute>().ToArray();

			

			var fields = a.Fields;

			if(fields.Length == 0)
			{
				fields = FieldRowAttribute.FindSerializedFields(ftype).ToArray();
			}

			Dictionary<string,float> psizes = null;

			if(cattributes.Length > 0)
			{
				fields = fields
				.OrderBy(f => {
					var ci = Array.FindIndex(cattributes, x => x.Name == f);
					return ci > -1 ? cattributes[ci].Order : 0;
				})
				.ToArray();
				psizes = new Dictionary<string,float>();
				foreach(var fs in cattributes){ psizes[fs.Name] = fs.Width; }
			}

			var sizes = FieldRowAttribute.ComputeWidths(fields, a.Widths, psizes);
			var isArrayItem = prop.propertyPath.Contains("[");

			var tindent = EditorGUI.indentLevel;

			EditorGUI.indentLevel = 0;
			EditorGUI.BeginProperty(pos, l, prop);

			if(a.ShowArrayLabel || (!isArrayItem && l != GUIContent.none))
			{
				pos = EditorGUI.PrefixLabel(pos, l);
			}
		
			var cols = pos.SplitHorizontally((double)a.Padding, sizes);

			var props = FindProps(fields, prop);
			for(var i = 0; i < props.Length; i++)
			{
				if(props[i] == null)
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
#endif

