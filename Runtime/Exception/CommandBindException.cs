// smidgens @ github

namespace Smidgenomics.Unity.Console
{
	using System;

	internal class ConsoleException : Exception
	{
		public ConsoleException() { }
		public ConsoleException(string msg) : base(msg) { } 
	}
}