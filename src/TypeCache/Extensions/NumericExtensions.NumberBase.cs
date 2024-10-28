using System.Numerics;

namespace TypeCache.Extensions;

public static partial class NumericExtensions
{
	/// <inheritdoc cref="INumberBase{TSelf}.Abs(TSelf)"/>
	/// <remarks>
	/// <c>=&gt; <typeparamref name="T"/>.Abs(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static T Abs<T>(this T @this)
		where T : INumberBase<T>
		=> T.Abs(@this);

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
}
