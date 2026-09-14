// smidgens @ github

namespace Smidgenomics.Unity.Console
{
	using UnityEngine;
	using System;
	using System.Reflection;
	using UnityObject = UnityEngine.Object;

	[Serializable]
	internal struct StringifiedMethod
	{
		public string name;
		// [type, return, params]
		public string[] types;

		public MethodInfo LoadAsMethod()
		{
			if (string.IsNullOrEmpty(name)) { return null; }

			if (types.Length < 2) { return null; }

			var ownerType = Type.GetType(types[0], false);
			var returnType = Type.GetType(types[1], false);

			if (ownerType == null || returnType == null)
			{
				return null;
			}

			var ptypes = new Type[types.Length - 2];

			for (var i = 0; i < ptypes.Length; i++)
			{
				ptypes[i] = Type.GetType(types[i + 2], false);
			}
			return ownerType.GetMethod(name, RFlags.ANY_INSTANCE_MEMBER, null, ptypes, null);

		}
	}

	[Serializable]
	internal sealed class SerializedMethod
	{
		public string Name => _member.name;
		public UnityObject Target => _target;

		public MethodInfo GetMethod()
		{
			return GetMethodCached();
		}

		[SerializeField] internal int _cacheKey; // cache helper
		[SerializeField] internal UnityObject _target;
		[SerializeField] internal StringifiedMethod _member;

		// key, method, init
		private (int, MethodInfo, bool) _cache;

		private MethodInfo GetMethodCached()
		{
			var (ckey,cmethod,cinit) = _cache;
			// clear cache?
			if (ckey != _cacheKey) { _cache = default; }

			// cache valid?
			if (cinit) { return cmethod; }

			// build cache
			ckey = (int)DateTime.Now.ToUnix();
			_cache = (ckey, LoadMethod(), true);

			return _cache.Item2;
		}

		private MethodInfo LoadMethod()
		{
			return LoadMethod(_member.name, _member.types);
		}
		
		internal static MethodInfo LoadMethod(string mname, string[] mtypes)
		{
			if (string.IsNullOrEmpty(mname))
			{
				return null;
			}

			if (mtypes.Length < 2)
			{
				return null;
			}

			var ownerType = Type.GetType(mtypes[0], false);
			var returnType = Type.GetType(mtypes[1], false);

			if (ownerType == null || returnType == null)
			{
				return null;
			}

			var pptypes = new Type[mtypes.Length - 2];

			for (var i = 2; i < mtypes.Length; i++)
			{
				pptypes[i - 2] = Type.GetType(mtypes[i], false);
			}
			return ownerType.GetMethod(mname, RFlags.ANY_INSTANCE_MEMBER, null, pptypes, null);
		}
	}
}

#if UNITY_EDITOR

namespace Smidgenomics.Unity.Console.Editor
{
	using System;
	using UnityEngine;
	using UnityEditor;
	using UnityObject = UnityEngine.Object;
	using SP = UnityEditor.SerializedProperty;
	using System.Reflection;
	using System.Collections.Generic;

	[CustomPropertyDrawer(typeof(SerializedMethod))]
	internal sealed class _SerializedMethod : PropertyDrawer
	{
		public const string FN_UNSET = "No Function";
		public const string FN_MISSING = "<missing>";
		public const string FN_INVALID = "<invalid>";
		
		private struct DrawerContext
		{
			public UnityObject TargetRef => target.objectReferenceValue;

			public SP prop, target, mName, mTypes, cacheKey;

			public string buttonLabel;
			public bool missing;
			public Rect rect, labelRect, targetRect, popupRect;

			public readonly MethodInfo LoadMethod()
			{
				if (string.IsNullOrEmpty(mName.stringValue))
				{
					return null;
				}

				if (mTypes.arraySize < 2)
				{
					return null;
				}
				var types = mTypes.GetArrayStringValues();
				return SerializedMethod.LoadMethod(mName.stringValue, types);
			}

			public static DrawerContext Init(SP prop)
			{
				var ctx = new DrawerContext
				{
					prop = prop,
					target = prop.FindPropertyRelative(nameof(SerializedMethod._target)),
					mName = prop.FindPropertyRelative(nameof(SerializedMethod._member) + "." + nameof(StringifiedMethod.name)),
					mTypes = prop.FindPropertyRelative(nameof(SerializedMethod._member) + "." + nameof(StringifiedMethod.types)),
					cacheKey = prop.FindPropertyRelative(nameof(SerializedMethod._cacheKey)),
				};
				ctx.buttonLabel = GetButtonLabel(ctx);
				return ctx;
			}

			private static string GetButtonLabel(in DrawerContext ctx)
			{
				if (ctx.missing)
				{
					return FN_MISSING;
				}

				var targetVal = ctx.target.objectReferenceValue;
				var methodName = ctx.mName;
				var blabel = FN_UNSET;
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
						: FN_INVALID;
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
			var rowCount = 3;
			var h = 0f;
			h += EditorStyles.helpBox.padding.bottom + EditorStyles.helpBox.padding.top;
			h += EditorGUIUtility.singleLineHeight * rowCount;
			h += (rowCount - 1) * EditorGUIUtility.standardVerticalSpacing;
			return h;
		}

		public override void OnGUI(Rect pos, SP prop, GUIContent l)
		{
			var ctx = DrawerContext.Init(prop);

			using (new EditorGUI.PropertyScope(pos, l, prop))
			{
				GUI.Box(pos, GUIContent.none, EditorStyles.helpBox);
				var innerBox = pos;
				innerBox.height -= EditorStyles.helpBox.padding.bottom + EditorStyles.helpBox.padding.top;
				innerBox.width -= EditorStyles.helpBox.padding.left + EditorStyles.helpBox.padding.right;
				innerBox.center = pos.center;

				var (lRect, r1, r2) =
				innerBox.SliceRows3(EditorGUIUtility.singleLineHeight, EditorGUIUtility.standardVerticalSpacing);
				
				EditorGUI.LabelField(lRect, prop.displayName);
				SelectTarget(r1, ctx);
				SelectMethod(r2, ctx);
			}
		}

		private static void SelectTarget(in Rect pos, in DrawerContext ctx)
		{
			var prevValue = ctx.TargetRef;
			using(var s = new EditorGUI.ChangeCheckScope())
			{
				EditorGUI.PropertyField(pos, ctx.target, GUIContent.none);
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
				if (EditorGUI.DropdownButton(pos, new GUIContent(ctx.buttonLabel), FocusType.Keyboard))
				{
					var m = MenuFactory.CallableMethods
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
			ctx.mName.stringValue = string.Empty;
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

#endif