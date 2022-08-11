// smidgens @ github

namespace Smidgenomics.Unity.Console.Editor
{
	using UnityEngine;
	using UnityEditor;
	using System;

	using UObject = UnityEngine.Object;
	using SP = UnityEditor.SerializedProperty;
	using SO = UnityEditor.SerializedObject;

	internal class _Base : Editor
	{
		protected void Find(in string name, ref SP prop)
		{
			prop = serializedObject.FindProperty(name);
		}

		protected static void Find(SO so, in string[] names, ref SP[] props)
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

		protected static void LayoutFields(params SP[] props)
		{
			foreach (var p in props)
			{
				EditorGUILayout.PropertyField(p);
			}
		}

	}
}