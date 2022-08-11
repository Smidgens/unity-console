namespace Smidgenomics.Unity.Console
{
	using System;
	using System.Reflection;
	using System.Linq.Expressions;

	internal static class Expression_
	{
		public static MethodInfo GetMethodInfo<T>(this Expression<Action<T>> expression)
		{
			var m = expression.Body as MethodCallExpression;
			if (m != null) { return m.Method; }
			throw new ArgumentException("Expression is not a method");
		}
	}
}