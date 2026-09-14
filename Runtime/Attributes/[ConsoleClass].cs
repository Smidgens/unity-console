// smidgens @ github

namespace Smidgenomics.Unity.Console
{
	using System;

	/// <summary>
	/// Look for Console handlers in class
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class ConsoleClassAttribute : Attribute
	{
		public ConsoleClassAttribute
		(
			string scopeName = null,
			bool scoped = false,
			bool exposeAll = false
		)
		{
			this.scoped = scoped;
			this.scopeName = scopeName ?? string.Empty;
			this.exposeAll = exposeAll;
		}

		internal bool exposeAll { get; }
		internal bool scoped { get; }
		internal string scopeName { get; }
	}
}