// smidgens @ github

namespace Smidgenomics.Unity.Console
{
	using System.Reflection;

	public interface IConsole
	{
		public IConsoleLog Log { get; }

		public void Exec(string input);

		public CommandHandle Add
		(
			string name,
			MemberInfo p,
			object ctx,
			string description
		);
		public void Remove(in CommandHandle cmd);
	}
}

namespace Smidgenomics.Unity.Console
{
	using System;
	using System.Collections;

	/// <summary>
	/// 
	/// </summary>
	public interface IConsoleLog
	{
		public delegate void LogAddedEvent(in ConsoleLogItem log);

		public event Action onLogsCleared;
		public event LogAddedEvent onLogAdded;

		/// <summary>
		/// Number of items
		/// </summary>
		public int Length { get; }

		/// <summary>
		/// Retrieves log item at index
		/// </summary>
		public ConsoleLogItem GetItemAt(int i);

		/// <summary>
		/// Convenience for UI toolkit/ListView
		/// </summary>
		internal IList LogList { get; }

		/// <summary>
		/// Add log item
		/// </summary>
		public void Append(string msg, ELogType type, long category);
	}
}

public interface IName
{
	public string Name { get; }
}

public interface IOrder
{
	public int Order { get; }
}