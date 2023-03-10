// Copyright (c) 2021 Samuel Abraham

using TypeCache.Extensions;
using static System.Globalization.CultureInfo;

namespace TypeCache.Extensions;

public static class NumberExtensions
{
	/// <inheritdoc cref="Math.Abs(sbyte)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Math"/>.Abs(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static sbyte Abs(this sbyte @this)
		=> Math.Abs(@this);

	/// <inheritdoc cref="Math.Abs(short)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Math"/>.Abs(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static short Abs(this short @this)
		=> Math.Abs(@this);

	/// <inheritdoc cref="Math.Abs(int)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Math"/>.Abs(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static int Abs(this int @this)
		=> Math.Abs(@this);

	/// <inheritdoc cref="Math.Abs(nint)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Math"/>.Abs(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static nint Abs(this nint @this)
		=> Math.Abs(@this);

	/// <inheritdoc cref="Math.Abs(long)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Math"/>.Abs(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static long Abs(this long @this)
		=> Math.Abs(@this);

	/// <inheritdoc cref="MathF.Abs(float)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="MathF"/>.Abs(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static float Abs(this float @this)
		=> MathF.Abs(@this);

	/// <inheritdoc cref="Math.Abs(double)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Math"/>.Abs(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static double Abs(this double @this)
		=> Math.Abs(@this);

	/// <inheritdoc cref="Math.Abs(decimal)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Math"/>.Abs(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static decimal Abs(this decimal @this)
		=> Math.Abs(@this);

	/// <inheritdoc cref="MathF.BitDecrement(float)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="MathF"/>.BitDecrement(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static float BitDecrement(this float @this)
		=> MathF.BitDecrement(@this);

	/// <inheritdoc cref="Math.BitDecrement(double)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Math"/>.BitDecrement(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static double BitDecrement(this double @this)
		=> Math.BitDecrement(@this);

	/// <inheritdoc cref="MathF.BitIncrement(float)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="MathF"/>.BitIncrement(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static float BitIncrement(this float @this)
		=> MathF.BitIncrement(@this);

	/// <inheritdoc cref="Math.BitIncrement(double)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Math"/>.BitIncrement(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static double BitIncrement(this double @this)
		=> Math.BitIncrement(@this);

	/// <inheritdoc cref="MathF.Ceiling(float)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="MathF"/>.Ceiling(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static float Ceiling(this float @this)
		=> MathF.Ceiling(@this);

	/// <inheritdoc cref="Math.Ceiling(double)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Math"/>.Ceiling(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static double Ceiling(this double @this)
		=> Math.Ceiling(@this);

	/// <inheritdoc cref="Math.Ceiling(decimal)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Math"/>.Ceiling(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static decimal Ceiling(this decimal @this)
		=> Math.Ceiling(@this);

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

	/// <inheritdoc cref="MathF.Floor(float)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="MathF"/>.Floor(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static float Floor(this float @this)
		=> MathF.Floor(@this);

	/// <inheritdoc cref="Math.Floor(double)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Math"/>.Floor(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static double Floor(this double @this)
		=> Math.Floor(@this);

	/// <inheritdoc cref="Math.Floor(decimal)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Math"/>.Floor(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static decimal Floor(this decimal @this)
		=> Math.Floor(@this);

	/// <inheritdoc cref="Math.Max(sbyte, sbyte)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Math"/>.Max(@<paramref name="this"/>.Item1, @<paramref name="this"/>.Item2);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static sbyte Max(this (sbyte, sbyte) @this)
		=> Math.Max(@this.Item1, @this.Item2);

	/// <inheritdoc cref="Math.Max(short, short)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Math"/>.Max(@<paramref name="this"/>.Item1, @<paramref name="this"/>.Item2);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static short Max(this (short, short) @this)
		=> Math.Max(@this.Item1, @this.Item2);

	/// <inheritdoc cref="Math.Max(int, int)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Math"/>.Max(@<paramref name="this"/>.Item1, @<paramref name="this"/>.Item2);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static int Max(this (int, int) @this)
		=> Math.Max(@this.Item1, @this.Item2);

	/// <inheritdoc cref="Math.Max(nint, nint)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Math"/>.Max(@<paramref name="this"/>.Item1, @<paramref name="this"/>.Item2);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static nint Max(this (nint, nint) @this)
		=> Math.Max(@this.Item1, @this.Item2);

