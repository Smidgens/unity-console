// smidgens @ github

namespace Smidgenomics.Unity.Console
{
	using UnityEngine;
	using System;
	using UnityEngine.UIElements;

#if SM_DEV
	[CreateAssetMenu(menuName = "Console/Resources", order = 25)]
#endif
	[ExcludeFromPreset]
	internal sealed class ConsoleResources : BaseSO
	{
		private const string RES_PATH = ConsoleConstants.RES_ROOT + "{defaults}";

		public static ConsoleResources GetInstance()
		{
			var (instance, init) = _cache;
			if (init)
			{
				return instance;
			}
			var path = RES_PATH;
			instance = Resources.Load<ConsoleResources>(path);
			_cache = (instance, true);
			return instance;
		}

		[SerializeField] internal StyleSheet _rootStyleSheet;
		[SerializeField] internal VisualTreeAsset _windowTemplate;
		[SerializeField] internal Texture _icons;

		private static (ConsoleResources, bool) _cache;
	}
}