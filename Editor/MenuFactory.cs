// smidgens @ github

#if UNITY_EDITOR

namespace Smidgenomics.Unity.Console.Editor
{
	using System;
	using UnityEngine;
	using UnityEditor;
	using System.Reflection;
	using System.Collections.Generic;
	using UnityObject = UnityEngine.Object;

	internal static class MenuFactory
	{
		public delegate void MethodFn(UnityObject t, MethodInfo m);
		public static readonly GUIContent _NO_FN_LABEL = new ("No Function");

		public struct MethodTarget
		{
			public UnityObject target;
			public IReadOnlyList<MethodInfo> properties;
			public IReadOnlyList<MethodInfo> methods;
		}

		public static GenericMenu CallableMethods
		(
			UnityObject owner,
			MethodInfo currentMethod,
			MethodFn fn
		)
		{
			var m = new GenericMenu();
			var isEmpty = currentMethod == null;

			m.AddItem(_NO_FN_LABEL, isEmpty, () => fn.Invoke(owner, null));
			m.AddSeparator(string.Empty);

			var items = FindAllCallables(owner);

			var groupNames = new HashSet<string>();

			foreach(var it in items)
			{
				var target = it.target;
				var tn = target.GetType().Name;

				var groupName = tn;

				groupNames.EnsureUnique(ref groupName);

				foreach (var pm in it.properties)
				{
					var methodName = pm.GetMenuDisplayName();
					var active = pm == currentMethod;
					m.AddItem(new GUIContent($"{groupName}/{methodName}"), active, () =>
					{
						fn.Invoke(target, pm);
					});
				}

				if (it.properties.Count > 0)
				{
					m.AddSeparator(groupName + "/");
				}
				
				foreach (var pm in it.methods)
				{
					var methodName = pm.GetMenuDisplayName();
					var active = pm == currentMethod;
					m.AddItem(new GUIContent($"{groupName}/{methodName}"), active, () =>
					{
						fn.Invoke(target, pm);
					});
				}
			}
			return m;
		}

		public static MethodTarget[] FindAllCallables(UnityObject ob)
		{
			if (!ob)
			{
				return Array.Empty<MethodTarget>();
			}

			if (!ob.TryGetGameObject(out GameObject go))
			{
				return new []
				{
					GetCallableMethods(ob)
				};
			}

			var components = go.GetComponents<Component>();
			var targets = new MethodTarget[components.Length + 1];
			targets[0] = GetCallableMethods(go);

			for (var i = 0; i < components.Length; i++)
			{
				targets[i + 1] = GetCallableMethods(components[i]);
			}
			return targets;
		}

		public static MethodTarget GetCallableMethods(UnityObject ob)
		{
			var targetType = ob.GetType();

			List<MethodInfo> methods = new();
			List<MethodInfo> props = new();
			foreach (var m in targetType.GetMethods(RFlags.ANY_INSTANCE_MEMBER))
			{
				if (targetType.DeclaresPrivate(m) || !ConsoleHelper.IsConsoleUsable(m))
				{
					continue;
				}
				(m.IsPropertyMethod() ? props : methods).Add(m);
			}

			return new MethodTarget
			{
				target = ob,
				methods = methods,
				properties = props,
			};
		}


	}
}

#endif