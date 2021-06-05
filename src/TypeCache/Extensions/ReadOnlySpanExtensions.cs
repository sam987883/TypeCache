// Copyright (c) 2021 Samuel Abraham

using System;
using System.Runtime.CompilerServices;

namespace TypeCache.Extensions
{
	public static class ReadOnlySpanExtensions
	{
		/// <summary>
		/// <c><see cref="BitConverter.ToBoolean(ReadOnlySpan{byte})"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool ToBoolean(this ReadOnlySpan<byte> @this)
			=> BitConverter.ToBoolean(@this);

		/// <summary>
		/// <c><see cref="BitConverter.ToChar(ReadOnlySpan{byte})"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static char ToChar(this ReadOnlySpan<byte> @this)
			=> BitConverter.ToChar(@this);

		/// <summary>
		/// <c><see cref="BitConverter.ToInt16(ReadOnlySpan{byte})"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static short ToInt16(this ReadOnlySpan<byte> @this)
			=> BitConverter.ToInt16(@this);

		/// <summary>
		/// <c><see cref="BitConverter.ToInt32(ReadOnlySpan{byte})"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int ToInt32(this ReadOnlySpan<byte> @this)
			=> BitConverter.ToInt32(@this);

		/// <summary>
		/// <c><see cref="BitConverter.ToInt64(ReadOnlySpan{byte})"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static long ToInt64(this ReadOnlySpan<byte> @this)
			=> BitConverter.ToInt64(@this);

		/// <summary>
		/// <c><see cref="BitConverter.ToUInt16(ReadOnlySpan{byte})"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ushort ToUInt16(this ReadOnlySpan<byte> @this)
			=> BitConverter.ToUInt16(@this);

		/// <summary>
		/// <c><see cref="BitConverter.ToUInt32(ReadOnlySpan{byte})"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint ToUInt32(this ReadOnlySpan<byte> @this)
			=> BitConverter.ToUInt32(@this);

		/// <summary>
		/// <c><see cref="BitConverter.ToUInt64(ReadOnlySpan{byte})"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ulong ToUInt64(this ReadOnlySpan<byte> @this)
			=> BitConverter.ToUInt64(@this);

		/// <summary>
		/// <c><see cref="BitConverter.ToSingle(ReadOnlySpan{byte})"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float ToSingle(this ReadOnlySpan<byte> @this)
			=> BitConverter.ToSingle(@this);

		/// <summary>
		/// <c><see cref="BitConverter.ToDouble(ReadOnlySpan{byte})"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double ToDouble(this ReadOnlySpan<byte> @this)
			=> BitConverter.ToDouble(@this);
	}
}
