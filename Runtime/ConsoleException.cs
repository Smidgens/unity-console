// smidgens @ github

namespace Smidgenomics.Unity.Console
{
	using System;

	internal sealed class ConsoleException : Exception
	{
		public ConsoleException() { }
		public ConsoleException(string msg) : base(msg) { } 
	}
}