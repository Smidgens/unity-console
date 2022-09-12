// smidgens @ github

namespace Smidgenomics.Unity.Console
{
	using System;

	internal class CommandBindException : Exception
	{
		public CommandBindException() { }
		public CommandBindException(string msg) : base(msg) { } 
	}
}