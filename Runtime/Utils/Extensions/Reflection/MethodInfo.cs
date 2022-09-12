// smidgens @ github

namespace Smidgenomics.Unity.Console
{
	using System.Reflection;
	using System.Linq;

	internal static class MethodInfo_
	{
		public static byte GetAccessLevel(this MethodInfo m)
		{
			if (m.IsFamily) { return 1; }
			if (m.IsPrivate) { return 0; }
			return 3;
		}

		public static bool TryGetProperty(this MethodInfo m, out PropertyInfo p)
		{
			var pname = m.GetPropertyName();
			p = m.ReflectedType.GetProperty(pname);
			if (!p.CanRead || !p.CanWrite) { return false; }
			return true;
		}

		public static bool IsVoid(this MethodInfo m) => m.ReturnType == typeof(void);

		public static string GetDisplayName(this MethodInfo m)
		{
			return m.IsGetOrSet() ? m.GetPropertyName() : m.Name;
		}

		public static bool IsGetOrSet(this MethodInfo m)
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

		private static readonly FixedArray4<char>
		_ACCESS_TOKENS = new FixedArray4<char>('-', '#', '?', '+');

		public static string GetDisplaySignature(this MethodInfo m)
		{
			var rt = m.ReturnType.GetNameOrAlias();
			var n = m.GetDisplayName();

			var parameters = m.GetParameters();

			var pnames =
			string.Join(", ", parameters
			.Select(x => x.ParameterType.GetNameOrAlias()));

			var accessToken = _ACCESS_TOKENS[m.GetAccessLevel()];

			var accessPrefix = $"[{accessToken}]";

			if (m.IsSpecialName)
			{
				if (m.IsVoid())
				{
					return $"{accessPrefix} {parameters[0].ParameterType.GetNameOrAlias()} {n}";
				}
				return $"{accessPrefix} {rt} {n}";
			}
			return $"{accessPrefix} {rt} {n} ({pnames})";
		}

	}
}