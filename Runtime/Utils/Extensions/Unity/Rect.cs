// smidgens @ github

namespace Smidgenomics.Unity.Console
{
	using UnityEngine;

	internal static partial class Rect_
	{
		public static void Pad(this ref Rect r, float p) => r.Pad(p, p, p, p);
		public static void Pad(this ref Rect r, float h, float v) => r.Pad(h, h, v, v);

		public static void Pad(this ref Rect rect, float l, float r, float t, float b)
		{
			var nr = rect;
			nr.width -= l + r;
			nr.height -= t + b;
			nr.x += l;
			nr.y += t;
		}

		public static Rect SliceTop(this ref Rect r, in float s)
		{
			var r2 = r;
			r2.height = s;
			r.height -= s;
			r.position += new Vector2(0f, s);
			return r2;
		}

		public static Rect SliceBottom(this ref Rect r, in float s)
		{
			var r2 = r;
			r2.height = s;
			r.height -= s;
			r2.y += r.height;
			return r2;
		}

		public static Rect SliceLeft(this ref Rect r, in float w)
		{
			var r2 = r;
			r2.width = w;
			r.width -= w;
			r.x += w;
			return r2;
		}

		public static Rect SliceX(this ref Rect r, in float s)
		{
			return s < 0f
			? r.SliceLeft(s * -1f)
			: r.SliceRight(s);
		}

		public static Rect SliceY(this ref Rect r, in float s)
		{
			return s < 0f
			? r.SliceTop(s * -1f)
			: r.SliceBottom(s);
		}

		public static Rect MinSize(this Rect r)
		{
			var min = Mathf.Min(r.width, r.height);
			r.size = new Vector2(min, min);
			return r;
		}

		public static Rect SliceMin(this ref Rect r)
		{
			if (r.width < r.height)
			{
				return r.SliceTop(r.width);
			}
			return r.SliceLeft(r.height);
		}


		public static Rect SliceRight(this ref Rect r, in float w)
		{
			var r2 = r;
			r2.width = w;
			r.width -= w;
			r2.x += r.width;
			return r2;
		}

		public static void Resize(this ref Rect r, in float s)
		{
			r = Resized(r, s);
		}

		public static Rect Resized(this Rect r, in float s)
		{
			var c = r.center;
			r.width += s * 2f;
			r.height += s * 2f;
			r.center = c;
			return r;
		}

		public static Rect Resized(this Rect r, float sx, float sy)
		{
			var c = r.center;
			r.width += sx * 2f;
			r.height += sy * 2f;
			r.center = c;
			return r;
		}

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

		public static Rect[] SplitVertically(this Rect r, int n, double pad = 0.0)
		{
			return r.SplitVertically(pad, r.height.Split(n));
		}

		public static Rect[] SplitVertically(this Rect pos, double pad, params float[] sizes)
		{
			var r = new Rect[sizes.Length];
			if (sizes.Length == 0) { return r; }
			var padding = GetSplitPadding(sizes.Length, pos.height, pad);
			var totalSize = pos.height - padding.total;

			var weights = totalSize.Split(sizes);

			var offset = 0f;
			for (var i = 0; i < weights.Length; i++)
			{
				r[i] = pos;
				r[i].y += offset;
				r[i].height = weights[i];
				offset += weights[i] + padding.offset;
			}
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