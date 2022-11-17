// Copyright (c) 2021 Samuel Abraham

using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using static TypeCache.Default;

namespace TypeCache.Extensions;

public static class MathExtensions
{
	/// <inheritdoc cref="Math.Abs(sbyte)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Math"/>.Abs(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static sbyte AbsoluteValue(this sbyte @this)
		=> Math.Abs(@this);

	/// <inheritdoc cref="Math.Abs(short)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Math"/>.Abs(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static short AbsoluteValue(this short @this)
		=> Math.Abs(@this);

	/// <inheritdoc cref="Math.Abs(int)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Math"/>.Abs(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static int AbsoluteValue(this int @this)
		=> Math.Abs(@this);

	/// <inheritdoc cref="Math.Abs(long)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Math"/>.Abs(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static long AbsoluteValue(this long @this)
		=> Math.Abs(@this);

	/// <inheritdoc cref="Math.Abs(float)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Math"/>.Abs(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static float AbsoluteValue(this float @this)
		=> Math.Abs(@this);

	/// <inheritdoc cref="Math.Abs(double)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Math"/>.Abs(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static double AbsoluteValue(this double @this)
		=> Math.Abs(@this);

	/// <inheritdoc cref="Math.Abs(decimal)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Math"/>.Abs(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static decimal AbsoluteValue(this decimal @this)
		=> Math.Abs(@this);

	/// <inheritdoc cref="Math.BitDecrement(double)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Math"/>.BitDecrement(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static double BitDecrement(this double @this)
		=> Math.BitDecrement(@this);

	/// <inheritdoc cref="Math.BitIncrement(double)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Math"/>.BitIncrement(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static double BitIncrement(this double @this)
		=> Math.BitIncrement(@this);

	/// <inheritdoc cref="Math.Ceiling(double)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Math"/>.Ceiling(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static double Ceiling(this double @this)
		=> Math.Ceiling(@this);

	/// <inheritdoc cref="Math.Ceiling(decimal)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Math"/>.Ceiling(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static decimal Ceiling(this decimal @this)
		=> Math.Ceiling(@this);

	/// <remarks>
	/// <code>
	/// (@<paramref name="this"/> &gt;= 0).Assert(<see langword="true"/>);<br/>
	/// <br/>
	/// <see langword="var"/> result = 1UL;<br/>
	/// <see langword="while"/> (@<paramref name="this"/> &gt; 0)<br/>
	/// {<br/>
	/// <see langword="    "/>result *= (ulong)@<paramref name="this"/>;<br/>
	/// <see langword="    "/>--@<paramref name="this"/>;<br/>
	/// }<br/>
	/// <see langword="return"/> result;
	/// </code>
	/// </remarks>
	public static ulong Factorial(this int @this)
	{
		(@this >= 0).AssertTrue();

		var result = 1UL;
		while (@this > 0)
		{
			result *= (ulong)@this;
			--@this;
		}
		return result;
	}

	/// <inheritdoc cref="Math.Floor(double)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Math"/>.Floor(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static double Floor(this double @this)
		=> Math.Floor(@this);

	/// <inheritdoc cref="Math.Floor(decimal)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Math"/>.Floor(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static decimal Floor(this decimal @this)
		=> Math.Floor(@this);

	/// <inheritdoc cref="Math.Round(double)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Math"/>.Round(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static double Round(this double @this)
		=> Math.Round(@this);

	/// <inheritdoc cref="Math.Round(double, int)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Math"/>.Round(@<paramref name="this"/>, <paramref name="digits"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static double Round(this double @this, int digits)
		=> Math.Round(@this, digits);

	/// <inheritdoc cref="Math.Round(double, MidpointRounding)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Math"/>.Round(@<paramref name="this"/>, <paramref name="rounding"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static double Round(this double @this, MidpointRounding rounding)
		=> Math.Round(@this, rounding);

	/// <inheritdoc cref="Math.Round(double, int, MidpointRounding)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Math"/>.Round(@<paramref name="this"/>, <paramref name="digits"/>, <paramref name="rounding"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static double Round(this double @this, int digits, MidpointRounding rounding)
		=> Math.Round(@this, digits, rounding);

	/// <inheritdoc cref="Math.Round(decimal)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Math"/>.Round(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static decimal Round(this decimal @this)
		=> Math.Round(@this);

	/// <inheritdoc cref="Math.Round(decimal, int)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Math"/>.Round(@<paramref name="this"/>, <paramref name="digits"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static decimal Round(this decimal @this, int digits)
		=> Math.Round(@this, digits);

	/// <inheritdoc cref="Math.Round(decimal, MidpointRounding)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Math"/>.Round(@<paramref name="this"/>, <paramref name="rounding"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static decimal Round(this decimal @this, MidpointRounding rounding)
		=> Math.Round(@this, rounding);

	/// <inheritdoc cref="Math.Round(decimal, int, MidpointRounding)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Math"/>.Round(@<paramref name="this"/>, <paramref name="digits"/>, <paramref name="rounding"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static decimal Round(this decimal @this, int digits, MidpointRounding rounding)
		=> Math.Round(@this, digits, rounding);

	/// <inheritdoc cref="Math.Sign(sbyte)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Math"/>.Sign(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static int Sign(this sbyte @this)
		=> Math.Sign(@this);

	/// <inheritdoc cref="Math.Sign(short)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Math"/>.Sign(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static int Sign(this short @this)
		=> Math.Sign(@this);

	/// <inheritdoc cref="Math.Sign(int)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Math"/>.Sign(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static int Sign(this int @this)
		=> Math.Sign(@this);

	/// <inheritdoc cref="Math.Sign(long)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Math"/>.Sign(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static int Sign(this long @this)
		=> Math.Sign(@this);

	/// <inheritdoc cref="Math.Sign(float)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Math"/>.Sign(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static int Sign(this float @this)
		=> Math.Sign(@this);

	/// <inheritdoc cref="Math.Sign(double)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Math"/>.Sign(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static int Sign(this double @this)
		=> Math.Sign(@this);

	/// <inheritdoc cref="Math.Sign(decimal)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Math"/>.Sign(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static int Sign(this decimal @this)
		=> Math.Sign(@this);

	/// <inheritdoc cref="Math.Truncate(double)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Math"/>.Truncate(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static double Truncate(this double @this)
		=> Math.Truncate(@this);

	/// <inheritdoc cref="Math.Truncate(decimal)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Math"/>.Truncate(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static decimal Truncate(this decimal @this)
		=> Math.Truncate(@this);
}
