// smidgens @ github

namespace Smidgenomics.Unity.Console
{
	using System.Text.RegularExpressions;

	internal static class String_
	{
		public static bool IsMatch(this string s, string pattern)
		{
			return Regex.IsMatch(s, pattern);
		}

		public static string ToSentenceCase(this string s)
		{
			return Regex.Replace(s, "([A-Z])", " $1", RegexOptions.Compiled).Trim();
		}

		public static string Capitalize(this string s)
		{
			if (s == null) { return null; }
			if (s.Length < 2) { return s.ToUpper(); }
			return s[0].ToString().ToUpper() + s.Substring(1);
		}

		public static bool Wildcard(this string s, string pattern)
		{
			return Regex.IsMatch(s, pattern.ToWildcardRegex());
		}

		public static string ToWildcardRegex(this string v)
		{
			return "^" + Regex.Escape(v).Replace("\\?", ".").Replace("\\*", ".*") + "$";
		}
	}
}