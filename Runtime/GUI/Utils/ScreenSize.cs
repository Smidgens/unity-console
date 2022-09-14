// smidgens @ github

namespace Smidgenomics.Unity.Console
{
	using UnityEngine;

	/// <summary>
	/// Utility for checking if screen size changed
	/// </summary>
	internal struct ScreenSize
	{
		public bool Resized()
		{
			if (Screen.width == _s.Item1 && Screen.height == _s.Item2)
			{
				return false;
			}
			_s = (Screen.width, Screen.height);
			return true;
		}
		private (int, int) _s;
	}
}