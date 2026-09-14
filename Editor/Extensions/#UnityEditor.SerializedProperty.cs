// smidgens @ github

#if UNITY_EDITOR

namespace Smidgenomics.Unity.Console.Editor
{
	using System;
	using UnityEditor;
	using UnityEngine;

	internal static class _SerializedProperty_
	{
		public static string[] GetArrayStringValues(this SerializedProperty p)
		{
			return GetArrayValues(p, GetStringValue);
		}

		private static string GetStringValue(SerializedProperty p) => p.stringValue;
		
		public static T[] GetArrayValues<T>(this SerializedProperty p, Func<SerializedProperty,T> fn)
		{
			if (!p.isArray)
			{
				return Array.Empty<T>();
			}
			var a = new T[p.arraySize];
			for (var i = 0; i < p.arraySize; i++)
			{
				a[i] = fn.Invoke(p.GetArrayElementAtIndex(i));
			}
			return a;
		}
	}
}

#endif