// Copyright (c) 2021 Samuel Abraham

namespace TypeCache.Extensions;

public static class BitConverterExtensions
{
	/// <inheritdoc cref="BitConverter.GetBytes(bool)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="BitConverter"/>.GetBytes(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static byte[] GetBytes(this bool @this)
		=> BitConverter.GetBytes(@this);

	/// <inheritdoc cref="BitConverter.GetBytes(char)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="BitConverter"/>.GetBytes(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static byte[] GetBytes(this char @this)
		=> BitConverter.GetBytes(@this);

	/// <inheritdoc cref="BitConverter.GetBytes(double)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="BitConverter"/>.GetBytes(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static byte[] GetBytes(this double @this)
		=> BitConverter.GetBytes(@this);

	/// <inheritdoc cref="BitConverter.GetBytes(float)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="BitConverter"/>.GetBytes(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static byte[] GetBytes(this float @this)
		=> BitConverter.GetBytes(@this);

	/// <inheritdoc cref="BitConverter.GetBytes(Half)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="BitConverter"/>.GetBytes(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static byte[] GetBytes(this Half @this)
		=> BitConverter.GetBytes(@this);

	/// <inheritdoc cref="BitConverter.GetBytes(int)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="BitConverter"/>.GetBytes(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static byte[] GetBytes(this int @this)
		=> BitConverter.GetBytes(@this);

	/// <inheritdoc cref="BitConverter.GetBytes(long)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="BitConverter"/>.GetBytes(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static byte[] GetBytes(this long @this)
		=> BitConverter.GetBytes(@this);

	/// <inheritdoc cref="BitConverter.GetBytes(short)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="BitConverter"/>.GetBytes(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static byte[] GetBytes(this short @this)
		=> BitConverter.GetBytes(@this);

	/// <inheritdoc cref="BitConverter.GetBytes(uint)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="BitConverter"/>.GetBytes(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static byte[] GetBytes(this uint @this)
		=> BitConverter.GetBytes(@this);

	/// <inheritdoc cref="BitConverter.GetBytes(ulong)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="BitConverter"/>.GetBytes(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static byte[] GetBytes(this ulong @this)
		=> BitConverter.GetBytes(@this);

	/// <inheritdoc cref="BitConverter.GetBytes(ushort)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="BitConverter"/>.GetBytes(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static byte[] GetBytes(this ushort @this)
		=> BitConverter.GetBytes(@this);

	/// <inheritdoc cref="BitConverter.ToBoolean(ReadOnlySpan{byte})"/>
	/// <remarks>
	/// <c>=&gt; <see cref="BitConverter"/>.ToBoolean(@<paramref name="this"/>.Span);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool ToBoolean(this ReadOnlySpan<byte> @this)
		=> BitConverter.ToBoolean(@this);

	/// <inheritdoc cref="BitConverter.ToBoolean(byte[], int)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="BitConverter"/>.ToBoolean(@<paramref name="this"/>, <paramref name="startIndex"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool ToBoolean(this byte[] @this, int startIndex = 0)
		=> BitConverter.ToBoolean(@this, startIndex);

	/// <inheritdoc cref="BitConverter.ToChar(ReadOnlySpan{byte})"/>
	/// <remarks>
	/// <c>=&gt; <see cref="BitConverter"/>.ToChar(@<paramref name="this"/>.Span);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static char ToChar(this ReadOnlySpan<byte> @this)
		=> BitConverter.ToChar(@this);

	/// <inheritdoc cref="BitConverter.ToChar(byte[], int)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="BitConverter"/>.ToChar(@<paramref name="this"/>, <paramref name="startIndex"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static char ToChar(this byte[] @this, int startIndex = 0)
		=> BitConverter.ToChar(@this, startIndex);

	/// <inheritdoc cref="BitConverter.GetBytes(long)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="BitConverter"/>.Int64BitsToDouble(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static double ToDouble(this long @this)
		=> BitConverter.Int64BitsToDouble(@this);

	/// <inheritdoc cref="BitConverter.ToDouble(ReadOnlySpan{byte})"/>
	/// <remarks>
	/// <c>=&gt; <see cref="BitConverter"/>.ToDouble(@<paramref name="this"/>.Span);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static double ToDouble(this ReadOnlySpan<byte> @this)
		=> BitConverter.ToDouble(@this);

