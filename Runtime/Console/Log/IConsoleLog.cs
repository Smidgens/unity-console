// smidgens @ github

namespace Smidgenomics.Unity.Console
{
	using System;

	/// <summary>
	/// 
	/// </summary>
	public interface IConsoleLog
	{
		/// <summary>
		/// Number of items
		/// </summary>
		public int Length { get; }
		/// <summary>
		/// Caching helper
		/// </summary>
		public uint ID { get; }

		/// <summary>
		/// Get log item at index
		/// </summary>
		public IConsoleLogItem this[int i] { get; }

		/// <summary>
		/// Add log item
		/// </summary>
		public void Append(string msg, int type);
	}

	public interface IConsoleLogItem
	{
		public int Type { get; }
		public string Text { get; }
		public DateTime Time { get; }
	}
}