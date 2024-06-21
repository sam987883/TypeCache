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
	public static T Abs<T>(this T @this)
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

	/// <exception cref="ArgumentOutOfRangeException"/>
	public static ulong Factorial(this int @this)
	{
		(@this >= 0).ThrowIfFalse();

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

	/// <inheritdoc cref="INumberBase{TSelf}.IsCanonical(TSelf)"/>
	/// <remarks>
	/// <c>=&gt; <typeparamref name="T"/>.IsCanonical(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool IsCanonical<T>(this T @this)
		where T : INumberBase<T>
		=> T.IsCanonical(@this);

	/// <inheritdoc cref="INumberBase{TSelf}.IsComplexNumber(TSelf)"/>
	/// <remarks>
	/// <c>=&gt; <typeparamref name="T"/>.IsComplexNumber(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool IsComplexNumber<T>(this T @this)
		where T : INumberBase<T>
		=> T.IsComplexNumber(@this);

	/// <inheritdoc cref="INumberBase{TSelf}.IsEvenInteger(TSelf)"/>
	/// <remarks>
	/// <c>=&gt; <typeparamref name="T"/>.IsEvenInteger(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool IsEvenInteger<T>(this T @this)
		where T : INumberBase<T>
		=> T.IsEvenInteger(@this);

	/// <inheritdoc cref="INumberBase{TSelf}.IsFinite(TSelf)"/>
	/// <remarks>
	/// <c>=&gt; <typeparamref name="T"/>.IsFinite(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool IsFinite<T>(this T @this)
		where T : INumberBase<T>
		=> T.IsFinite(@this);

	/// <inheritdoc cref="INumberBase{TSelf}.IsImaginaryNumber(TSelf)"/>
	/// <remarks>
	/// <c>=&gt; <typeparamref name="T"/>.IsImaginaryNumber(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool IsImaginaryNumber<T>(this T @this)
		where T : INumberBase<T>
		=> T.IsImaginaryNumber(@this);

	/// <inheritdoc cref="INumberBase{TSelf}.IsInfinity(TSelf)"/>
	/// <remarks>
	/// <c>=&gt; <typeparamref name="T"/>.IsInfinity(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool IsInfinity<T>(this T @this)
		where T : INumberBase<T>
		=> T.IsInfinity(@this);

	/// <inheritdoc cref="INumberBase{TSelf}.IsInteger(TSelf)"/>
	/// <remarks>
	/// <c>=&gt; <typeparamref name="T"/>.IsInteger(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool IsInteger<T>(this T @this)
		where T : INumberBase<T>
		=> T.IsInteger(@this);

	/// <inheritdoc cref="INumberBase{TSelf}.IsNaN(TSelf)"/>
	/// <remarks>
	/// <c>=&gt; <typeparamref name="T"/>.IsNaN(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool IsNaN<T>(this T @this)
		where T : INumberBase<T>
		=> T.IsNaN(@this);

	/// <inheritdoc cref="INumberBase{TSelf}.IsNegative(TSelf)"/>
	/// <remarks>
	/// <c>=&gt; <typeparamref name="T"/>.IsNegative(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool IsNegative<T>(this T @this)
		where T : INumberBase<T>
		=> T.IsNegative(@this);

	/// <inheritdoc cref="INumberBase{TSelf}.IsNegativeInfinity(TSelf)"/>
	/// <remarks>
	/// <c>=&gt; <typeparamref name="T"/>.IsNegativeInfinity(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool IsNegativeInfinity<T>(this T @this)
		where T : INumberBase<T>
		=> T.IsNegativeInfinity(@this);

	/// <inheritdoc cref="INumberBase{TSelf}.IsNormal(TSelf)"/>
	/// <remarks>
	/// <c>=&gt; <typeparamref name="T"/>.IsNormal(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool IsNormal<T>(this T @this)
		where T : INumberBase<T>
		=> T.IsNormal(@this);

	/// <inheritdoc cref="INumberBase{TSelf}.IsOddInteger(TSelf)"/>
	/// <remarks>
	/// <c>=&gt; <typeparamref name="T"/>.IsOddInteger(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool IsOddInteger<T>(this T @this)
		where T : INumberBase<T>
		=> T.IsOddInteger(@this);

	/// <inheritdoc cref="INumberBase{TSelf}.IsPositive(TSelf)"/>
	/// <remarks>
	/// <c>=&gt; <typeparamref name="T"/>.IsPositive(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool IsPositive<T>(this T @this)
		where T : INumberBase<T>
		=> T.IsPositive(@this);

	/// <inheritdoc cref="INumberBase{TSelf}.IsPositiveInfinity(TSelf)"/>
	/// <remarks>
	/// <c>=&gt; <typeparamref name="T"/>.IsPositiveInfinity(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool IsPositiveInfinity<T>(this T @this)
		where T : INumberBase<T>
		=> T.IsPositiveInfinity(@this);

	/// <inheritdoc cref="INumberBase{TSelf}.IsRealNumber(TSelf)"/>
	/// <remarks>
	/// <c>=&gt; <typeparamref name="T"/>.IsRealNumber(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool IsRealNumber<T>(this T @this)
		where T : INumberBase<T>
		=> T.IsRealNumber(@this);

	/// <inheritdoc cref="INumberBase{TSelf}.IsSubnormal(TSelf)"/>
	/// <remarks>
	/// <c>=&gt; <typeparamref name="T"/>.IsSubnormal(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool IsSubnormal<T>(this T @this)
		where T : INumberBase<T>
		=> T.IsSubnormal(@this);

	/// <inheritdoc cref="INumberBase{TSelf}.IsZero(TSelf)"/>
	/// <remarks>
	/// <c>=&gt; <typeparamref name="T"/>.IsZero(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool IsZero<T>(this T @this)
		where T : INumberBase<T>
		=> T.IsZero(@this);

	/// <inheritdoc cref="INumber{TSelf}.Max(TSelf, TSelf)"/>
	/// <remarks>
	/// <c>=&gt; <typeparamref name="T"/>.Max(@<paramref name="this"/>.Item1, @<paramref name="this"/>.Item2);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static T Max<T>(this (T, T) @this)
		where T : INumber<T>
		=> T.Max(@this.Item1, @this.Item2);

	/// <inheritdoc cref="INumber{TSelf}.Min(TSelf, TSelf)"/>
	/// <remarks>
	/// <c>=&gt; <typeparamref name="T"/>.Min(@<paramref name="this"/>.Item1, @<paramref name="this"/>.Item2);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static T Min<T>(this (T, T) @this)
		where T : INumber<T>
		=> T.Min(@this.Item1, @this.Item2);

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

	/// <inheritdoc cref="decimal.GetBits(decimal)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="decimal"/>.GetBits(@<paramref name="this"/>).SelectMany(_ =&gt; _.GetBytes()).ToArray();</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static byte[] ToBytes(this decimal @this)
		=> decimal.GetBits(@this).SelectMany(_ => _.GetBytes()).ToArray();

	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.ToString("D", <paramref name="provider"/> ?? <see cref="InvariantCulture"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static string ToText(this Guid @this, IFormatProvider? provider = null)
		=> @this.ToString("D", provider ?? InvariantCulture);

	/// <inheritdoc cref="IFloatingPoint{TSelf}.Truncate(TSelf)"/>
	/// <remarks>
	/// <c>=&gt; <typeparamref name="T"/>.Truncate(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static T Truncate<T>(this T @this)
		where T : IFloatingPoint<T>
		=> T.Truncate(@this);
}
