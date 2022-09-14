// smidgens @ github

namespace Smidgenomics.Unity.Console
{
	using System;
	using System.Reflection;
	using System.Collections.Generic;

	internal static class MethodHelper
	{
		public static MethodInfo LoadMethod(string mname, string[] mtypes)
		{
			if (string.IsNullOrEmpty(mname)) { return null; }

			if (mtypes.Length < 2) { return null; }

			var ownerType = Type.GetType(mtypes[0], false);
			var returnType = Type.GetType(mtypes[1], false);

			if (ownerType == null || returnType == null)
			{
				return null;
			}

			//var ptypes = new List<Type>();

			var pptypes = new Type[mtypes.Length - 2];

			for (var i = 2; i < mtypes.Length; i++)
			{
				pptypes[i - 2] = Type.GetType(mtypes[i], false);

				//ptypes.Add(Type.GetType(mtypes[i], false));
			}
			return ownerType.GetMethod(mname, RFlags.ANY_INSTANCE_MEMBER, null, pptypes, null);
			//return ownerType.GetMethod(mname, RFlags.ANY_INSTANCE_MEMBER, null, ptypes.ToArray(), null);
		}

		public static MethodInfo GetMethod(Action a) => a.Method;
		public static MethodInfo GetMethod<T>(Action<T> a) => a.Method;
		public static MethodInfo GetMethod<T1, T2>(Action<T1,T2> a) => a.Method;
		public static MethodInfo GetMethod<T1, T2, T3>(Action<T1,T2,T3> a) => a.Method;
	}
}