	/// <inheritdoc cref="Math.Max(long, long)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Math"/>.Max(@<paramref name="this"/>.Item1, @<paramref name="this"/>.Item2);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static long Max(this (long, long) @this)
		=> Math.Max(@this.Item1, @this.Item2);

	/// <inheritdoc cref="Math.Max(byte, byte)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Math"/>.Max(@<paramref name="this"/>.Item1, @<paramref name="this"/>.Item2);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static byte Max(this (byte, byte) @this)
		=> Math.Max(@this.Item1, @this.Item2);

	/// <inheritdoc cref="Math.Max(ushort, ushort)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Math"/>.Max(@<paramref name="this"/>.Item1, @<paramref name="this"/>.Item2);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static ushort Max(this (ushort, ushort) @this)
		=> Math.Max(@this.Item1, @this.Item2);

	/// <inheritdoc cref="Math.Max(uint, uint)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Math"/>.Max(@<paramref name="this"/>.Item1, @<paramref name="this"/>.Item2);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static uint Max(this (uint, uint) @this)
		=> Math.Max(@this.Item1, @this.Item2);

	/// <inheritdoc cref="Math.Max(nuint, nuint)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Math"/>.Max(@<paramref name="this"/>.Item1, @<paramref name="this"/>.Item2);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static nuint Max(this (nuint, nuint) @this)
		=> Math.Max(@this.Item1, @this.Item2);

	/// <inheritdoc cref="Math.Max(ulong, ulong)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Math"/>.Max(@<paramref name="this"/>.Item1, @<paramref name="this"/>.Item2);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static ulong Max(this (ulong, ulong) @this)
		=> Math.Max(@this.Item1, @this.Item2);

	/// <inheritdoc cref="MathF.Max(float, float)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="MathF"/>.Max(@<paramref name="this"/>.Item1, @<paramref name="this"/>.Item2);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static float Max(this (float, float) @this)
		=> MathF.Max(@this.Item1, @this.Item2);

	/// <inheritdoc cref="Math.Max(double, double)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Math"/>.Max(@<paramref name="this"/>.Item1, @<paramref name="this"/>.Item2);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static double Max(this (double, double) @this)
		=> Math.Max(@this.Item1, @this.Item2);

	/// <inheritdoc cref="Math.Max(decimal, decimal)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Math"/>.Max(@<paramref name="this"/>.Item1, @<paramref name="this"/>.Item2);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static decimal Max(this (decimal, decimal) @this)
		=> Math.Max(@this.Item1, @this.Item2);

	/// <remarks>
	/// <c>=&gt; (@<paramref name="this"/>.Item1.Value, @<paramref name="this"/>.Item2.Value).Max();</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static Index Max(this (Index, Index) @this)
		=> (@this.Item1.Value, @this.Item2.Value).Max();

	/// <inheritdoc cref="Math.Min(sbyte, sbyte)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Math"/>.Min(@<paramref name="this"/>.Item1, @<paramref name="this"/>.Item2);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static sbyte Min(this (sbyte, sbyte) @this)
		=> Math.Min(@this.Item1, @this.Item2);

	/// <inheritdoc cref="Math.Min(short, short)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Math"/>.Min(@<paramref name="this"/>.Item1, @<paramref name="this"/>.Item2);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static short Min(this (short, short) @this)
		=> Math.Min(@this.Item1, @this.Item2);

	/// <inheritdoc cref="Math.Min(int, int)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Math"/>.Min(@<paramref name="this"/>.Item1, @<paramref name="this"/>.Item2);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static int Min(this (int, int) @this)
		=> Math.Min(@this.Item1, @this.Item2);

	/// <inheritdoc cref="Math.Min(nint, nint)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Math"/>.Min(@<paramref name="this"/>.Item1, @<paramref name="this"/>.Item2);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static nint Min(this (nint, nint) @this)
		=> Math.Min(@this.Item1, @this.Item2);

	/// <inheritdoc cref="Math.Min(long, long)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Math"/>.Min(@<paramref name="this"/>.Item1, @<paramref name="this"/>.Item2);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static long Min(this (long, long) @this)
		=> Math.Min(@this.Item1, @this.Item2);

	/// <inheritdoc cref="Math.Min(byte, byte)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Math"/>.Min(@<paramref name="this"/>.Item1, @<paramref name="this"/>.Item2);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static byte Min(this (byte, byte) @this)
		=> Math.Min(@this.Item1, @this.Item2);

