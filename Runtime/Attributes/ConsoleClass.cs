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
		public bool exposeAll = false;
		public bool scoped = false;
		public string displayName = string.Empty;

		public ConsoleClassAttribute()
		{

		}

		public ConsoleClassAttribute(string displayName = "")
		{
			this.displayName = displayName;
			this.scoped = true;
		}

		public ConsoleClassAttribute(bool exposeAll)
		{
			this.exposeAll = exposeAll;
		}
	}
}