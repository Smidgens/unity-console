// smidgens @ github

namespace Smidgenomics.Unity.Console
{
	using UnityEngine;

	[AddComponentMenu(Config.AddComponentMenu.CONSOLE_HANDLER)]
	internal class ConsoleHandler : MonoBehaviour
	{
		public string GetKeyword()
		{
			if(_keywordMode == KeywordMode.Custom)
			{
				return _keyword;
			}

			if(_keywordMode == KeywordMode.Auto)
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
			KEYWORD = nameof(_keyword),
			METHOD = nameof(_handler),
			KW_MODE = nameof(_keywordMode);
		}
#endif

		[SerializeField] private Console _console = default;
		[SerializeField] private string _keyword = "";
		[SerializeField] public SerializedMember _handler = default;
		[SerializeField] private KeywordMode _keywordMode = default;

		private enum KeywordMode
		{
			Custom,
			[InspectorName("Object")]
			Auto
		}

		private Console.HandlerKey _key = null;

		private void OnEnable()
		{

			if (!_console)
			{
				enabled = false;
				return;
			}

			var keyword = GetKeyword();

			if(keyword.Length == 0) { return; }

			var m = _handler.GetMethod();

			if (m == null)
			{
				enabled = false;
				Debug.Log("Error binding command handler");
				return;
			}

			_key = _console.AddHandler(keyword, _handler.Target, m);
		}

		private void OnDisable()
		{
			if(_key != null)
			{
				_console.RemoveHandler(_key);
				_key = null;
			}
		}
	}
}