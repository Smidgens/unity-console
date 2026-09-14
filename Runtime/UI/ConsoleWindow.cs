// smidgens @ github

namespace Smidgenomics.Unity.Console
{
	using System.Collections.Generic;
	using global::Unity.Properties;
	using UnityEngine;
	using UnityEngine.UIElements;

	[UxmlElement("ConsoleWindow", libraryPath = "Console")]
	[Icon("UIToolkit/Icons/Button.png")]
	internal sealed partial class ConsoleWindow : VisualElement
	{
		[UxmlAttribute("console-asset")]
		internal ConsoleAsset Console { get; set; }

		public ConsoleWindow()
		{
			var template = ConsoleResources.GetInstance()._windowTemplate;
			if (!template)
			{
				return;
			}
			template.CloneTree(this);
			RegisterCallback<AttachToPanelEvent>(OnAttached);
			RegisterCallback<DetachFromPanelEvent>(OnDetached);
		}

		private bool _moused;
		private TextField _input;
		private ListView _logList;
		private Button _closeButton;
		private ConsoleLogFilters _filters;
		private readonly InputHistory _inputHistory = new ();
		private readonly List<ConsoleLogItem> _filteredLogs = new();

		private VisualElement _boundRoot;

		private bool _filtersActive;

		private bool IsFiltering()
		{
			return false;
		}

		private void BindEvents()
		{
			Console.Log.onLogAdded -= OnLogAdded;
			Console.Log.onLogAdded += OnLogAdded;

			Console.Log.onLogsCleared -= OnLogCleared;
			Console.Log.onLogsCleared += OnLogCleared;

			_closeButton.clicked += OnClose;
			// cycle input history
			_input.RegisterCallback<KeyDownEvent>(OnInputKeyDown, TrickleDown.TrickleDown);
			// handle submit
			_input.RegisterCallback<NavigationSubmitEvent>(OnSubmit, TrickleDown.TrickleDown);
		}

		private void BindList()
		{
			if (!Console || _logList == null)
			{
				return;
			}
			_logList.bindItem = BindLogItem;
			_logList.itemsSource = Console.Log.LogList;
		}

		private void OnClose()
		{
			// how should this work?
		}

		private void OnLogCleared()
		{
			if (_filtersActive)
			{
				_filteredLogs.Clear();
			}
		}

		private void OnFiltersChanged(in ConsoleLogFilter filter)
		{
			var prevActive = _filtersActive;
			_filtersActive = filter.IsSet();

			if (prevActive && !_filtersActive)
			{
				_filtersActive = false;
				_logList.itemsSource = Console.Log.LogList;
			}

			if (!prevActive && _filtersActive)
			{
				_logList.itemsSource = _filteredLogs;
			}

			if (_filtersActive)
			{
				InitFilteredList();
			}
			
		}

		private void InitFilteredList()
		{
			_filteredLogs.Clear();
			for (int i = 0; i < Console.Log.Length; i++)
			{
				var item = Console.Log.GetItemAt(i);

				if (item.Filter(_filters.Value))
				{
					_filteredLogs.Add(item);
				}
			}
		}

		private void OnLogAdded(in ConsoleLogItem item)
		{
			if (!IsFiltering())
			{
				return;
			}
			if (!item.Filter(_filters.Value))
			{
				return;
			}
			_filteredLogs.Add(item);
		}

		private void OnAttached(AttachToPanelEvent ev)
		{
			if (Application.isPlaying && Console)
			{
				_input = this.Q<TextField>();
				_logList = this.Q<ListView>();
				_closeButton = this.Q<Button>(name: "Close");
				_filters = this.Q<ConsoleLogFilters>();
				_filters.SetConsole(Console);
				_filters.onChange += OnFiltersChanged;

				BindEvents();
				BindList();
				RegisterCallback<MouseEnterEvent>(OnMouseEnter);
				RegisterCallback<MouseLeaveEvent>(OnMouseLeave);
				RegisterCallback<FocusEvent>(OnFocus);
				
				Console.Clear();
				Console.Init();

				if (panel != null)
				{
					_boundRoot = panel.visualTree;
					_boundRoot.styleSheets.Add(ConsoleResources.GetInstance()._rootStyleSheet);
				}
				
			}
		}

		private void OnDetached(DetachFromPanelEvent ev)
		{
			if (Application.isPlaying && Console)
			{
				Console.Log.onLogAdded -= OnLogAdded;
				Console.Log.onLogsCleared -= OnLogCleared;

				if (_boundRoot != null)
				{
					_boundRoot.styleSheets.Remove(ConsoleResources.GetInstance()._rootStyleSheet);
				}
				// UnregisterAllRemovableCallbacks();
			}
		}

		private void OnFocus(FocusEvent ev)
		{
			FocusInput();
			ScrollToEnd();
		}

		private void OnMouseEnter(MouseEnterEvent ev)
		{
			if (_moused)
			{
				return;
			}
			_moused = true;
			FocusInput();
			ScrollToEnd();
		}

		private void OnMouseLeave(MouseLeaveEvent ev)
		{
			if (!visible)
			{
				_moused = false;
			}
		}

		private void OnInputKeyDown(KeyDownEvent ev)
		{
			if (ev.keyCode == KeyCode.UpArrow)
			{
				_input.value = _inputHistory.Back();
				ev.StopPropagation();
				_input.focusController.IgnoreEvent(ev);
			}
			else if (ev.keyCode == KeyCode.DownArrow)
			{
				_input.value = _inputHistory.Forward();
				ev.StopPropagation();
				_input.focusController.IgnoreEvent(ev);
			}
		}

		private void OnSubmit(NavigationSubmitEvent ev)
		{
			ev.StopImmediatePropagation();
			var cmd = _input.value;
			_input.value = string.Empty;
			_input.Focus();
			SubmitCommand(cmd);
		}
		
		private void SubmitCommand(string input)
		{
			input = input.Trim();
			if (input.Length == 0)
			{
				return;
			}
			_inputHistory.Append(input);
			Console.Exec(input);
			ScrollToEnd();
		}

		private void FocusInput()
		{
			_input?.Focus();
		}

		private void ScrollToEnd()
		{
			if (_logList != null && Console && Console.Log.Length > 0)
			{
				_logList.ScrollToItem(Console.Log.Length - 1);
			}
		}

		private void BindLogItem(VisualElement el, int i)
		{
			var item = Console.Log.GetItemAt(i);
			el.dataSource = item;
			el.viewDataKey = i.ToString();
		}
		
	}
}