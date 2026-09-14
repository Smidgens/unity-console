// smidgens @ github

namespace Smidgenomics.Unity.Console
{
	using UnityEngine;

	[CreateAssetMenu(menuName = ConsoleConstants.CREATE_MENU_ROOT + "Console Command", order = 2)]
	internal sealed class ConsoleCommandAsset : BaseSO
	{
		internal bool IsBound => _binding.IsBound;
		internal void Bind(IConsole c) => _binding.Bind(c);
		internal void Unbind() => _binding.Unbind();

		[SerializeField] private CommandBindingRef _binding;
	}
}