// smidgens @ github

namespace Smidgenomics.Unity.Console
{
	using UnityEngine;
	using System;
	using Conditional = System.Diagnostics.ConditionalAttribute;

	[Conditional("UNITY_EDITOR")]
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
	internal class BoolDropdownAttribute : PropertyAttribute
	{
		public BoolDropdownAttribute() { }
		public BoolDropdownAttribute(string l0, string l1) => Labels = new string[] { l0, l1 };
		internal string[] Labels { get; } = { "False", "True" };
	}
}