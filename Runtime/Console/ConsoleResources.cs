// smidgens @ github

namespace Smidgenomics.Unity.Console
{
	using UnityEngine;
	using System;

#if SMIDGENOMICS_DEV
	[CreateAssetMenu(menuName = "Console/Resources")]
#endif
	internal class ConsoleResources : ScriptableObject
	{
		public static ConsoleResources Instance => GetInstance();

		public static ConsoleResources GetInstance()
		{
			var (instance, init) = _cache;
			if (init) { return instance; }
			var path = Config.ResourcePath.DEFAULTS;
			instance = Resources.Load<ConsoleResources>(path);
			_cache = (instance, true);
			return instance;
		}

		public DefaultStyles Styles => _styles;
		public ConsoleTheme Theme => _theme;

		[Serializable]
		public struct DefaultStyles
		{
			public GUIStyle text, input, timestamp;
		}

		[SerializeField] ConsoleTheme _theme = default;
		[SerializeField] DefaultStyles _styles = default;

		private static (ConsoleResources, bool) _cache = default;



	}
}