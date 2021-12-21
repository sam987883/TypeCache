// Copyright (c) 2021 Samuel Abraham

using System;
using System.Runtime.CompilerServices;
using static TypeCache.Default;

namespace TypeCache.Extensions;

public static class MathExtensions
{
	/// <summary>
	/// <c>=&gt; <see cref="Math"/>.Abs(@<paramref name="this"/>);</c>
	/// </summary>
	/// <exception cref="OverflowException"/>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static sbyte AbsoluteValue(this sbyte @this)
		=> Math.Abs(@this);

	/// <summary>
	/// <c>=&gt; <see cref="Math"/>.Abs(@<paramref name="this"/>);</c>
	/// </summary>
	/// <exception cref="OverflowException"/>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static short AbsoluteValue(this short @this)
		=> Math.Abs(@this);

	/// <summary>
	/// <c>=&gt; <see cref="Math"/>.Abs(@<paramref name="this"/>);</c>
	/// </summary>
	/// <exception cref="OverflowException"/>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static int AbsoluteValue(this int @this)
		=> Math.Abs(@this);

	/// <summary>
	/// <c>=&gt; <see cref="Math"/>.Abs(@<paramref name="this"/>);</c>
	/// </summary>
	/// <exception cref="OverflowException"/>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static long AbsoluteValue(this long @this)
		=> Math.Abs(@this);

	/// <summary>
	/// <c>=&gt; <see cref="Math"/>.Abs(@<paramref name="this"/>);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static float AbsoluteValue(this float @this)
		=> Math.Abs(@this);

	/// <summary>
	/// <c>=&gt; <see cref="Math"/>.Abs(@<paramref name="this"/>);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static double AbsoluteValue(this double @this)
		=> Math.Abs(@this);

	/// <summary>
	/// <c>=&gt; <see cref="Math"/>.Abs(@<paramref name="this"/>);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static decimal AbsoluteValue(this decimal @this)
		=> Math.Abs(@this);

	/// <summary>
	/// <c>=&gt; <see cref="Math"/>.BitDecrement(@<paramref name="this"/>);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static double BitDecrement(this double @this)
		=> Math.BitDecrement(@this);

	/// <summary>
	/// <c>=&gt; <see cref="Math"/>.BitIncrement(@<paramref name="this"/>);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static double BitIncrement(this double @this)
		=> Math.BitIncrement(@this);

	/// <summary>
	/// <c>=&gt; <see cref="Math"/>.Ceiling(@<paramref name="this"/>);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static double Ceiling(this double @this)
		=> Math.Ceiling(@this);

	/// <summary>
	/// <c>=&gt; <see cref="Math"/>.Ceiling(@<paramref name="this"/>);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static decimal Ceiling(this decimal @this)
		=> Math.Ceiling(@this);

	/// <summary>
	/// <c>=&gt; <see cref="Math"/>.Floor(@<paramref name="this"/>);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static double Floor(this double @this)
		=> Math.Floor(@this);

	/// <summary>
	/// <c>=&gt; <see cref="Math"/>.Floor(@<paramref name="this"/>);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static decimal Floor(this decimal @this)
		=> Math.Floor(@this);

	/// <summary>
	/// <c>=&gt; <see cref="Math"/>.Round(@<paramref name="this"/>);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static double Round(this double @this)
		=> Math.Round(@this);

	/// <summary>
	/// <c>=&gt; <see cref="Math"/>.Round(@<paramref name="this"/>, <paramref name="digits"/>);</c>
	/// </summary>
	/// <exception cref="ArgumentOutOfRangeException"/>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static double Round(this double @this, int digits)
		=> Math.Round(@this, digits);

	/// <summary>
	/// <c>=&gt; <see cref="Math"/>.Round(@<paramref name="this"/>, <paramref name="rounding"/>);</c>
	/// </summary>
	/// <exception cref="ArgumentException"/>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static double Round(this double @this, MidpointRounding rounding)
		=> Math.Round(@this, rounding);

