namespace Smidgenomics.Unity.Console
{
	using System;
	using System.Reflection;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityObject = UnityEngine.Object;
	using System.Linq;

	internal enum AccessMod { Private, Protected, Internal, Public }

	internal static class Reflection_
	{
		public static string GetDisplayName(this Type t)
		{
			return _DISPLAY_ALIAS.TryGetValue(t, out var n) ? n : t.Name;
		}

		public static AccessMod GetAccessMod(this MethodInfo m)
		{
			if (m.IsFamily) { return AccessMod.Protected; }
			if (m.IsPrivate) { return AccessMod.Private; }
			return AccessMod.Public;
		}

		public static bool IsVoid(this MethodInfo m)
		{
			return m.ReturnType == typeof(void);
		}

		public static bool TryGetGameObject(this UnityObject ob, out GameObject go)
		{
			go = null;
			if (!ob) { return false; }
			var tt = ob.GetType();
			// scene reference
			if (IsUnityComponent(tt)) { go = (ob as Component).gameObject; }
			else if (tt == typeof(GameObject)) { go = ob as GameObject; }
			return go;
		}

		public static bool OwnsMethod(this Type t, MethodInfo m)
		{
			return m.DeclaringType == t;
		}

		public static bool IsDeclaringPrivate(this Type t, MethodInfo m)
		{
			return m.DeclaringType == t && m.IsPrivate;
		}

		public static bool IsRefType(this ParameterInfo p)
		{
			return p.IsRetval || p.IsOut || p.IsIn;
		}

		public static string GetDisplaySignature(this MethodInfo m)
		{
			var rt = m.ReturnType.GetDisplayName();
			var n = m.GetDisplayName();

			var parameters = m.GetParameters();

			var pnames =
			string.Join(", ", parameters
			.Select(x => x.ParameterType.GetDisplayName()));

			var accessPrefix = $"[{m.GetAccessMod().GetLabel()}]";

			if (m.IsSpecialName)
			{
				if (m.IsVoid())
				{
					return $"{accessPrefix} {parameters[0].ParameterType.GetDisplayName()} {n}";
				}
				return $"{accessPrefix} {rt} {n}";
			}
			return $"{accessPrefix} {rt} {n} ({pnames})";
		}

		public static bool IsUnityComponent(this Type t)
		{
			return DerivesFrom(t, typeof(Component));
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

		public static string GetDisplayName(this MethodInfo m)
		{
			return m.IsProperty() ? m.GetPropertyName() : m.Name;
		}

		public static bool IsProperty(this MethodInfo m)
		{
			return
			m.Name.Length > 4
			&& m.IsSpecialName
			&& m.Name[3] == '_';
		}

		public static string GetPropertyName(this MethodInfo m)
		{
			return m.Name.Substring(4); // get_ or set_
		}

		public static char GetLabel(this AccessMod t)
		{
			switch (t)
			{
				case AccessMod.Public: return '+';
				case AccessMod.Protected: return '#';
				case AccessMod.Private: return '-';
			}
			return '?';
		}

		private static readonly Dictionary<Type, string> _DISPLAY_ALIAS = new Dictionary<Type, string>()
		{
			{ typeof(int), "int" },
			{ typeof(string), "string" },
			{ typeof(double), "double" },
			{ typeof(float), "float" },
			{ typeof(bool), "bool" },
			{ typeof(void), "void" },
		};
	}
}