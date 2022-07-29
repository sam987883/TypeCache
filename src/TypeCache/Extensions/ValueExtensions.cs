// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using TypeCache.Collections.Extensions;
using static TypeCache.Default;

namespace TypeCache.Extensions;

public static class ValueExtensions
{
	/// <remarks>
	/// <code>
	/// <see langword="while"/> (--<paramref name="count"/> &gt; -1)<br/>
	/// <see langword="    yield return"/> @<paramref name="this"/>;
	/// </code>
	/// </remarks>
	public static IEnumerable<T> Repeat<T>(this T @this, int count)
		where T : unmanaged
	{
		while (--count > -1)
			yield return @this;
	}

	/// <remarks>
	/// <c>=&gt; (@<paramref name="this"/>, <paramref name="value"/>) = (<paramref name="value"/>, @<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static void Swap<T>(this ref T @this, ref T value)
		where T : struct
		=> (@this, value) = (value, @this);

	public static IEnumerable<int> To(this int @this, int end, int increment = 0)
	{
		int? i = increment switch
		{
			0 when @this < end => 1,
			0 when @this > end => -1,
			< 0 when @this < end => null,
			> 0 when @this > end => null,
			_ when @this == end => 0,
			_ => increment
		};

		if (!i.HasValue)
			yield break;

		if (i == 0)
			yield return @this;

		increment = i.Value;

		if (@this < end)
		{
			while (@this <= end)
			{
				yield return @this;
				@this += increment;
			}
		}
		else if (@this > end)
		{
			while (@this >= end)
			{
				yield return @this;
				@this += increment;
			}
		}
	}

	/// <inheritdoc cref="BitConverter.GetBytes(bool)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="BitConverter"/>.GetBytes(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static byte[] ToBytes(this bool @this)
		=> BitConverter.GetBytes(@this);

	/// <inheritdoc cref="BitConverter.GetBytes(char)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="BitConverter"/>.GetBytes(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static byte[] ToBytes(this char @this)
		=> BitConverter.GetBytes(@this);

	/// <inheritdoc cref="decimal.GetBits(decimal)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="decimal"/>.GetBits(@<paramref name="this"/>).Map(i => i.ToBytes()).Gather().ToArray();</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static byte[] ToBytes(this decimal @this)
		=> decimal.GetBits(@this).Map(i => i.ToBytes()).Gather().ToArray();

	/// <inheritdoc cref="BitConverter.GetBytes(double)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="BitConverter"/>.GetBytes(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static byte[] ToBytes(this double @this)
		=> BitConverter.GetBytes(@this);

	/// <inheritdoc cref="BitConverter.GetBytes(float)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="BitConverter"/>.GetBytes(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static byte[] ToBytes(this float @this)
		=> BitConverter.GetBytes(@this);

	/// <inheritdoc cref="BitConverter.GetBytes(Half)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="BitConverter"/>.GetBytes(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static byte[] ToBytes(this Half @this)
		=> BitConverter.GetBytes(@this);

	/// <inheritdoc cref="BitConverter.GetBytes(int)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="BitConverter"/>.GetBytes(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static byte[] ToBytes(this int @this)
		=> BitConverter.GetBytes(@this);

	/// <inheritdoc cref="BitConverter.GetBytes(long)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="BitConverter"/>.GetBytes(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static byte[] ToBytes(this long @this)
		=> BitConverter.GetBytes(@this);

	/// <inheritdoc cref="BitConverter.GetBytes(short)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="BitConverter"/>.GetBytes(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static byte[] ToBytes(this short @this)
		=> BitConverter.GetBytes(@this);

	/// <inheritdoc cref="BitConverter.GetBytes(uint)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="BitConverter"/>.GetBytes(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static byte[] ToBytes(this uint @this)
		=> BitConverter.GetBytes(@this);

	/// <inheritdoc cref="BitConverter.GetBytes(ulong)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="BitConverter"/>.GetBytes(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static byte[] ToBytes(this ulong @this)
		=> BitConverter.GetBytes(@this);

	/// <inheritdoc cref="BitConverter.GetBytes(ushort)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="BitConverter"/>.GetBytes(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static byte[] ToBytes(this ushort @this)
		=> BitConverter.GetBytes(@this);

	/// <inheritdoc cref="BitConverter.GetBytes(long)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="BitConverter"/>.Int64BitsToDouble(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static double ToDouble(this long @this)
		=> BitConverter.Int64BitsToDouble(@this);

	/// <inheritdoc cref="BitConverter.Int16BitsToHalf(short)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="BitConverter"/>.Int16BitsToHalf(@<paramref name="this"/>);</c>
	/// </remarks>
	public static Half ToInt16(this short @this)
		=> BitConverter.Int16BitsToHalf(@this);

	/// <inheritdoc cref="BitConverter.UInt16BitsToHalf(ushort)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="BitConverter"/>.UInt16BitsToHalf(@<paramref name="this"/>);</c>
	/// </remarks>
	public static Half ToUInt16(this ushort @this)
		=> BitConverter.UInt16BitsToHalf(@this);

	/// <inheritdoc cref="BitConverter.HalfToInt16Bits(Half)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="BitConverter"/>.HalfToInt16Bits(@<paramref name="this"/>);</c>
	/// </remarks>
	public static short ToInt16(this Half @this)
		=> BitConverter.HalfToInt16Bits(@this);

	/// <inheritdoc cref="BitConverter.HalfToUInt16Bits(Half)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="BitConverter"/>.HalfToUInt16Bits(@<paramref name="this"/>);</c>
	/// </remarks>
	public static ushort ToUInt16(this Half @this)
		=> BitConverter.HalfToUInt16Bits(@this);

	/// <inheritdoc cref="BitConverter.SingleToInt32Bits(float)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="BitConverter"/>.SingleToInt32Bits(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static int ToInt32(this float @this)
		=> BitConverter.SingleToInt32Bits(@this);

	/// <inheritdoc cref="BitConverter.DoubleToInt64Bits(double)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="BitConverter"/>.DoubleToInt64Bits(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static long ToInt64(this double @this)
		=> BitConverter.DoubleToInt64Bits(@this);

	/// <inheritdoc cref="BitConverter.Int32BitsToSingle(int)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="BitConverter"/>.Int32BitsToSingle(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static float ToSingle(this int @this)
		=> BitConverter.Int32BitsToSingle(@this);
}