	/// <summary>
	/// <c>=&gt; <see cref="Math"/>.Round(@<paramref name="this"/>, <paramref name="digits"/>, <paramref name="rounding"/>);</c>
	/// </summary>
	/// <exception cref="ArgumentException"/>
	/// <exception cref="ArgumentOutOfRangeException"/>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static double Round(this double @this, int digits, MidpointRounding rounding)
		=> Math.Round(@this, digits, rounding);

	/// <summary>
	/// <c>=&gt; <see cref="Math"/>.Round(@<paramref name="this"/>);</c>
	/// </summary>
	/// <exception cref="OverflowException"/>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static decimal Round(this decimal @this)
		=> Math.Round(@this);

	/// <summary>
	/// <c>=&gt; <see cref="Math"/>.Round(@<paramref name="this"/>, <paramref name="digits"/>);</c>
	/// </summary>
	/// <exception cref="ArgumentException"/>
	/// <exception cref="OverflowException"/>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static decimal Round(this decimal @this, int digits)
		=> Math.Round(@this, digits);

	/// <summary>
	/// <c>=&gt; <see cref="Math"/>.Round(@<paramref name="this"/>, <paramref name="rounding"/>);</c>
	/// </summary>
	/// <exception cref="ArgumentException"/>
	/// <exception cref="OverflowException"/>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static decimal Round(this decimal @this, MidpointRounding rounding)
		=> Math.Round(@this, rounding);

	/// <summary>
	/// <c>=&gt; <see cref="Math"/>.Round(@<paramref name="this"/>, <paramref name="digits"/>, <paramref name="rounding"/>);</c>
	/// </summary>
	/// <exception cref="ArgumentException"/>
	/// <exception cref="ArgumentOutOfRangeException"/>
	/// <exception cref="OverflowException"/>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static decimal Round(this decimal @this, int digits, MidpointRounding rounding)
		=> Math.Round(@this, digits, rounding);

	/// <summary>
	/// <c>=&gt; <see cref="Math"/>.Sign(@<paramref name="this"/>);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static int Sign(this sbyte @this)
		=> Math.Sign(@this);

	/// <summary>
	/// <c>=&gt; <see cref="Math"/>.Sign(@<paramref name="this"/>);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static int Sign(this short @this)
		=> Math.Sign(@this);

	/// <summary>
	/// <c>=&gt; <see cref="Math"/>.Sign(@<paramref name="this"/>);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static int Sign(this int @this)
		=> Math.Sign(@this);

	/// <summary>
	/// <c>=&gt; <see cref="Math"/>.Sign(@<paramref name="this"/>);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static int Sign(this long @this)
		=> Math.Sign(@this);

	/// <summary>
	/// <c>=&gt; <see cref="Math"/>.Sign(@<paramref name="this"/>);</c>
	/// </summary>
	/// <exception cref="ArithmeticException"/>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static int Sign(this float @this)
		=> Math.Sign(@this);

	/// <summary>
	/// <c>=&gt; <see cref="Math"/>.Sign(@<paramref name="this"/>);</c>
	/// </summary>
	/// <exception cref="ArithmeticException"/>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static int Sign(this double @this)
		=> Math.Sign(@this);

	/// <summary>
	/// <c>=&gt; <see cref="Math"/>.Sign(@<paramref name="this"/>);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static int Sign(this decimal @this)
		=> Math.Sign(@this);

	/// <summary>
	/// <c>=&gt; <see cref="Math"/>.Truncate(@<paramref name="this"/>);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static double Truncate(this double @this)
		=> Math.Truncate(@this);

	/// <summary>
	/// <c>=&gt; <see cref="Math"/>.Truncate(@<paramref name="this"/>);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static decimal Truncate(this decimal @this)
		=> Math.Truncate(@this);
}
