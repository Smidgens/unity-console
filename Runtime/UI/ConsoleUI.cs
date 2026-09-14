// smidgens @ github

namespace Smidgenomics.Unity.Console
{
	using UnityEngine;
	using System;
	using System.Collections.Generic;
	using UnityEngine.Events;
	using UnityEngine.UIElements;

	/// <summary>
	/// UI toolkit replacement of Console GUI script
	/// </summary>
	[AddComponentMenu(ConsoleConstants.COMPONENT_ROOT + "Console UI")]
	[DisallowMultipleComponent]
	[RequireComponent(typeof(UIDocument))]
	[Obsolete("Use ConsoleWindow element in UXML")]
	internal sealed class ConsoleUI : BaseMono
	{
		[SerializeField] private ConsoleAsset _console;
		[SerializeField] private UnityEvent _onCloseButton;
		
		private ListView _logList;
		private TextField _input;
		private Button _closeButton;

		// cycle 
		private readonly InputHistory _inputHistory = new ();
		private readonly YieldInstruction _frameYield = new WaitForEndOfFrame();

		private void Start()
		{
			_console.Init();
		}

		private void OnEnable()
		{
			// listview init
			BindUI();
			StartCoroutine(YieldRoutine(_frameYield, OnOpenConsole));
		}

		private void OnDisable()
		{
			// listview cleanup
		}

		private void OnDestroy()
		{
			
		}

		private System.Collections.IEnumerator YieldRoutine(YieldInstruction y, Action fn)
		{
			yield return y;
			fn.Invoke();
		}

		private void OnOpenConsole()
		{
			ScrollToEnd();
			_input.Focus();
		}

		private void OnClose()
		{
			_onCloseButton.Invoke();
		}

		private void BindUI()
		{
			var doc = GetComponent<UIDocument>();
			var root = doc.rootVisualElement;
			_input = root.Q<TextField>();
			_logList = root.Q<ListView>();
			_logList.bindItem = BindLogItem;
			_logList.itemsSource = _console.Log.LogList;
			_closeButton = root.Q<Button>(name: "Close");
			_closeButton.clicked += OnClose;

			// cycle input history
			_input.RegisterCallback<KeyDownEvent>(OnInputKeyDown, TrickleDown.TrickleDown);

			// handle submit
			_input.RegisterCallback<NavigationSubmitEvent>(OnSubmit, TrickleDown.TrickleDown);
		}

		private void ScrollToEnd()
		{
			if (_console.Log.Length > 0)
			{
				_logList.ScrollToItem(_console.Log.Length - 1);
			}
		}

		private void OnConsoleCleared()
		{
			
		}

		private void OnLogAdded()
		{
			
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
			_console.Exec(input);
			ScrollToEnd();
		}

		private void BindLogItem(VisualElement el, int i)
		{
			var item = _console.Log.GetItemAt(i);
			el.dataSource = item;
			el.viewDataKey = i.ToString();

			var cls = GetLogClass(item);
			if (!string.IsNullOrEmpty(cls))
			{
				el.AddToClassList(cls);
			}
		}

		// used for tinting logs
		private const string CLS_LOG_PREFIX = "sm-console-log__";

		private static string GetLogClass(in ConsoleLogItem item)
		{
			if (item.type == ELogType.Normal)
			{
				return string.Empty;
			}
			return $"{CLS_LOG_PREFIX}{item.type.ToString().ToLower()}";
		}
	}

}