// smidgens @ github

namespace Smidgenomics.Unity.Console
{
	using System.Reflection;

	internal static class RFlags
	{
		public const BindingFlags
		// instance
		ANY_INSTANCE_MEMBER =
		BindingFlags.Instance
		| BindingFlags.Public
		| BindingFlags.NonPublic,
		// static
		ANY_STATIC_MEMBER =
		BindingFlags.Static
		| BindingFlags.NonPublic
		| BindingFlags.Public;
	}
}