// smidgens @ github

namespace Smidgenomics.Unity.Console
{
	using System;

	/// <summary>
	/// Ignore member of ConsoleClass when expose all is enabled
	/// </summary>
	[AttributeUsage(AttributeTargets.Method|AttributeTargets.Field|AttributeTargets.Property)]
	public sealed class ConsoleHiddenAttribute : Attribute
	{
	
	}
}