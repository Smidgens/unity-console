// smidgens @ github

namespace Smidgenomics.Unity.Console
{
	using UnityEngine;

	[AddComponentMenu(Config.AddComponentMenu.CONSOLE_HANDLER)]
	internal class ConsoleCommand : MonoBehaviour
	{
		[SerializeField] private ConsoleReference _console = default;
		[SerializeField] private CommandBindingRef _binding = default;

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
				_console.Console.Log.Append(msg, -1);
			}
		}
	}
}