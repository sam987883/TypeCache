// Copyright (c) 2021 Samuel Abraham

using System.Numerics;
using TypeCache.Extensions;
using static System.Globalization.CultureInfo;

namespace TypeCache.Extensions;

public static class NumberExtensions
{
	/// <inheritdoc cref="INumberBase{TSelf}.Abs(TSelf)"/>
	/// <remarks>
	/// <c>=&gt; <typeparamref name="T"/>.Abs(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static T AbsoluteValue<T>(this T @this)
		where T : INumberBase<T>
		=> T.Abs(@this);

	/// <inheritdoc cref="IFloatingPointIeee754{TSelf}.BitDecrement(TSelf)"/>
	/// <remarks>
	/// <c>=&gt; <typeparamref name="T"/>.BitDecrement(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static T BitDecrement<T>(this T @this)
		where T : IFloatingPointIeee754<T>
		=> T.BitDecrement(@this);

	/// <inheritdoc cref="IFloatingPointIeee754{TSelf}.BitIncrement(TSelf)"/>
	/// <remarks>
	/// <c>=&gt; <typeparamref name="T"/>.BitIncrement(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static T BitIncrement<T>(this T @this)
		where T : IFloatingPointIeee754<T>
		=> T.BitIncrement(@this);

	/// <inheritdoc cref="IFloatingPoint{TSelf}.Ceiling(TSelf)"/>
	/// <remarks>
	/// <c>=&gt; <typeparamref name="T"/>.Ceiling(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static T Ceiling<T>(this T @this)
		where T : IFloatingPoint<T>
		=> T.Ceiling(@this);

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

	/// <inheritdoc cref="IFloatingPoint{TSelf}.Floor(TSelf)"/>
	/// <remarks>
	/// <c>=&gt; <typeparamref name="T"/>.Floor(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static T Floor<T>(this T @this)
		where T : IFloatingPoint<T>
		=> T.Floor(@this);

	/// <inheritdoc cref="INumber{TSelf}.Max(TSelf, TSelf)"/>
	/// <remarks>
	/// <c>=&gt; <typeparamref name="T"/>.Max(@<paramref name="this"/>.Item1, @<paramref name="this"/>.Item2);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static T Maximum<T>(this (T, T) @this)
		where T : INumber<T>
		=> T.Max(@this.Item1, @this.Item2);

	/// <inheritdoc cref="INumber{TSelf}.Max(TSelf, TSelf)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="int"/>.Max(@<paramref name="this"/>.Item1.Value, @<paramref name="this"/>.Item2.Value);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static Index Maximum(this (Index, Index) @this)
		=> (@this.Item1.Value, @this.Item2.Value).Maximum();

	/// <inheritdoc cref="INumber{TSelf}.Min(TSelf, TSelf)"/>
	/// <remarks>
	/// <c>=&gt; <typeparamref name="T"/>.Min(@<paramref name="this"/>.Item1, @<paramref name="this"/>.Item2);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static T Minimum<T>(this (T, T) @this)
		where T : INumber<T>
		=> T.Min(@this.Item1, @this.Item2);

	/// <inheritdoc cref="INumber{TSelf}.Min(TSelf, TSelf)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="int"/>.Min(@<paramref name="this"/>.Item1.Value, @<paramref name="this"/>.Item2.Value);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static Index Minimum(this (Index, Index) @this)
		=> (@this.Item1.Value, @this.Item2.Value).Minimum();

	public static IEnumerable<T> Repeat<T>(this T @this, int count)
		where T : unmanaged
	{
		while (--count > -1)
			yield return @this;
	}

	/// <inheritdoc cref="IFloatingPoint{TSelf}.Round(TSelf)"/>
	/// <remarks>
	/// <c>=&gt; <typeparamref name="T"/>.Round(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static T Round<T>(this T @this)
		where T : IFloatingPoint<T>
		=> T.Round(@this);

