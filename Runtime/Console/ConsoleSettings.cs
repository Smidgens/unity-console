// smidgens @ github

namespace Smidgenomics.Unity.Console
{
	using System;
	using UnityEngine;

	internal enum AttributeSearchScope
	{
		None,
		[InspectorName("Search Console Assemblies")]
		Explicit,
		[InspectorName("Search All Assemblies")]
		Global,
	}

	[Flags]
	internal enum DefaultCommand
	{
		None = 0,
		[InspectorName("clear")]
		Clear = 1,
		[InspectorName("list")]
		List = 2,
		[InspectorName("list (filter)")]
		ListWildcard = 4,
		[InspectorName("describe")]
		Describe = 8,
		[InspectorName("value_of")]
		Inspect = 16,
		[InspectorName("exec")]
		Exec = 32,
		All = ~0
	}

	[Serializable]
	internal class ConsoleSettings
	{
		[ToggleEnum]
		public AttributeSearchScope commandAttributes = AttributeSearchScope.Explicit;

		[ToggleEnum]
		public DefaultCommand builtInCommands = DefaultCommand.All;

	}
}