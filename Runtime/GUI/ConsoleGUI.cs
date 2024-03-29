// smidgens @ github

namespace Smidgenomics.Unity.Console
{
	using UnityEngine;
	using System;
	using System.Collections.Generic;
	using UnityEngine.Events;

	internal class IconAtlas
	{
		private const string RES_PATH = "smidgenomics.console/{icons}";
		public AtlasSprite shell { get; private set; } = default;
		public static IconAtlas Instance => _getter.Invoke();
		private IconAtlas() { }
		private static Func<IconAtlas> _getter = GetInstanceInit;
		private static IconAtlas _instance = null;

		private static IconAtlas GetInstance() => _instance;
		private static IconAtlas GetInstanceInit()
		{
			var tex = Resources.Load<Texture>(RES_PATH);
			_instance = new IconAtlas
			{
				shell = new AtlasSprite(tex, Vector2.one, default)
			};
			_getter = GetInstance;
			return _instance;
		}
	}

	[AddComponentMenu(Config.AddComponentMenu.CONSOLE_GUI)]
	internal partial class ConsoleGUI : MonoBehaviour
	{
		public const double AREA_PADDING = 2.0;
		public const float INPUT_AREA_HEIGHT = 50f;
		public const float TOOLBAR_HEIGHT = 40f;
		public const float SCROLL_BAR_WIDTH = 12f;
		public const float AUTO_SCROLL_THRESHOLD = 50f;

		public void Draw()
		{
			OnDraw();
		}

		public static float ScrollbarWidth
		{
			get => GUI.skin.verticalScrollbar.fixedWidth + 5f;
		}

		public void ScrollBottom()
		{
			var y = _items.TotalHeight;
			_scroll = new Vector2(0, y);
		}

		public void FocusInput(float delay = 0.2f)
		{
			this.SetTimeout(delay, SetInputAsFocus);
		}

		private void SetInputAsFocus() => _activeFocus = _inputId;

		public void ClearFocus()
		{
			this.SetTimeout(0.2f, () =>
			{
				Utils.SetFocus(null);
				_activeFocus = null;
			});
		}

		[SerializeField] private ConsoleAsset _console = default;
		[SerializeField] private ConsoleTheme _themeOverride = default;
		[SerializeField] private int _depth = 1;
		[SerializeField] private bool _manualMode = false;
		[SerializeField] private bool _autoScroll = true;
		[SerializeField] private bool _blocking = false;
		[SerializeField] private UnityEvent<Rect> _onToolbarArea = default;

		private static readonly ILazy<ConsoleResources>
		_RESOURCES = LazyValue.Get(ConsoleResources.GetInstance);

		private LogState _lastLogState;


		private struct LogState
		{
			public uint lastId;
			public int lastLength;
		}

		private ConsoleStyles _STYLES = default;

		private ConsoleTheme _Theme => _themeOverride ?? _RESOURCES.Value.DefaultTheme;
		private InputHistory _inputLog = new InputHistory();
		private string _inputId = Guid.NewGuid().ToString();
		private string _inputValue = "";
		private Vector2 _scroll = default;
		private LayoutRects _layout = new LayoutRects();
		private ItemGUI _items = null;
		private string _activeFocus = null;
		private Action _guiFn = NoOp.Action.A0;

		private List<IConsoleWidget> _widgets = new List<IConsoleWidget>();

		private delegate void RectFn(in Rect r);

		private RectFn _toolbarAreaFn = NoOpRect;

		private class Icons
		{
			public AtlasSprite shell;
		}

		private static class ErrMsg
		{
			public const string
			MISSING_RESOURCES = "Missing required Console Resources",
			MISSING_CONSOLE = "Missing reference to Console";
		}

		private readonly struct ConsoleStyles
		{
			public readonly GUIStyle text, input, date;
			public ConsoleStyles(GUIStyle t, GUIStyle i, GUIStyle d)
			{
				text = t;
				input = i;
				date = d;
			}
		}

