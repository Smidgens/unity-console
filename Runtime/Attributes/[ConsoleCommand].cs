// smidgens @ github

namespace Smidgenomics.Unity.Console
{
	using System;

	/// <summary>
	/// Mark static member discoverable by Console
	/// </summary>
	[AttributeUsage(AttributeTargets.Method|AttributeTargets.Field|AttributeTargets.Property)]
	public sealed class ConsoleCommandAttribute : Attribute
	{
		public ConsoleCommandAttribute
		(
			string name = null,
			string description = null
		)
		{
			this.name = name ?? string.Empty;
			this.description = description ?? string.Empty;
		}

		internal string name { get; }
		internal string description { get; }
	}
}