// smidgens @ github

namespace Smidgenomics.Unity.Console.Editor
{
	using UnityEngine;
	using UnityEditor;
	using System.Reflection;
	using System.Collections.Generic;
	using UnityObject = UnityEngine.Object;
	using System.Linq;

	internal static class MenuFactory
	{
		public delegate void MethodFn(UnityObject t, MethodInfo m);
		public static readonly GUIContent _NO_FN_LABEL = new GUIContent("No Function");

		public struct MethodTarget
		{
			public UnityObject target;
			public MethodInfo[] methods;
			public int firstMethodIndex;
		}

		public static GenericMenu ConsoleCallables
		(
			UnityObject owner,
			MethodInfo currentMethod,
			MethodFn fn
		)
		{
			//var (currentTarget, currentMethod) = currentValue;
			var m = new GenericMenu();
			var isEmpty = currentMethod == null;

			m.AddItem(_NO_FN_LABEL, isEmpty, () => fn.Invoke(owner, null));
			m.AddSeparator("");

			var items = FindAllCallables(owner);

			var groupNames = new HashSet<string>();

			foreach(var it in items)
			{
				var target = it.target;
				var tn = target.GetType().Name;

				var groupName = tn;

				groupNames.EnsureUnique(ref groupName);

				for(var i = 0; i < it.methods.Length; i++)
				{
					var method = it.methods[i];
					if (i == it.firstMethodIndex && i != 0)
					{
						m.AddSeparator(groupName + "/");
					}

					var methodName = method.GetDisplaySignature();
					var active = method == currentMethod;

					m.AddItem(new GUIContent($"{groupName}/{methodName}"), active, () =>
					{
						fn.Invoke(target, method);
					});
				}

			}
			return m;
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

			for (var i = 0; i < components.Length; i++)
			{
				targets[i + 1] = GetConsoleCallableMethods(components[i]);
			}
			return targets;
		}

		public static MethodTarget GetConsoleCallableMethods(UnityObject t)
		{
			var targetType = t.GetType();

			var firstMethodIndex = -1;

			var methods = targetType
			.GetMethods(RFlags.ANY_INSTANCE_MEMBER)
			.Where(m =>
			{
				if (targetType.DeclaresPrivate(m))
				{
					return false;
				}
				return CSupport.IsConsoleUsable(m);
			})
			.OrderBy(x => !x.IsSpecialName)
			.ThenByDescending(x => x.GetAccessLevel())
			.ThenBy(x => x.Name)
			.ToArray();

			for (var i = 0; i < methods.Length; i++)
			{
				if (!methods[i].IsGetOrSet())
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