		private void Awake()
		{
			if(_onToolbarArea.GetPersistentEventCount() > 0)
			{
				_toolbarAreaFn = InvokeToolbarHandler;
			}

			_STYLES = GetStyles();

			foreach (var w in GetComponentsInChildren<IConsoleWidget>())
			{
				_widgets.Add(w);
			}
			Sort.ByOrder(_widgets);

			_items = new ItemGUI();
			_items.console = _console;
			_items.Init(_STYLES);

			_console.Init();

			if (!_manualMode)
			{
				_guiFn = OnDraw;
			}
		}

		private void OnEnable()
		{
			if (!_RESOURCES.Value)
			{
				enabled = false;
				Debug.Log(ErrMsg.MISSING_RESOURCES);
				return;
			}

			if (!_console || !_Theme)
			{
				enabled = false;
				Debug.Log(ErrMsg.MISSING_CONSOLE);
				return;
			}
			FocusInput();
		}

		private void Update()
		{
			CheckScrollState();
		}

		private void OnGUI()
		{
			GUI.depth = _depth;
			_guiFn.Invoke();
		}

		private static void NoOpRect(in Rect r) { }

		private void InvokeToolbarHandler(in Rect r)
		{
			_onToolbarArea.Invoke(r);
		}

		private void CheckScrollState()
		{
			var cl = _console.Log.Length;
			var cid = _console.Log.ID;

			var changed =
			_lastLogState.lastLength != cl
			|| _lastLogState.lastId != cid;

			if (changed)
			{
				_lastLogState.lastId = cid;
				_lastLogState.lastLength = cl;
			}

			if(changed && _autoScroll)
			{
				ScrollBottom();
			}
		}

		private ConsoleStyles GetStyles()
		{
			var thstyles = _Theme.Styles;
			return new ConsoleStyles
			(
				thstyles.text,
				thstyles.input,
				thstyles.timestamp
			);
		}

		private void OnDraw()
		{
			if (_activeFocus != null)
			{
				Utils.SetFocus(_activeFocus);
				_activeFocus = null;
			}

			HandleEvents();

			_layout.Update();
			_items.theme = _Theme;
			// draw handlers
			DrawBackground(_layout.Window);
			DrawToolbar(_layout.Toolbar);
			DrawViewport(_layout.Viewport);
			DrawInput(_layout.Input);
			DrawScrollbar(_scroll, _layout.Viewport);
			BorderGUI.BorderTop(_layout.Viewport, _Theme.WindowColors.border);
			BorderGUI.BorderTop(_layout.Input, _Theme.WindowColors.border);
		}

		private void DrawBackground(in Rect pos)
		{
			if (_blocking)
			{
				GUI.Button(pos, "", GUIStyle.none);
			}
			CGUI.Color(pos, _Theme.WindowColors.background);
		}

		private void DrawViewport(in Rect pos)
		{
			var size = new Vector2(pos.width - ScrollbarWidth, _items.TotalHeight);
			var content = new Rect(default, size);
			_scroll =
			GUI.BeginScrollView(pos, _scroll, content, false, true, GUIStyle.none, GUIStyle.none);
			_items.Draw(size, _scroll.y);
			GUI.EndScrollView();
		}

		private void DrawToolbar(in Rect pos)
		{
			_toolbarAreaFn.Invoke(pos);

			var area = pos;
			foreach(var widget in _widgets)
			{
				if (widget == null) { continue; }

				if (!widget.Enabled) { continue; }
				var placement = widget.Placement;
				if (placement == 0) { continue; }

				var w = widget.GetWidth(pos.height);

				var r = placement < 0
				? area.SliceLeft(w)
				: area.SliceRight(w);

				widget.OnWidgetGUI(r);

				if (placement < 0)
				{
					CGUI.Color(area.SliceLeft(1f), _Theme.WindowColors.border);
					//BorderGUI.BorderRight(r, _Theme.WindowColors.border);
				}
				else
				{
					CGUI.Color(area.SliceRight(1f), _Theme.WindowColors.border);
					//BorderGUI.BorderLeft(r, _Theme.WindowColors.border);
				}
			}
		}

		private void DrawInput(in Rect pos)
		{
			var area = pos;
			var ico = area.SliceLeft(area.height);
			var c = ico.center;
			ico.size *= 0.6f;
			ico.center = c;
			IconAtlas.Instance.shell.Draw(ico);
			GUI.SetNextControlName(_inputId);
			_inputValue = GUI.TextField(area, _inputValue, _STYLES.input);
		}

