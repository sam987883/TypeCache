using System.Numerics;

namespace TypeCache.Extensions;

public static partial class NumericExtensions
{
	/// <inheritdoc cref="IFloatingPoint{TSelf}.Ceiling(TSelf)"/>
	/// <remarks>
	/// <c>=&gt; <typeparamref name="T"/>.Ceiling(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static T Ceiling<T>(this T @this)
		where T : IFloatingPoint<T>
		=> T.Ceiling(@this);

	/// <inheritdoc cref="IFloatingPoint{TSelf}.Floor(TSelf)"/>
	/// <remarks>
	/// <c>=&gt; <typeparamref name="T"/>.Floor(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static T Floor<T>(this T @this)
		where T : IFloatingPoint<T>
		=> T.Floor(@this);

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

	/// <inheritdoc cref="IFloatingPoint{TSelf}.Truncate(TSelf)"/>
	/// <remarks>
	/// <c>=&gt; <typeparamref name="T"/>.Truncate(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static T Truncate<T>(this T @this)
		where T : IFloatingPoint<T>
		=> T.Truncate(@this);
}
