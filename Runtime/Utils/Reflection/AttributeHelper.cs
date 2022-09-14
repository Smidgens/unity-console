// smidgens @ github

namespace Smidgenomics.Unity.Console
{
	using System;
	using System.Reflection;
	using System.Collections.Generic;
	using MInfo = System.Reflection.MemberInfo;

	internal static class AttributeHelper
	{
		public static IEnumerable<Assembly> Assemblies<T>() where T : Attribute
		{
			var assemblies = GetAssemblies();
			var r = new List<Assembly>();
			foreach (var a in assemblies)
			{
				if (!a.IsDefined(typeof(T))) { continue; }
				r.Add(a);
			}
			return r;
		}

		public static IEnumerable<MInfo> Members<T>(Type t) where T : Attribute
		{
			var types = new List<MInfo>();
			foreach (var m in t.GetMembers(RFlags.ANY_STATIC_MEMBER))
			{
				if (!m.IsDefined(typeof(T))) { continue; }
				types.Add(m);
			}
			return types;
		}

		private static Assembly[] _assemblyCache = null;

		private static Assembly[] GetAssemblies()
		{
			if(_assemblyCache == null)
			{
				_assemblyCache = AppDomain.CurrentDomain.GetAssemblies();
			}
			return _assemblyCache;
		}
	}
}