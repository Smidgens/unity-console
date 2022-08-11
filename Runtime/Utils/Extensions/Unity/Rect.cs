// smidgens @ github

namespace Smidgenomics.Unity.Console
{
	using UnityEngine;

	internal static partial class Rect_
	{

		public static Rect Resize(this Rect r, float s)
		{
			var c = r.center;
			r.width += s * 2f;
			r.height += s * 2f;
			r.center = c;
			return r;
		}

		public static Rect Resize(this Rect r, float sx, float sy)
		{
			var c = r.center;
			r.width += sx * 2f;
			r.height += sy * 2f;
			r.center = c;
			return r;
		}


		public static Rect[] SplitHorizontally(this Rect r, int n, double pad = 0.0)
		{
			return r.SplitHorizontally(pad, r.width.Split(n));
		}

		public static Rect[] SplitHorizontally(this Rect pos, params float[] widths)
		{
			return SplitHorizontally(pos, 0.0, widths);
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

		public static Rect[] SplitVertically(this Rect pos, params float[] widths)
		{
			return SplitVertically(pos, 0.0, widths);
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

		public static Rect Pad(this Rect r, float p)
		{
			return r.Pad(p, p, p, p);
		}

		public static Rect Pad(this Rect r, float h, float v)
		{
			return r.Pad(h, h, v, v);
		}

		public static Rect Pad(this Rect r, Vector2 p)
		{
			return r.Pad(p.x, p.x, p.y, p.y);
		}

		public static Rect Pad(this Rect r, RectOffset o)
		{
			return r.Pad(o.left, o.right, o.top, o.bottom);
		}

		public static Rect Pad(this Rect rect, float l, float r, float t, float b)
		{
			var nr = rect;
			nr.width -= l + r;
			nr.height -= t + b;
			nr.x += l;
			nr.y += t;
			return nr;
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