// smidgens @ github

namespace Smidgenomics.Unity.Console
{
	using System.Collections.Generic;

	internal static class HashSet_
	{
		public static void EnsureUnique(this HashSet<string> set, ref string key)
		{
			var pi = 1;
			var initial = key;
			while (set.Contains(key))
			{
				key = $"{initial} ({pi})"; pi++;
			}
			set.Add(key);
		}
	}
}

