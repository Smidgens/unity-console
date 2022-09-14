// smidgens @ github

namespace Smidgenomics.Unity.Console
{
	internal static class Empty
	{
		public static T New<T>() where T : new()
		{
			return new T();
		}

		public const string STRING = "";

		public static class Array
		{
			public readonly static string[] STRING = { };
			public readonly static int[] INT = { };
		}
	}
}