// smidgens @ github

namespace Smidgenomics.Unity.Console.Editor
{
	using UnityEngine;

	/// <summary>
	/// UnityEngine.Rect helpers
	/// </summary>
	internal static class Rect_
	{
		public static Rect Pad(this Rect r, float v)
		{
			var c = r.center;
			r.size -= Vector2.one * v;
			r.center = c;
			return r;
		}

		public static Rect PadLeft(this Rect r, float v)
		{
			var c = r.center;
			r.width -= v;
			r.position += Vector2.right * v;
			return r;
		}

		private struct SplitPad { public float offset, total; }
	}
}

namespace Smidgenomics.Unity.Console.Editor
{
	using SP = UnityEditor.SerializedProperty;

	internal static class SerializedProperty_
	{
		public static string[] GetStringArray(this SP p)
		{
			var a = new string[p.arraySize];
			for (var i = 0; i < p.arraySize; i++)
			{
				a[i] = p.GetArrayElementAtIndex(i).stringValue;
			}
			return a;
		}
	}
}

namespace Smidgenomics.Unity.Console.Editor
{
	using System;
	using System.Reflection;
	using System.Collections.Generic;
	using UnityEngine;
	using SO = UnityEditor.SerializedObject;

	internal static class SerializedObject_
	{
		public static void FindVisibleFields(this SO o, List<string> l)
		{
			FindVisibleFields(o.targetObject.GetType(), l);
		}

		private static void FindVisibleFields(Type t, List<string> l)
		{
			foreach (var f in t.GetFields(RFlags.ANY_INSTANCE_MEMBER))
			{
				if (!IsInspectorVisible(f)) { continue; }
				l.Add(f.Name);
			}
		}

		private static bool IsInspectorVisible(FieldInfo m)
		{
			if (m.IsNotSerialized) { return false; }
			if (m.GetCustomAttribute<HideInInspector>() != null) { return false; }

			if (!m.IsPublic)
			{
				if (m.GetCustomAttribute<SerializeField>() == null) { return false; }
			}
			if (m.GetCustomAttribute<NonSerializedAttribute>() != null) { return false; }
			return true;
		}
	}
}

namespace Smidgenomics.Unity.Console.Editor
{
	using System.Collections.Generic;

	internal static class HashSet_
	{
		public static void EnsureUnique(this HashSet<string> set, ref string key)
		{
			var pi = 1;
			var initial = key;
			while (set.Contains(key))
			{
				key = $"{initial} ({pi})"; pi++;
			}
			set.Add(key);
		}


	}
}