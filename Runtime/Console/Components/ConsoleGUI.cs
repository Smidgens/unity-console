// smidgens @ github

namespace Smidgenomics.Unity.Console
{
	using UnityEngine;
	using System;
	using System.Collections.Generic;

	using LZStyle = System.Lazy<UnityEngine.GUIStyle>;

	[AddComponentMenu(Config.AddComponentMenu.CONSOLE_GUI)]
	internal class ConsoleGUI : MonoBehaviour
	{
		public const double AREA_PADDING = 2.0;
		public const float INPUT_AREA_HEIGHT = 30f;
		public const float HEADER_HEIGHT = 25f;
		public readonly static Color BORDER_COLOR = Color.black * 1f;

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
			this.DelayCall(delay, () => _activeFocus = _inputId);
		}

		public void ClearFocus()
		{
			this.DelayCall(0.2f, () =>
			{
				Utils.SetFocus(null);
				_activeFocus = null;
			});
		}

		[SerializeField] private Console _console = default;
		[SerializeField] private ConsoleTheme _themeOverride = default;

		private static readonly Lazy<ConsoleDefaults>
		_DEFAULTS = new Lazy<ConsoleDefaults>(() => ConsoleDefaults.Instance);

		private readonly static LZStyle
		_inputStyle = new LZStyle(() => _DEFAULTS.Value.Styles.input),
		_textStyle = new LZStyle(() => _DEFAULTS.Value.Styles.text),
		_timestampStyle = new LZStyle(() => _DEFAULTS.Value.Styles.timestamp);

		private ConsoleTheme _Theme => _themeOverride ?? _DEFAULTS.Value.Theme;
		private InputHistory _inputLog = new InputHistory();

		private readonly string _inputId = Guid.NewGuid().ToString();

		private string _inputValue = "";
		private Vector2 _scroll = default;

		private OuterLayout _layout = new OuterLayout();
		private ItemLayout _items = new ItemLayout();
		private string _activeFocus = null;

		private void Awake()
		{
			_items.console = _console;
			_items.textStyle = _textStyle.Value;
			_items.dateStyle = _timestampStyle.Value;
			_items.Init();
			_console.Init();
		}

		private void OnEnable()
		{
			if (!_DEFAULTS.Value)
			{
				enabled = false;
				Debug.Log($"{nameof(ConsoleGUI)}: Missing required Console Resources");
				return;
			}

			if (!_console || !_Theme)
			{
				enabled = false;
				Debug.Log($"{nameof(ConsoleGUI)}: Missing ref to Console or Theme");
				return;
			}
			FocusInput();
		}

		private void OnGUI()
		{
			if(_activeFocus != null)
			{
				Utils.SetFocus(_activeFocus);
			}

			HandleEvents();

			_layout.Update();
			_items.theme = _Theme;
			_items.Update(_layout.Content.width - ScrollbarWidth);

			// draw handlers
			DrawBackground(_layout.Window);

			DrawHeader(_layout.Header);
			DrawLogs(_layout.Content);
			DrawInput(_layout.Input);

			CGUI.BorderTop(_layout.Content, BORDER_COLOR);
			CGUI.BorderTop(_layout.Input, BORDER_COLOR);
		}

		private void CycleInputs(int direction)
		{
			if (direction < 0) { _inputValue = _inputLog.Back(); }
			else if (direction > 0) { _inputValue = _inputLog.Forward(); }
			FocusInput(0.8f);
		}

		private void HandleEvents()
		{
			Utils.HandleKey(KeyCode.Escape, ClearFocus);
			Utils.HandleKey(KeyCode.Return, ConfirmInput);
			Utils.HandleKey(KeyCode.UpArrow, () => CycleInputs(-1));
			Utils.HandleKey(KeyCode.DownArrow, () => CycleInputs(1));
		}

		private void ConfirmInput()
		{
			if(_inputValue.Length == 0) { return; }
			var v = _inputValue.Trim();
			if (v.Length == 0) { return; }
			_inputValue = "";
			_inputLog.Append(v);
			ScrollBottom();
			_console.Process(v);
			FocusInput();
		}

		private void DrawBackground(in Rect pos)
		{
			CGUI.Draw(pos, _Theme.BackgroundColor);
		}

		private void DrawLogs(in Rect pos)
		{
			var size = new Vector2(_items.Width, _items.TotalHeight);
			var content = new Rect(default, size);
			using (var scope = new GUI.ScrollViewScope(pos, _scroll, content, false, true, GUIStyle.none, GUIStyle.none))
			{
				_scroll = scope.scrollPosition;
				_items.Draw();
			}
		}

