// smidgens @ github

namespace Smidgenomics.Unity.Console
{
	using System;

	/// <summary>
	/// Bind commands in static class
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class ConsoleClassAttribute : Attribute
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="scopeName">If provided, all commands will have their paths prefixed with "scopeName."</param>
		/// <param name="exposeAll">If true will register every static field with console</param>
		public ConsoleClassAttribute
		(
			string scopeName = null,
			bool exposeAll = false
		)
		{
			scoped = !string.IsNullOrEmpty(scopeName);
			this.scopeName = scopeName ?? string.Empty;
			this.exposeAll = exposeAll;
		}

		internal bool exposeAll { get; }
		internal bool scoped { get; }
		internal string scopeName { get; }
	}
}