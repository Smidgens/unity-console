// smidgens @ github

namespace Smidgenomics.Unity.Console
{
	using System;

	/// <summary>
	/// Look for Console handlers in assembly
	/// </summary>
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
	public class ConsoleAssembly : Attribute
	{
		// only use types
		internal Type[] Types { get; } = null;

		public ConsoleAssembly() { }
		public ConsoleAssembly(params Type[] types)
		{
			Types = types;
		}
	}
}