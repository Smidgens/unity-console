// smidgens @ github

namespace Smidgenomics.Unity.Console
{
	using System;

	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
	internal class ConsoleVariableAttribute : Attribute
	{
		public ConsoleVariableAttribute()
		{

		}
	}
}