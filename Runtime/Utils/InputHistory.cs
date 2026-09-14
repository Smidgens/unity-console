// smidgens @ github

namespace Smidgenomics.Unity.Console
{
	using System.Collections.Generic;
	using UnityEngine;

	internal sealed class InputHistory
	{
		public void Append(string v)
		{
			if (string.IsNullOrEmpty(v))
			{
				return;
			}
			_history.Add(v);
			_index = _history.Count;
		}

		public string Back()
		{
			_index = Mathf.Max(0, _index - 1);
			return GetCurrent();
		}

		public string Forward()
		{
			_index = Mathf.Min(_history.Count - 1, _index + 1);
			return GetCurrent();
		}

		public void Clear()
		{
			_history.Clear();
			_index = 0;
		}

		private readonly List<string> _history = new ();
		private int _index;

		private string GetCurrent()
		{
			if (_index >= 0 && _index < _history.Count)
			{
				return _history[_index];
			}
			return string.Empty;
		}
	}
}