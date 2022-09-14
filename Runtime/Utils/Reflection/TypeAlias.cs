// smidgens @ github

namespace Smidgenomics.Unity.Console
{
	using System;
	using System.Collections.Generic;

	internal static class TypeAlias
	{
		public static string Get(Type t)
		{
			return _ALIAS.TryGetValue(t, out var v) ? v : null;
		}

		private static readonly Dictionary<Type, string>
		_ALIAS = new Dictionary<Type, string>()
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