// smidgens @ github

namespace Smidgenomics.Unity.Console
{
	using UnityEngine;

#if !CONSOLE_DISABLE_CM
	[CreateAssetMenu(menuName = Config.CreateAssetMenu.THEME)]
#endif
	internal class ConsoleTheme : ScriptableObject
	{
		public Color BackgroundColor => _backgroundColor;

		public Color FindColor(int l)
		{
			return _logColors.Select(l);
		}

		[SerializeField]
		private Color _backgroundColor = Color.black * 0.5f;

		[Expand]
		[SerializeField]
		private LogColors _logColors = LogColors.Default;
	}
}