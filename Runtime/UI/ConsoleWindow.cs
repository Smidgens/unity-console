// smidgens @ github

namespace Smidgenomics.Unity.Console
{
	using System;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.UIElements;

	[UxmlElement("Window", libraryPath = "Console")]
	[Icon("UIToolkit/Icons/Button.png")]
	public sealed partial class ConsoleWindow : VisualElement
	{
		public event Action onClosePressed;

		public bool ShowCloseButton
		{
			get => _closeButton?.visible ?? false;
			set
			{
				if (_closeButton != null)
				{
					_closeButton.visible = value;
				}
			}
		}

		public bool ShowClearButton
		{
			get => _clearButton?.visible ?? false;
			set
			{
				if (_clearButton != null)
				{
					_clearButton.visible = value;
				}
			}
		}

		[UxmlAttribute("console-asset")]
		internal ConsoleAsset Console { get; set; }
		internal VisualElement Toolbar { get; private set; }

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

		public void ResetFilters()
		{
			_filters.ResetFilters();
		}

		public void AddToolbarItem(VisualElement visualElement)
		{
			if (Toolbar == null)
			{
#if SM_DEV
				if (Application.isPlaying)
				{
					Debug.Log("Console toolbar is null in play mode");
					return;
				}
#endif
				// should only be possible outside play mode
				return;
			}
			visualElement.AddToClassList("sm-console__toolbar__item--left");
			var container = Toolbar.Q(name: "LeftControls");
			container.Add(visualElement);
		}

		private bool _moused;
		private TextField _input;
		private ListView _logList;
		private ConsoleButton _closeButton;
		private ConsoleButton _clearButton;
		private ConsoleFilters _filters;
		private readonly InputHistory _inputHistory = new ();
		private readonly List<ConsoleLogItem> _filteredLogs = new();

		private VisualElement _boundRoot;

		private bool _filtersActive;

		private bool IsFiltering()
		{
			return _filtersActive;
		}

		private void BindEvents()
		{
			Console.Log.onLogAdded -= OnLogAdded;
			Console.Log.onLogAdded += OnLogAdded;

			Console.Log.onLogsCleared -= OnLogCleared;
			Console.Log.onLogsCleared += OnLogCleared;

			if (_closeButton != null)
			{
				_closeButton.clicked += OnCloseButton;
			}

			if (_clearButton != null)
			{
				_clearButton.clicked += OnClearButton;
			}

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

			_logList.itemsSource = _filtersActive
			? _filteredLogs
			: Console.Log.LogList;
		}

		private void OnClearButton()
		{
			Console?.Clear();
		}

		private void OnCloseButton()
		{
			onClosePressed?.Invoke();
		}

		private void OnLogCleared()
		{
			if (_filters.Value.IsSet())
			{
				_filteredLogs.Clear();
			}
		}

		private void OnFiltersChanged(in ConsoleLogFilter filter)
		{
			var prevActive = _filtersActive;
			_filtersActive = filter.IsSet();

			if (_filtersActive)
			{
				InitFilteredList();
			}

			if (_filtersActive != prevActive)
			{
				BindList();
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
			_logList.Rebuild();
		}

		private void OnLogAdded(in ConsoleLogItem item)
		{
			if (!visible)
			{
				// TODO: do nothing if invisible and add new items when window is shown
			}
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
				Toolbar = this.Q(name: "Toolbar");
				_input = this.Q<TextField>();
				_logList = this.Q<ListView>();
				_closeButton = this.Q<ConsoleButton>(name: "Close");
				_clearButton = this.Q<ConsoleButton>(name: "Clear");
				_filters = this.Q<ConsoleFilters>();
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
			OnShowConsole();
		}

		private void OnMouseLeave(MouseLeaveEvent ev)
		{
			if (!visible)
			{
				var prevMoused = _moused;
				_moused = false;
				if (prevMoused)
				{
					OnHideConsole();
				}
			}
		}

		private void OnShowConsole()
		{
			FocusInput();
			ScrollToEnd();
		}

		private void OnHideConsole()
		{
			// _logList.dataSource = null;
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
		
	}
}