	/// <inheritdoc cref="BitConverter.ToDouble(byte[], int)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="BitConverter"/>.ToDouble(@<paramref name="this"/>, <paramref name="startIndex"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static double ToDouble(this byte[] @this, int startIndex = 0)
		=> BitConverter.ToDouble(@this, startIndex);

	/// <inheritdoc cref="BitConverter.Int16BitsToHalf(short)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="BitConverter"/>.Int16BitsToHalf(@<paramref name="this"/>);</c>
	/// </remarks>
	public static Half ToInt16(this short @this)
		=> BitConverter.Int16BitsToHalf(@this);

	/// <inheritdoc cref="BitConverter.HalfToInt16Bits(Half)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="BitConverter"/>.HalfToInt16Bits(@<paramref name="this"/>);</c>
	/// </remarks>
	public static short ToInt16(this Half @this)
		=> BitConverter.HalfToInt16Bits(@this);

	/// <inheritdoc cref="BitConverter.ToInt16(ReadOnlySpan{byte})"/>
	/// <remarks>
	/// <c>=&gt; <see cref="BitConverter"/>.ToInt16(@<paramref name="this"/>.Span);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static short ToInt16(this ReadOnlySpan<byte> @this)
		=> BitConverter.ToInt16(@this);

	/// <inheritdoc cref="BitConverter.ToInt16(byte[], int)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="BitConverter"/>.ToInt16(@<paramref name="this"/>, <paramref name="startIndex"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static short ToInt16(this byte[] @this, int startIndex = 0)
		=> BitConverter.ToInt16(@this, startIndex);

	/// <inheritdoc cref="BitConverter.SingleToInt32Bits(float)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="BitConverter"/>.SingleToInt32Bits(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static int ToInt32(this float @this)
		=> BitConverter.SingleToInt32Bits(@this);

	/// <inheritdoc cref="BitConverter.ToInt32(ReadOnlySpan{byte})"/>
	/// <remarks>
	/// <c>=&gt; <see cref="BitConverter"/>.ToInt32(@<paramref name="this"/>.Span);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static int ToInt32(this ReadOnlySpan<byte> @this)
		=> BitConverter.ToInt32(@this);

	/// <inheritdoc cref="BitConverter.ToInt32(byte[], int)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="BitConverter"/>.ToInt32(@<paramref name="this"/>, <paramref name="startIndex"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static int ToInt32(this byte[] @this, int startIndex = 0)
		=> BitConverter.ToInt32(@this, startIndex);

	/// <inheritdoc cref="BitConverter.DoubleToInt64Bits(double)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="BitConverter"/>.DoubleToInt64Bits(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static long ToInt64(this double @this)
		=> BitConverter.DoubleToInt64Bits(@this);

	/// <inheritdoc cref="BitConverter.ToInt64(ReadOnlySpan{byte})"/>
	/// <remarks>
	/// <c>=&gt; <see cref="BitConverter"/>.ToInt64(@<paramref name="this"/>.Span);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static long ToInt64(this ReadOnlySpan<byte> @this)
		=> BitConverter.ToInt64(@this);

	/// <inheritdoc cref="BitConverter.ToInt64(byte[], int)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="BitConverter"/>.ToInt64(@<paramref name="this"/>, <paramref name="startIndex"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static long ToInt64(this byte[] @this, int startIndex = 0)
		=> BitConverter.ToInt64(@this, startIndex);

	/// <inheritdoc cref="BitConverter.Int32BitsToSingle(int)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="BitConverter"/>.Int32BitsToSingle(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static float ToSingle(this int @this)
		=> BitConverter.Int32BitsToSingle(@this);

	/// <inheritdoc cref="BitConverter.ToSingle(ReadOnlySpan{byte})"/>
	/// <remarks>
	/// <c>=&gt; <see cref="BitConverter"/>.ToSingle(@<paramref name="this"/>.Span);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static float ToSingle(this ReadOnlySpan<byte> @this)
		=> BitConverter.ToSingle(@this);

