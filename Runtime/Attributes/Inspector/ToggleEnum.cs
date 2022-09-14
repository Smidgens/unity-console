// smidgens @ github

namespace Smidgenomics.Unity.Console
{
	using System;
	using UnityEngine;
	using Conditional = System.Diagnostics.ConditionalAttribute;

	[Conditional("UNITY_EDITOR")]
	[AttributeUsage(AttributeTargets.Field)]
	internal class ToggleEnumAttribute : PropertyAttribute
	{
		public ToggleEnumAttribute(string label = null)
		{
			this.label = label;
		}
		internal string label { get; }
	}
}