		private void DrawScrollbar(in Vector2 scroll, in Rect pos)
		{
			var r = pos;
			var barArea = r.SliceRight(SCROLL_BAR_WIDTH);

			var th = _items.TotalHeight;

			BorderGUI.BorderLeft(barArea, _Theme.WindowColors.border);
			if(th < pos.height)
			{
				return;
			}
			var ts = scroll.y / th;
			var t = pos.height / th;
			var knob = barArea;
			knob.height *= t;
			knob.y += ts * pos.height;
			CGUI.Color(knob, _Theme.WindowColors.scroll);
		}

		private void CycleInputs(int direction)
		{
			if (direction < 0) { _inputValue = _inputLog.Back(); }
			else if (direction > 0) { _inputValue = _inputLog.Forward(); }
			FocusInput(0.8f);
		}

		private void CycleInputPrev() => CycleInputs(-1);
		private void CycleInputNext() => CycleInputs(1);

		private void HandleEvents()
		{
			Utils.OnKeyboard(KeyCode.Escape, ClearFocus);
			Utils.OnKeyboard(KeyCode.Return, ConfirmInput);
			Utils.OnKeyboard(KeyCode.UpArrow, CycleInputPrev);
			Utils.OnKeyboard(KeyCode.DownArrow, CycleInputNext);
		}

		private void ConfirmInput()
		{
			if(_inputValue.Length == 0) { return; }
			var v = _inputValue.Trim();
			if (v.Length == 0) { return; }
			_inputValue = string.Empty;
			_inputLog.Append(v);
			ScrollBottom();
			_console.Exec(v);
			FocusInput();
		}

		private struct LayoutRects
		{
			public Rect Window { get; private set; }
			public Rect Toolbar { get; private set; }
			public Rect Viewport { get; private set; }
			public Rect Input { get; private set; }

			public void Update()
			{
				if(_screenSize.Resized())
				{
					Refresh();
				}
			}

			private ScreenSize _screenSize;

			private void Refresh()
			{
				Window = new Rect(default, new Vector2(Screen.width, Screen.height));
				var r = Window;
				Toolbar = r.SliceTop(TOOLBAR_HEIGHT);
				Input = r.SliceBottom(INPUT_AREA_HEIGHT);
				Viewport = r;
			}
		}
	}
}


namespace Smidgenomics.Unity.Console
{
	using UnityEngine;
	using System.Collections.Generic;

	internal partial class ConsoleGUI : MonoBehaviour
	{
		private class ItemGUI
		{
			public float TotalHeight => GetTotalHeight();
			public ConsoleAsset console = default;
			public ConsoleTheme theme = default;

			public void Init(in ConsoleStyles styles)
			{
				_dateStyle = styles.date;
				_textStyle = styles.text;
				_dateSize = CalcSize(_DATE_FORMAT, _dateStyle);
			}

			public void Draw(in Vector2 size, in float scroll)
			{
				OnBeforeDraw(size);

				var itemRect = new Rect(default, new Vector2(size.x, 1f));
				var max = scroll + size.y;

				var si = FindStartIndex(scroll);

				for (var i = si; i < console.Log.Length; i++)
				{
					var item = _items[i];
					var y = item.offset;
					if (y > max) { break; }
					var r = itemRect;
					r.height = item.height;
					r.y = y;
					var logItem = console.Log[i];
					var dateRect = r.SliceLeft(_dateSize.x);
					GUI.Label(dateRect, item.time, _dateStyle);
					var color = theme.FindColor(logItem.Type);
					DrawText(r, logItem.Text, color);
				}
			}

			private const string _DATE_FORMAT = "[00:00:00]";
			private static readonly ILazy<GUIContent>
			_CALC_LABEL = LazyValue.Get(Empty.New<GUIContent>);
			private Vector2 _dateSize = default;
			private List<DisplayItem> _items = new List<DisplayItem>();
			private int _count = 0;
			private Vector2 _viewSize = default;
			private GUIStyle _dateStyle = default, _textStyle = default;
			private uint _logID = 0;

