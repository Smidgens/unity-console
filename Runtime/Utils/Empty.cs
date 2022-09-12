// smidgens @ github

namespace Smidgenomics.Unity.Console
{
	internal static class Empty
	{
		public static T New<T>() where T : new()
		{
			return new T();
		}
	}
}