// smidgens @ github

namespace Smidgenomics.Unity.Console
{
	using UnityEngine;

	internal static class BorderGUI
	{
		public static void Border(in Rect pos, in Color c)
		{
			var w = 1f;
			var hl = pos;
			hl.width = w;
			var vl = pos;
			vl.height = w;
			var l0 = hl;
			var l1 = hl;
			l1.position += new Vector2(pos.width - w, 0f);
			var l2 = vl;
			var l3 = vl;
			l3.position += new Vector2(0f, pos.height - w);
			CGUI.Draw(l0, c);
			CGUI.Draw(l1, c);
			CGUI.Draw(l2, c);
			CGUI.Draw(l3, c);
		}

		public static void BorderTop(in Rect pos, in Color c)
		{
			var div = pos;
			div.height = 1f;
			CGUI.Draw(div, c);
		}
	}
}