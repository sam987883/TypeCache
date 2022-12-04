// Copyright (c) 2021 Samuel Abraham

using System.Collections;

namespace TypeCache.Extensions;

public static class ComparerExtensions
{
	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.Compare(<paramref name="value1"/>, <paramref name="value2"/>) == 0;</c>
	/// </summary>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool EqualTo(this IComparer @this, object value1, object value2)
		=> @this.Compare(value1, value2) == 0;

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.Compare(<paramref name="value1"/>, <paramref name="value2"/>) == 0;</c>
	/// </summary>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool EqualTo<T>(this IComparer<T> @this, T value1, T value2)
		=> @this.Compare(value1, value2) == 0;

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.Compare(<paramref name="value1"/>, <paramref name="value2"/>) &gt; 0;</c>
	/// </summary>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool GreaterThan(this IComparer @this, object value1, object value2)
		=> @this.Compare(value1, value2) > 0;

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.Compare(<paramref name="value1"/>, <paramref name="value2"/>) &gt; 0;</c>
	/// </summary>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool GreaterThan<T>(this IComparer<T> @this, T value1, T value2)
		=> @this.Compare(value1, value2) > 0;

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.Compare(<paramref name="value1"/>, <paramref name="value2"/>) &gt;= 0;</c>
	/// </summary>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool GreaterThanOrEqualTo(this IComparer @this, object value1, object value2)
		=> @this.Compare(value1, value2) >= 0;

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.Compare(<paramref name="value1"/>, <paramref name="value2"/>) &gt;= 0;</c>
	/// </summary>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool GreaterThanOrEqualTo<T>(this IComparer<T> @this, T value1, T value2)
		=> @this.Compare(value1, value2) >= 0;

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.Compare(<paramref name="value1"/>, <paramref name="value2"/>) &lt; 0;</c>
	/// </summary>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool LessThan(this IComparer @this, object value1, object value2)
		=> @this.Compare(value1, value2) < 0;

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.Compare(<paramref name="value1"/>, <paramref name="value2"/>) &lt; 0;</c>
	/// </summary>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool LessThan<T>(this IComparer<T> @this, T value1, T value2)
		=> @this.Compare(value1, value2) < 0;

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.Compare(<paramref name="value1"/>, <paramref name="value2"/>) &lt;= 0;</c>
	/// </summary>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool LessThanOrEqualTo(this IComparer @this, object value1, object value2)
		=> @this.Compare(value1, value2) <= 0;

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.Compare(<paramref name="value1"/>, <paramref name="value2"/>) &lt;= 0;</c>
	/// </summary>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool LessThanOrEqualTo<T>(this IComparer<T> @this, T value1, T value2)
		=> @this.Compare(value1, value2) <= 0;

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.GreaterThan(<paramref name="value1"/>, <paramref name="value2"/>) ? <paramref name="value1"/> : <paramref name="value2"/>;</c>
	/// </summary>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static object Maximum(this IComparer @this, object value1, object value2)
		=> @this.GreaterThan(value1, value2) ? value1 : value2;

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.GreaterThan(<paramref name="value1"/>, <paramref name="value2"/>) ? <paramref name="value1"/> : <paramref name="value2"/>;</c>
	/// </summary>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static T Maximum<T>(this IComparer<T> @this, T value1, T value2)
		=> @this.GreaterThan(value1, value2) ? value1 : value2;

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.LessThan(<paramref name="value1"/>, <paramref name="value2"/>) ? <paramref name="value1"/> : <paramref name="value2"/>;</c>
	/// </summary>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static object Minimum(this IComparer @this, object value1, object value2)
		=> @this.LessThan(value1, value2) ? value1 : value2;

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.LessThan(<paramref name="value1"/>, <paramref name="value2"/>) ? <paramref name="value1"/> : <paramref name="value2"/>;</c>
	/// </summary>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static T Minimum<T>(this IComparer<T> @this, T value1, T value2)
		=> @this.LessThan(value1, value2) ? value1 : value2;
}
