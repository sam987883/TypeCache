// Copyright (c) 2021 Samuel Abraham

using System;
using System.Runtime.CompilerServices;
using TypeCache.Collections.Extensions;

namespace TypeCache.Extensions
{
	public static class BitConverterExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool ToBoolean(this byte[] @this, int startIndex = 0)
			=> BitConverter.ToBoolean(@this, startIndex);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static byte[] ToBytes(this bool @this)
			=> BitConverter.GetBytes(@this);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static byte[] ToBytes(this char @this)
			=> BitConverter.GetBytes(@this);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static byte[] ToBytes(this short @this)
			=> BitConverter.GetBytes(@this);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static byte[] ToBytes(this int @this)
			=> BitConverter.GetBytes(@this);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static byte[] ToBytes(this long @this)
			=> BitConverter.GetBytes(@this);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static byte[] ToBytes(this ushort @this)
			=> BitConverter.GetBytes(@this);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static byte[] ToBytes(this uint @this)
			=> BitConverter.GetBytes(@this);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static byte[] ToBytes(this ulong @this)
			=> BitConverter.GetBytes(@this);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static byte[] ToBytes(this float @this)
			=> BitConverter.GetBytes(@this);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static byte[] ToBytes(this double @this)
			=> BitConverter.GetBytes(@this);

		public static byte[] ToBytes(this decimal @this)
			=> decimal.GetBits(@this).ToMany(BitConverter.GetBytes).ToArray();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static char ToChar(this byte[] @this, int startIndex = 0)
			=> BitConverter.ToChar(@this, startIndex);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static short ToInt16(this byte[] @this, int startIndex = 0)
			=> BitConverter.ToInt16(@this, startIndex);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int ToInt32(this float @this)
			=> BitConverter.SingleToInt32Bits(@this);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int ToInt32(this byte[] @this, int startIndex = 0)
			=> BitConverter.ToInt32(@this, startIndex);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static long ToInt64(this double @this)
			=> BitConverter.DoubleToInt64Bits(@this);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static long ToInt64(this byte[] @this, int startIndex = 0)
			=> BitConverter.ToInt64(@this, startIndex);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ushort ToUInt16(this byte[] @this, int startIndex = 0)
			=> BitConverter.ToUInt16(@this, startIndex);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint ToUInt32(this byte[] @this, int startIndex = 0)
			=> BitConverter.ToUInt32(@this, startIndex);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ulong ToUInt64(this byte[] @this, int startIndex = 0)
			=> BitConverter.ToUInt64(@this, startIndex);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float ToSingle(this int @this)
			=> BitConverter.Int32BitsToSingle(@this);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float ToSingle(this byte[] @this, int startIndex = 0)
			=> BitConverter.ToSingle(@this, startIndex);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double ToDouble(this long @this)
			=> BitConverter.Int64BitsToDouble(@this);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double ToDouble(this byte[] @this, int startIndex = 0)
			=> BitConverter.ToDouble(@this, startIndex);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string ToText(this byte[] @this, int startIndex = 0)
			=> BitConverter.ToString(@this, startIndex);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string ToText(this byte[] @this, int startIndex, int length)
			=> BitConverter.ToString(@this, startIndex, length);
	}
}
