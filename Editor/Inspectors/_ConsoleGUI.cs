// smidgens @ github

namespace Smidgenomics.Unity.Console.Editor
{
	using UnityEditor;

	[CustomEditor(typeof(ConsoleGUI))]
	internal class _ConsoleGUI : _Base { }

	[CustomEditor(typeof(ConsoleTheme))]
	internal class _ConsoleTheme : _Base { }
}