// smidgens @ github

namespace Smidgenomics.Unity.Console
{
	using UnityEngine;
	using System;
	using System.Reflection;

	[Serializable]
	internal class CommandBindingRef
	{
		public bool IsBound =>
		_handle != CommandHandle.Empty && _console != null;

		public void Bind(IConsole c)
		{
			if (IsBound)
			{
				throw new ConsoleException("Command is already bound");
			}
			BindHandler(c);
		}

		public void Unbind()
		{
			if (!IsBound) { return; }
			_console.Remove(_handle);
			_console = null;
		}

#if UNITY_EDITOR
		internal static class __FP
		{
			public const string
			KEYWORD = nameof(_keyword),
			KEYWORD_MODE = nameof(_keywordMode),
			DESCRIPTION = nameof(_description),

			HANDLER_METHOD =
			nameof(_handler) + "." + SerializedMethod._FN.M_NAME,
			HANDLER_TARGET =
			nameof(_handler) + "." + SerializedMethod._FN.TARGET,
			HANDLER = nameof(_handler);
		}
#endif

		internal string GetKeyword()
		{
			if (_keywordMode == KeywordMode.Custom)
			{
				return _keyword;
			}

			if (_keywordMode == KeywordMode.NameMethod)
			{
				if (!_handler.Target) { return "-"; }
				if (_handler.Name.Length == 0) { return "-"; }
				var fnName = _handler.Name;
				if (fnName.Length > 4 && fnName[3] == '_')
				{
					fnName = fnName.Substring(4);
				}
				return $"{_handler.Target.name}.{fnName}";
			}
			else if (_keywordMode == KeywordMode.NameTypeMethod)
			{
				if (!_handler.Target) { return "-"; }
				if (_handler.Name.Length == 0) { return "-"; }
				var fnName = _handler.Name;
				if (fnName.Length > 4 && fnName[3] == '_')
				{
					fnName = fnName.Substring(4);
				}
				var tt = _handler.Target.GetType().Name;
				return $"{_handler.Target.name}.{tt}.{fnName}";
			}
			return "";
		}

		[SerializeField] private string _keyword = string.Empty;
		[TextArea(1, 4)]
		[SerializeField] private string _description = string.Empty;
		[SerializeField] public SerializedMethod _handler = default;
		[SerializeField] private KeywordMode _keywordMode = default;

		private CommandHandle _handle = CommandHandle.Empty;
		private IConsole _console = null;

		private enum KeywordMode
		{
			Custom,
			[InspectorName("ObName.Method")]
			NameMethod,
			[InspectorName("ObName.Type.Method")]
			NameTypeMethod,
		}

		private void BindHandler(IConsole c)
		{
			var m = _handler.GetMethod();

			if (m == null)
			{
				throw new ConsoleException("Error binding command handler");
			}

			var keyword = GetKeyword();

			if (keyword.Length == 0)
			{
				throw new ConsoleException("Error binding command handler");
			}

			if (m.IsGetOrSet() && m.Name[0] == 's')
			{
				_handle = BindAsProperty(c, keyword, m);
			}
			else
			{
				_handle = BindAsMethod(c, keyword, m);
			}
			_console = c;
		}

		private CommandHandle BindAsProperty(IConsole c, string keyword, MethodInfo m)
		{
			var p = m.GetBackingProperty();
			if (p == null)
			{
				throw new ConsoleException("Error binding command handler");
			}
			return c.Add(keyword, p, _handler.Target, _description);
		}

		private CommandHandle BindAsMethod(IConsole c, string keyword, MethodInfo m)
		{
			return c.Add(keyword, m, _handler.Target, _description);
		}
	}
}


#if UNITY_EDITOR


namespace Smidgenomics.Unity.Console.Editor
{
	using UnityEngine;
	using UnityEditor;
	using SP = UnityEditor.SerializedProperty;

	[CustomPropertyDrawer(typeof(CommandBindingRef))]
	internal class _CommandBindingRef : PropertyDrawer
	{

		public static readonly float LINE_HEIGHT = EditorGUIUtility.singleLineHeight;

		public const byte
		FIELD_COUNT = 4,
		MARGIN_Y = 2,
		FIXED_ROWS = 2;

		public override float GetPropertyHeight(SP prop, GUIContent l)
		{
			if (!_hasInit)
			{
				_init = Init(prop);
				_hasInit = true;
			}

			var margins = MARGIN_Y * FIELD_COUNT;
			var desc = prop.FindPropertyRelative(CommandBindingRef.__FP.DESCRIPTION);
			var descriptionHeight = EditorGUI.GetPropertyHeight(desc, true);

			return
			//_init.fixedHeight
			+ descriptionHeight
			+ _init.keywordHeight
			+ _init.handlerHeight
			+ margins;
		}

		public override void OnGUI(Rect pos, SP prop, GUIContent l)
		{
			var ctx = CreateContext(prop, l);

			EditorGUI.BeginProperty(pos, l, prop);
			{
				var dheight = EditorGUI.GetPropertyHeight(ctx.description);
				var kwRect = pos.SliceTop(_init.keywordHeight);
				KeywordField(kwRect, ctx);
				FieldBottom(ref pos, _init.handlerHeight, ctx.handler);
				FieldBottom(ref pos, dheight, ctx.description);
			}
			EditorGUI.EndProperty();
		}

