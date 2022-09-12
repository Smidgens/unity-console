// smidgens @ github

namespace Smidgenomics.Unity.Console
{
	using UnityEngine;
	using UnityObject = UnityEngine.Object;

	internal static partial class UnityObject_
	{
		public static bool TryGetGameObject(this UnityObject ob, out GameObject go)
		{
			go = null;
			if (!ob) { return false; }
			var tt = ob.GetType();
			// scene reference
			if (tt.IsUnityComponent()) { go = (ob as Component).gameObject; }
			else if (tt == typeof(GameObject)) { go = ob as GameObject; }
			return go;
		}
	}
}