// smidgens @ github

namespace Smidgenomics.Unity.Console
{
	using UnityEngine;

	internal static class BorderGUI
	{
		public static void Border(in Rect pos, in Color c, in float w = 1f)
		{
			var br = pos;
			CGUI.Color(br.SliceLeft(w), c);
			CGUI.Color(br.SliceRight(w), c);
			CGUI.Color(br.SliceTop(w), c);
			CGUI.Color(br.SliceBottom(w), c);
		}

		public static void BorderTop(in Rect pos, in Color c, in float w = 1f)
		{
			var br = pos;
			CGUI.Color(br.SliceTop(w), c);
		}

		public static void BorderLeft(in Rect pos, in Color c, in float w = 1f)
		{
			var br = pos;
			CGUI.Color(br.SliceLeft(w), c);
		}

		public static void BorderRight(in Rect pos, in Color c, in float w = 1f)
		{
			var br = pos;
			CGUI.Color(br.SliceRight(w), c);
		}

		public static void BorderBottom(in Rect pos, in Color c, in float w = 1f)
		{
			var br = pos;
			CGUI.Color(br.SliceBottom(w), c);
		}
	}
}