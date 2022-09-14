// smidgens @ github

namespace Smidgenomics.Unity.Console.Editor
{
	using UnityEngine;
	using UnityEditor;
	using System;
	using System.Reflection;
	using SP = UnityEditor.SerializedProperty;

	[CustomPropertyDrawer(typeof(ToggleEnumAttribute))]
	internal class _ToggleEnum : _BaseDrawer
	{
		protected override float GetHeight(SP property, GUIContent label)
		{
			return (_LINE_HEIGHT + _MARGIN_Y) * (1 + _init.n);
		}

		protected override void OnInit(SP first, GUIContent l)
		{
			_init = Init(first, this);
		}

		protected override bool ShouldDrawPrefix(SP prop, GUIContent l)
		{
			return false;
		}

		protected override void OnDrawGUI(in Rect r, SP prop, GUIContent l)
		{
			if (prop.propertyType != SerializedPropertyType.Enum) { return; }

			var pos = r;

			if (l != GUIContent.none && !fieldInfo.FieldType.IsArray)
			{
				EditorGUI.LabelField(SliceLine(ref pos), l);
			}

			var ti = EditorGUI.indentLevel;
			EditorGUI.indentLevel = 0;
			var vi = 1;
			pos.SliceLeft(_INDENT * (ti + 1));
			var evalue = prop.intValue;

			if (_init.isFlags)
			{
				for (var i = 0; i < _init.n; i++)
				{
					var ltxt = _init.labels[i + 2];
					var active = (evalue & vi) != 0;
					var row = SliceLine(ref pos);
					var na = EditorGUI.ToggleLeft(row, ltxt, active);

					if (na != active)
					{
						if (!na)
						{
							evalue &= ~vi;
						}
						else { evalue |= vi; }
					}
					vi = vi * 2;
				}
				prop.intValue = evalue;
			}
			else
			{
				for (var i = 0; i < _init.n; i++)
				{
					var ltxt = _init.labels[i];
					var active = prop.enumValueIndex == i;
					var row = SliceLine(ref pos);
					var na = EditorGUI.ToggleLeft(row, ltxt, active);
					if (na != active)
					{
						prop.enumValueIndex = i;
					}
				}
			}
			EditorGUI.indentLevel = ti;
		}

		private InitContext _init = default;

		private struct InitContext
		{
			public FlagsAttribute flags;
			public string[] labels;
			public int n;
			public bool isFlags;
		}

		private static InitContext Init(SP prop, _ToggleEnum d)
		{
			var ftype = d.fieldInfo.FieldType;
			var isFlags = d.fieldInfo.FieldType.IsDefined(typeof(FlagsAttribute));
			var labels = prop.enumDisplayNames;
			var n = labels.Length;

			var values = Enum.GetValues(ftype) as int[];

			if (isFlags) { n -= 2; }

			var labelstart = 0;
			var labelend = values.Length;

			if (isFlags)
			{
				labelstart += 1;
				labelend -= 1;
			}

			for (var i = labelstart; i < labelend; i++)
			{
				var val = values[i];
				var la = GetAttribute<InspectorNameAttribute>(ftype, val);
				if (la != null)
				{
					labels[i + labelstart] = la.displayName;
				}
			}

			return new InitContext
			{
				isFlags = isFlags,
				labels = labels,
				n = n,
				flags = d.fieldInfo.GetCustomAttribute<FlagsAttribute>()
			};
		}

		public static AT GetAttribute<AT>(Type enumType, int value)
		where AT : Attribute
		{
			var name = Enum.GetName(enumType, value);
			var a = enumType.GetField(name).GetCustomAttribute<AT>();
			return a;
		}

	}
}