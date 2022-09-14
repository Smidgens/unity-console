// smidgens @ github

namespace Smidgenomics.Unity.Console
{
	interface IFixedArray<T>
	{
		public int Length { get; }
		public T this[int i] { get; set; }
	}
}

namespace Smidgenomics.Unity.Console
{
	interface IFixedStack<T>
	{
		public int Capacity { get; }
		public int Count { get; }
		public T Peek();
		public T Pop();
		public void Push(in T v);
	}
}

namespace Smidgenomics.Unity.Console
{
	using System;

	internal static class FixedCollection
	{
		public static class Stack
		{
			public static IFixedStack<T> N16<T>() => FixedStack<T>.PO2(4);
			public static IFixedStack<T> N32<T>() => FixedStack<T>.PO2(5);
		}

		public static class Array
		{
		}

	}

	/// <summary>
	/// Pseudo stack with fixed size
	/// </summary>
	internal struct FixedStack<T> : IFixedStack<T>
	{
		public const byte MAX_P2 = 5; // 32

		// 0,1,2,3,4,5
		// 1,2,4,8,16,32

		public int Count => _n;
		public int Capacity => _max;

		public static FixedStack<T> PO2(byte pow = 4)
		{
			if(pow > MAX_P2)
			{
				throw new InvalidOperationException("Invalid stack capacity");
			}
			IFixedArray<T> arr;
			switch (pow)
			{
				case 0:
				case 1:
					arr = new FixedArray2<T>();
					break;
				case 2:
					arr = new FixedArray4<T>();
					break;
				case 3:
					arr = new FixedArray8<T>();
					break;
				case 4:
					arr = new FixedArray16<T>();
					break;
				case 5:
					arr = new FixedArray32<T>();
					break;
				default:
					arr = new FixedArray2<T>();
					break;
			}
			return new FixedStack<T>(arr);
		}

		private FixedStack(IFixedArray<T> arr)
		{
			_l = arr;
			_max = (byte)arr.Length;
			_n = 0;
		}

		public void Push(in T v)
		{
			if(_n == _max)
			{
				throw new OverflowException("Cannot push to full stack");
			}
			_l[_n] = v;
			_n++;
		}

		public T Peek()
		{
			if (_n == 0) { throw new IndexOutOfRangeException(); }
			return _l[_n - 1];
		}

		public T Pop()
		{
			if (_n == 0) { throw new IndexOutOfRangeException(); }
			_n--;
			return _l[_n];
		}

		private IFixedArray<T> _l;
		private byte _n;
		private byte _max;

	}
}


namespace Smidgenomics.Unity.Console
{
	using System;

	internal struct FixedArray2<T> : IFixedArray<T>
	{
		public int Length => 2;

		public T this[int i]
		{
			get => GetAt(i);
			set => SetAt(i, value);
		}

		public FixedArray2(T v0 = default, T v1 = default)
		{
			_0 = v0; _1 = v1;
		}

		private T _0, _1;

		private T GetAt(in int i) => i switch
		{
			0 => _0,
			1 => _1,
			_ => throw new IndexOutOfRangeException()
		};

		private void SetAt(in int i, in T v)
		{
			switch (i)
			{
				case 0: _0 = v; return;
				case 1: _1 = v; return;
			}
			throw new IndexOutOfRangeException();
		}
	}
}

namespace Smidgenomics.Unity.Console
{
	using System;

	internal struct FixedArray4<T> : IFixedArray<T>
	{
		public int Length => 4;

		public T this[int i]
		{
			get => GetAt(i);
			set => SetAt(i, value);
		}

		public FixedArray4
		(
			T v0 = default, T v1 = default,
			T v2 = default, T v3 = default
		)
		{
			_0 = v0; _1 = v1; _2 = v2; _3 = v3;
		}

		private T _0, _1, _2, _3;

		private T GetAt(in int i) => i switch
		{
			0 => _0,
			1 => _1,
			2 => _2,
			3 => _3,
			_ => throw new IndexOutOfRangeException()
		};

