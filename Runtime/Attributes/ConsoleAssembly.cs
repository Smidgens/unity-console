// smidgens @ github

namespace Smidgenomics.Unity.Console
{
	using System;

	/// <summary>
	/// Look for Console handlers in assembly
	/// </summary>
	[AttributeUsage(AttributeTargets.Assembly)]
	public class ConsoleAssembly : Attribute
	{
		public ConsoleAssembly() { }
	}
}