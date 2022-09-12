// smidgens @ github

namespace Smidgenomics.Unity.Console
{
	using UnityEngine;
	using System;
	using System.Reflection;
	using UnityObject = UnityEngine.Object;

	[Serializable]
	internal struct StringifiedMember
	{
		public string name;
		// [type, return, params]
		public string[] types;

		public MethodInfo LoadAsMethod()
		{
			if (string.IsNullOrEmpty(name)) { return null; }

			if (types.Length < 2) { return null; }

			var ownerType = Type.GetType(types[0], false);
			var returnType = Type.GetType(types[1], false);

			if (ownerType == null || returnType == null)
			{
				return null;
			}

			var ptypes = new Type[types.Length - 2];

			for (var i = 0; i < ptypes.Length; i++)
			{
				ptypes[i] = Type.GetType(types[i + 2], false);
			}
			return ownerType.GetMethod(name, RFlags.ANY_INSTANCE_MEMBER, null, ptypes, null);

		}
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
			M_NAME = nameof(_member) + "." + nameof(StringifiedMember.name),
			M_TYPES = nameof(_member) + "." + nameof(StringifiedMember.types),
			CACHE_KEY = nameof(_cacheKey),
			TARGET = nameof(_target);
		}
		#endif

		[SerializeField] private int _cacheKey; // cache helper
		[SerializeField] private UnityObject _target;
		[SerializeField] StringifiedMember _member = default;

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
