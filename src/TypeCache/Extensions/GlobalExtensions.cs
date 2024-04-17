// Copyright (c) 2021 Samuel Abraham

using System.Linq.Expressions;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace TypeCache.Extensions;

public static class GlobalExtensions
{
	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/> &gt;= <paramref name="minimum"/> &amp;&amp; @<paramref name="this"/> &lt;= <paramref name="maximum"/>;</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool Between<T>(this T @this, T minimum, T maximum)
		where T : IComparisonOperators<T, T, bool>
		=> @this >= minimum && @this <= maximum;

	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/> &gt; <paramref name="minimum"/> &amp;&amp; @<paramref name="this"/> &lt; <paramref name="maximum"/>;</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool InBetween<T>(this T @this, T minimum, T maximum)
		where T : IComparisonOperators<T, T, bool>
		=> @this > minimum && @this < maximum;

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

	/// <inheritdoc cref="Expression.Constant(object?, Type)"/>
	public static ConstantExpression ToConstantExpression<T>(this T? @this)
		=> @this is not null ? Expression.Constant(@this, @this.GetType()) : Expression.Constant(null);

	/// <inheritdoc cref="StrongBox{T}.StrongBox(T)"/>
	/// <remarks>
	/// <c>=&gt; <see langword="new"/>(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static StrongBox<T> ToStrongBox<T>(this T @this)
		where T : struct
		=> new(@this);

	/// <inheritdoc cref="Tuple.Create{T1}(T1)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Tuple"/>.Create(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static Tuple<T> ToTuple<T>(this T @this)
		=> Tuple.Create(@this);

	/// <inheritdoc cref="ValueTuple.Create{T1}(T1)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="ValueTuple"/>.Create(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static ValueTuple<T> ToValueTuple<T>(this T @this)
		=> ValueTuple.Create(@this);

	/// <inheritdoc cref="ToWeakReference{T}(T)"/>
	/// <remarks>
	/// <c>=&gt; <see langword="new"/>(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static WeakReference<T> ToWeakReference<T>(this T @this)
		where T : class
		=> new(@this);
}
