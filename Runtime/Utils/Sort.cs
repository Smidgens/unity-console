// smidgens @ github

namespace Smidgenomics.Unity.Console
{
	using System;
	using System.Collections.Generic;

	internal static class Sort
	{
		public static void ByName<T>(List<T> l, bool desc = false) where T : IName
		{
			Comparison<T> cmp = CompareName;
			if (desc) { cmp = CompareNameDesc; }
			l.Sort(cmp);
		}

		public static void ByOrder<T>(List<T> l, bool desc = false) where T : IOrder
		{
			Comparison<T> cmp = CompareOrder;
			if (desc) { cmp = CompareOrderDesc; }
			l.Sort(cmp);
		}

		private static int CompareName<T>(T a, T b)
		where T : IName => String.Compare(a.Name, b.Name, StringComparison.Ordinal);

		private static int CompareNameDesc<T>(T a, T b)
		where T : IName => String.Compare(b.Name, a.Name, StringComparison.Ordinal);

		private static int CompareOrder<T>(T a, T b)
		where T : IOrder => a.Order.CompareTo(b.Order);

		private static int CompareOrderDesc<T>(T a, T b)
		where T : IOrder => b.Order.CompareTo(a.Order);

	}

}