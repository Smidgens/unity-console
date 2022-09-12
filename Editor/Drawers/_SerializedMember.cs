// smidgens @ github

namespace Smidgenomics.Unity.Console.Editor
{
	using System;
	using UnityEngine;
	using UnityEditor;
	using UnityObject = UnityEngine.Object;
	using SP = UnityEditor.SerializedProperty;
	using System.Reflection;
	using System.Collections.Generic;

	[CustomPropertyDrawer(typeof(SerializedMember))]
	internal class _SerializedMember : PropertyDrawer
	{
		public static class Label
		{
			public const string NO_FN = "No Function";
			public const string MISSING = "<missing>";
			public const string INVALID = "<invalid>";
		}

		public const float LINE_PAD = 0f;
		public const float BOX_PAD = 3f;
		public static readonly float LABEL_HEIGHT =
		EditorGUIUtility.singleLineHeight + 5f;

		public const float ROW_HEIGHT = 20f;
		public const int ROWS = 2;
		public static readonly Color BORDER_COLOR = Color.black * 0.3f;

		public static readonly float DRAWER_HEIGHT =
		LABEL_HEIGHT
		+ BOX_PAD
		+ ROW_HEIGHT * ROWS
		+ (ROWS - 1) * LINE_PAD
		+ LINE_PAD;

		private struct DrawerContext
		{
			public UnityObject TargetRef => target.objectReferenceValue;


			public SP prop, target, mName, mTypes, cacheKey;

			public string buttonLabel;
			public bool missing;
			public Rect rect, labelRect, targetRect, popupRect;

			public MethodInfo LoadMethod()
			{
				if (string.IsNullOrEmpty(mName.stringValue)) { return null; }
				if(mTypes.arraySize < 2) { return null; }
				var types = mTypes.GetStringArray();
				return ConsoleReflection.LoadMethod(mName.stringValue, types);
			}

			public static DrawerContext Init(SP prop)
			{
				var ctx = new DrawerContext
				{
					prop = prop,
					target = prop.FindPropertyRelative(SerializedMember._FN.TARGET),
					mName = prop.FindPropertyRelative(SerializedMember._FN.M_NAME),
					mTypes = prop.FindPropertyRelative(SerializedMember._FN.M_TYPES),
					cacheKey = prop.FindPropertyRelative(SerializedMember._FN.CACHE_KEY),
				};
				ctx.buttonLabel = GetButtonLabel(ctx);
				return ctx;
			}

			private static string GetButtonLabel(in DrawerContext ctx)
			{
				if (ctx.missing) { return Label.MISSING; }

				var targetVal = ctx.target.objectReferenceValue;
				var methodName = ctx.mName;
				var blabel = Label.NO_FN;
				if (targetVal && methodName.stringValue.Length > 0)
				{
					var mn = methodName.stringValue;
					if (mn.Length > 4 && mn[3] == '_')
					{
						mn = mn.Substring(4);
					}
					blabel = mn;
				}
				if (ctx.mTypes.arraySize > 2)
				{
					blabel += ": ";
					for (var i = 2; i < ctx.mTypes.arraySize; i++)
					{
						var item = ctx.mTypes.GetArrayElementAtIndex(i);
						var pt = Type.GetType(item.stringValue, false);
						var tname = pt != null
						? pt.GetNameOrAlias()
						: Label.INVALID;
						blabel += tname;

						if (i < ctx.mTypes.arraySize - 1)
						{
							blabel += ", ";
						}
					}
				}
				return blabel;
			}
		}

		public override float GetPropertyHeight(SP _p, GUIContent _l)
		{
			return DRAWER_HEIGHT;
		}

		public override void OnGUI(Rect pos, SP prop, GUIContent l)
		{
			var ctx = DrawerContext.Init(prop);

			using (new EditorGUI.PropertyScope(pos, l, prop))
			{
				var labelPos = pos;
				labelPos.height = LABEL_HEIGHT;

				var labelLine = pos;
				labelLine.height = 1f;
				labelLine.position += new Vector2(0f, LABEL_HEIGHT);

				var innerBox = pos;
				innerBox.height -= labelPos.height + LINE_PAD;
				innerBox.position += new Vector2(0f, pos.height - innerBox.height);

				innerBox.width -= BOX_PAD * 2f;
				innerBox.position += new Vector2(BOX_PAD, 0f);
				var rows = innerBox.SplitVertically(2, (double)LINE_PAD);

				for (var i = 0; i < rows.Length; i++)
				{
					var c = rows[i].center;
					rows[i].height = EditorGUIUtility.singleLineHeight;
					rows[i].center = c;
				}

				BorderGUI.Border(pos, BORDER_COLOR);
				EditorGUI.DrawRect(labelLine, BORDER_COLOR);
				EditorGUI.LabelField(labelPos.Pad(2f).PadLeft(2f), prop.displayName);
				SelectTarget(rows[0], ctx);
				SelectMethod(rows[1], ctx);
			}
		}

		private static void SelectTarget(in Rect pos, in DrawerContext ctx)
		{
			var prevValue = ctx.TargetRef;
			using(var s = new EditorGUI.ChangeCheckScope())
			{
				EditorGUI.PropertyField(pos.Pad(2f), ctx.target, GUIContent.none);
				if (s.changed)
				{
					var diffType = ctx.TargetRef?.GetType() != prevValue?.GetType();
					if (diffType)
					{
						ctx.mName.stringValue = "";
						ctx.mTypes.arraySize = 0;
					}
					SetTimestamp(ctx.cacheKey);
				}
			}
		}

		private static void SelectMethod(in Rect pos, in DrawerContext ctx)
		{
			using (new EditorGUI.DisabledScope(!ctx.TargetRef))
			{
				var ctxcp = ctx;
				if (GUI.Button(pos, ctx.buttonLabel, EditorStyles.popup))
				{
					var m = MenuFactory.ConsoleCallables
					(
						ctx.TargetRef,
						ctx.LoadMethod(),
						(t, m) => Set(ctxcp, t, m)
					);
					m.DropDown(pos);
				}
			}
		}

		private static void SetTimestamp(SP p) => p.intValue = (int)(DateTime.Now).ToUnix();

		private static void SetStringArray(SP prop, in List<string> l)
		{
			prop.arraySize = l.Count;
			for (var i = 0; i < l.Count; i++)
			{
				prop.GetArrayElementAtIndex(i).stringValue = l[i];
			}
		}

		private static void ClearMethod(ref DrawerContext ctx)
		{
			ctx.mName.stringValue = "";
			ctx.mTypes.arraySize = 0;
			SetTimestamp(ctx.cacheKey);
		}

		private static void Set(DrawerContext ctx, UnityObject t, MethodInfo m)
		{
			var prop = ctx.prop;

			if (m == null)
			{
				ClearMethod(ref ctx);
				prop.serializedObject.ApplyModifiedProperties();
				return;
			}

			var types = new List<string>
			{
				t.GetType().AssemblyQualifiedName,
				m.ReturnType.AssemblyQualifiedName,
			};

			foreach(var p in m.GetParameters())
			{
				types.Add(p.ParameterType.AssemblyQualifiedName);
			}

			ctx.mTypes.arraySize = types.Count;
			ctx.target.objectReferenceValue = t;
			ctx.mName.stringValue = m.Name;
			SetTimestamp(ctx.cacheKey);
			SetStringArray(ctx.mTypes, types);
			prop.serializedObject.ApplyModifiedProperties();
		}
	}
}