		private void SetAt(in int i, in T v)
		{
			switch (i)
			{
				case 0: _0 = v; return;
				case 1: _1 = v; return;
				case 2: _2 = v; return;
				case 3: _3 = v; return;
			}
			throw new IndexOutOfRangeException();
		}
	}
}

namespace Smidgenomics.Unity.Console
{
	using System;

	internal struct FixedArray8<T> : IFixedArray<T>
	{
		public int Length => 8;

		public T this[int i]
		{
			get => GetAt(i);
			set => SetAt(i, value);
		}

		public FixedArray8
		(
			T v0 = default, T v1 = default,
			T v2 = default, T v3 = default,
			T v4 = default, T v5 = default,
			T v6 = default, T v7 = default
		)
		{
			_0 = v0; _1 = v1; _2 = v2; _3 = v3;
			_4 = v4; _5 = v5; _6 = v6; _7 = v7;
		}

		private T _0, _1, _2, _3, _4, _5, _6, _7;

		private T GetAt(in int i) => i switch
		{
			0 => _0,
			1 => _1,
			2 => _2,
			3 => _3,
			4 => _4,
			5 => _5,
			6 => _6,
			7 => _7,
			_ => throw new IndexOutOfRangeException()
		};

		private void SetAt(in int i, in T v)
		{
			switch (i)
			{
				case 0: _0 = v; return;
				case 1: _1 = v; return;
				case 2: _2 = v; return;
				case 3: _3 = v; return;
				case 4: _4 = v; return;
				case 5: _5 = v; return;
				case 6: _6 = v; return;
				case 7: _7 = v; return;
			}
			throw new IndexOutOfRangeException();
		}
	}
}

namespace Smidgenomics.Unity.Console
{
	using System;

	internal struct FixedArray16<T> : IFixedArray<T>
	{
		public int Length => 16;

		public FixedArray16(FixedArray8<T> v0 = default, FixedArray8<T> v1 = default)
		{
			_0 = v0;
			_1 = v1;
		}

		public T this[int i]
		{
			get => GetAt(i);
			set => SetAt(i, value);
		}

		public int IndexOfRef(in T v)
		{
			if (typeof(T).IsValueType) { return -1; }
			for(var i = 0; i < Length; i++)
			{
				if(GetAt(i).Equals(v)) { return i; }
			}
			return -1;
		}

		private FixedArray8<T> _0, _1;


		private T GetAt(in int i)
		{
			if (i >= Length || i < 0)
			{
				throw new IndexOutOfRangeException();
			}
			var ii = i % 8;
			var oi = i / 8;
			return oi == 0 ? _0[ii] : _1[ii];
		}

		private void SetAt(in int i, in T v)
		{
			if (i >= Length || i < 0)
			{
				throw new IndexOutOfRangeException();
			}
			var ii = i % 8;
			switch (i / 8)
			{
				case 0: _0[ii] = v; break;
				case 1: _1[ii] = v; break;
			}
		}
	}
}

namespace Smidgenomics.Unity.Console
{
	using System;

	internal struct FixedArray32<T> : IFixedArray<T>
	{
		public int Length => 32;

		private FixedArray8<T> _0, _1, _2, _3;

		public T this[int i]
		{
			get => GetAt(i);
			set => SetAt(i, value);
		}

		private T GetAt(in int i)
		{
			var ii = (i % 8);
			return (i / 8) switch
			{
				0 => _0[ii],
				1 => _1[ii],
				2 => _2[ii],
				3 => _3[ii],
				_ => throw new IndexOutOfRangeException()
			};
		}

		private void SetAt(in int i, in T v)
		{
			if (i >= Length || i < 0)
			{
				throw new IndexOutOfRangeException();
			}
			var ii = (i % 8);
			switch (i / 8)
			{
				case 0: _0[ii] = v; break;
				case 1: _1[ii] = v; break;
				case 2: _2[ii] = v; break;
				case 3: _3[ii] = v; break;
			}
		}
	}
}