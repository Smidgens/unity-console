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
	using Conditional = System.Diagnostics.ConditionalAttribute;

	[Conditional("UNITY_EDITOR")]
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
	public class FieldSizeAttribute : Attribute
	{
		public string Name { get; }
		public float Width { get; } = -1f;
		public int Order { get; set; } = 0;
		public FieldSizeAttribute(string name, float w, int order = 0)
		{
			Name = name;
			Width = w;
			Order = order;
		}
	}

	[Conditional("UNITY_EDITOR")]
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
	public class InlinedAttribute : PropertyAttribute
	{
		public bool ShowArrayLabel { get; set; } = false;
		public string[] Fields { get; private set; } = { };
		public float Padding { get; set; } = 5f;
		public float[] Widths { get; set; } = { };
		public Type FieldsFrom { get; set; } = default;

		public InlinedAttribute() { }

		public InlinedAttribute(params float[] widths)
		{
			Widths = widths;
		}

		public InlinedAttribute(params string[] fields)
		{
			Fields = fields;
		}

		public InlinedAttribute(Type type, params string[] exclude)
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