// smidgens @ github

namespace Smidgenomics.Unity.Console
{
	using System;

	internal static class DateTime_
	{
		public static double ToUnixMS(this DateTime d)
		{
			return d.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds;
		}

		public static double ToUnix(this DateTime d)
		{
			return d.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
		}

		public static string ToLogTime(this DateTime d)
		{
			return d.ToString("HH:mm:ss");
		}
	}
}