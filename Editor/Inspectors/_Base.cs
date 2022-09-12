// smidgens @ github

namespace Smidgenomics.Unity.Console.Editor
{
	using UnityEditor;
	using System.Collections.Generic;

	using SP = UnityEditor.SerializedProperty;
	using SO = UnityEditor.SerializedObject;

	internal class _Base : Editor
	{
		public override void OnInspectorGUI()
		{
			if(_fieldNames == null)
			{
				_fieldNames = new List<string>();
				serializedObject.FindVisibleFields(_fieldNames);
			}

			serializedObject.UpdateIfRequiredOrScript();
			foreach (var n in _fieldNames)
			{
				if (!ShowField(n)) { continue; }
				var p = serializedObject.FindProperty(n);
				if (p == null) { continue; }
				EditorGUILayout.PropertyField(p);
			}
			serializedObject.ApplyModifiedProperties();
		}

		protected virtual bool ShowField(string name)
		{
			return true;
		}

		protected void Find(string name, ref SP prop)
		{
			prop = serializedObject.FindProperty(name);
		}

		protected static void Find(SO so, string[] names, ref SP[] props)
		{
			if (props.Length != names.Length)
			{
				props = new SP[names.Length];
			}
			for (var i = 0; i < names.Length; i++)
			{
				props[i] = so.FindProperty(names[i]);
			}
		}

		private List<string> _fieldNames = null;

	}
}