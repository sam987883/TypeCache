// Copyright (c) 2021 Samuel Abraham

using System;
using System.Runtime.CompilerServices;
using TypeCache.Collections.Extensions;

namespace TypeCache.Extensions
{
	public static class BitConverterExtensions
	{
		/// <summary>
		/// <c><see cref="BitConverter.ToBoolean(byte[], int)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool ToBoolean(this byte[] @this, int startIndex = 0)
			=> BitConverter.ToBoolean(@this, startIndex);

		/// <summary>
		/// <c><see cref="BitConverter.ToBoolean(ReadOnlySpan{byte})"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool ToBoolean(this ReadOnlySpan<byte> @this)
			=> BitConverter.ToBoolean(@this);

		/// <summary>
		/// <c><see cref="BitConverter.GetBytes(bool)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static byte[] ToBytes(this bool @this)
			=> BitConverter.GetBytes(@this);

		/// <summary>
		/// <c><see cref="BitConverter.GetBytes(char)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static byte[] ToBytes(this char @this)
			=> BitConverter.GetBytes(@this);

		/// <summary>
		/// <c><see cref="BitConverter.GetBytes(short)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static byte[] ToBytes(this short @this)
			=> BitConverter.GetBytes(@this);

		/// <summary>
		/// <c><see cref="BitConverter.GetBytes(int)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static byte[] ToBytes(this int @this)
			=> BitConverter.GetBytes(@this);

		/// <summary>
		/// <c><see cref="BitConverter.GetBytes(long)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static byte[] ToBytes(this long @this)
			=> BitConverter.GetBytes(@this);

		/// <summary>
		/// <c><see cref="BitConverter.GetBytes(ushort)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static byte[] ToBytes(this ushort @this)
			=> BitConverter.GetBytes(@this);

		/// <summary>
		/// <c><see cref="BitConverter.GetBytes(uint)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static byte[] ToBytes(this uint @this)
			=> BitConverter.GetBytes(@this);

		/// <summary>
		/// <c><see cref="BitConverter.GetBytes(ulong)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static byte[] ToBytes(this ulong @this)
			=> BitConverter.GetBytes(@this);

		/// <summary>
		/// <c><see cref="BitConverter.GetBytes(float)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static byte[] ToBytes(this float @this)
			=> BitConverter.GetBytes(@this);

		/// <summary>
		/// <c><see cref="BitConverter.GetBytes(double)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static byte[] ToBytes(this double @this)
			=> BitConverter.GetBytes(@this);

		/// <summary>
		/// <code>decimal.GetBits(@this).ToMany(BitConverter.GetBytes).ToArray();</code>
		/// </summary>
		public static byte[] ToBytes(this decimal @this)
			=> decimal.GetBits(@this).ToMany(BitConverter.GetBytes).ToArray();

		/// <summary>
		/// <c><see cref="BitConverter.ToChar(byte[], int)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static char ToChar(this byte[] @this, int startIndex = 0)
			=> BitConverter.ToChar(@this, startIndex);


		/// <summary>
		/// <c><see cref="BitConverter.ToChar(ReadOnlySpan{byte})"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static char ToChar(this ReadOnlySpan<byte> @this)
			=> BitConverter.ToChar(@this);

		/// <summary>
		/// <c><see cref="BitConverter.ToInt16(byte[], int)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static short ToInt16(this byte[] @this, int startIndex = 0)
			=> BitConverter.ToInt16(@this, startIndex);

		/// <summary>
		/// <c><see cref="BitConverter.ToInt16(ReadOnlySpan{byte})"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static short ToInt16(this ReadOnlySpan<byte> @this)
			=> BitConverter.ToInt16(@this);

		/// <summary>
		/// <c><see cref="BitConverter.SingleToInt32Bits(float)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int ToInt32(this float @this)
			=> BitConverter.SingleToInt32Bits(@this);

		/// <summary>
		/// <c><see cref="BitConverter.ToInt32(byte[], int)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int ToInt32(this byte[] @this, int startIndex = 0)
			=> BitConverter.ToInt32(@this, startIndex);

		/// <summary>
		/// <c><see cref="BitConverter.ToInt32(ReadOnlySpan{byte})"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int ToInt32(this ReadOnlySpan<byte> @this)
			=> BitConverter.ToInt32(@this);

		/// <summary>
		/// <c><see cref="BitConverter.DoubleToInt64Bits(double)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static long ToInt64(this double @this)
			=> BitConverter.DoubleToInt64Bits(@this);

		/// <summary>
		/// <c><see cref="BitConverter.ToInt64(byte[], int)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static long ToInt64(this byte[] @this, int startIndex = 0)
			=> BitConverter.ToInt64(@this, startIndex);

		/// <summary>
		/// <c><see cref="BitConverter.ToInt64(ReadOnlySpan{byte})"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static long ToInt64(this ReadOnlySpan<byte> @this)
			=> BitConverter.ToInt64(@this);

		/// <summary>
		/// <c><see cref="BitConverter.ToUInt16(byte[], int)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ushort ToUInt16(this byte[] @this, int startIndex = 0)
			=> BitConverter.ToUInt16(@this, startIndex);

