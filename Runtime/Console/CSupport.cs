// smidgens @ github

namespace Smidgenomics.Unity.Console
{
	using System;
	using System.Reflection;
	using System.Collections.Generic;
	using UnityEngine;

	internal static class CSupport
	{
		public static readonly IReadOnlyCollection<Type>
		SUPPORTED_ARG_TYPES = new Type[]
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
			return Array.IndexOf(SUPPORTED_ARG_TYPES as Type[], t) > -1;
		}

	}
}