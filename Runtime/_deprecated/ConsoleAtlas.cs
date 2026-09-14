// // smidgens @ github
//
// namespace Smidgenomics.Unity.Console
// {
// 	internal enum EAtlasIcon
// 	{
// 		Shell,
// 	}
// }
//
// namespace Smidgenomics.Unity.Console
// {
// 	using UnityEngine;
// 	using System;
//
// 	[Serializable]
// 	internal struct AtlasSprite
// 	{
// 		public static readonly AtlasSprite fill = TileSize(1);
//
// 		public static AtlasSprite TileSize(int n)
// 		{
// 			n = Mathf.Max(n, 1);
// 			var size = Vector2.one * (1f / n);
// 			return new AtlasSprite
// 			{
// 				_coords = new Rect(default, size)
// 			};
// 		}
//
// 		public AtlasSprite(Texture tex, Rect coords)
// 		{
// 			_texture = new LazyLoadReference<Texture>(tex);
// 			_coords = coords;
// 		}
//
// 		public readonly void Draw(in Rect pos)
// 		{
// 			CGUI.DrawTexture(pos, _texture.asset, _coords);
// 		}
//
// 		[SerializeField] internal Rect _coords;
// 		[SerializeField] internal LazyLoadReference<Texture> _texture;
// 	}
// }
//
// namespace Smidgenomics.Unity.Console
// {
// 	using UnityEngine;
// 	using System;
//
// 	internal sealed class ConsoleAtlas
// 	{
// 		public static ref readonly AtlasSprite Shell => ref GetInstance()._shellIcon;
// 		private ConsoleAtlas() { }
// 		private static ConsoleAtlas _instance;
// 		private AtlasSprite _shellIcon;
//
// 		private static Rect GetIconCoords(EAtlasIcon icon)
// 		{
// 			return icon switch
// 			{
// 				EAtlasIcon.Shell => new Rect(0f, 0f, 0.25f,0.25f),
// 				_ => default
// 			};
// 		}
//
// 		private static ConsoleAtlas GetInstance()
// 		{
// 			if (_instance == null)
// 			{
// 				var tex = ConsoleResources.GetInstance().Icons;
// 				_instance = new ConsoleAtlas
// 				{
// 					_shellIcon = new AtlasSprite(tex, GetIconCoords(EAtlasIcon.Shell))
// 				};
// 			}
// 			return _instance;
// 		}
// 	}
// }
//
// #if UNITY_EDITOR
//
// namespace Smidgenomics.Unity.Console.Editor
// {
// 	using UnityEngine;
// 	using UnityEditor;
//
// 	[CustomPropertyDrawer(typeof(AtlasSprite))]
// 	internal sealed class _AtlasIcon : PropertyDrawer
// 	{
// 		public override float GetPropertyHeight(SerializedProperty property, GUIContent l)
// 		{
// 			var h = EditorGUIUtility.singleLineHeight * 3f;
// 			if (l != GUIContent.none)
// 			{
// 				h += EditorGUIUtility.singleLineHeight;
// 				h += EditorGUIUtility.standardVerticalSpacing;
// 			}
// 			h += EditorGUIUtility.standardVerticalSpacing * 2f;
// 			return h;
// 		}
//
// 		public override void OnGUI(Rect pos, SerializedProperty prop, GUIContent l)
// 		{
// 			EditorGUI.BeginProperty(pos, l, prop);
// 			
// 			var ti = EditorGUI.indentLevel;
// 			EditorGUI.indentLevel = 0;
// 			
// 			if (l != GUIContent.none)
// 			{
// 				var labelRow = pos.SliceTop(EditorGUIUtility.singleLineHeight);
// 				EditorGUI.HandlePrefixLabel(pos, labelRow, l);
// 				pos.SliceLeft(EditorGUIUtility.standardVerticalSpacing);
// 			}
//
// 			var texProp = prop.FindPropertyRelative(nameof(AtlasSprite._texture));
// 			var coordsProp = prop.FindPropertyRelative(nameof(AtlasSprite._coords));
//
// 			var iconArea = pos.SliceLeft(pos.height);
// 			pos.SliceLeft(EditorGUIUtility.standardVerticalSpacing);
//
// 			PreviewIcon(iconArea, texProp.objectReferenceValue as Texture, coordsProp.rectValue);
//
// 			var texHeight = EditorGUIUtility.singleLineHeight;
// 			var texRect = pos.SliceTop(texHeight);
// 			pos.SliceTop(EditorGUIUtility.standardVerticalSpacing);
// 			EditorGUI.PropertyField(texRect, texProp, GUIContent.none);
// 			EditorGUI.PropertyField(pos, coordsProp, GUIContent.none);
// 			EditorGUI.indentLevel = ti;
// 			EditorGUI.EndProperty();
// 		}
//
// 		private static void PreviewIcon(in Rect rect, Texture tex, Rect coords)
// 		{
// 			var inner = rect.Resized(-rect.height * 0.1f);
// 			GUI.Box(rect, GUIContent.none, EditorStyles.helpBox);
// 			CGUI.DrawTexture(inner, tex, coords);
// 		}
// 	}
//
// }
//
// #endif