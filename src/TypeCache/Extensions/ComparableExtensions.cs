// Copyright (c) 2021 Samuel Abraham

using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using static TypeCache.Default;

namespace TypeCache.Extensions;

public static class ComparableExtensions
{
	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.GreaterThanOrEqualTo(<paramref name="value1"/>) &amp;&amp; @<paramref name="this"/>.LessThanOrEqualTo(<paramref name="value2"/>);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static bool Between(this IComparable @this, object value1, object value2)
		=> @this.GreaterThanOrEqualTo(value1) && @this.LessThanOrEqualTo(value2);

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.GreaterThanOrEqualTo(<paramref name="value1"/>) &amp;&amp; @<paramref name="this"/>.LessThanOrEqualTo(<paramref name="value2"/>);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static bool Between<T>(this IComparable<T> @this, T value1, T value2)
		=> @this.GreaterThanOrEqualTo(value1) && @this.LessThanOrEqualTo(value2);

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.CompareTo(<paramref name="value"/>) == 0;</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static bool EqualTo(this IComparable @this, object value)
		=> @this.CompareTo(value) == 0;

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.CompareTo(<paramref name="value"/>) == 0;</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static bool EqualTo<T>(this IComparable<T> @this, T value)
		=> @this.CompareTo(value) == 0;

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.CompareTo(<paramref name="value"/>) &gt; 0;</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static bool GreaterThan(this IComparable @this, object value)
		=> @this.CompareTo(value) > 0;

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.CompareTo(<paramref name="value"/>) &gt; 0;</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static bool GreaterThan<T>(this IComparable<T> @this, T value)
		=> @this.CompareTo(value) > 0;

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.CompareTo(<paramref name="value"/>) &gt;= 0;</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static bool GreaterThanOrEqualTo(this IComparable @this, object value)
		=> @this.CompareTo(value) >= 0;

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.CompareTo(<paramref name="value"/>) &gt;= 0;</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static bool GreaterThanOrEqualTo<T>(this IComparable<T> @this, T value)
		=> @this.CompareTo(value) >= 0;

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.GreaterThan(<paramref name="value1"/>) &amp;&amp; @<paramref name="this"/>.LessThan(<paramref name="value2"/>)</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static bool InBetween(this IComparable @this, object value1, object value2)
		=> @this.GreaterThan(value1) && @this.LessThan(value2);

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.GreaterThan(<paramref name="value1"/>) &amp;&amp; @<paramref name="this"/>.LessThan(<paramref name="value2"/>)</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static bool InBetween<T>(this IComparable<T> @this, T value1, T value2)
		=> @this.GreaterThan(value1) && @this.LessThan(value2);

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.CompareTo(<paramref name="value"/>) &lt; 0;</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static bool LessThan(this IComparable @this, object value)
		=> @this.CompareTo(value) < 0;

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.CompareTo(<paramref name="value"/>) &lt; 0;</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static bool LessThan<T>(this IComparable<T> @this, T value)
		=> @this.CompareTo(value) < 0;

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.CompareTo(<paramref name="value"/>) &lt;= 0;</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static bool LessThanOrEqualTo(this IComparable @this, object value)
		=> @this.CompareTo(value) <= 0;

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.CompareTo(<paramref name="value"/>) &lt;= 0;</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static bool LessThanOrEqualTo<T>(this IComparable<T> @this, T value)
		=> @this.CompareTo(value) <= 0;
}
