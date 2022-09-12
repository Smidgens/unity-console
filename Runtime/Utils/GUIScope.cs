// smidgens @ github

namespace Smidgenomics.Unity.Console
{
	using System;
	using UnityEngine;

	internal static partial class GUIScope
	{
		public struct GUIColor : IDisposable
		{
			public static GUIColor Content(Color c)
			{
				return new GUIColor(c, GetContent, SetContent);
			}

			public static GUIColor Background(Color c)
			{
				return new GUIColor(c, GetBackground, SetBackground);
			}

			public static GUIColor Text(Color c)
			{
				return new GUIColor(c, GetColor, SetColor);
			}

			private static void SetContent(Color c) => GUI.contentColor = c;
			private static void SetBackground(Color c) => GUI.backgroundColor = c;
			private static void SetColor(Color c) => GUI.color = c;

			private GUIColor(Color c, Func<Color> getFn, Action<Color> setFn)
			{
				_prevColor = getFn.Invoke();
				_setFn = setFn;
				_disposed = false;
				setFn.Invoke(c);
			}

			public void Dispose()
			{
				Dispose(disposing: true);
			}

			private void Dispose(bool disposing)
			{
				if (_disposed) { return; }
				if (!disposing) { return; }
				_disposed = true;
				GUI.contentColor = _prevColor;
				_setFn.Invoke(_prevColor);
			}

			private static Color GetContent() => GUI.contentColor;
			private static Color GetColor() => GUI.color;
			private static Color GetBackground() => GUI.backgroundColor;

			private bool _disposed;
			private Action<Color> _setFn;
			private Color _prevColor;

		}
	}
}