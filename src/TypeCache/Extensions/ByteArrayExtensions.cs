// Copyright (c) 2021 Samuel Abraham

using System;
using System.Runtime.CompilerServices;

namespace TypeCache.Extensions
{
	public static class ByteArrayExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool ToBoolean(this byte[] @this, int startIndex = 0)
			=> BitConverter.ToBoolean(@this, startIndex);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static char ToChar(this byte[] @this, int startIndex = 0)
			=> BitConverter.ToChar(@this, startIndex);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static short ToInt16(this byte[] @this, int startIndex = 0)
			=> BitConverter.ToInt16(@this, startIndex);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int ToInt32(this byte[] @this, int startIndex = 0)
			=> BitConverter.ToInt32(@this, startIndex);

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
		public static float ToSingle(this byte[] @this, int startIndex = 0)
			=> BitConverter.ToSingle(@this, startIndex);

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
