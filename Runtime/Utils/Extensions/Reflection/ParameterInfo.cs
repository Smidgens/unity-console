// smidgens @ github

namespace Smidgenomics.Unity.Console
{
	using System;
	using System.Reflection;

	internal static class ParameterInfo_
	{
		public static bool IsRefType(this ParameterInfo p)
		{
			return p.IsRetval || p.IsOut || p.IsIn;
		}
	}
}