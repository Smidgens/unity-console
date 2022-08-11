// smidgens @ github

namespace Smidgenomics.Unity.Console
{
	using System;

	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
	internal class ConsoleHandlerAttribute : Attribute
	{
		public ConsoleHandlerAttribute()
		{

		}
	}
}