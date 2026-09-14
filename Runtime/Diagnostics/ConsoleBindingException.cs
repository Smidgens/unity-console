// smidgens @ github

#pragma warning disable 0414

namespace Smidgenomics.Unity.Console
{
	using System;

	/// <summary>
	/// Error binding command to console
	/// </summary>
	[System.Serializable]
	public sealed class ConsoleBindingException : Exception
	{
		public ConsoleBindingException()
		{
		}

		public ConsoleBindingException(string msg) : base(msg)
		{
		}

		public ConsoleBindingException(string msg, Exception inner) : base(msg, inner)
		{
		}
	}
}