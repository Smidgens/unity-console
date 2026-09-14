// smidgens @ github

namespace Smidgenomics.Unity.Console
{
	using System.Reflection;

	public interface IConsole
	{
		public IConsoleLog Log { get; }

		/// <summary>
		/// Execute input
		/// </summary>
		public void Exec(string input);

		/// <summary>
		/// Bind class member
		/// </summary>
		/// <exception cref="ConsoleBindingException">Cannot bind member</exception>
		public CommandHandle Bind
		(
			string name,
			MemberInfo p,
			object ctx,
			string description
		);
		
		/// <summary>
		/// Unbind command
		/// </summary>
		public void Unbind(in CommandHandle cmd);
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