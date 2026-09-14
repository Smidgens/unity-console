// smidgens @ github

namespace Smidgenomics.Unity.Console
{
	using UnityEngine;

	[AddComponentMenu(ConsoleConstants.COMPONENT_ROOT + "Console Command")]
	internal sealed class ConsoleCommand : BaseMono
	{
		[SerializeField] private ConsoleReference _console;
		[SerializeField] private CommandBindingRef _binding;

		private void OnEnable()
		{
			try
			{
				if (!_console.IsSet)
				{
					throw new ConsoleException("Missing console reference");
				}
				_binding.Bind(_console.Console);
			}
			catch(ConsoleException e)
			{
				enabled = false;
				LogError(e.Message);
			}
		}

		private void OnDisable()
		{
			_binding.Unbind();
		}

		private void LogError(string msg)
		{
			if (Application.isEditor)
			{
				Debug.LogWarning(msg, this);
			}
			if (_console.IsSet)
			{
				_console.Console.Log.Append(msg, ELogType.Warning, 0);
			}
		}
	}
}