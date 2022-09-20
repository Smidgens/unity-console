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
		public ThemeConfig.GUIStyles Styles => _styles;
		public ThemeConfig.WindowColors WindowColors => _windowColors;

		public Color FindColor(int l)
		{
			return _logColors.Select(l);
		}

		[SerializeField] private ThemeConfig.GUIStyles _styles = default;

		[Expand]
		[SerializeField]
		private ThemeConfig.WindowColors _windowColors = ThemeConfig.defaultWindowColors;

		[Expand]
		[SerializeField]
		private ThemeConfig.LogColors _logColors = ThemeConfig.defaultLogColors;
	}
}