// smidgens @ github

namespace Smidgenomics.Unity.Console
{
	using System;
	using Bytes32 = System.ValueTuple<byte, byte, byte, byte>;

	internal static class BitUtility
	{
		public static Bytes32 GetBytes(in float v) => To32((uint)v);

		private static Bytes32 To32(in uint bits)
		{
			var bytes =
			(
				GetByte(bits, 3),
				GetByte(bits, 2),
				GetByte(bits, 1),
				GetByte(bits, 0)
			);
			if (BitConverter.IsLittleEndian) { Reverse(ref bytes); }
			return bytes;
		}
		private static byte GetByte(in uint v, in int i) => (byte)(v >> (i * 8));

		private static void Reverse<T>(ref (T, T, T, T) v)
		{
			Swap(ref v.Item1, ref v.Item4);
			Swap(ref v.Item2, ref v.Item3);
		}

		private static void Swap<T>(ref T v1, ref T v2)
		{
			var t = v1; v1 = v2; v2 = t;
		}
	}
}