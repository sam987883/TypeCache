// Copyright (c) 2021 Samuel Abraham

using System.Linq.Expressions;
using System.Numerics;
using System.Runtime.CompilerServices;
using TypeCache.Reflection;
using TypeCache.Utilities;

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
	/// <c>=&gt; <see cref="ValueBox{T}"/>.GetValue(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static object Box<T>(this T @this)
		where T : struct
		=> ValueBox<T>.GetValue(@this);

	/// <inheritdoc cref="Expression.Constant(object?, Type)"/>
	public static ConstantExpression ConstantExpression<T>(this T? @this)
		=> @this is not null ? Expression.Constant(@this, @this.GetType()) : Expression.Constant(null);

	public static IDictionary<string, object?> Fields<T>(this T @this)
		=> @this!.GetType().Fields()
			.Select(_ => (_.Key, _.Value.GetValue(@this)))
			.ToDictionary(@this.GetType().Fields().Keys.IsCaseSensitive() ? StringComparer.Ordinal : StringComparer.OrdinalIgnoreCase);

	public static IDictionary<string, object?> Properties<T>(this T @this)
		=> @this!.GetType().Properties()
			.Where(_ => _.Value.GetMethod is MethodEntity)
			.Select(_ => (_.Key, _.Value.GetValue(@this)))
			.ToDictionary(@this.GetType().Properties().Keys.IsCaseSensitive() ? StringComparer.Ordinal : StringComparer.OrdinalIgnoreCase);

	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/> &gt; <paramref name="minimum"/> &amp;&amp; @<paramref name="this"/> &lt; <paramref name="maximum"/>;</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool InBetween<T>(this T @this, T minimum, T maximum)
		where T : IComparisonOperators<T, T, bool>
		=> @this > minimum && @this < maximum;

	public static IEnumerable<T> Repeat<T>(this T @this, int count)
	{
		while (--count > -1)
			yield return @this;
	}

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

	/// <summary>
	/// Returns the C# code of the expression.
	/// </summary>
	/// <remarks>
	/// <c>=&gt; <paramref name="code"/> ?? <see cref="string.Empty"/>;</c>
	/// </remarks>
	/// <param name="code">Leave this parameter as <c><see langword="null"/></c></param>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static string ToCode<T>(this T? @this, [AllowNull][CallerArgumentExpression(nameof(@this))] string? code = null)
		=> code ?? string.Empty;

	/// <inheritdoc cref="StrongBox{T}.StrongBox(T)"/>
	/// <remarks>
	/// <c>=&gt; <see langword="new"/>(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static StrongBox<T> ToStrongBox<T>(this T @this)
		where T : struct
		=> new(@this);

	/// <inheritdoc cref="Tuple{T1}"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Tuple"/>.Create(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static Tuple<T> ToTuple<T>(this T @this)
		=> Tuple.Create(@this);

	/// <inheritdoc cref="ValueTuple{T1}"/>
	/// <remarks>
	/// <c>=&gt; <see cref="ValueTuple"/>.Create(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static ValueTuple<T> ToValueTuple<T>(this T @this)
		=> ValueTuple.Create(@this);

	/// <inheritdoc cref="WeakReference{T}"/>
	/// <remarks>
	/// <c>=&gt; <see langword="new"/>(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static WeakReference<T> ToWeakReference<T>(this T @this)
		where T : class
		=> new(@this);
}
