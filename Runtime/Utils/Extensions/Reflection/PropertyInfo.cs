// smidgens @ github

namespace Smidgenomics.Unity.Console
{
	using System.Reflection;

	internal static class PropertyInfo_
	{
		public static bool IsReadWrite(this PropertyInfo p) => p.CanRead && p.CanWrite;
	}
}