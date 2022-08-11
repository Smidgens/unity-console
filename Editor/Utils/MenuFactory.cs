// smidgens @ github

namespace Smidgenomics.Unity.Console.Editor
{
	using UnityEngine;
	using UnityEditor;
	using System.Reflection;
	using System.Collections.Generic;
	using UnityObject = UnityEngine.Object;

	internal static class MenuFactory
	{
		public delegate void MethodFn(UnityObject t, MethodInfo m);
		public static readonly GUIContent _NO_FN_LABEL = new GUIContent("No Function");

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

			var items = ConsoleReflection.FindAllCallables(owner);

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

		
	}
}