// smidgens @ github

#if UNITY_EDITOR

namespace Smidgenomics.Unity.Console.Editor
{
	using UnityEngine;
	using UnityEditor;

	using SP = UnityEditor.SerializedProperty;
	internal abstract class _BaseDrawer : PropertyDrawer
	{
		public sealed override float GetPropertyHeight(SP property, GUIContent label)
		{
			if (!_hasInit)
			{
				_hasInit = true;
				OnInit(property, label);
			}
			return GetHeight(property, label);
		}

		public sealed override void OnGUI(Rect pos, SP prop, GUIContent l)
		{
			EditorGUI.BeginProperty(pos, l, prop);
			if (l != GUIContent.none && ShouldDrawPrefix(prop, l))
			{
				pos = EditorGUI.PrefixLabel(pos, l);
			}
			OnDrawGUI(pos, prop, l);
			EditorGUI.EndProperty();
		}

		protected virtual bool ShouldDrawPrefix(SP prop, GUIContent l)
		{
			return true;
		}

		protected virtual float GetHeight(SP prop, GUIContent l)
		{
			return base.GetPropertyHeight(prop, l);
		}

		protected virtual void OnDrawGUI(in Rect pos, SP prop, GUIContent l) { }

		protected virtual void OnInit(SP first, GUIContent l) { }

		private bool _hasInit;

	}
}

#endif