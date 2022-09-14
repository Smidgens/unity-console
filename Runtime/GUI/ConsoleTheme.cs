// smidgens @ github

namespace Smidgenomics.Unity.Console
{
	using UnityEngine;

#if !CONSOLE_DISABLE_CM
	[CreateAssetMenu(menuName = Config.CreateAssetMenu.THEME, order = 10)]
#endif
	internal class ConsoleTheme : ScriptableObject
	{
		public Color BackgroundColor => _windowColors.background;

		public Color FindColor(int l)
		{
			return _logColors.Select(l);
		}

		[Header("COLORS")]
		[Expand(label:"Window")]
		[SerializeField]
		private ThemeConfig.WindowColors _windowColors = ThemeConfig.defaultWindowColors;

		[Expand(label:"Log")]
		[SerializeField]
		private ThemeConfig.LogColors _logColors = ThemeConfig.defaultLogColors;
	}
}