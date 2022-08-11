// smidgens @ github

namespace Smidgenomics.Unity.Console
{
	using UnityEngine;
	using System;
	using System.Collections;

	/// <summary>
	/// Coroutine helpers
	/// </summary>
	internal static class ScriptRoutine
	{
		public static void DelayCall(MonoBehaviour c, float t, Action fn)
		{
			if (t <= 0f) { return; }
			c.StartCoroutine(DelayRoutine(t, fn));
		}

		private static IEnumerator DelayRoutine(float t, Action fn)
		{
			yield return new WaitForSeconds(t);
			fn.Invoke();
		}
	}
}