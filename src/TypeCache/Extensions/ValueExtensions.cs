// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using TypeCache.Collections.Extensions;
using static TypeCache.Default;

namespace TypeCache.Extensions;

public static class ValueExtensions
{
	/// <summary>
	/// <code>
	/// <see langword="while"/> (--<paramref name="count"/> &gt; -1)<br/>
	/// <see langword="    yield return"/> @<paramref name="this"/>;
	/// </code>
	/// </summary>
	public static IEnumerable<T> Repeat<T>(this T @this, int count)
		where T : unmanaged
	{
		while (--count > -1)
			yield return @this;
	}

	/// <summary>
	/// <c>=&gt; (@<paramref name="this"/>, <paramref name="value"/>) = (<paramref name="value"/>, @<paramref name="this"/>);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
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

	/// <summary>
	/// <c>=&gt; <see cref="BitConverter"/>.GetBytes(@<paramref name="this"/>);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static byte[] ToBytes(this bool @this)
		=> BitConverter.GetBytes(@this);

	/// <summary>
	/// <c>=&gt; <see cref="BitConverter"/>.GetBytes(@<paramref name="this"/>);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static byte[] ToBytes(this char @this)
		=> BitConverter.GetBytes(@this);

	/// <summary>
	/// <c>=&gt; <see cref="decimal"/>.GetBits(@<paramref name="this"/>).Map(i => i.ToBytes()).Gather().ToArray();</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static byte[] ToBytes(this decimal @this)
		=> decimal.GetBits(@this).Map(i => i.ToBytes()).Gather().ToArray();

	/// <summary>
	/// <c>=&gt; <see cref="BitConverter"/>.GetBytes(@<paramref name="this"/>);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static byte[] ToBytes(this double @this)
		=> BitConverter.GetBytes(@this);

	/// <summary>
	/// <c>=&gt; <see cref="BitConverter"/>.GetBytes(@<paramref name="this"/>);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static byte[] ToBytes(this float @this)
		=> BitConverter.GetBytes(@this);

	/// <summary>
	/// <c>=&gt; <see cref="BitConverter"/>.GetBytes(@<paramref name="this"/>);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static byte[] ToBytes(this Half @this)
		=> BitConverter.GetBytes(@this);

	/// <summary>
	/// <c>=&gt; <see cref="BitConverter"/>.GetBytes(@<paramref name="this"/>);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static byte[] ToBytes(this int @this)
		=> BitConverter.GetBytes(@this);

	/// <summary>
	/// <c>=&gt; <see cref="BitConverter"/>.GetBytes(@<paramref name="this"/>);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static byte[] ToBytes(this long @this)
		=> BitConverter.GetBytes(@this);

	/// <summary>
	/// <c>=&gt; <see cref="BitConverter"/>.GetBytes(@<paramref name="this"/>);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static byte[] ToBytes(this short @this)
		=> BitConverter.GetBytes(@this);

	/// <summary>
	/// <c>=&gt; <see cref="BitConverter"/>.GetBytes(@<paramref name="this"/>);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static byte[] ToBytes(this uint @this)
		=> BitConverter.GetBytes(@this);

	/// <summary>
	/// <c>=&gt; <see cref="BitConverter"/>.GetBytes(@<paramref name="this"/>);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static byte[] ToBytes(this ulong @this)
		=> BitConverter.GetBytes(@this);

	/// <summary>
	/// <c>=&gt; <see cref="BitConverter"/>.GetBytes(@<paramref name="this"/>);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static byte[] ToBytes(this ushort @this)
		=> BitConverter.GetBytes(@this);

	/// <summary>
	/// <c>=&gt; <see cref="BitConverter"/>.Int64BitsToDouble(@<paramref name="this"/>);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static double ToDouble(this long @this)
		=> BitConverter.Int64BitsToDouble(@this);

	/// <summary>
	/// <c>=&gt; <see cref="BitConverter"/>.SingleToInt32Bits(@<paramref name="this"/>);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static int ToInt32(this float @this)
		=> BitConverter.SingleToInt32Bits(@this);

	/// <summary>
	/// <c>=&gt; <see cref="BitConverter"/>.DoubleToInt64Bits(@<paramref name="this"/>);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static long ToInt64(this double @this)
		=> BitConverter.DoubleToInt64Bits(@this);

	/// <summary>
	/// <c>=&gt; <see cref="BitConverter"/>.Int32BitsToSingle(@<paramref name="this"/>);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static float ToSingle(this int @this)
		=> BitConverter.Int32BitsToSingle(@this);
}