		private static InitContext _init = default;
		private static bool _hasInit = false;

		private struct InitContext
		{
			public float fixedHeight;
			public float handlerHeight;
			public float keywordHeight;
			public GUIContent kwPrefix;
		}

		private struct DrawerContext
		{
			public SP
			handlerMethod,
			handlerTarget,
			property,
			keyword,
			description,
			handler,
			keywordMode;
		}

		private static DrawerContext CreateContext(SP prop, GUIContent l)
		{
			var ctx = new DrawerContext
			{
				property = prop,
				keyword = prop.FindPropertyRelative(CommandBindingRef.__FP.KEYWORD),
				handler = prop.FindPropertyRelative(CommandBindingRef.__FP.HANDLER),
				description = prop.FindPropertyRelative(CommandBindingRef.__FP.DESCRIPTION),
				keywordMode = prop.FindPropertyRelative(CommandBindingRef.__FP.KEYWORD_MODE),
				handlerTarget = prop.FindPropertyRelative(CommandBindingRef.__FP.HANDLER_TARGET),
				handlerMethod = prop.FindPropertyRelative(CommandBindingRef.__FP.HANDLER_METHOD),
			};
			return ctx;
		}

		private static void KeywordField(in Rect pos, in DrawerContext ctx)
		{
			var r = pos;
			r = EditorGUI.PrefixLabel(r, _init.kwPrefix);

			FieldTop(ref r, LINE_HEIGHT, ctx.keywordMode);
			var lrect = GetLineRectTop(ref r);

			switch (ctx.keywordMode.enumValueIndex)
			{
				case 0:
					EditorGUI.PropertyField(lrect, ctx.keyword, GUIContent.none);
					break;
				case 1:
					MutedBox(lrect, GetAutoKeyword(ctx));
					break;
				case 2:
					MutedBox(lrect, GetAutoKeyword2(ctx));
					break;
				default:
					MutedBox(lrect, "N/A");
					break;
			}
		}

		private static void MutedBox(in Rect pos, string txt)
		{
			var te = GUI.enabled;
			GUI.enabled = false;
			GUI.Box(pos, GUIContent.none, EditorStyles.textArea);
			EditorGUI.LabelField(pos, txt, _MUTED_LABEL.Value);
			GUI.enabled = te;
		}

		private static readonly ILazy<GUIStyle> _MUTED_LABEL = LazyValue.Get<GUIStyle>(() =>
		{
			var s = new GUIStyle(EditorStyles.whiteMiniLabel);
			s.fontSize = 8;
			s.alignment = TextAnchor.MiddleLeft;
			return s;
		});

		private static string GetAutoKeyword(in DrawerContext ctx)
		{
			if (!HandlerIsValid(ctx))
			{
				return "-";
			}
			var hTarget = ctx.handlerTarget.objectReferenceValue;
			var fnName = ctx.handlerMethod.stringValue;

			if (fnName.Length > 4 && fnName[3] == '_')
			{
				fnName = fnName.Substring(4);
			}
			return $"{hTarget.name}.{fnName}";
		}

		private static string GetAutoKeyword2(in DrawerContext ctx)
		{
			if (!HandlerIsValid(ctx))
			{
				return "-";
			}
			var hTarget = ctx.handlerTarget.objectReferenceValue;
			var fnName = ctx.handlerMethod.stringValue;

			if (fnName.Length > 4 && fnName[3] == '_')
			{
				fnName = fnName.Substring(4);
			}
			var tt = hTarget.GetType().Name;
			return $"{hTarget.name}.{tt}.{fnName}";
		}

		private static bool HandlerIsValid(in DrawerContext ctx)
		{
			var ht = ctx.handlerTarget.objectReferenceValue;
			return
			ht
			&& !string.IsNullOrEmpty(ctx.handlerMethod.stringValue);
		}

		private static InitContext Init(SP prop)
		{
			var kw = prop.FindPropertyRelative(CommandBindingRef.__FP.KEYWORD);
			var handler = prop.FindPropertyRelative(CommandBindingRef.__FP.HANDLER);
			var ctx = new InitContext
			{
				kwPrefix = new GUIContent(kw.displayName),
				keywordHeight = (LINE_HEIGHT + 0) * 2,
				handlerHeight = EditorGUI.GetPropertyHeight(handler, true),
				fixedHeight = LINE_HEIGHT * FIXED_ROWS,
			};
			return ctx;
		}

		private static void FieldBottom(ref Rect pos, in float h, SP prop)
		{
			pos.SliceBottom(MARGIN_Y);
			var r = pos.SliceBottom(h);
			EditorGUI.PropertyField(r, prop);
		}

		private static Rect GetLineRectTop(ref Rect pos)
		{
			var r = pos.SliceTop(LINE_HEIGHT);
			pos.SliceTop(MARGIN_Y);
			return r;
		}

		private static void FieldTop(ref Rect pos, in float h, SP prop)
		{
			var r = pos.SliceTop(h);
			pos.SliceTop(MARGIN_Y);
			EditorGUI.PropertyField(r, prop, GUIContent.none);
		}

	}

}


#endif