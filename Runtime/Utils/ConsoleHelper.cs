// smidgens @ github

namespace Smidgenomics.Unity.Console
{
	using System;
	using System.Reflection;
	using System.Collections.Generic;

	internal static class ConsoleHelper
	{
		public static bool IsConsoleUsable(ParameterInfo p)
		{
			if (p.IsOptional || p.IsRefType())
			{
				return false;
			}
			return IsConsoleUsable(p.ParameterType);
		}

		public static bool IsConsoleUsable(MemberInfo m)
		{
			return m.MemberType switch
			{
				MemberTypes.Method => IsConsoleUsable(m as MethodInfo),
				MemberTypes.Property => IsConsoleUsable((m as PropertyInfo)!.PropertyType),
				MemberTypes.Field => IsConsoleUsable((m as FieldInfo)!.FieldType),
				_ => false
			};
		}

		public static bool IsConsoleUsable(MethodInfo x)
		{
			if (!x.IsVoid())
			{
				return false;
			}
			foreach (var p in x.GetParameters())
			{
				if (!IsConsoleUsable(p))
				{
					return false;
				}
			}
			return true;
		}

		public static bool IsConsoleUsable(Type t)
		{
			return Array.IndexOf(ConsoleConstants.CONSOLE_ARG_TYPES, t) > -1;
		}
		
		public static List<ConsoleCallableInfo> FindConsoleCallables()
		{
			var assemblies = ReflectionUtils.GetAssembliesWithAttr<ConsoleAssembly>();

			var handles = new List<ConsoleCallableInfo>();

			foreach (var a in assemblies)
			{
				foreach(var t in a.GetTypes())
				{
					if (!t.IsClass)
					{
						continue;
					}
					var cclass = t.GetCustomAttribute<ConsoleClassAttribute>();

					if (cclass == null)
					{
						continue;
					}

					var scope = cclass.scoped;

					var scopePrefix = string.Empty;

					if (scope)
					{
						scopePrefix = !string.IsNullOrEmpty(cclass.scopeName)
						? cclass.scopeName
						: t.Name;
					}

					if(!string.IsNullOrEmpty(cclass?.scopeName))
					{
						scopePrefix = cclass.scopeName;
					}

					var members = FindConsoleMembers(t, cclass.exposeAll);

					foreach (var m in members)
					{
						var attr = m.GetCustomAttribute<ConsoleCommandAttribute>();
						var name = attr.name ?? string.Empty;

						if (string.IsNullOrEmpty(name))
						{
							name = m.Name;
						}
						else if (!string.IsNullOrEmpty(scopePrefix))
						{
							name = $"{scopePrefix}.{name}";
						}
						
						handles.Add(new ConsoleCallableInfo
						{
							member = m,
							keyword = name,
							description =  attr.description,
						});
					}
				}
			}
			return handles;
		}
	
		private static IEnumerable<MemberInfo> FindConsoleMembers(Type t, bool exposeAll)
		{
			var allowedTypes = MemberTypes.Field | MemberTypes.Method | MemberTypes.Property;
			
			var members = exposeAll
			? t.GetMembers(RFlags.ANY_STATIC_MEMBER)
			: ReflectionUtils.MembersWithAttribute<ConsoleCommandAttribute>(t);
			
			var l = new List<MemberInfo>();
			
			foreach (var m in members)
			{
				if (exposeAll)
				{
					if (!allowedTypes.HasFlag(m.MemberType))
					{
						continue;
					}
					if (m.MemberType == MemberTypes.Method && (m as MethodInfo)!.IsSpecialName)
					{
						continue;
					}
				}
				if (m.IsDefined(typeof(ConsoleHiddenAttribute)))
				{
					continue;
				}
				if (!IsConsoleUsable(m))
				{
					continue;
				}
				l.Add(m);
			}
			return l;
		}
	}
}