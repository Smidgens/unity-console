// smidgens @ github

namespace Smidgenomics.Unity.Console
{
	internal static class Array_
	{
		public static T FirstOrDefault<T>(this T[] arr)
		{
			if(arr.Length > 0) { return arr[0]; }
			return default;
		}
	}
}