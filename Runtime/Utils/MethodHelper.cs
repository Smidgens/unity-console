// smidgens @ github

namespace Smidgenomics.Unity.Console
{
	using System;
	using System.Reflection;

	internal static class MethodHelper
	{
		public static MethodInfo GetMethod(Action a) => a.Method;
		public static MethodInfo GetMethod<T>(Action<T> a) => a.Method;
		public static MethodInfo GetMethod<T1, T2>(Action<T1,T2> a) => a.Method;
		public static MethodInfo GetMethod<T1, T2, T3>(Action<T1,T2,T3> a) => a.Method;
	}
}