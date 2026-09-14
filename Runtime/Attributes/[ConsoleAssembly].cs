// smidgens @ github

namespace Smidgenomics.Unity.Console
{
	using System;

	/// <summary>
	/// Include assembly in console
	/// </summary>
	[AttributeUsage(AttributeTargets.Assembly)]
	public sealed class ConsoleAssembly : Attribute
	{
	}
}