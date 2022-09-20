// smidgens @ github

namespace Smidgenomics.Unity.Console
{
	using System;
	using UnityEngine;

	internal static class BitEncode
	{

		public static float ReadFloat(byte[] bytes, in int startIndex)
		{
			return BitConverter.ToSingle(bytes, startIndex);
		}

		public static float ReadInt(byte[] bytes, in int startIndex)
		{
			return BitConverter.ToInt32(bytes, startIndex);
		}

		public static double ReadDouble(byte[] bytes, in int startIndex)
		{
			return BitConverter.ToDouble(bytes, startIndex);
		}

		public static Vector3 ReadVector3(byte[] bytes, in int startIndex)
		{
			var x = ReadFloat(bytes, startIndex);
			var y = ReadFloat(bytes, startIndex + 4);
			var z = ReadFloat(bytes, startIndex + 8);
			return new Vector3(x, y, z);
		}

		public static Vector2 ReadVector2(byte[] bytes, in int startIndex)
		{
			var x = ReadFloat(bytes, startIndex);
			var y = ReadFloat(bytes, startIndex + 4);
			return new Vector2(x, y);
		}

		public static Vector4 ReadVector4(byte[] bytes, in int startIndex)
		{
			var x = ReadFloat(bytes, startIndex);
			var y = ReadFloat(bytes, startIndex + 4);
			var z = ReadFloat(bytes, startIndex + 8);
			var w = ReadFloat(bytes, startIndex + 12);
			return new Vector4(x, y, z, w);
		}
		public static Color ReadColor(byte[] bytes, in int startIndex)
		{
			var r = ReadFloat(bytes, startIndex);
			var g = ReadFloat(bytes, startIndex + 4);
			var b = ReadFloat(bytes, startIndex + 8);
			var a = ReadFloat(bytes, startIndex + 12);
			return new Color(r, g, b, a);
		}

		public static void Set(byte[] bytes, in int startIndex, in Vector4 v)
		{
			Set(bytes, startIndex, v.x, v.y, v.z, v.w);
		}

		public static void Set(byte[] bytes, in int startIndex, in Vector3 v)
		{
			Set(bytes, startIndex, v.x);
			Set(bytes, startIndex + 4, v.y);
			Set(bytes, startIndex + 8, v.z);
		}

		public static void Set(byte[] bytes, in int startIndex, in Vector2 v)
		{
			Set(bytes, startIndex, v.x);
			Set(bytes, startIndex + 4, v.y);
		}

		public static void Set(byte[] bytes, in int startIndex, in Color v)
		{
			Set(bytes, startIndex, v.r, v.g, v.b, v.a);
		}

		public static void Set(byte[] bytes, in int startIndex, in float v)
		{
			Set(bytes, startIndex, GetBytes(v));
		}

		public static void Set(byte[] bytes, in int startIndex, in bool v)
		{
			byte b = 0;
			if (v) { b = 1; }
			bytes[startIndex] = b;
		}

		public static void Set(byte[] bytes, in int startIndex, in int v)
		{
			Set(bytes, startIndex, GetBytes(v));
		}

		public static void Set(byte[] bytes, in int startIndex, in uint v)
		{
			Set(bytes, startIndex, GetBytes(v));
		}

		public static void Set(byte[] bytes, in int startIndex, in ulong v)
		{
			Set(bytes, startIndex, GetBytes(v));
		}

		public static void Set(byte[] bytes, in int startIndex, in double v)
		{
			Set(bytes, startIndex, GetBytes((ulong)v));
		}

		public static (byte, byte, byte, byte) GetBytes(in float v) => GetBytes((uint)v);
		public static (byte, byte, byte, byte) GetBytes(in int v) => GetBytes((uint)v);

		private static void Set(byte[] bytes, in int startIndex, in float v1, in float v2, in float v3, in float v4)
		{
			Set(bytes, startIndex, v1);
			Set(bytes, startIndex + 4, v2);
			Set(bytes, startIndex + 8, v3);
			Set(bytes, startIndex + 12, v4);
		}

		private static void Set
		(
			byte[] bytes,
			in int startIndex,
			in (byte, byte, byte, byte) b
		)
		{
			var (v0, v1, v2, v3) = b;
			bytes[startIndex] = v0;
			bytes[startIndex + 1] = v1;
			bytes[startIndex + 2] = v2;
			bytes[startIndex + 3] = v3;
		}

		private static void Set
		(
			byte[] bytes,
			in int startIndex,
			in (byte, byte, byte, byte, byte, byte, byte, byte) b
		)
		{
			var (v0, v1, v2, v3, v4, v5, v6, v7) = b;
			bytes[startIndex] = v0;
			bytes[startIndex + 1] = v1;
			bytes[startIndex + 2] = v2;
			bytes[startIndex + 3] = v3;
			bytes[startIndex + 4] = v4;
			bytes[startIndex + 5] = v5;
			bytes[startIndex + 6] = v6;
			bytes[startIndex + 7] = v7;
		}

		private static (byte, byte, byte, byte) GetBytes(in uint bits)
		{
			return BitConverter.IsLittleEndian
			? (
				GetByte(bits, 0),
				GetByte(bits, 1),
				GetByte(bits, 2),
				GetByte(bits, 3)
			)
			: (
				GetByte(bits, 3),
				GetByte(bits, 2),
				GetByte(bits, 1),
				GetByte(bits, 0)
			);
		}

		private static (byte, byte, byte, byte, byte, byte, byte, byte) GetBytes(in ulong bits)
		{
			return BitConverter.IsLittleEndian
			? (
				GetByte(bits, 0),
				GetByte(bits, 1),
				GetByte(bits, 2),
				GetByte(bits, 3),
				GetByte(bits, 4),
				GetByte(bits, 5),
				GetByte(bits, 6),
				GetByte(bits, 7)
			)
			: (
				GetByte(bits, 7),
				GetByte(bits, 6),
				GetByte(bits, 5),
				GetByte(bits, 4),
				GetByte(bits, 3),
				GetByte(bits, 2),
				GetByte(bits, 1),
				GetByte(bits, 0)
			);
		}

		private static byte GetByte(in uint v, in byte i) => (byte)(v >> (i * 8));
		private static byte GetByte(in ulong v, in byte i) => (byte)(v >> (i * 8));
	}
}