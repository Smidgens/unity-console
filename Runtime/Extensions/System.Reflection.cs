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
			return typeof(Component).IsAssignableFrom(t);
		}

		public static bool DeclaresPrivate(this Type t, MethodInfo m)
		{
			return m.DeclaringType == t && m.IsPrivate;
		}

		public static bool IsStatic(this Type t) => t.IsAbstract && t.IsSealed;
		
		public static string GetNameOrAlias(this Type t) => _TYPE_ALIAS.GetValueOrDefault(t) ?? t.Name;
		
		private static readonly Dictionary<Type, string> _TYPE_ALIAS = new()
		{
			// https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/built-in-types
			{ typeof(int), "int" },
			{ typeof(string), "string" },
			{ typeof(double), "double" },
			{ typeof(float), "float" },
			{ typeof(bool), "bool" },
			{ typeof(long), "long" },
			{ typeof(void), "void" },
			{ typeof(object), "object" },
		};
	}
}



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

namespace Smidgenomics.Unity.Console
{
	using System.Reflection;

	internal static class MethodInfo_
	{
		public static bool IsPropertyMethod(this MethodInfo m)
		{
			return
			m.Name.Length > 4
			&& m.IsSpecialName
			&& m.Name[3] == '_';
		}

		public static string GetOwningPropertyName(this MethodInfo m)
		{
			return m.Name[4..]; // get_ or set_
		}
		
		public static bool IsVoid(this MethodInfo m) => m.ReturnType == typeof(void);

		public static PropertyInfo GetBackingProperty(this MethodInfo m)
		{
			var pname = m.GetOwningPropertyName();
			var p = m.ReflectedType?.GetProperty(pname);
			if (p == null)
			{
				return null;
			}
			if (!p.CanRead || !p.CanWrite)
			{
				return null;
			}
			return p;
		}
	}
}


namespace Smidgenomics.Unity.Console
{
	using System.Reflection;

	internal static class PropertyInfo_
	{
		public static bool IsReadWrite(this PropertyInfo p) => p.CanRead && p.CanWrite;
	}
}