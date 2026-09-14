// smidgens @ github

namespace Smidgenomics.Unity.Console
{
	using System;
	using System.Collections.Generic;
	using UnityEngine;

	internal static class ConsoleConstants
	{
		public const string CREATE_MENU_ROOT = "Console/";
		public const string COMPONENT_ROOT = "Smidgenomics/Console/";
		public const string RES_ROOT = "smidgenomics.console/";

		public static readonly Type[] CONSOLE_ARG_TYPES =
		{
			typeof(string),
			typeof(float),
			typeof(int),
			typeof(bool),
			typeof(UnityEngine.Vector2),
			typeof(UnityEngine.Vector3),
			typeof(UnityEngine.Vector4),
			typeof(UnityEngine.Color),
		};

		public static readonly Color[] LOG_COLORS =
		{
			Color.red, // error
			Color.yellow, // warning
			Color.white, // normal
			Color.cyan, // info
			Color.green, // success
			Color.magenta, // expression
		};
	}
}