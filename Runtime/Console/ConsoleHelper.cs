// smidgens @ github

namespace Smidgenomics.Unity.Console
{
	using System;
	using System.Reflection;
	using System.Collections.Generic;
	using CompilerGen = System.Runtime.CompilerServices.CompilerGeneratedAttribute;

	internal static class ConsoleHelper
	{
		public static List<CommandHandle> FindAttributes(IConsole c, AttributeSearchScope opts = default)
		{
			var filter = opts == AttributeSearchScope.Explicit;
			var assemblies = filter
			? AttributeHelper.Assemblies<ConsoleAssembly>()
			: AppDomain.CurrentDomain.GetAssemblies();

			var handles = new List<CommandHandle>();

			foreach (var a in assemblies)
			{
				if (a.IsDefined(typeof(HideInConsoleAttribute)))
				{
					continue;
				}

				foreach(var t in a.GetTypes())
				{
					if (t.IsDefined(typeof(HideInConsoleAttribute)))
					{
						continue;
					}

					var cclass = t.GetCustomAttribute<ConsoleClassAttribute>();

					if (filter && cclass == null)
					{
						continue;
					}
					var scope = cclass != null ? cclass.scoped : false;

					var scopePrefix = string.Empty;

					if (scope && cclass != null)
					{
						scopePrefix = !string.IsNullOrEmpty(cclass.displayName)
						? cclass.displayName
						: t.Name;
					}


					if(!string.IsNullOrEmpty(cclass?.displayName))
					{
						scopePrefix = cclass.displayName;
					}

					var fields = FindFields(t, cclass);

					foreach (var m in fields)
					{
						var attr = m.GetCustomAttribute<ConsoleCommandAttribute>();
						var name = attr?.name ?? string.Empty;
						var description = attr?.description ?? string.Empty;

						if (string.IsNullOrEmpty(name))
						{
							name = m.Name;
						}

						if (!string.IsNullOrEmpty(scopePrefix))
						{
							name = $"{scopePrefix}.{name}";
						}

						var h = c.AddCommand(name, m, null, description);
						handles.Add(h);
					}
				}
			}
			return handles;
		}

		private static IEnumerable<MemberInfo> FindFields(Type t, ConsoleClassAttribute a)
		{
			if(a == null || !a.exposeAll)
			{
				return AttributeHelper.Members<ConsoleCommandAttribute>(t);
			}

			var l = new List<MemberInfo>();

			foreach (var m in t.GetMethods(RFlags.ANY_STATIC_MEMBER))
			{
				if (m.IsSpecialName) { continue; }

				if (!ConsoleReflection.IsConsoleUsable(m))
				{
					continue;
				}
				if (m.IsDefined(typeof(HideInConsoleAttribute))) { continue; }

				l.Add(m);
			}
			foreach (var p in t.GetProperties(RFlags.ANY_STATIC_MEMBER))
			{
				if (!ConsoleReflection.IsConsoleUsable(p.PropertyType))
				{
					continue;
				}
				if (p.IsDefined(typeof(HideInConsoleAttribute))) { continue; }
				l.Add(p);
			}
			foreach (var f in t.GetFields(RFlags.ANY_STATIC_MEMBER))
			{
				if (!ConsoleReflection.IsConsoleUsable(f.FieldType))
				{
					continue;
				}
				if (f.IsDefined(typeof(CompilerGen)))
				{
					continue;
				}
				if (f.IsDefined(typeof(HideInConsoleAttribute))) { continue; }
				l.Add(f);
			}
			return l;
		}
	}
}