	/// <inheritdoc cref="Math.Min(ushort, ushort)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Math"/>.Min(@<paramref name="this"/>.Item1, @<paramref name="this"/>.Item2);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static ushort Min(this (ushort, ushort) @this)
		=> Math.Min(@this.Item1, @this.Item2);

	/// <inheritdoc cref="Math.Min(uint, uint)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Math"/>.Min(@<paramref name="this"/>.Item1, @<paramref name="this"/>.Item2);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static uint Min(this (uint, uint) @this)
		=> Math.Min(@this.Item1, @this.Item2);

	/// <inheritdoc cref="Math.Min(nuint, nuint)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Math"/>.Min(@<paramref name="this"/>.Item1, @<paramref name="this"/>.Item2);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static nuint Min(this (nuint, nuint) @this)
		=> Math.Min(@this.Item1, @this.Item2);

	/// <inheritdoc cref="Math.Min(ulong, ulong)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Math"/>.Min(@<paramref name="this"/>.Item1, @<paramref name="this"/>.Item2);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static ulong Min(this (ulong, ulong) @this)
		=> Math.Min(@this.Item1, @this.Item2);

	/// <inheritdoc cref="MathF.Min(float, float)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="MathF"/>.Min(@<paramref name="this"/>.Item1, @<paramref name="this"/>.Item2);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static float Min(this (float, float) @this)
		=> MathF.Min(@this.Item1, @this.Item2);

	/// <inheritdoc cref="Math.Min(double, double)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Math"/>.Min(@<paramref name="this"/>.Item1, @<paramref name="this"/>.Item2);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static double Min(this (double, double) @this)
		=> Math.Min(@this.Item1, @this.Item2);

	/// <inheritdoc cref="Math.Min(decimal, decimal)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Math"/>.Min(@<paramref name="this"/>.Item1, @<paramref name="this"/>.Item2);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static decimal Min(this (decimal, decimal) @this)
		=> Math.Min(@this.Item1, @this.Item2);

	/// <remarks>
	/// <c>=&gt; (@<paramref name="this"/>.Item1.Value, @<paramref name="this"/>.Item2.Value).Min();</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static Index Min(this (Index, Index) @this)
		=> (@this.Item1.Value, @this.Item2.Value).Min();

	public static IEnumerable<T> Repeat<T>(this T @this, int count)
		where T : unmanaged
	{
		while (--count > -1)
			yield return @this;
	}

	/// <inheritdoc cref="MathF.Round(float)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="MathF"/>.Round(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static float Round(this float @this)
		=> MathF.Round(@this);

	/// <inheritdoc cref="Math.Round(double)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Math"/>.Round(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static double Round(this double @this)
		=> Math.Round(@this);

	/// <inheritdoc cref="Math.Round(decimal)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Math"/>.Round(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static decimal Round(this decimal @this)
		=> Math.Round(@this);

	/// <inheritdoc cref="MathF.Round(float, int)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="MathF"/>.Round(@<paramref name="this"/>, <paramref name="digits"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static float Round(this float @this, int digits)
		=> MathF.Round(@this, digits);

	/// <inheritdoc cref="Math.Round(double, int)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Math"/>.Round(@<paramref name="this"/>, <paramref name="digits"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static double Round(this double @this, int digits)
		=> Math.Round(@this, digits);

	/// <inheritdoc cref="Math.Round(decimal, int)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Math"/>.Round(@<paramref name="this"/>, <paramref name="digits"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static decimal Round(this decimal @this, int digits)
		=> Math.Round(@this, digits);

	/// <inheritdoc cref="MathF.Round(float, MidpointRounding)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="MathF"/>.Round(@<paramref name="this"/>, <paramref name="rounding"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static float Round(this float @this, MidpointRounding rounding)
		=> MathF.Round(@this, rounding);

	/// <inheritdoc cref="Math.Round(double, MidpointRounding)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Math"/>.Round(@<paramref name="this"/>, <paramref name="rounding"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static double Round(this double @this, MidpointRounding rounding)
		=> Math.Round(@this, rounding);

	/// <inheritdoc cref="Math.Round(decimal, MidpointRounding)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Math"/>.Round(@<paramref name="this"/>, <paramref name="rounding"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static decimal Round(this decimal @this, MidpointRounding rounding)
		=> Math.Round(@this, rounding);

	/// <inheritdoc cref="MathF.Round(float, int, MidpointRounding)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="MathF"/>.Round(@<paramref name="this"/>, <paramref name="digits"/>, <paramref name="rounding"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static float Round(this float @this, int digits, MidpointRounding rounding)
		=> MathF.Round(@this, digits, rounding);

