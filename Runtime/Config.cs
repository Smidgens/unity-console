// smidgens @ github

namespace Smidgenomics.Unity.Console
{
	internal static class Config
	{
		public static class ResourcePath
		{
			public const string
			DEFAULTS = _PREFIX + "{defaults}";
			private const string _PREFIX = "smidgenomics.console/";
		}

		public static class Defaults
		{
			public const float
			INPUT_HEIGHT = 21f,
			SCROLLBAR_WIDTH = 18f;
		}

		public static class AddComponentMenu
		{
			public const string
			CONSOLE_GUI = _PREFIX + "Console GUI",
			CONSOLE_HANDLER = _PREFIX + "Console Command",
			TOOLBAR_BUTTON = _PREFIX + "Toolbar/Console Button (Toolbar)",
			TOOLBAR_DROPDOWN = _PREFIX + "Toolbar/Console Dropdown (Toolbar)";
			private const string _PREFIX = "Smidgenomics/Console/";
		}

		public static class CreateAssetMenu
		{
			public const string
			CONSOLE = _PREFIX + "Console",
			CONSOLE_CMD = _PREFIX + "Console Command",
			THEME = _PREFIX + "Console Theme";

			private const string _PREFIX = "Console/";
		}
	}
}