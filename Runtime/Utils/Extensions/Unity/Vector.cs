// smidgens @ github

namespace Smidgenomics.Unity.Console
{
	using UnityEngine;

	internal static class Vector2_
	{
		public static Vector2 Clamp(this Vector2 v, float min, float max)
		{
			if (min > max)
			{
				var t = min;
				min = max;
				max = t;
			}
			v.x = Mathf.Clamp(v.x, min, max);
			v.y = Mathf.Clamp(v.y, min, max);
			return v;
		}
	}
}