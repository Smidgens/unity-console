// smidgens @ github

namespace Smidgenomics.Unity.Console
{
	using UnityEngine;
	using System;
	using System.Collections.Generic;
	using UnityEngine.Events;

	[AddComponentMenu(ConsoleConstants.COMPONENT_ROOT + "Console GUI")]
	[DisallowMultipleComponent]
	[Obsolete("Use Console UI (UI Toolkit)")]
	internal sealed class ConsoleGUI : BaseMono
	{
			
	}

}


//
// namespace Smidgenomics.Unity.Console
// {
// 	using UnityEngine;
// 	using System;
// 	using System.Collections.Generic;
// 	using UnityEngine.Events;
//
// 	[Flags]
// 	internal enum EConsoleGUIFlags
// 	{
// 		All = ~0,
// 		None = 0,
// 		[Obsolete]
// 		Manual = 1,
// 		AutoScroll = 2,
// 		Blocking = 4,
// 		Toolbar = 8,
// 		Default = All
// 	}
//
// 	[AddComponentMenu(ConsoleConstants.COMPONENT_ROOT + "Console GUI")]
// 	[DisallowMultipleComponent]
// 	internal sealed class ConsoleGUI : BaseMono
// 	{
// 		public const float INPUT_AREA_HEIGHT = 50f;
// 		public const int REF_HEIGHT = 1080;
//
// 		public static float ToolbarHeight => 40f * ScreenScale;
// 		public static float ScreenScale => Screen.height / ((float)REF_HEIGHT);
// 		public static float ScrollbarWidth => GUI.skin.verticalScrollbar.fixedWidth * ScreenScale * 2;
//
// 		public void ScrollBottom()
// 		{
// 			var y = _items.TotalHeight;
// 			_scroll = new Vector2(0, y);
// 		}
//
// 		public void FocusInput(float delay = 0.2f)
// 		{
// 			this.SetTimeout(delay, SetInputAsFocus);
// 		}
//
// 		private void SetInputAsFocus() => _activeFocus = _inputId;
//
// 		public void ClearFocus()
// 		{
// 			this.SetTimeout(0.2f, () =>
// 			{
// 				Utils.SetFocus(null);
// 				_activeFocus = null;
// 			});
// 		}
//
// 		public bool HasFlag(EConsoleGUIFlags flag) => _flags.HasFlag(flag);
//
// 		[SerializeField] private ConsoleAsset _console;
// 		[SerializeField] private ConsoleTheme _themeOverride;
// 		[SerializeField] private int _depth = -10;
//
// 		[ToggleEnum]
// 		[SerializeField] private EConsoleGUIFlags _flags = EConsoleGUIFlags.Default;
// 		
// 		[HideInInspector]
// 		[SerializeField] internal UnityEvent<Rect> _onToolbarArea;
//
// 		private static readonly Lazy<ConsoleResources> _RESOURCES = new(ConsoleResources.GetInstance);
//
// 		private LogState _lastLogState;
//
// 		private struct LogState
// 		{
// 			public uint lastId;
// 			public int lastLength;
// 		}
//
// 		private ConsoleStyles _STYLES;
//
// 		private ConsoleTheme _Theme => _themeOverride ?? _RESOURCES.Value.DefaultTheme;
// 		private readonly InputHistory _inputLog = new ();
// 		private readonly string _inputId = Guid.NewGuid().ToString();
// 		private string _inputValue = string.Empty;
// 		private Vector2 _scroll;
// 		private LayoutRects _layout;
// 		private ItemGUI _items;
// 		private string _activeFocus;
// 		
// 		private static class ErrMsg
// 		{
// 			public const string
// 			MISSING_RESOURCES = "Missing required Console Resources",
// 			MISSING_CONSOLE = "Missing reference to Console";
// 		}
//
// 		private readonly struct ConsoleStyles
// 		{
// 			public readonly GUIStyle text, input, date;
// 			public ConsoleStyles(GUIStyle t, GUIStyle i, GUIStyle d)
// 			{
// 				text = GetScaledStyleCopy(t);
// 				input = GetScaledStyleCopy(i);
// 				date = GetScaledStyleCopy(d);
// 			}
// 		}
//
// 		private void Awake()
// 		{
// 			_STYLES = GetStyles();
// 			_items = new ItemGUI
// 			{
// 				console = _console
// 			};
// 			_items.Init(_STYLES);
//
// 			_console.Init();
//
// 			_layout.drawToolbar = _onToolbarArea.GetPersistentEventCount() > 0;
// 		}
//
// 		private void OnEnable()
// 		{
// 			if (!_RESOURCES.Value)
// 			{
// 				enabled = false;
// 				Debug.Log(ErrMsg.MISSING_RESOURCES);
// 				return;
// 			}
//
// 			if (!_console || !_Theme)
// 			{
// 				enabled = false;
// 				Debug.Log(ErrMsg.MISSING_CONSOLE);
// 				return;
// 			}
// 			FocusInput();
// 		}
//
// 		private void Update()
// 		{
// 			CheckScrollState();
// 		}
//
// 		private bool _init;
//
// 		private void OnGUI()
// 		{
// 			if (!_init)
// 			{
// 				_init = true;
// 			}
// 			
// 			GUI.depth = _depth;
// 			OnDraw();
// 			//
// 			// if (Event.current.type != EventType.Layout && Event.current.type != EventType.Repaint)
// 			// {
// 			// 	Event.current.Use();
// 			// }
// 		}
//
// 		private void CheckScrollState()
// 		{
// 			var cl = _console.Log.Length;
// 			var cid = _console.Log.ID;
//
// 			var changed =
// 			_lastLogState.lastLength != cl
// 			|| _lastLogState.lastId != cid;
//
// 			if (changed)
// 			{
// 				_lastLogState.lastId = cid;
// 				_lastLogState.lastLength = cl;
// 			}
//
// 			if(changed && _flags.HasFlag(EConsoleGUIFlags.AutoScroll))
// 			{
// 				ScrollBottom();
// 			}
// 		}
//
// 		private ConsoleStyles GetStyles()
// 		{
// 			var thstyles = _Theme.Styles;
// 			return new ConsoleStyles
// 			(
// 				thstyles.text,
// 				thstyles.input,
// 				thstyles.timestamp
// 			);
// 		}
//
// 		private void OnDraw()
// 		{
// 			if (_activeFocus != null)
// 			{
// 				Utils.SetFocus(_activeFocus);
// 				_activeFocus = null;
// 			}
//
// 			HandleEvents();
//
// 			if (_layout.drawToolbar)
// 			{
// 				_onToolbarArea.Invoke(_layout.Toolbar);
// 			}
//
// 			_layout.Update();
// 			_items.theme = _Theme;
// 			// draw handlers
// 			DrawBackground(_layout.Window);
// 			DrawViewport(_layout.Viewport);
// 			DrawInput(_layout.Input);
// 			DrawScrollbar(_scroll, _layout.Viewport);
// 			CGUI.DrawBorderTop(_layout.Viewport, _Theme.WindowColors.border);
// 			CGUI.DrawBorderTop(_layout.Input, _Theme.WindowColors.border);
// 		}
//
// 		private void DrawBackground(in Rect pos)
// 		{
// 			if (_flags.HasFlag(EConsoleGUIFlags.Blocking))
// 			{
// 				GUI.Button(pos, GUIContent.none, GUIStyle.none);
// 			}
// 			CGUI.DrawRect(pos, _Theme.WindowColors.background);
// 		}
//
// 		private void DrawViewport(in Rect pos)
// 		{
// 			var size = new Vector2(pos.width - ScrollbarWidth, _items.TotalHeight);
// 			var content = new Rect(default, size);
// 			_scroll =
// 			GUI.BeginScrollView(pos, _scroll, content, false, true, GUIStyle.none, GUIStyle.none);
// 			_items.Draw(size, _scroll.y);
// 			GUI.EndScrollView();
// 		}
//
// 		private void DrawInput(in Rect pos)
// 		{
// 			var area = pos;
// 			var ico = area.SliceLeft(area.height);
// 			var c = ico.center;
// 			ico.size *= 0.6f;
// 			ico.center = c;
// 			ConsoleAtlas.Shell.Draw(ico);
// 			GUI.SetNextControlName(_inputId);
// 			_inputValue = GUI.TextField(area, _inputValue, _STYLES.input);
// 		}
//
// 		private void DrawScrollbar(in Vector2 scroll, in Rect pos)
// 		{
// 			var r = pos;
// 			var barArea = r.SliceRight(ScrollbarWidth);
//
// 			var th = _items.TotalHeight;
//
// 			CGUI.DrawBorderLeft(barArea, _Theme.WindowColors.border);
// 			if(th < pos.height)
// 			{
// 				return;
// 			}
// 			var ts = scroll.y / th;
// 			var t = pos.height / th;
// 			var knob = barArea;
// 			knob.height *= t;
// 			knob.y += ts * pos.height;
// 			CGUI.DrawRect(knob, _Theme.WindowColors.scroll);
// 		}
//
// 		private void CycleInputs(int direction)
// 		{
// 			if (direction < 0)
// 			{
// 				_inputValue = _inputLog.Back();
// 			}
// 			else if (direction > 0)
// 			{
// 				_inputValue = _inputLog.Forward();
// 			}
// 			FocusInput(0.8f);
// 		}
//
// 		private void CycleInputPrev() => CycleInputs(-1);
// 		private void CycleInputNext() => CycleInputs(1);
//
// 		private void HandleEvents()
// 		{
// 			Utils.OnKeyboard(KeyCode.Escape, ClearFocus);
// 			Utils.OnKeyboard(KeyCode.Return, ConfirmInput);
// 			Utils.OnKeyboard(KeyCode.UpArrow, CycleInputPrev);
// 			Utils.OnKeyboard(KeyCode.DownArrow, CycleInputNext);
// 		}
//
// 		private void ConfirmInput()
// 		{
// 			if(_inputValue.Length == 0) { return; }
// 			var v = _inputValue.Trim();
// 			if (v.Length == 0) { return; }
// 			_inputValue = string.Empty;
// 			_inputLog.Append(v);
// 			ScrollBottom();
// 			_console.Exec(v);
// 			FocusInput();
// 		}
// 		
// 		/// <summary>
// 		/// Utility for checking if screen size changed
// 		/// </summary>
// 		internal struct ScreenSize
// 		{
// 			public bool Resized()
// 			{
// 				if (Screen.width == _s.Item1 && Screen.height == _s.Item2)
// 				{
// 					return false;
// 				}
// 				_s = (Screen.width, Screen.height);
// 				return true;
// 			}
// 			private (int, int) _s;
// 		}
//
// 		private struct LayoutRects
// 		{
// 			public Rect Window { get; private set; }
// 			public Rect Toolbar { get; private set; }
// 			public Rect Viewport { get; private set; }
// 			public Rect Input { get; private set; }
//
// 			public bool drawToolbar;
//
// 			public void Update()
// 			{
// 				if(_screenSize.Resized())
// 				{
// 					Refresh();
// 				}
// 			}
//
// 			private ScreenSize _screenSize;
//
// 			private void Refresh()
// 			{
// 				Window = new Rect(default, new Vector2(Screen.width, Screen.height));
// 				var r = Window;
// 				if (drawToolbar)
// 				{
// 					Toolbar = r.SliceTop(ToolbarHeight * ScreenScale);
// 				}
// 				Input = r.SliceBottom(INPUT_AREA_HEIGHT * ScreenScale);
// 				Viewport = r;
// 			}
// 		}
// 		
// 		// misc utilities
// 		private static class Utils
// 		{
// 			public static void OnKeyboard(KeyCode key, Action fn)
// 			{
// 				var e = Event.current;
// 				var ke = e is { type: EventType.KeyDown } && e.keyCode == key ? e : null;
// 				if (ke == null) { return; }
// 				ke.Use();
// 				fn.Invoke();
// 			}
//
// 			public static void SetFocus(string id)
// 			{
// 				if (Event.current == null) { return; }
// 				GUI.FocusControl(id);
// 			}
// 		}
//
// 		private static GUIStyle GetScaledStyleCopy(GUIStyle s)
// 		{
// 			var ns = new GUIStyle(s)
// 			{
// 				fontSize = (int)(s.fontSize * ScreenScale)
// 			};
// 			return ns;
// 		}
//
// 		private sealed class ItemGUI
// 		{
// 			public float TotalHeight => GetTotalHeight();
// 			public ConsoleAsset console;
// 			public ConsoleTheme theme;
//
// 			public void Init(in ConsoleStyles styles)
// 			{
// 				_dateStyle = styles.date;
// 				_textStyle = styles.text;
// 				_dateSize = CalcSize(_DATE_FORMAT, _dateStyle);
// 			}
//
// 			public void Draw(in Vector2 size, in float scroll)
// 			{
// 				OnBeforeDraw(size);
//
// 				var itemRect = new Rect(default, new Vector2(size.x, 1f));
// 				var max = scroll + size.y;
//
// 				var si = FindStartIndex(scroll);
//
// 				for (var i = si; i < console.Log.Length; i++)
// 				{
// 					var item = _items[i];
// 					var y = item.offset;
// 					if (y > max) { break; }
// 					var r = itemRect;
// 					r.height = item.height;
// 					r.y = y;
// 					var logItem = console.Log[i];
// 					var dateRect = r.SliceLeft(_dateSize.x);
// 					GUI.Label(dateRect, item.time, _dateStyle);
// 					var color = theme.FindColor((int)logItem.type);
// 					DrawText(r, logItem.text, color);
// 				}
// 			}
//
// 			private const string _DATE_FORMAT = "[00:00:00]";
// 			private static readonly Lazy<GUIContent> _CALC_LABEL = new(() => new GUIContent());
// 			private Vector2 _dateSize;
// 			private readonly List<DisplayItem> _items = new ();
// 			private int _count;
// 			private Vector2 _viewSize;
// 			private GUIStyle _dateStyle, _textStyle;
// 			private uint _logID;
//
// 			private struct DisplayItem
// 			{
// 				public float max => height + offset;
// 				public float height, offset;
// 				public uint logId;
// 				public string time;
// 			}
//
// 			private void OnBeforeDraw(in Vector2 size)
// 			{
// 				if (_logID != console.Log.ID)
// 				{
// 					Reset();
// 					_logID = console.Log.ID;
// 				}
//
// 				if (_viewSize != size)
// 				{
// 					Reset();
// 					_viewSize = size;
// 				}
//
// 				EnsureLength();
// 				EnsureHeights(_viewSize.x - _dateSize.x);
// 			}
//
// 			private void Reset()
// 			{
// 				_count = 0;
// 				_logID = 0;
// 			}
//
// 			private int FindStartIndex(float scroll)
// 			{
// 				for (var i = 0; i < _items.Count; i++)
// 				{
// 					var h = _items[i].height;
// 					var y = _items[i].offset;
// 					if (y >= scroll || y + h >= scroll) { return i; }
// 				}
// 				return 0;
// 			}
//
// 			private float GetTotalHeight()
// 			{
// 				var n = Mathf.Min(console.Log.Length, _items.Count);
// 				if(n == 0) { return 0f; }
// 				return _items[n - 1].max;
// 			}
//
// 			private void EnsureLength()
// 			{
// 				var diff = console.Log.Length - _items.Count;
// 				if (diff < 0) { return; }
// 				for (var i = 0; i < diff; i++)
// 				{
// 					_items.Add(default);
// 				}
// 			}
//
// 			private void EnsureHeights(in float itemWidth)
// 			{
// 				var diff = console.Log.Length - _count;
// 				if (diff <= 0) { return; }
// 				RefreshFromIndex(_count, itemWidth);
// 				_count = console.Log.Length;
// 			}
//
// 			private void RefreshFromIndex(int i, in float itemWidth)
// 			{
// 				float y = 0f;
//
// 				if (i > 0 && _items.Count > 0)
// 				{
// 					var prev = _items[i - 1];
// 					y = prev.max;
// 				}
//
// 				for (; i < console.Log.Length; i++)
// 				{
// 					var litem = console.Log[i];
// 					var h = CalcHeight(litem.text, _textStyle, itemWidth);
// 					var item = _items[i];
// 					item.height = h;
// 					item.offset = y;
// 					if (item.logId != _logID)
// 					{
// 						item.time = litem.timestamp.ToLogTime();
// 						item.logId = _logID;
// 					}
// 					_items[i] = item;
// 					y += h;
// 				}
// 			}
//
// 			private void DrawText(in Rect pos, string txt, Color c)
// 			{
// 				var tc = GUI.contentColor;
// 				GUI.contentColor = c;
// 				{
// 					GUI.Label(pos, txt, _textStyle);
// 				}
// 				GUI.contentColor = tc;
// 			}
//
// 			private static Vector2 CalcSize(string txt, GUIStyle style)
// 			{
// 				var l = _CALC_LABEL.Value;
// 				l.text = txt;
// 				return style.CalcSize(l);
// 			}
//
// 			private static float CalcHeight(string txt, GUIStyle style, float w)
// 			{
// 				var l = _CALC_LABEL.Value;
// 				_CALC_LABEL.Value.text = txt;
// 				return style.CalcHeight(l, w);
// 			}
// 		}
// 	
// 	}
// }
//
// #if UNITY_EDITOR
//
// namespace Smidgenomics.Unity.Console.Editor
// {
// 	using UnityEditor;
//
// 	[CustomEditor(typeof(ConsoleGUI))]
// 	internal sealed class _ConsoleGUI : _Base<ConsoleGUI>
// 	{
// 		public override void OnInspectorGUI()
// 		{
// 			base.OnInspectorGUI();
//
// 			serializedObject.UpdateIfRequiredOrScript();
//
// 			EditorGUILayout.PropertyField(_toolbarProp);
//
// 			serializedObject.ApplyModifiedProperties();
//
// 		}
//
// 		private SerializedProperty _toolbarProp;
// 		
// 		private void OnEnable()
// 		{
// 			_toolbarProp = serializedObject.FindProperty(nameof(ConsoleGUI._onToolbarArea));
// 		}
// 	}
// 	
// 	
// }
//
//
// #endif