// smidgens @ github

#pragma warning disable 0414

namespace Smidgenomics.Unity.Console
{
	using UnityEngine;

	public abstract class BaseSO : ScriptableObject
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

	[CustomEditor(typeof(BaseSO), true)]
	internal sealed class _BaseSO : _Base
	{
	}
}

#endif