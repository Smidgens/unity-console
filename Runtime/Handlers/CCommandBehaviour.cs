// smidgens @ github

namespace Smidgenomics.Unity.Console
{
	using UnityEngine;
	using System.Reflection;

	[AddComponentMenu(Config.AddComponentMenu.CONSOLE_HANDLER)]
	internal class CCommandBehaviour : MonoBehaviour
	{
		internal string GetKeyword()
		{
			if(_keywordMode == KeywordMode.Custom)
			{
				return _keyword;
			}

			if(_keywordMode == KeywordMode.NameTypeMethod)
			{
				if (!_handler.Target) { return "-"; }
				if(_handler.Name.Length == 0) { return "-"; }
				var fnName = _handler.Name;
				if(fnName.Length > 4 && fnName[3] == '_')
				{
					fnName = fnName.Substring(4);
				}
				var tt = _handler.Target.GetType().Name;
				return $"{_handler.Target.name}.{tt}.{fnName}";
			}
			return "";
		}

#if UNITY_EDITOR
		internal static class _Fields
		{
			public const string
			CONSOLE = nameof(_console),
			NAME = nameof(_keyword),
			METHOD = nameof(_handler),
			NAME_MODE = nameof(_keywordMode);
		}
#endif

		[SerializeField] private Console _console = default;
		[SerializeField] private string _keyword = "";
		[SerializeField] public SerializedMember _handler = default;
		[SerializeField] private KeywordMode _keywordMode = default;

		private enum KeywordMode
		{
			Custom,
			[InspectorName("Auto (From Object)")]
			NameTypeMethod,
		}

		private CommandHandle _handle = default;

		private void OnEnable()
		{
			if (!_console)
			{
				enabled = false;
				return;
			}
			BindHandler();
		}

		private void OnDisable()
		{
			if(_handle.IsValid)
			{
				_console.RemoveCommand(_handle);
				_handle = default;
			}
		}

		private void BindHandler()
		{
			var m = _handler.GetMethod();

			if (m == null)
			{
				enabled = false;
				Debug.Log("Error binding command handler");
				return;
			}

			var keyword = GetKeyword();

			if (keyword.Length == 0) { return; }

			if (m.IsGetOrSet() && m.Name[0] == 's')
			{
				BindAsProperty(keyword, m);
			}
			else
			{ 
				BindAsMethod(keyword, m);
			}
		}

		private void BindAsProperty(string keyword, MethodInfo m)
		{
			if(m.TryGetProperty(out var p))
			{
				_handle = (_console as IConsole).AddCommand(keyword, p, _handler.Target);
			}
		}

		private void BindAsMethod(string keyword, MethodInfo m)
		{
			_handle = (_console as IConsole).AddCommand(keyword, m, _handler.Target);
		}
	}
}