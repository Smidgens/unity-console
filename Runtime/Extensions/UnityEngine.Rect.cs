// smidgens @ github

namespace Smidgenomics.Unity.Console
{
	using UnityEngine;

	internal static partial class Rect_
	{
		public static Rect Padded(this Rect r, RectOffset ro)
		{
			var c = r.center;
			r.width -= ro.left + ro.right;
			r.height -= ro.top + ro.bottom;
			r.center = c;
			return r;
		}

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

		public static (Rect, Rect, Rect) SliceRows3(this ref Rect r, float lineHeight, float space)
		{
			var row1 = r.SliceTop(lineHeight);
			r.SliceTop(space);
			var row2 = r.SliceTop(lineHeight);
			r.SliceTop(space);
			var row3 = r.SliceTop(lineHeight);
			r.SliceTop(space);
			return (row1, row2, row3);
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

		public static Rect SliceRight(this ref Rect r, in float w)
		{
			var r2 = r;
			r2.width = w;
			r.width -= w;
			r2.x += r.width;
			return r2;
		}

		public static Rect Resized(this Rect r, in float s)
		{
			var c = r.center;
			r.width += s * 2f;
			r.height += s * 2f;
			r.center = c;
			return r;
		}

	}
}