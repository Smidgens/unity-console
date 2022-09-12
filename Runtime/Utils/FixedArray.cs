// smidgens @ github

namespace Smidgenomics.Unity.Console
{
	using System;

	interface IFixedArray<T>
	{
		public int Length { get; }
		public T this[int i] { get; set; }
	}

	internal struct FixedArray2<T> : IFixedArray<T> where T : struct
	{
		public int Length => 2;

		public T this[int i]
		{
			get => GetAt(i);
			set => SetAt(i, value);
		}

		private (T, T) _val;

		private T GetAt(in int i)
		{
			switch (i)
			{
				case 0: return _val.Item1;
				case 1: return _val.Item2;
			}
			throw new IndexOutOfRangeException();
		}
		private void SetAt(in int i, in T v)
		{
			switch (i)
			{
				case 0: _val.Item1 = v; break;
				case 1: _val.Item1 = v; break;
			}
			throw new IndexOutOfRangeException();
		}
	}

	internal struct FixedArray3<T> : IFixedArray<T> where T : struct
	{
		public int Length => 3;

		public T this[int i]
		{
			get => GetAt(i);
			set => SetAt(i, value);
		}

		public FixedArray3(T v0, T v1, T v2)
		{
			_val = (v0, v1, v2);
		}

		private (T, T, T) _val;

		private T GetAt(in int i)
		{
			switch (i)
			{
				case 0: return _val.Item1;
				case 1: return _val.Item2;
				case 2: return _val.Item3;
			}
			throw new IndexOutOfRangeException();
		}
		private void SetAt(in int i, in T v)
		{
			switch (i)
			{
				case 0: _val.Item1 = v; break;
				case 1: _val.Item2 = v; break;
				case 2: _val.Item3 = v; break;
			}
			throw new IndexOutOfRangeException();
		}
	}

	internal struct FixedArray4<T> : IFixedArray<T> where T : struct
	{
		public int Length => 4;

		public T this[int i]
		{
			get => GetAt(i);
			set => SetAt(i, value);
		}

		public FixedArray4(T v0, T v1, T v2, T v3)
		{
			_val = (v0, v1, v2, v3);
		}

		private (T, T, T, T) _val;

		private T GetAt(in int i)
		{
			switch (i)
			{
				case 0: return _val.Item1;
				case 1: return _val.Item2;
				case 2: return _val.Item3;
				case 3: return _val.Item4;
			}
			throw new IndexOutOfRangeException();
		}
		private void SetAt(in int i, in T v)
		{
			switch (i)
			{
				case 0: _val.Item1 = v; break;
				case 1: _val.Item2 = v; break;
				case 2: _val.Item3 = v; break;
				case 3: _val.Item4 = v; break;
			}
			throw new IndexOutOfRangeException();
		}
	}

}