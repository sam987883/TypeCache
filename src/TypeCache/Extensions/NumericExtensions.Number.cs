using System.Numerics;

namespace TypeCache.Extensions;

public static partial class NumericExtensions
{
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

	/// <inheritdoc cref="INumber{TSelf}.Sign(TSelf)"/>
	/// <remarks>
	/// <c>=&gt; <typeparamref name="T"/>.Sign(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static int Sign<T>(this T @this)
		where T : INumber<T>
		=> T.Sign(@this);
}
