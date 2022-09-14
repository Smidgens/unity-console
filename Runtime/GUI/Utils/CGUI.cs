// smidgens @ github

namespace Smidgenomics.Unity.Console
{
	using UnityEngine;

	internal static class CGUI
	{
		public static Texture TextureWhite => _whiteTex.Value;

		public static void Color(in Rect pos, in Color c)
		{
			Draw(pos, TextureWhite, c);
		}

		public static void Icon(in Rect pos,Texture tex)
		{
			Icon(pos, tex, Vector2.one, Vector2.zero);
		}

		public static void Icon
		(
			in Rect area,
			Texture tex,
			in Vector2 offset,
			in Vector2 size
		)
		{
			if(size.x == 0 || size.y == 0) { return; }
			GUI.BeginClip(area);
			var sx = 1f / size.x;
			var sy = 1f / size.y;
			var ir = area;
			ir.size = new Vector2
			(
				sx * area.width,
				sy * area.width
			);
			ir.position = new Vector2
			(
				-offset.x * area.width * sx,
				-offset.y * area.height * sy
			);
			GUI.DrawTexture(ir, tex, ScaleMode.StretchToFill);
			GUI.EndClip();
		}

		public static void Draw(in Rect pos, Texture tex, Color c)
		{
			if (!tex) { return; }
			var tc = GUI.color;
			GUI.color = c;
			GUI.DrawTexture(pos, tex, ScaleMode.StretchToFill);
			GUI.color = tc;
		}

		private static LazyTex _whiteTex = LazyTex.New(UnityEngine.Color.white, 1);
	}
}