		private void DrawHeader(in Rect pos)
		{
			//GUIHelper.DrawRect(pos, _headerColor);
		}

		private void DrawInput(in Rect pos)
		{
			GUI.SetNextControlName(_inputId);
			_inputValue = GUI.TextField(pos, _inputValue, _inputStyle.Value);
		}

		private class ItemLayout
		{
			public float Width => _width;
			public float TotalHeight { get; private set; }

			public Console console = default;
			public ConsoleTheme theme = default;

			public GUIStyle
			dateStyle = default,
			textStyle = default;

			private Vector2 _dateSize = default;

			public void Init()
			{
				_dateSize = dateStyle.CalcSize(new GUIContent("[00:00:00]"));
			}

			public void Update(in float width)
			{
				var shouldClear = false;

				if(_id != console.Log.ID)
				{
					shouldClear = true;
				}

				if (width != _width)
				{
					shouldClear = true;
					_width = width;
				}
				if (_items.Count > console.Log.Length) { shouldClear = true; }
				if (shouldClear) { Reset(); }
				if(_items.Count < console.Log.Length) { Regenerate(); }
			}

			public void Draw()
			{
				for(var i = 0; i < _items.Count; i++) { DrawItem(i); }
			}

			private void Reset()
			{
				_items.Clear();
				_id = console.Log.ID;
				TotalHeight = 0f;
			}

			private void DrawItem(int i)
			{
				var item = console.Log.GetItem(i);
				var ditem = _items[i];
				var tc = GUI.contentColor;
				GUI.contentColor = theme.FindColor(item.Type);
				{
					GUI.Label(ditem.itemArea, ditem.label, textStyle);
				}
				GUI.contentColor = tc;
				DrawTimestamp(ditem.dateArea, item.Date);
			}

			private void DrawTimestamp(in Rect pos, in DateTime d)
			{
				var l = d.ToLogTime();
				GUI.Label(pos, l, dateStyle);
			}

			private float _width = 0f;
			private int _id = -1;

			private List<DisplayItem> _items = new List<DisplayItem>();

			private struct DisplayItem
			{
				public GUIContent label;
				public Rect dateArea, itemArea;
			}

			private void Regenerate()
			{
				var log = console.Log;
				var i = _items.Count;
				while(i < log.Length)
				{
					var item = log.GetItem(i);
					var ditem = new DisplayItem
					{
						label = GetFormattedLabel(item.Text, item.Type),
					};

					var stampWidth = _dateSize.x;
					var itemWidth = _width - stampWidth;
					var height = textStyle.CalcHeight(ditem.label, itemWidth);
					var rowOffset = new Vector2(0, TotalHeight);
					var itemPos = rowOffset;
					itemPos.x += stampWidth;
					var size = new Vector2(itemWidth, height);
					var stampSize = new Vector2(stampWidth, height);
					ditem.itemArea = new Rect(itemPos, size);
					ditem.dateArea = new Rect(rowOffset, stampSize);
					_items.Add(ditem);
					TotalHeight += height;
					i++;
				}
			}

			private static GUIContent GetFormattedLabel(string text, int type)
			{
				if(type != 0)
				{
					text = $"<b>{text}</b>";
				}
				return new GUIContent(text);
			}
		}

		private class OuterLayout
		{
			public Rect Window { get; private set; }
			public Rect Header { get; private set; }
			public Rect Content { get; private set; }
			public Rect Input { get; private set; }

			public void Update()
			{
				if(Screen.width == _w && Screen.height == _h)
				{
					return;
				}
				Recalculate();
				_w = Screen.width;
				_h = Screen.height;
			}

			private int _w = -1, _h = -1;

			private static float[] _SIZES =
			{
				HEADER_HEIGHT,
				1f, // scroll, fill space
				INPUT_AREA_HEIGHT,
			};

			private void Recalculate()
			{
				Window = Utils.GetScreenRect();
				var rows = Window.SplitVertically(AREA_PADDING, _SIZES);
				Header = rows[0];
				Content = rows[1];
				Input = rows[2];
			}
		}

		// misc utilities
		private static class Utils
		{
			public static void HandleKey(KeyCode key, Action fn)
			{
				var e = Event.current;
				var ke = e != null && e.type == EventType.KeyDown && e.keyCode == key ? e : null;
				if (ke == null) { return; }
				ke.Use();
				fn.Invoke();
			}

			public static Rect GetScreenRect()
			{
				return new Rect(default, new Vector2(Screen.width, Screen.height));
			}

			public static void SetFocus(string id)
			{
				if (Event.current == null || string.IsNullOrEmpty(id)) { return; }
				GUI.FocusControl(id);
			}

		}
	}
}