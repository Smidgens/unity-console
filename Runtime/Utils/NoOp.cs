// smidgens @ github

namespace Smidgenomics.Unity.Console
{
	/// <summary>
	/// Common placeholder delegates
	/// </summary>
	internal static class NoOp
	{
		public static class Action
		{
			public static void A0() { }
			public static void A1<T>(T _) { }
		}

		public static class Predicate
		{
			public static bool False() => false;
			public static bool False<T>(T _) => false;
			public static bool True() => true;
			public static bool True<T>(T _) => true;
		}

		public static class Func
		{
			public static T Default<T>() => default;
		}
	}
}