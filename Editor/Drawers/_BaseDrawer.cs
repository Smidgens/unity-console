// smidgens @ github

namespace Smidgenomics.Unity.Console.Editor
{
	using UnityEngine;
	using UnityEditor;

	using SP = UnityEditor.SerializedProperty;
	internal abstract class _BaseDrawer : PropertyDrawer
	{
		public override sealed float GetPropertyHeight(SP property, GUIContent label)
		{
			if (!_hasInit)
			{
				_hasInit = true;
				OnInit(property, label);
			}
			return GetHeight(property, label);
		}

		public override sealed void OnGUI(Rect pos, SP prop, GUIContent l)
		{
			EditorGUI.BeginProperty(pos, l, prop);
			if (ShouldDrawPrefix(prop, l))
			{
				EditorGUI.LabelField(SliceLine(ref pos), l);
			}
			OnDrawGUI(pos, prop, l);
			EditorGUI.EndProperty();
		}

		protected const byte _MARGIN_Y = 2;
		protected const byte _INDENT = 15;
		protected static readonly float _LINE_HEIGHT = EditorGUIUtility.singleLineHeight;

		protected virtual bool ShouldDrawPrefix(SP prop, GUIContent l)
		{
			return l != GUIContent.none
			&& !fieldInfo.FieldType.IsArray;
		}

		protected virtual float GetHeight(SP prop, GUIContent l)
		{
			return base.GetPropertyHeight(prop, l);
		}

		protected virtual void OnDrawGUI(in Rect pos, SP prop, GUIContent l) { }

		protected virtual void OnInit(SP first, GUIContent l) { }

		protected static Rect SliceLine(ref Rect pos)
		{
			var r = pos.SliceTop(_LINE_HEIGHT);
			pos.SliceTop(_MARGIN_Y);
			return r;
		}

		private bool _hasInit = false;

	}
}