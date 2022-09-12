// smidgens @ github

namespace Smidgenomics.Unity.Console
{
	using System;

	/// <summary>
	/// Mark static member discoverable by Console
	/// </summary>
	[AttributeUsage
	(
		AttributeTargets.Method
		| AttributeTargets.Field
		| AttributeTargets.Property
	)]
	public class ConsoleCommandAttribute : Attribute
	{

		public ConsoleCommandAttribute
		(
			string name = STR_EMPTY,
			string description = STR_EMPTY
		)
		{
			this.name = name;
			this.description = description;
		}

		internal string name { get; set; } = STR_EMPTY;
		internal string description { get; set; } = STR_EMPTY;

		private const string STR_EMPTY = "";
	}
}
