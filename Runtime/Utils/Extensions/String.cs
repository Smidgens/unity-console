// smidgens @ github

namespace Smidgenomics.Unity.Console
{
	using System.Text.RegularExpressions;

	internal static partial class String_
	{
		public static bool IsMatch(this string s, string pattern)
		{
			return Regex.IsMatch(s, pattern);
		}
		public static string ToRichBold(this string s)
		{
			return s.ToHTMLTagged("b");
		}

		public static string ToRichItalic(this string s)
		{
			return s.ToHTMLTagged("i");
		}

		public static string ToDurationString(this int seconds)
		{
			return ToDurationString(System.Convert.ToSingle(seconds));
		}

		public static string ToDurationString(this double seconds)
		{
			return ToDurationString(System.Convert.ToSingle(seconds));
		}

		public static string ToDurationString(this float seconds)
		{
			var hours = (int)(seconds / 3600f);
			seconds = seconds - hours * 3600f;
			var minutes = (int)(seconds / 60f);
			seconds = seconds - minutes * 60f;
			var rsecs = ((int)seconds);
			var ms = (int)((seconds - rsecs) * 1000f);
			return string.Join("", new string[]
			{
				hours > 0 ? $"{hours}h" : "",
				minutes > 0 ? $"{minutes}m" : "",
				rsecs > 0 ? $"{rsecs}s" : "",
				rsecs < 0 || ms > 0 ? $"{ms}ms" : ""
			});
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

		public static string Slice(this string s, int v)
		{
			return v >= 0 ? s.Slice(v, 0) : s.Slice(0, v);
		}

		public static string Slice(this string s, int start = 0, int end = 0)
		{
			if (string.IsNullOrEmpty(s)) { return null; }
			if (start >= s.Length || start < 0) { return null; }
			if (end == 0) { return s.Substring(start); }
			var endIndex = end;
			if (endIndex < 0) { endIndex = s.Length + end; } // get index from end
			if (endIndex <= start) { return null; }
			return s.Substring(start, endIndex - start);
		}

		public static string ToWildcardRegex(this string v)
		{
			return "^" + Regex.Escape(v).Replace("\\?", ".").Replace("\\*", ".*") + "$";
		}

		private static string ToHTMLTagged(this string v, string t)
		{
			return $"<{t}>{v}</{t}>";
		}
	}
}