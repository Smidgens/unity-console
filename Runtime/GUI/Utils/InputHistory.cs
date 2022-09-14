// smidgens @ github

namespace Smidgenomics.Unity.Console
{
	using System.Collections.Generic;

	internal class InputHistory
	{
		public void Append(string v)
		{
			if (string.IsNullOrEmpty(v)) { return; }
			_history.Add(v);
			_index = _history.Count;
		}

		public string Back()
		{
			var ni = _index - 1;
			if (ni >= 0) { _index = ni; }
			return GetCurrent();
		}

		public string Forward()
		{
			var ni = _index + 1;
			if (ni <= _history.Count) { _index = ni; }
			return GetCurrent();
		}

		public void Clear()
		{
			_history.Clear();
			_index = 0;
		}

		private static List<string> _history = new List<string>();
		private static int _index = 0;

		private string GetCurrent()
		{
			if (_index >= 0 && _index < _history.Count) { return _history[_index]; }
			return "";
		}
	}
}