// smidgens @ github

namespace Smidgenomics.Unity.Console
{
	using System;

	/// <summary>
	/// Look for Console handlers in class
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public class ConsoleClassAttribute : Attribute
	{
		internal bool exposeAll = false;
		internal bool scoped = false;
		internal string scopeName = string.Empty;

		public ConsoleClassAttribute
		(
			string scopeName = Empty.STRING,
			bool scoped = false,
			bool exposeAll = false
		)
		{
			this.scoped = scoped;
			this.scopeName = scopeName;
			this.exposeAll = exposeAll;
		}
	}
}