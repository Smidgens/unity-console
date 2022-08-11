// smidgens @ github

namespace Smidgenomics.Unity.Console
{
	using UnityEngine;
	using System;
	using System.Reflection;
	using UnityObject = UnityEngine.Object;

	[Serializable]
	internal struct MemberInfo
	{
		public string name;
		public string[] types;
	}

	[Serializable]
	internal class SerializedMember
	{
		public string Name => _member.name;
		public UnityObject Target => _target;

		public MethodInfo GetMethod()
		{
			return GetMethodCached();
		}

		// editor helper
		#if UNITY_EDITOR
		internal static class _FN
		{
			public const string
			M_NAME = nameof(_member) + "." + nameof(MemberInfo.name),
			M_TYPES = nameof(_member) + "." + nameof(MemberInfo.types),
			CACHE_KEY = nameof(_cacheKey),
			TARGET = nameof(_target);
		}
		#endif

		[SerializeField] private int _cacheKey; // cache helper
		[SerializeField] private UnityObject _target;
		[SerializeField] MemberInfo _member = default;

		// key, method, init
		private (int, MethodInfo, bool) _cache;

		private MethodInfo GetMethodCached()
		{
			var (ckey,cmethod,cinit) = _cache;
			// clear cache?
			if (ckey != _cacheKey) { _cache = default; }

			// cache valid?
			if (cinit) { return cmethod; }

			// build cache
			ckey = (int)DateTime.Now.ToUnix();
			_cache = (ckey, LoadMethod(), true);

			return _cache.Item2;
		}

		private MethodInfo LoadMethod()
		{
			return ConsoleReflection.LoadMethod(_member);
		}
	}
}
