// smidgens @ github

namespace Smidgenomics.Unity.Console
{
	using System;

	/// <summary>
	/// Parsing input failed
	/// </summary>
	internal class ConsoleParseException : Exception
	{
		public ConsoleParseException() : base() { }
		public ConsoleParseException(string msg) : base(msg) { }
	}
}