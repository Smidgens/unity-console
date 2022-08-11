// smidgens @ github

namespace Smidgenomics.Unity.Console
{
	using System;
	using System.Collections.Generic;

	internal interface IOutputLog
	{
		public int Length { get; }
		public int ID { get; }
		public void Append(string msg, int type);
		public IOutputLogItem GetItem(int i);
	}

	internal interface IOutputLogItem
	{
		public int Type { get; }
		public string Text { get; }
		public DateTime Date { get; }
	}

	internal class OutputLog : IOutputLog
	{
		public int Length => _items.Count;
		public int ID { get; private set; } = GetTimestamp();

		public IOutputLogItem GetItem(int i)
		{
			if (i < 0 || i >= _items.Count) { return null; }
			return _items[i];
		}

		public int CountItems(int t)
		{
			throw new NotImplementedException();
		}

		public void Append(string text, int type = 0)
		{
			var date = DateTime.Now;
			_items.Add(new LogItem(text, date, type));
		}

		public void Clear()
		{
			ID = GetTimestamp();
			_items.Clear();
		}

		public class LogItem : IOutputLogItem
		{
			public string Text { get; } = "";
			public int Type { get; } = 0;
			public DateTime Date { get; } = default;

			public LogItem(string t, DateTime date, int type = 0)
			{
				Type = type;
				Text = t;
				Date = date;
			}
		}

		private List<LogItem> _items = new List<LogItem>();

		private static int GetTimestamp()
		{
			return (int)(DateTime.Now).ToUnixMS();
		}
	}
}