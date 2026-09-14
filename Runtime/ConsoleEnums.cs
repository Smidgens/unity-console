// smidgens @ github

namespace Smidgenomics.Unity.Console
{
	internal enum ECommandType
	{
		MethodCall,
		Assignment,
	}
}

namespace Smidgenomics.Unity.Console
{
	public enum ELogType
	{
		Error = -2,
		Warning = -1,
		Normal = 0,
		Info = 1,
		Success = 2,
		Expression = 3
	}
}

namespace Smidgenomics.Unity.Console
{
	using System;
	using UnityEngine;

	/// <summary>
	/// Inclusion of built-in console commands
	/// </summary>
	[Flags]
	internal enum EDefaultConsoleCommand
	{
		None = 0,
		[InspectorName("clear")] Clear = 1,
		[InspectorName("list")] List = 2,
		[InspectorName("list (filter)")] ListWildcard = 4,
		[InspectorName("describe (command)")] Describe = 8,
		[InspectorName("inspect (variable)")] Inspect = 16,
		[InspectorName("exec (file)")] Exec = 32,
		All = ~0
	}
	
}