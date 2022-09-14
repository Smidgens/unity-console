// smidgens @ github

namespace Smidgenomics.Unity.Console
{
	using UnityEngine;
	using System;

	internal static class ThemeConfig
	{
		public static readonly LogColors
		defaultLogColors = new LogColors
		{
			normal = Color.white,
			warning = new Color(0.858f, 0.619f, 0.078f),
			info = new Color(0.109f, 0.749f, 0.831f),
			success = new Color(0.2f, 0.721f, 0f),
			error = new Color(1f, 0.219f, 0.227f),
			expression = new Color(1f, 0.5529412f, 1f),
		};

		public static readonly WindowColors
		defaultWindowColors = new WindowColors
		{
			background = Color.black * 0.5f,
		};

		[Serializable]
		internal struct WindowColors
		{
			public Color
			background;
		}


		[Serializable]
		internal struct LogColors
		{
			public Color
			normal,
			info,
			warning,
			success,
			error,
			expression;

			public Color Select(int t) => t switch
			{
				-2 => error,
				-1 => warning,
				0 => normal,
				1 => info,
				2 => success,
				3 => expression,
				_ => normal
			};
		}
	}


	
}