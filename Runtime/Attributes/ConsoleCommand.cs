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
			string name = Empty.STRING,
			string description = Empty.STRING
		)
		{
			this.name = name;
			this.description = description;
		}

		internal string name { get; }
		internal string description { get; }
	}
}