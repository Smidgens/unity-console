// smidgens @ github

namespace Smidgenomics.Unity.Console
{
	using System;
	using UnityEngine;

	// TODO: property drawer

	[AttributeUsage(AttributeTargets.Field)]
	internal class ListWrapperOptions : Attribute
	{
		// todo: configure buttons, header, etc
	}

	[Serializable]
	internal struct ListWrapper<T>
	{
		public T this[int i]
		{
			get => _items[i];
		}
		[SerializeField] private T[] _items;
	}
}