// smidgens @ github

namespace Smidgenomics.Unity.Console.Editor
{
	using UnityEditor;

	[CustomEditor(typeof(ConsoleCommand))]
	internal class _ConsoleCommand : _Base { }

	[CustomEditor(typeof(ConsoleCommandAsset))]
	internal class _ConsoleCommandAsset : _Base { }

}