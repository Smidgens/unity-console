// smidgens @ github

namespace Smidgenomics.Unity.Console
{
	using System;
	using System.Reflection;
	using System.Collections.Generic;
	using UnityEngine;

	internal static class RFlags
	{
		public const BindingFlags
		// instance
		ANY_INSTANCE_MEMBER =
		BindingFlags.Instance
		| BindingFlags.Public
		| BindingFlags.NonPublic,
		// static
		ANY_STATIC_MEMBER =
		BindingFlags.Static
		| BindingFlags.NonPublic
		| BindingFlags.Public;
	}

	internal static class ConsoleReflection
	{
		public static Type[] CONSOLE_SUPPORTED_TYPES =
		{
			typeof(string),
			typeof(float),
			typeof(int),
			typeof(bool),
			typeof(Vector2),
			typeof(Vector3),
			typeof(Vector4),
			typeof(Color),
		};

		public static MethodInfo LoadMethod(StringifiedMember mi)
		{
			return LoadMethod(mi.name, mi.types);
		}

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

			var ptypes = new List<Type>();

			for (var i = 2; i < mtypes.Length; i++)
			{
				ptypes.Add(Type.GetType(mtypes[i], false));
			}
			return ownerType.GetMethod(mname, RFlags.ANY_INSTANCE_MEMBER, null, ptypes.ToArray(), null);
		}

		public static bool IsConsoleUsable(ParameterInfo p)
		{
			if (p.IsOptional) { return false; }
			if (p.IsRefType()) { return false; }
			return IsConsoleUsable(p.ParameterType);
		}

		public static bool IsConsoleUsable(MemberInfo x)
		{
			switch (x.MemberType)
			{
				case MemberTypes.Method:
					return IsConsoleUsable(x as MethodInfo);
			}
			return false;
		}

		public static bool IsConsoleUsable(MethodInfo x)
		{
			if (!x.IsVoid()) { return false; }
			foreach (var p in x.GetParameters())
			{
				if (!IsConsoleUsable(p)) { return false; }
			}
			return true;
		}

		public static bool IsConsoleUsable(Type t)
		{
			return Array.IndexOf(CONSOLE_SUPPORTED_TYPES, t) > -1;
		}

	}
}