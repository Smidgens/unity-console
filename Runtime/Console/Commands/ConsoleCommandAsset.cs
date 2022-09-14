// smidgens @ github

namespace Smidgenomics.Unity.Console
{
	using UnityEngine;

	[CreateAssetMenu(menuName = Config.CreateAssetMenu.CONSOLE_CMD, order = 2)]
	internal class ConsoleCommandAsset : ScriptableObject
	{
		internal bool IsBound => _binding.IsBound;
		internal void Bind(IConsole c) => _binding.Bind(c);
		internal void Unbind() => _binding.Unbind();

		[SerializeField] private CommandBindingRef _binding = default;
	}
}