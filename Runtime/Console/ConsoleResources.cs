// smidgens @ github

namespace Smidgenomics.Unity.Console
{
	using UnityEngine;
	using System;

#if SMIDGENOMICS_DEV
	[CreateAssetMenu(menuName = "Console/Resources", order = 25)]
#endif
	internal class ConsoleResources : ScriptableObject
	{
		public static ConsoleResources GetInstance()
		{
			var (instance, init) = _cache;
			if (init) { return instance; }
			var path = Config.ResourcePath.DEFAULTS;
			instance = Resources.Load<ConsoleResources>(path);
			_cache = (instance, true);
			return instance;
		}

		public Font DefaultFont => _font;
		public ConsoleTheme DefaultTheme => _theme;
		public Texture Icons => _icons;

		[Serializable]
		public struct DefaultStyles
		{
			public GUIStyle text, input, timestamp;
		}

		[SerializeField] Font _font = default;
		[SerializeField] ConsoleTheme _theme = default;
		[SerializeField] Texture _icons = default;

		private static (ConsoleResources, bool) _cache = default;
	}
}