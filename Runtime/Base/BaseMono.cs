// smidgens @ github

namespace Smidgenomics.Unity.Console
{
	using UnityEngine;

	public abstract class BaseMono : MonoBehaviour
	{
		
	}
}

//=================================================
//=============== EDITOR ==========================
//=================================================

#if UNITY_EDITOR

namespace Smidgenomics.Unity.Console.Editor
{
	using UnityEditor;

	[CustomEditor(typeof(BaseMono), true)]
	internal sealed class _BaseMono : _Base
	{
	}
}

#endif