	/// <inheritdoc cref="IFloatingPoint{TSelf}.Round(TSelf, int)"/>
	/// <remarks>
	/// <c>=&gt; <typeparamref name="T"/>.Round(@<paramref name="this"/>, <paramref name="digits"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static T Round<T>(this T @this, int digits)
		where T : IFloatingPoint<T>
		=> T.Round(@this, digits);

	/// <inheritdoc cref="IFloatingPoint{TSelf}.Round(TSelf, MidpointRounding)"/>
	/// <remarks>
	/// <c>=&gt; <typeparamref name="T"/>.Round(@<paramref name="this"/>, <paramref name="rounding"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static T Round<T>(this T @this, MidpointRounding rounding)
		where T : IFloatingPoint<T>
		=> T.Round(@this, rounding);

	/// <inheritdoc cref="IFloatingPoint{TSelf}.Round(TSelf, int, MidpointRounding)"/>
	/// <remarks>
	/// <c>=&gt; <typeparamref name="T"/>.Round(@<paramref name="this"/>, <paramref name="digits"/>, <paramref name="rounding"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static T Round<T>(this T @this, int digits, MidpointRounding rounding)
		where T : IFloatingPoint<T>
		=> T.Round(@this, digits, rounding);

	/// <inheritdoc cref="INumber{TSelf}.Sign(TSelf)"/>
	/// <remarks>
	/// <c>=&gt; <typeparamref name="T"/>.Sign(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static int Sign<T>(this T @this)
		where T : INumber<T>
		=> T.Sign(@this);

	/// <remarks>
	/// <c>=&gt; (@<paramref name="this"/>, <paramref name="value"/>) = (<paramref name="value"/>, @<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static void Swap<T>(this ref T @this, ref T value)
		where T : struct
		=> (@this, value) = (value, @this);

	/// <remarks>
	/// <c>=&gt; (@<paramref name="this"/>.Item2, @<paramref name="this"/>.Item1);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static (T, T) Swap<T>(this (T, T) @this)
		=> (@this.Item2, @this.Item1);

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

	/// <inheritdoc cref="decimal.GetBits(decimal)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="decimal"/>.GetBits(@<paramref name="this"/>).SelectMany(i => i.GetBytes()).ToArray();</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static byte[] ToBytes(this decimal @this)
		=> decimal.GetBits(@this).SelectMany(_ => _.GetBytes()).ToArray();

	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.ToString("O", <see cref="InvariantCulture"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static string ToISO8601(this DateOnly @this, IFormatProvider? provider = null)
		=> @this.ToString("O", provider ?? InvariantCulture);

	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.ToString("O", <see cref="InvariantCulture"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static string ToISO8601(this DateTime @this, IFormatProvider? provider = null)
		=> @this.ToString("O", provider ?? InvariantCulture);

	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.ToString("O", <see cref="InvariantCulture"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static string ToISO8601(this DateTimeOffset @this, IFormatProvider? provider = null)
		=> @this.ToString("O", provider ?? InvariantCulture);

	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.ToString("O", <see cref="InvariantCulture"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static string ToISO8601(this TimeOnly @this, IFormatProvider? provider = null)
		=> @this.ToString("O", provider ?? InvariantCulture);

	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.ToString("D", <see cref="InvariantCulture"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static string ToText(this Guid @this, IFormatProvider? provider = null)
		=> @this.ToString("D", provider ?? InvariantCulture);

	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.ToString("c", <see cref="InvariantCulture"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static string ToText(this TimeSpan @this, IFormatProvider? provider = null)
		=> @this.ToString("c", provider ?? InvariantCulture);

	/// <inheritdoc cref="IFloatingPoint{TSelf}.Truncate(TSelf)"/>
	/// <remarks>
	/// <c>=&gt; <typeparamref name="T"/>.Truncate(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static T Truncate<T>(this T @this)
		where T : IFloatingPoint<T>
		=> T.Truncate(@this);
}
