// smidgens @ github

namespace Smidgenomics.Unity.Console
{
	using System;
	using System.Reflection;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityObject = UnityEngine.Object;
	using System.Linq;

	internal static class ConsoleReflection
	{
		public static class BFlag
		{
			public const BindingFlags
			ANY_INSTANCE_METHOD =
			BindingFlags.NonPublic
			| BindingFlags.Public
			| BindingFlags.Instance;
		}

		public static Type[] CONSOLE_PARAM_TYPES =
		{
			typeof(string),
			typeof(float),
			typeof(int),
			typeof(bool),
			typeof(Vector2),
			typeof(Vector3),
			typeof(Vector4),
			typeof(Color),
		};

		public struct MethodTarget
		{
			public UnityObject target;
			public MethodInfo[] methods;
			public int firstMethodIndex;
		}

		public static Type FindType(string aqName) => Type.GetType(aqName, false);

		public static MethodInfo LoadMethod(MemberInfo mi)
		{
			return LoadMethod(mi.name, mi.types);
		}

		public static MethodInfo LoadMethod(in string mname, in string[] mtypes)
		{
			if (string.IsNullOrEmpty(mname)) { return null; }

			if (mtypes.Length < 2) { return null; }

			var ownerType = FindType(mtypes[0]);
			var returnType = FindType(mtypes[1]);

			if(ownerType == null || returnType == null)
			{
				return null;
			}

			var ptypes = new List<Type>();

			for (var i = 2; i < mtypes.Length; i++)
			{
				ptypes.Add(FindType(mtypes[i]));
			}
			return ownerType.GetMethod(mname, BFlag.ANY_INSTANCE_METHOD, null, ptypes.ToArray(), new ParameterModifier[0]);
		}

		public static MethodTarget[] FindAllCallables(UnityObject t)
		{
			if (!t) { return new MethodTarget[0]; }

			if (!t.TryGetGameObject(out GameObject go))
			{
				return new MethodTarget[]
				{
					GetConsoleCallableMethods(t)
				};
			}

			var components = go.GetComponents<Component>();
			var targets = new MethodTarget[components.Length + 1];
			targets[0] = GetConsoleCallableMethods(go);

			for(var i = 0; i < components.Length; i++)
			{
				targets[i + 1] = GetConsoleCallableMethods(components[i]);
			}
			return targets;
		}

		public static bool IsConsoleUsable(ParameterInfo p)
		{
			if (p.IsOptional) { return false; }
			if (p.ParameterType.IsArray) { return false; }
			if (p.IsRefType()) { return false; }
			return Array.IndexOf(CONSOLE_PARAM_TYPES, p.ParameterType) > -1;
		}

		public static bool IsConsoleUsable(MethodInfo x)
		{
			if (!x.IsVoid()) { return false; }
			if (x.ContainsGenericParameters) { return false; }
			foreach (var p in x.GetParameters())
			{
				if (!IsConsoleUsable(p)) { return false; }
			}
			return true;
		}

		public static MethodTarget GetConsoleCallableMethods(UnityObject t)
		{
			var targetType = t.GetType();

			var firstMethodIndex = -1;

			var methods = targetType
			.GetMethods(BFlag.ANY_INSTANCE_METHOD)
			.Where(m =>
			{
				if (targetType.IsDeclaringPrivate(m))
				{
					return false;
				}
				return IsConsoleUsable(m);
			})
			.OrderBy(x => !x.IsSpecialName)
			.ThenByDescending(x => x.GetAccessMod())
			.ThenBy(x => x.Name)
			.ToArray();

			for(var i = 0; i < methods.Length; i++)
			{
				if (!methods[i].IsProperty())
				{
					firstMethodIndex = i;
					break;
				}
			}

			return new MethodTarget
			{
				target = t,
				methods = methods,
				firstMethodIndex = firstMethodIndex,
			};
		}
	}
}