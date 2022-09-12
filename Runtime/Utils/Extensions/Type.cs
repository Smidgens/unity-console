// smidgens @ github

namespace Smidgenomics.Unity.Console
{
	using UnityEngine;
	using System;
	using System.Reflection;
	using System.Collections.Generic;

	internal static class Type_
	{
		public static bool IsUnityComponent(this Type t)
		{
			return t.DerivesFrom(typeof(Component));
		}

		public static bool DeclaresPrivate(this Type t, MethodInfo m)
		{
			return m.DeclaringType == t && m.IsPrivate;
		}

		public static bool DerivesFrom(this Type t, Type bt)
		{
			var rootType = typeof(object);
			var current = t.BaseType;
			while (current != rootType && current != bt)
			{
				current = current.BaseType;
			}
			return current == bt;
		}

		public static bool IsStatic(this Type t) => t.IsAbstract && t.IsSealed;
		public static string GetNameOrAlias(this Type t) => TypeAlias.Get(t) ?? t.Name;
	}
}