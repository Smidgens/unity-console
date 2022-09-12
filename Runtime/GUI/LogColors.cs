// smidgens @ github

namespace Smidgenomics.Unity.Console
{
	using UnityEngine;
	using System;

	[Serializable]
	internal struct LogColors
	{
		public static LogColors Default
		{
			get
			{
				return new LogColors
				{
					_normal = Color.white,
					_warning = new Color(0.858f, 0.619f, 0.078f),
					_info = new Color(0.109f, 0.749f, 0.831f),
					_success = new Color(0.2f, 0.721f, 0f),
					_error = new Color(1f, 0.219f, 0.227f),
					_expression = new Color(1f, 0.5529412f, 1f),
				};
			}
		}

		[SerializeField]
		private Color
		_normal,
		_info,
		_warning,
		_success,
		_error,
		_expression;

		public Color Select(int t)
		{
			switch (t)
			{
				case -2: return _error;
				case -1: return _warning;
				case 1: return _info;
				case 2: return _success;
				case 3: return _expression;
			}
			return _normal;
		}
	}
}