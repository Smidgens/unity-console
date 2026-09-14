// // smidgens @ github
//
// namespace Smidgenomics.Unity.Console
// {
// 	using System;
// 	using UnityEngine;
//
// 	internal static class CGUI
// 	{
// 		public static void DrawTexture
// 		(
// 			in Rect area,
// 			Texture tex,
// 			in Rect coords
// 		)
// 		{
// 			if (!tex)
// 			{
// 				return;
// 			}
// 			var size = coords.size;
// 			var offset = coords.position;
// 			
// 			if(size.x == 0 || size.y == 0) { return; }
// 			GUI.BeginClip(area);
// 			var sx = 1f / size.x;
// 			var sy = 1f / size.y;
// 			var ir = area;
// 			ir.size = new Vector2
// 			(
// 				sx * area.width,
// 				sy * area.width
// 			);
// 			ir.position = new Vector2
// 			(
// 				-offset.x * area.width * sx,
// 				-offset.y * area.height * sy
// 			);
// 			GUI.DrawTexture(ir, tex, ScaleMode.StretchToFill);
// 			GUI.EndClip();
// 		}
// 	}
// }