		/// <summary>
		/// <c><see cref="BitConverter.ToUInt16(ReadOnlySpan{byte})"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ushort ToUInt16(this ReadOnlySpan<byte> @this)
			=> BitConverter.ToUInt16(@this);

		/// <summary>
		/// <c><see cref="BitConverter.ToUInt32(byte[], int)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint ToUInt32(this byte[] @this, int startIndex = 0)
			=> BitConverter.ToUInt32(@this, startIndex);

		/// <summary>
		/// <c><see cref="BitConverter.ToUInt32(ReadOnlySpan{byte})"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint ToUInt32(this ReadOnlySpan<byte> @this)
			=> BitConverter.ToUInt32(@this);

		/// <summary>
		/// <c><see cref="BitConverter.ToUInt64(byte[], int)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ulong ToUInt64(this byte[] @this, int startIndex = 0)
			=> BitConverter.ToUInt64(@this, startIndex);

		/// <summary>
		/// <c><see cref="BitConverter.ToUInt64(ReadOnlySpan{byte})"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ulong ToUInt64(this ReadOnlySpan<byte> @this)
			=> BitConverter.ToUInt64(@this);

		/// <summary>
		/// <c><see cref="BitConverter.Int32BitsToSingle(int)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float ToSingle(this int @this)
			=> BitConverter.Int32BitsToSingle(@this);

		/// <summary>
		/// <c><see cref="BitConverter.ToSingle(byte[], int)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float ToSingle(this byte[] @this, int startIndex = 0)
			=> BitConverter.ToSingle(@this, startIndex);

		/// <summary>
		/// <c><see cref="BitConverter.ToSingle(ReadOnlySpan{byte})"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float ToSingle(this ReadOnlySpan<byte> @this)
			=> BitConverter.ToSingle(@this);

		/// <summary>
		/// <c><see cref="BitConverter.Int64BitsToDouble(long)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double ToDouble(this long @this)
			=> BitConverter.Int64BitsToDouble(@this);

		/// <summary>
		/// <c><see cref="BitConverter.ToDouble(byte[], int)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double ToDouble(this byte[] @this, int startIndex = 0)
			=> BitConverter.ToDouble(@this, startIndex);

		/// <summary>
		/// <c><see cref="BitConverter.ToDouble(ReadOnlySpan{byte})"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double ToDouble(this ReadOnlySpan<byte> @this)
			=> BitConverter.ToDouble(@this);

		/// <summary>
		/// <c><see cref="BitConverter.ToString(byte[], int)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string ToText(this byte[] @this, int startIndex = 0)
			=> BitConverter.ToString(@this, startIndex);

		/// <summary>
		/// <c><see cref="BitConverter.ToString(byte[], int, int)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string ToText(this byte[] @this, int startIndex, int length)
			=> BitConverter.ToString(@this, startIndex, length);

		/// <summary>
		/// <c><see cref="BitConverter.TryWriteBytes(Span{byte}, bool)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool TryWriteBytes(this Span<byte> @this, bool value)
			=> BitConverter.TryWriteBytes(@this, value);

		/// <summary>
		/// <c><see cref="BitConverter.TryWriteBytes(Span{byte}, char)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool TryWriteBytes(this Span<byte> @this, char value)
			=> BitConverter.TryWriteBytes(@this, value);

		/// <summary>
		/// <c><see cref="BitConverter.TryWriteBytes(Span{byte}, short)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool TryWriteBytes(this Span<byte> @this, short value)
			=> BitConverter.TryWriteBytes(@this, value);

		/// <summary>
		/// <c><see cref="BitConverter.TryWriteBytes(Span{byte}, int)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool TryWriteBytes(this Span<byte> @this, int value)
			=> BitConverter.TryWriteBytes(@this, value);

		/// <summary>
		/// <c><see cref="BitConverter.TryWriteBytes(Span{byte}, long)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool TryWriteBytes(this Span<byte> @this, long value)
			=> BitConverter.TryWriteBytes(@this, value);

		/// <summary>
		/// <c><see cref="BitConverter.TryWriteBytes(Span{byte}, ushort)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool TryWriteBytes(this Span<byte> @this, ushort value)
			=> BitConverter.TryWriteBytes(@this, value);

		/// <summary>
		/// <c><see cref="BitConverter.TryWriteBytes(Span{byte}, uint)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool TryWriteBytes(this Span<byte> @this, uint value)
			=> BitConverter.TryWriteBytes(@this, value);

		/// <summary>
		/// <c><see cref="BitConverter.TryWriteBytes(Span{byte}, ulong)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool TryWriteBytes(this Span<byte> @this, ulong value)
			=> BitConverter.TryWriteBytes(@this, value);

		/// <summary>
		/// <c><see cref="BitConverter.TryWriteBytes(Span{byte}, float)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool TryWriteBytes(this Span<byte> @this, float value)
			=> BitConverter.TryWriteBytes(@this, value);

		/// <summary>
		/// <c><see cref="BitConverter.TryWriteBytes(Span{byte}, double)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool TryWriteBytes(this Span<byte> @this, double value)
			=> BitConverter.TryWriteBytes(@this, value);
	}
}
