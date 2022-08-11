// smidgens @ github

namespace Smidgenomics.Unity.Console.Editor
{
	using UnityEngine;

	/// <summary>
	/// UnityEngine.Rect helpers
	/// </summary>
	internal static class Rect_
	{
		public static Rect[] SplitHorizontally(this Rect pos, double pad, params float[] widths)
		{
			var r = new Rect[widths.Length];
			if (widths.Length == 0) { return r; }
			var padding = GetSplitPadding(widths.Length, pos.width, pad);
			var totalSize = pos.width - padding.total;
			var w = totalSize.Split(widths);
			var offset = 0f;
			for (var i = 0; i < w.Length; i++)
			{
				r[i] = pos;
				r[i].x += offset;
				r[i].width = w[i];
				offset += w[i] + padding.offset;
			}
			return r;
		}

		public static Rect Pad(this Rect r, float v)
		{
			var c = r.center;
			r.size -= Vector2.one * v;
			r.center = c;
			return r;
		}

		public static Rect PadLeft(this Rect r, float v)
		{
			var c = r.center;
			r.width -= v;
			r.position += Vector2.right * v;
			return r;
		}



		private static SplitPad GetSplitPadding(int n, float v, double p)
		{
			if (n < 2) { return default; }
			var o = System.Convert.ToSingle(p);
			// ratio
			if (o < 1) { o = o * v; }
			return new SplitPad { offset = o, total = o * (n - 1) };
		}
		private struct SplitPad { public float offset, total; }
	}
}

namespace Smidgenomics.Unity.Console.Editor
{
	using System.Linq;

	/// <summary>
	/// float helpers
	/// </summary>
	internal static class Float_
	{
		public static float[] Split(this float v, params float[] weights)
		{
			if (weights.Length == 0) { return new float[0]; }
			var flex = v;
			// absolute weights, >1
			foreach (var w in weights) { if (w > 1f) { flex -= w; } }
			return weights.Select((w, i) => w > 1f ? w : w * flex).ToArray();
		}
	}
}

namespace Smidgenomics.Unity.Console.Editor
{
	using SP = UnityEditor.SerializedProperty;

	internal static class SerializedProperty_
	{
		public static string[] GetStringArray(this SP p)
		{
			var a = new string[p.arraySize];
			for (var i = 0; i < p.arraySize; i++)
			{
				a[i] = p.GetArrayElementAtIndex(i).stringValue;
			}
			return a;
		}
	}
}

namespace Smidgenomics.Unity.Console.Editor
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