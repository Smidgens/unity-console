// smidgens @ github

namespace Smidgenomics.Unity.Console
{
	using UnityEngine;

	internal struct LazyTex : ILazy<Texture>
	{
		public bool IsValueCreated => _value;
		public Texture Value => Get();

		public static LazyTex New(Color c, int size)
		{
			return new LazyTex(c, size);
		}

		private LazyTex(Color c, int size)
		{
			_s = size;
			_c = c;
			_value = null;
		}

		private Texture Get()
		{
			if (_value == null)
			{
				_value = Create(_c, _s);
			}
			return _value;
		}

		private Texture _value;
		private Color _c;
		private int _s;

		private static Texture2D Create(Color c, int size)
		{
			var t = new Texture2D(size, size);
			t.SetPixel(0, 0, c);
			t.Apply();
			t.wrapMode = TextureWrapMode.Repeat;
			t.filterMode = FilterMode.Point;
			return t;
		}
	}
}