	/// <inheritdoc cref="Math.Round(double, int, MidpointRounding)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Math"/>.Round(@<paramref name="this"/>, <paramref name="digits"/>, <paramref name="rounding"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static double Round(this double @this, int digits, MidpointRounding rounding)
		=> Math.Round(@this, digits, rounding);

	/// <inheritdoc cref="Math.Round(decimal, int, MidpointRounding)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Math"/>.Round(@<paramref name="this"/>, <paramref name="digits"/>, <paramref name="rounding"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static decimal Round(this decimal @this, int digits, MidpointRounding rounding)
		=> Math.Round(@this, digits, rounding);

	/// <inheritdoc cref="Math.Sign(sbyte)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Math"/>.Sign(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static int Sign(this sbyte @this)
		=> Math.Sign(@this);

	/// <inheritdoc cref="Math.Sign(short)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Math"/>.Sign(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static int Sign(this short @this)
		=> Math.Sign(@this);

	/// <inheritdoc cref="Math.Sign(int)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Math"/>.Sign(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static int Sign(this int @this)
		=> Math.Sign(@this);

	/// <inheritdoc cref="Math.Sign(nint)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Math"/>.Sign(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static int Sign(this nint @this)
		=> Math.Sign(@this);

	/// <inheritdoc cref="Math.Sign(long)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Math"/>.Sign(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static int Sign(this long @this)
		=> Math.Sign(@this);

	/// <inheritdoc cref="MathF.Sign(float)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Math"/>.Sign(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static int Sign(this float @this)
		=> MathF.Sign(@this);

	/// <inheritdoc cref="Math.Sign(double)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Math"/>.Sign(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static int Sign(this double @this)
		=> Math.Sign(@this);

	/// <inheritdoc cref="Math.Sign(decimal)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Math"/>.Sign(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static int Sign(this decimal @this)
		=> Math.Sign(@this);

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
			do
			{
				yield return @this;
				@this += increment;
			} while (@this <= end);
		}
		else if (@this > end)
		{
			do
			{
				yield return @this;
				@this += increment;
			} while (@this >= end);
		}
	}

	/// <inheritdoc cref="decimal.GetBits(decimal)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="decimal"/>.GetBits(@<paramref name="this"/>).SelectMany(_ =&gt; _.GetBytes()).ToArray();</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static byte[] ToBytes(this decimal @this)
		=> decimal.GetBits(@this).SelectMany(_ => _.GetBytes()).ToArray();

	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.ToString("O", <paramref name="provider"/> ?? <see cref="InvariantCulture"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static string ToISO8601(this DateOnly @this, IFormatProvider? provider = null)
		=> @this.ToString("O", provider ?? InvariantCulture);

	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.ToString("O", <paramref name="provider"/> ?? <see cref="InvariantCulture"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static string ToISO8601(this DateTime @this, IFormatProvider? provider = null)
		=> @this.ToString("O", provider ?? InvariantCulture);

	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.ToString("O", <paramref name="provider"/> ?? <see cref="InvariantCulture"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static string ToISO8601(this DateTimeOffset @this, IFormatProvider? provider = null)
		=> @this.ToString("O", provider ?? InvariantCulture);

	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.ToString("O", <paramref name="provider"/> ?? <see cref="InvariantCulture"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static string ToISO8601(this TimeOnly @this, IFormatProvider? provider = null)
		=> @this.ToString("O", provider ?? InvariantCulture);

	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.ToString("D", <paramref name="provider"/> ?? <see cref="InvariantCulture"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static string ToText(this Guid @this, IFormatProvider? provider = null)
		=> @this.ToString("D", provider ?? InvariantCulture);

	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.ToString("c", <paramref name="provider"/> ?? <see cref="InvariantCulture"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static string ToText(this TimeSpan @this, IFormatProvider? provider = null)
		=> @this.ToString("c", provider ?? InvariantCulture);

	/// <inheritdoc cref="MathF.Truncate(float)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="MathF"/>.Truncate(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static float Truncate(this float @this)
		=> MathF.Truncate(@this);

	/// <inheritdoc cref="Math.Truncate(double)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Math"/>.Truncate(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static double Truncate(this double @this)
		=> Math.Truncate(@this);

	/// <inheritdoc cref="Math.Truncate(decimal)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Math"/>.Truncate(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static decimal Truncate(this decimal @this)
		=> Math.Truncate(@this);
}
