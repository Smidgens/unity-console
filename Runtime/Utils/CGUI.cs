// smidgens @ github

namespace Smidgenomics.Unity.Console
{
	using UnityEngine;
	using System;

	internal static class CGUI
	{
		public static Texture2D TextureWhite => GetWhiteTex();

		public static void Draw(in Rect pos, in Color c)
		{
			Draw(pos, TextureWhite, c);
		}

		public static void Draw(in Rect pos, Texture2D tex, Color c)
		{
			if (!tex) { return; }
			var tc = GUI.color;
			GUI.color = c;
			GUI.DrawTexture(pos, tex, ScaleMode.StretchToFill);
			GUI.color = tc;
		}

		public static void BorderTop(in Rect pos, in Color c)
		{
			var div = pos;
			div.height = 1f;
			Draw(div, c);
		}

		public static void Border(in Rect pos, in Color c, bool l,bool r,bool t, bool b)
		{
			// todo
		}

		public static void Border(in Rect pos, in Color c)
		{
			var w = 1f;
			var lines = new Rect[4];
			var hl = pos;
			hl.width = w;
			var vl = pos;
			vl.height = w;
			lines[0] = hl;
			lines[1] = hl;
			lines[1].position += new Vector2(pos.width - w, 0f);
			lines[2] = vl;
			lines[3] = vl;
			lines[3].position += new Vector2(0f, pos.height - w);

			foreach (var l in lines)
			{
				Draw(l, c);
			}
		}



		private static Texture2D GetWhiteTex()
		{
			if(!_whiteTex)
			{
				_whiteTex = new Texture2D(1, 1);
				_whiteTex.SetPixel(0, 0, Color.white);
				_whiteTex.Apply();
				_whiteTex.wrapMode = TextureWrapMode.Repeat;
				_whiteTex.filterMode = FilterMode.Point;
				return _whiteTex;
			}
			return _whiteTex;
		}


		private static Texture2D _whiteTex = null;
	}
}