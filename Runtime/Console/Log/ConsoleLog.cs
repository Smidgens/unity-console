// smidgens @ github

namespace Smidgenomics.Unity.Console
{
	using System;
	using System.Collections.Generic;

	internal class OutputLog : IConsoleLog
	{
		public int Length => _items.Count;
		public uint ID { get; private set; } = 1;

		public IConsoleLogItem this[int i]
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

		private readonly struct LogItem : IConsoleLogItem
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