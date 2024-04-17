// Copyright (c) 2021 Samuel Abraham

namespace TypeCache.Extensions;

public static class ComparableExtensions
{
	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.CompareTo(<paramref name="value"/>) == 0;</c>
	/// </summary>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool EqualTo(this IComparable @this, object value)
		=> @this.CompareTo(value) == 0;

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.CompareTo(<paramref name="value"/>) == 0;</c>
	/// </summary>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool EqualTo<T>(this IComparable<T> @this, T value)
		=> @this.CompareTo(value) == 0;

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.CompareTo(<paramref name="value"/>) &gt; 0;</c>
	/// </summary>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool GreaterThan(this IComparable @this, object value)
		=> @this.CompareTo(value) > 0;

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.CompareTo(<paramref name="value"/>) &gt; 0;</c>
	/// </summary>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool GreaterThan<T>(this IComparable<T> @this, T value)
		=> @this.CompareTo(value) > 0;

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.CompareTo(<paramref name="value"/>) &gt;= 0;</c>
	/// </summary>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool GreaterThanOrEqualTo(this IComparable @this, object value)
		=> @this.CompareTo(value) >= 0;

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.CompareTo(<paramref name="value"/>) &gt;= 0;</c>
	/// </summary>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool GreaterThanOrEqualTo<T>(this IComparable<T> @this, T value)
		=> @this.CompareTo(value) >= 0;

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.CompareTo(<paramref name="value"/>) &lt; 0;</c>
	/// </summary>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool LessThan(this IComparable @this, object value)
		=> @this.CompareTo(value) < 0;

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.CompareTo(<paramref name="value"/>) &lt; 0;</c>
	/// </summary>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool LessThan<T>(this IComparable<T> @this, T value)
		=> @this.CompareTo(value) < 0;

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.CompareTo(<paramref name="value"/>) &lt;= 0;</c>
	/// </summary>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool LessThanOrEqualTo(this IComparable @this, object value)
		=> @this.CompareTo(value) <= 0;

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.CompareTo(<paramref name="value"/>) &lt;= 0;</c>
	/// </summary>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool LessThanOrEqualTo<T>(this IComparable<T> @this, T value)
		=> @this.CompareTo(value) <= 0;
}
