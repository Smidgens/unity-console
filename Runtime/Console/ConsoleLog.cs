// smidgens @ github

namespace Smidgenomics.Unity.Console
{
	using System;
	using System.Collections;
	using System.Collections.Generic;

	[Serializable]
	internal sealed class ConsoleLog : IConsoleLog
	{
		public event Action onLogsCleared;
		public event IConsoleLog.LogAddedEvent onLogAdded;

		public int Length => _items.Count;

		IList IConsoleLog.LogList => _items;

		public ConsoleLogItem GetItemAt(int i)
		{
			if (i < 0 || i >= _items.Count)
			{
				throw new IndexOutOfRangeException();
			}
			return _items[i];
		}

		public void Append(string text, ELogType type = 0, long category = 0)
		{
			var log = new ConsoleLogItem(text, DateTime.Now, type, category);
			_items.Add(log);
			onLogAdded?.Invoke(log);
		}

		public void Clear()
		{
			_items.Clear();
			onLogsCleared?.Invoke();
		}

		private readonly List<ConsoleLogItem> _items = new ();

	}
}