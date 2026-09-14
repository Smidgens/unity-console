// smidgens @ github

namespace Smidgenomics.Unity.Console
{
	using UnityEngine;
	using System.Reflection;

#if !CONSOLE_DISABLE_CM
	[CreateAssetMenu(menuName = ConsoleConstants.CREATE_MENU_ROOT + "Console")]
#endif
	[ExcludeFromPreset]
	public sealed class ConsoleAsset : BaseSO, IConsole
	{
		[Expand(true)]
		[SerializeField] private ConsoleSettings _settings = ConsoleSettings.Default;
		[SerializeField] internal ConsoleCommandAsset[] _commands = {};
		[SerializeField] internal ConsoleLogBitFlags _logCategories;

		private readonly Console _console = new ();

		// lazy init
		private delegate void InitFn(ConsoleAsset c);
		private static InitFn _initFn = InitStart;

		public IConsoleLog Log => _console.Log;
		public void Exec(string input)
		{
			_console.Exec(input);
		}

		internal void Init() => _initFn.Invoke(this);

		private static void InitStart(ConsoleAsset c)
		{
			c._console.Clear();
			c._console.InitDefaultCommands(c._settings.builtInCommands);
			c._console.FindAssemblyCommands();

			foreach (var cmd in c._commands)
			{
				if (!cmd.IsBound)
				{
					cmd.Bind(c._console);
				}
			}

			c._console.AddLog("Console initialized", ELogType.Info);
			_initFn = NoOp;
		}

		public CommandHandle Add(string cName, MemberInfo p, object ctx, string description)
		{
			return _console.Add(cName, p, ctx, description);
		}

		public void Remove(in CommandHandle cmd)
		{
			_console.Remove(cmd);
		}

		internal void Clear()
		{
			_console.Clear();
		}

		private static void NoOp<T>(T _) { }
	}
}