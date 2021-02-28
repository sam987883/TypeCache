// Copyright (c) 2021 Samuel Abraham

using System;
using System.Runtime.CompilerServices;

namespace TypeCache.Extensions
{
	public static class ReadOnlySpanExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool ToBoolean(this ReadOnlySpan<byte> @this, int startIndex = 0)
			=> BitConverter.ToBoolean(@this);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static char ToChar(this ReadOnlySpan<byte> @this, int startIndex = 0)
			=> BitConverter.ToChar(@this);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static short ToInt16(this ReadOnlySpan<byte> @this, int startIndex = 0)
			=> BitConverter.ToInt16(@this);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int ToInt32(this ReadOnlySpan<byte> @this, int startIndex = 0)
			=> BitConverter.ToInt32(@this);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static long ToInt64(this ReadOnlySpan<byte> @this, int startIndex = 0)
			=> BitConverter.ToInt64(@this);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ushort ToUInt16(this ReadOnlySpan<byte> @this, int startIndex = 0)
			=> BitConverter.ToUInt16(@this);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint ToUInt32(this ReadOnlySpan<byte> @this, int startIndex = 0)
			=> BitConverter.ToUInt32(@this);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ulong ToUInt64(this ReadOnlySpan<byte> @this, int startIndex = 0)
			=> BitConverter.ToUInt64(@this);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float ToSingle(this ReadOnlySpan<byte> @this, int startIndex = 0)
			=> BitConverter.ToSingle(@this);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double ToDouble(this ReadOnlySpan<byte> @this, int startIndex = 0)
			=> BitConverter.ToDouble(@this);
	}
}
