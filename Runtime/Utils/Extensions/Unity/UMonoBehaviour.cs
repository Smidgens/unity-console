// smidgens @ github

namespace Smidgenomics.Unity.Console
{
	using UnityEngine;
	using System;
	using System.Collections;

	internal static partial class MonoBehaviour_
	{
		public static void SetTimeout(this MonoBehaviour c, float t, Action fn)
		{
			if (t <= 0f) { return; }
			c.StartCoroutine(Timeout(t, fn));
		}

		private static IEnumerator Timeout(float t, Action fn)
		{
			yield return new WaitForSeconds(t);
			fn.Invoke();
		}
	}
}