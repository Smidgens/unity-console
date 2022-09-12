// smidgens @ github

namespace Smidgenomics.Unity.Console
{
	using System;

	/// <summary>
	/// Lazy value
	/// </summary>
	internal interface ILazy<T>
	{
		public bool IsValueCreated { get; }
		public T Value { get; }
	}

	/// <summary>
	/// Lazy value with init context
	/// </summary>
	internal interface ILazy<T, CT>
	{
		public bool IsValueCreated { get; }
		public T Get(in CT ctx);
	}

	internal static class LazyValue
	{
		public static ILazy<T> Get<T>(Func<T> fn)
		{
			return new LZValue<T>(fn);
		}

		public static ILazy<T,CT> Get<T,CT>(Func<CT, T> fn)
		{
			return new LazyVal<T,CT>(fn);
		}

		private struct LazyVal<T, CT> : ILazy<T, CT>
		{
			public bool IsValueCreated => _getter == GetValue;

			public LazyVal(Func<CT, T> initFn)
			{
				_value = default;
				_initFn = initFn;
				_getter = null;
				_getter = InitGet;
			}

			public T Get(in CT ctx)
			{
				return _getter.Invoke(ctx);
			}

			private delegate T GetterFn(in CT ctx);
			private Func<CT, T> _initFn;
			private GetterFn _getter;
			private T _value;

			private T InitGet(in CT ctx)
			{
				if (_initFn != null)
				{
					_value = _initFn.Invoke(ctx);
					_initFn = null;
				}
				_getter = GetValue;
				return _value;
			}
			private T GetValue(in CT ctx) => _value;
		}

		private struct LZValue<T> : ILazy<T>
		{
			public bool IsValueCreated => _getter == GetValue;
			public T Value => _getter.Invoke();

			public LZValue(Func<T> initFn)
			{
				_value = default;
				_initFn = initFn;
				_getter = null;
				_getter = InitGet;
			}

			private T _value;
			private Func<T> _initFn;
			private Func<T> _getter;

			private T InitGet()
			{
				if (_initFn != null)
				{
					_value = _initFn.Invoke();
					_initFn = null;
				}
				_getter = GetValue;
				return _value;
			}
			private T GetValue() => _value;
		}
	}
}