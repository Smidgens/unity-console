// smidgens @ github

namespace Smidgenomics.Unity.Console
{
	using UnityEngine;
	using System;

	internal static partial class MonoBehaviour_
	{
		public static void DelayCall(this MonoBehaviour c, float t, Action fn)
		{
			if (t <= 0f) { return; }
			ScriptRoutine.DelayCall(c, t, fn);
		}
	}
}