			private struct DisplayItem
			{
				public float max => height + offset;
				public float height, offset;
				public uint logId;
				public string time;
			}

			private void OnBeforeDraw(in Vector2 size)
			{
				if (_logID != console.Log.ID)
				{
					Reset();
					_logID = console.Log.ID;
				}

				if (_viewSize != size)
				{
					Reset();
					_viewSize = size;
				}

				EnsureLength();
				EnsureHeights(_viewSize.x - _dateSize.x);
			}

			private void Reset()
			{
				_count = 0;
				_logID = 0;
			}

			private int FindStartBS(in float scroll)
			{
				if(console.Log.Length == 0) { return 0; }
				if(scroll == 0) { return 0; }
				var low = 0;
				var high = console.Log.Length - 1;

				if(high < 2) { return 0; }

				// 0,1,2

				var mid = 0;

				while(low != high)
				{
					mid = (low + high) / 2;
					var current = _items[mid];
					var y = current.offset;
		
					var max = y + current.height;

					if(y <= scroll && scroll < max)
					{
						return mid;
					}
					if(scroll > max)
					{
						low = mid + 1;
					}
					else
					{
						high = mid - 1;
					}
				}
				return mid > 0 ? mid - 1 : mid;
			}

			private int Compare(in DisplayItem a, in DisplayItem b)
			{
				return 0;
			}

			private int FindStartIndex(float scroll)
			{
				for (var i = 0; i < _items.Count; i++)
				{
					var h = _items[i].height;
					var y = _items[i].offset;
					if (y >= scroll || y + h >= scroll) { return i; }
				}
				return 0;
			}

			private float GetTotalHeight()
			{
				var n = Mathf.Min(console.Log.Length, _items.Count);
				if(n == 0) { return 0f; }
				return _items[n - 1].max;
			}

			private void EnsureLength()
			{
				var diff = console.Log.Length - _items.Count;
				if (diff < 0) { return; }
				for (var i = 0; i < diff; i++)
				{
					_items.Add(default);
				}
			}

			private void EnsureHeights(in float itemWidth)
			{
				var diff = console.Log.Length - _count;
				if (diff <= 0) { return; }
				RefreshFromIndex(_count, itemWidth);
				_count = console.Log.Length;
			}

			private void RefreshFromIndex(int i, in float itemWidth)
			{
				float y = 0f;

				if (i > 0 && _items.Count > 0)
				{
					var prev = _items[i - 1];
					y = prev.max;
				}

				for (; i < console.Log.Length; i++)
				{
					var litem = console.Log[i];
					var h = CalcHeight(litem.Text, _textStyle, itemWidth);
					var item = _items[i];
					item.height = h;
					item.offset = y;
					if (item.logId != _logID)
					{
						item.time = litem.Time.ToLogTime();
						item.logId = _logID;
					}
					_items[i] = item;
					y += h;
				}
			}

			private void DrawText(in Rect pos, string txt, Color c)
			{
				var tc = GUI.contentColor;
				GUI.contentColor = c;
				{
					GUI.Label(pos, txt, _textStyle);
				}
				GUI.contentColor = tc;
			}

			private static Vector2 CalcSize(string txt, GUIStyle style)
			{
				var l = _CALC_LABEL.Value;
				l.text = txt;
				return style.CalcSize(l);
			}

			private static float CalcHeight(string txt, GUIStyle style, float w)
			{
				var l = _CALC_LABEL.Value;
				_CALC_LABEL.Value.text = txt;
				return style.CalcHeight(l, w);
			}
		}
	}
}


namespace Smidgenomics.Unity.Console
{
	using System;
	using UnityEngine;

	internal partial class ConsoleGUI : MonoBehaviour
	{
		// misc utilities
		private static class Utils
		{
			public static void OnKeyboard(KeyCode key, Action fn)
			{
				var e = Event.current;
				var ke = e != null && e.type == EventType.KeyDown && e.keyCode == key ? e : null;
				if (ke == null) { return; }
				ke.Use();
				fn.Invoke();
			}

			public static void SetFocus(string id)
			{
				if (Event.current == null) { return; }
				GUI.FocusControl(id);
			}
		}
	}
}