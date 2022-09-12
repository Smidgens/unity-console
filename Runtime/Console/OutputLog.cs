// smidgens @ github

namespace Smidgenomics.Unity.Console
{
	using System;
	using System.Collections.Generic;

	internal interface IOutputLog
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
		public IOutputLogItem this[int i] { get; }

		/// <summary>
		/// Add log item
		/// </summary>
		public void Append(string msg, int type);
	}

	internal interface IOutputLogItem
	{
		public int Type { get; }
		public string Text { get; }
		public DateTime Time { get; }
	}

	internal class OutputLog : IOutputLog
	{
		public int Length => _items.Count;
		public uint ID { get; private set; } = 1;

		public IOutputLogItem this[int i]
		{
			get
			{
				if (i < 0 || i >= _items.Count)
				{
					throw new IndexOutOfRangeException();
				}
				return _items[i];
			}
		}

		public void Append(string text, int type = 0)
		{
			_items.Add(new LogItem(text, DateTime.Now, type));
		}

		public void Clear()
		{
			_items.Clear();
			ID++;
		}

		private readonly List<LogItem> _items = new List<LogItem>();

		private readonly struct LogItem : IOutputLogItem
		{
			public string Text { get; }
			public int Type { get; }
			public DateTime Time { get; }

			public LogItem(string t, DateTime date, int type = 0)
			{
				Type = type;
				Text = t;
				Time = date;
			}
		}

	}
}