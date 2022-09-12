// smidgens @ github

namespace Smidgenomics.Unity.Console
{
	using System;

	internal enum AttributeSearchScope
	{
		None,
		Explicit,
		Global,
	}

	[Serializable]
	internal class ConsoleSettings
	{
		public AttributeSearchScope attributes = AttributeSearchScope.Explicit;
	}
}