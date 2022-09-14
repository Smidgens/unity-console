// smidgens @ github

namespace Smidgenomics.Unity.Console
{
	using System;
	using UnityEngine;
	using Conditional = System.Diagnostics.ConditionalAttribute;

	// display type fields expanded
	[AttributeUsage(AttributeTargets.Field)]
	[Conditional("UNITY_EDITOR")]
	internal class ExpandAttribute : PropertyAttribute
	{
		public ExpandAttribute(string label = null)
		{
			this.label = label;
		}

		internal string label { get; }
	}
}
