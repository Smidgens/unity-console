// smidgens @ github


namespace Smidgenomics.Unity.Console
{
	using UnityEngine;
	using System;

	[Serializable]
	internal struct AtlasIcon
	{
		public static readonly AtlasIcon fill = new AtlasIcon
		{
			_size = Vector2.one,
			_offset = Vector2.zero,
		};

		public AtlasIcon(Texture tex, Vector2 size, Vector2 offset)
		{
			_size = size;
			_offset = offset;
			_texture = new LazyLoadReference<Texture>(tex);
		}

		public void Draw(in Rect pos)
		{
			CGUI.Icon(pos, _texture.asset, _offset, _size);
		}

#if UNITY_EDITOR

		internal static class __FN
		{
			public const string
			SIZE = nameof(_size),
			OFFSET = nameof(_offset),
			TEXTURE = nameof(_texture);
		}
#endif
		[SerializeField] private Vector2 _size;
		[SerializeField] private Vector2 _offset;
		[SerializeField] private LazyLoadReference<Texture> _texture;
	}

}


#if UNITY_EDITOR
namespace Smidgenomics.Unity.Console
{
	using UnityEngine;
	using UnityEditor;

	[CustomPropertyDrawer(typeof(AtlasIcon))]
	internal class _AtlasIcon : PropertyDrawer
	{
		public const byte MARGIN_Y = 2;
		public const byte MARGIN_X = 2;
		public const byte INDENT_W = 15;
		public const byte ROWS = 5;
		public const byte ICON_WIDTH = 50;

		public const string TEX_LABEL = "Atlas";

		public static readonly float LINE_HEIGHT = EditorGUIUtility.singleLineHeight;

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			return
			(LINE_HEIGHT + MARGIN_Y) * ROWS
			+ MARGIN_Y;
		}

		public override void OnGUI(Rect pos, SerializedProperty prop, GUIContent l)
		{
			var ti = EditorGUI.indentLevel;

			EditorGUI.indentLevel = 0;

			var ctx = new DrawerContext
			{
				prop = prop,
				texture = prop.FindPropertyRelative(AtlasIcon.__FN.TEXTURE),
				offset = prop.FindPropertyRelative(AtlasIcon.__FN.OFFSET),
				size = prop.FindPropertyRelative(AtlasIcon.__FN.SIZE),
			};

			EditorGUI.BeginProperty(pos, l, prop);
			{
				pos.SliceBottom(MARGIN_Y);

				if (!fieldInfo.FieldType.IsArray)
				{
					EditorGUI.LabelField(pos.SliceTop(LINE_HEIGHT), l);
					pos.SliceTop(MARGIN_Y);
					pos.SliceLeft(INDENT_W);
				}
				
				var texCol = pos.SliceRight(ICON_WIDTH);
				var texRect = texCol.SliceMin();

				PreviewIcon(texRect, ctx);
				pos.SliceRight(MARGIN_X);
				Fields(ref pos, ctx);
				Buttons(ref pos, ctx);
			}
			EditorGUI.EndProperty();
			EditorGUI.indentLevel = ti;
		}

		private delegate void FieldFn(ref Rect pos, in DrawerContext ctx);

		private struct DrawerContext
		{
			public SerializedProperty
			prop, texture, size, offset;
		}

		private static void Fields(ref Rect pos, in DrawerContext ctx)
		{
			Field(ref pos, ctx.texture, 40f, TEX_LABEL);
			Field(ref pos, ctx.size, 40f);
			Field(ref pos, ctx.offset, 40f);
		}

		private static void Buttons(ref Rect pos, in DrawerContext ctx)
		{
			var area = LineRect(ref pos);
			var barea = area.SliceRight(100f);
			var bw = barea.width / _OFFSET_BTNS.Length;
			for (var i = 0; i < _OFFSET_BTNS.Length; i++)
			{
				OffsetButton(barea.SliceLeft(bw), i, ctx);
			}
		}

		private static readonly IFixedArray<(string, int, int)>
		_OFFSET_BTNS = new FixedArray4<(string, int, int)>
		(
			("-x", -1, 0),
			("+x", 1, 0),
			("-y", 0, -1),
			("+y", 0, 1)
		);

		private delegate void OffsetFn(in DrawerContext ctx);

		private static void OffsetButton(in Rect pos, in int i, in DrawerContext ctx)
		{
			var (l, x, y) = _OFFSET_BTNS[i];
			var s = EditorStyles.miniButtonMid;
			if (i == 0) { s = EditorStyles.miniButtonLeft; }
			if (i == _OFFSET_BTNS.Length - 1) { s = EditorStyles.miniButtonRight; }
			if (GUI.Button(pos, l, s))
			{
				AddOffset(ctx, x, y);
			}
		}

		private static void AddOffset(in DrawerContext ctx, in float x, in float y)
		{
			var size = ctx.size.vector2Value;
			var offset = ctx.offset.vector2Value;
			offset.x += size.x * x;
			offset.y += size.y * y;

			if (offset.x < 0f || offset.y < 0f)
			{
				return;
			}

			if (offset.x >= 1f || offset.y >= 1f)
			{
				return;
			}
			ctx.offset.vector2Value = offset;
		}

		private static Rect LineRect(ref Rect pos)
		{
			var area = pos.SliceTop(LINE_HEIGHT);
			pos.SliceTop(MARGIN_Y);
			return area;
		}

		private static void PreviewIcon(in Rect rect, in DrawerContext ctx)
		{
			var tex = ctx.texture.objectReferenceValue as Texture;
			var offset = ctx.offset.vector2Value;
			var size = ctx.size.vector2Value;
			BorderGUI.Border(rect, Color.black);
			CGUI.Color(rect, Color.black * 0.2f);
			if (!tex) { return; }
			var ir = rect.Resized(-2f);
			CGUI.Icon(ir, tex, offset, size);
		}

		private static void Field(ref Rect pos,
		SerializedProperty prop,
		float lw = 0f,
		string label = null
		)
		{
			if(lw > 0f)
			{
				var br = LineRect(ref pos);

				var lr = br.SliceLeft(lw);
				br.SliceLeft(2f);

				if(label == null)
				{
					label = prop.displayName;
				}
				EditorGUI.LabelField(lr, label);
				EditorGUI.PropertyField(br, prop, GUIContent.none);
			}
			else
			{
				EditorGUI.PropertyField(LineRect(ref pos), prop, GUIContent.none);
			}
		}

	}

}

#endif