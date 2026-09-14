// smidgens @ github

namespace Smidgenomics.Unity.Console
{
	using BF = System.Reflection.BindingFlags;
	internal static class RFlags
	{
		// instance
		public const BF ANY_INSTANCE_MEMBER = BF.Instance | BF.Public | BF.NonPublic;
		// static
		public const BF ANY_STATIC_MEMBER = BF.Static | BF.NonPublic | BF.Public;
	}
}

namespace Smidgenomics.Unity.Console
{
	using System;
	using System.Reflection;
	using System.Collections.Generic;
	using UnityEngine;
	using MInfo = System.Reflection.MemberInfo;

	internal static class ReflectionUtils
	{
		public static IEnumerable<Assembly> GetAssembliesWithAttr<T>() where T : Attribute
		{
			var assemblies = GetAllAssemblies();
			var r = new List<Assembly>();
			foreach (var a in assemblies)
			{
				if (!a.IsDefined(typeof(T)))
				{
					continue;
				}
				r.Add(a);
			}
			return r;
		}

		public static IEnumerable<MInfo> MembersWithAttribute<T>(Type t) where T : Attribute
		{
			var types = new List<MInfo>();
			foreach (var m in t.GetMembers(RFlags.ANY_STATIC_MEMBER))
			{
				if (!m.IsDefined(typeof(T)))
				{
					continue;
				}
				types.Add(m);
			}
			return types;
		}

		private static Assembly[] _assemblyCache;

		private static Assembly[] GetAllAssemblies()
		{
			if(_assemblyCache == null)
			{
				_assemblyCache = AppDomain.CurrentDomain.GetAssemblies();
			}
			return _assemblyCache;
		}

		// can field be drawn by inspector
		public static bool IsInspectorField(FieldInfo f)
		{
			// explicitly public but non-serialized
			if (f.IsPublic && f.IsDefined(typeof(NonSerializedAttribute)))
			{
				return false;
			}

			// explicitly hidden
			if (f.IsDefined(typeof(HideInInspector)))
			{
				return false;
			}

			// private, non serialized
			if (!f.IsPublic && !f.IsDefined(typeof(SerializeField)))
			{
				return false;
			}

			// at this point, either the field is public, or private and using SerializeField
			return true;
		}
	}
}