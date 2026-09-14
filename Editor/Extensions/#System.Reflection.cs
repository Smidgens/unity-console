// smidgens @ github

#if UNITY_EDITOR

namespace Smidgenomics.Unity.Console.Editor
{
	using System;
	using System.Collections.Generic;
	using System.Reflection;
	using System.Linq;

	internal static class _MethodInfo_
	{
		public static byte GetAccessLevel(this MethodInfo m)
		{
			if (m.IsFamily) { return 1; }
			if (m.IsPrivate) { return 0; }
			return 3;
		}
		
		// Find all fields that Unity would default render in the inspector
		public static IReadOnlyList<FieldInfo> FindInspectorFields<T>(this Type owner)
		{
			// NOTE: doesn't work properly for unity components, flags might need to be different

			var baseType = typeof(T);

			List<FieldInfo> fields = new List<FieldInfo>();
			LinkedList<Type> hierarchy = new LinkedList<Type>(); // linked for efficient prepend

			// traverse parent hierarchy, stop at base type
			Type currentType = owner;
			while (currentType != baseType && currentType != null)
			{
				hierarchy.AddFirst(currentType);
				currentType = currentType.BaseType;
			}

			BindingFlags fieldFlags = BindingFlags.NonPublic
			| BindingFlags.Public
			| BindingFlags.DeclaredOnly
			| BindingFlags.Instance;

			// append fields in
			// same order as Unity would normally list them
			foreach (Type htype in hierarchy)
			{
				foreach (FieldInfo field in htype.GetFields(fieldFlags))
				{
					if (!ReflectionUtils.IsInspectorField(field))
					{
						continue;
					}
					fields.Add(field);
				}
			}
			return fields;
		}

		public static string GetDisplayName(this MethodInfo m)
		{
			return m.IsPropertyMethod() ? m.GetOwningPropertyName() : m.Name;
		}

		private const string _ACCESS_TOKENS = "-#?+";

		public static string GetMenuDisplayName(this MethodInfo m)
		{
			var rt = m.ReturnType.GetNameOrAlias();
			var n = m.GetDisplayName();

			var parameters = m.GetParameters();

			var pnames =
			string.Join(", ", parameters
			.Select(x => x.ParameterType.GetNameOrAlias()));

			var accessToken = _ACCESS_TOKENS[m.GetAccessLevel()];

			var accessPrefix = $"[{accessToken}]";

			if (m.IsSpecialName)
			{
				if (m.IsVoid())
				{
					return $"{accessPrefix} {parameters[0].ParameterType.GetNameOrAlias()} {n}";
				}
				return $"{accessPrefix} {rt} {n}";
			}
			return $"{accessPrefix} {rt} {n} ({pnames})";
		}

	}
}

#endif