	/// <inheritdoc cref="BitConverter.ToSingle(byte[], int)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="BitConverter"/>.ToSingle(@<paramref name="this"/>, <paramref name="startIndex"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static float ToSingle(this byte[] @this, int startIndex = 0)
		=> BitConverter.ToSingle(@this, startIndex);

	/// <inheritdoc cref="BitConverter.ToString(byte[])"/>
	/// <remarks>
	/// <c>=&gt; <see cref="BitConverter"/>.ToString(@<paramref name="this"/>.Span);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static string ToText(this ReadOnlySpan<byte> @this)
		=> BitConverter.ToString(@this.ToArray());

	/// <remarks>
	/// <c>=&gt; <see cref="BitConverter"/>.ToString(@<paramref name="this"/>.Span, <paramref name="length"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static string ToText(this ReadOnlySpan<byte> @this, int startIndex, int length)
		=> BitConverter.ToString(@this.ToArray(), startIndex, length);

	/// <inheritdoc cref="BitConverter.ToString(byte[], int)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="BitConverter"/>.ToString(@<paramref name="this"/>, <paramref name="startIndex"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static string ToText(this byte[] @this, int startIndex = 0)
		=> BitConverter.ToString(@this, startIndex);

	/// <inheritdoc cref="BitConverter.ToString(byte[], int, int)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="BitConverter"/>.ToString(@<paramref name="this"/>, <paramref name="startIndex"/>, <paramref name="length"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static string ToText(this byte[] @this, int startIndex, int length)
		=> BitConverter.ToString(@this, startIndex, length);

	/// <inheritdoc cref="BitConverter.UInt16BitsToHalf(ushort)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="BitConverter"/>.UInt16BitsToHalf(@<paramref name="this"/>);</c>
	/// </remarks>
	public static Half ToUInt16(this ushort @this)
		=> BitConverter.UInt16BitsToHalf(@this);

	/// <inheritdoc cref="BitConverter.HalfToUInt16Bits(Half)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="BitConverter"/>.HalfToUInt16Bits(@<paramref name="this"/>);</c>
	/// </remarks>
	public static ushort ToUInt16(this Half @this)
		=> BitConverter.HalfToUInt16Bits(@this);

	/// <inheritdoc cref="BitConverter.ToUInt16(ReadOnlySpan{byte})"/>
	/// <remarks>
	/// <c>=&gt; <see cref="BitConverter"/>.ToUInt16(@<paramref name="this"/>.Span);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static ushort ToUInt16(this ReadOnlySpan<byte> @this)
		=> BitConverter.ToUInt16(@this);

	/// <inheritdoc cref="BitConverter.ToUInt16(byte[], int)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="BitConverter"/>.ToUInt16(@<paramref name="this"/>, <paramref name="startIndex"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static ushort ToUInt16(this byte[] @this, int startIndex = 0)
		=> BitConverter.ToUInt16(@this, startIndex);

	/// <inheritdoc cref="BitConverter.ToUInt32(ReadOnlySpan{byte})"/>
	/// <remarks>
	/// <c>=&gt; <see cref="BitConverter"/>.ToUInt32(@<paramref name="this"/>.Span);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static uint ToUInt32(this ReadOnlySpan<byte> @this)
		=> BitConverter.ToUInt32(@this);

	/// <inheritdoc cref="BitConverter.ToUInt32(byte[], int)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="BitConverter"/>.ToUInt32(@<paramref name="this"/>, <paramref name="startIndex"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static uint ToUInt32(this byte[] @this, int startIndex = 0)
		=> BitConverter.ToUInt32(@this, startIndex);

	/// <inheritdoc cref="BitConverter.ToUInt64(ReadOnlySpan{byte})"/>
	/// <remarks>
	/// <c>=&gt; <see cref="BitConverter"/>.ToUInt64(@<paramref name="this"/>.Span);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static ulong ToUInt64(this ReadOnlySpan<byte> @this)
		=> BitConverter.ToUInt64(@this);

	/// <inheritdoc cref="BitConverter.ToUInt64(byte[], int)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="BitConverter"/>.ToUInt64(@<paramref name="this"/>, <paramref name="startIndex"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static ulong ToUInt64(this byte[] @this, int startIndex = 0)
		=> BitConverter.ToUInt64(@this, startIndex);
}
