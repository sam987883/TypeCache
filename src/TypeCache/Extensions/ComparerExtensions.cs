// Copyright (c) 2021 Samuel Abraham

using System.Collections;

namespace TypeCache.Extensions;

public static class ComparerExtensions
{
	extension<T>(IComparer<T> @this)
	{
		/// <summary>
		/// <c>=&gt; @this.Compare(<paramref name="value1"/>, <paramref name="value2"/>) == 0;</c>
		/// </summary>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public bool EqualTo(T value1, T value2)
			=> @this.Compare(value1, value2) == 0;

		/// <summary>
		/// <c>=&gt; @this.Compare(<paramref name="value1"/>, <paramref name="value2"/>) &gt; 0;</c>
		/// </summary>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public bool GreaterThan(T value1, T value2)
			=> @this.Compare(value1, value2) > 0;

		/// <summary>
		/// <c>=&gt; @this.Compare(<paramref name="value1"/>, <paramref name="value2"/>) &gt;= 0;</c>
		/// </summary>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public bool GreaterThanOrEqualTo(T value1, T value2)
			=> @this.Compare(value1, value2) >= 0;

		/// <summary>
		/// <c>=&gt; @this.Compare(<paramref name="value1"/>, <paramref name="value2"/>) &lt; 0;</c>
		/// </summary>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public bool LessThan(T value1, T value2)
			=> @this.Compare(value1, value2) < 0;

		/// <summary>
		/// <c>=&gt; @this.Compare(<paramref name="value1"/>, <paramref name="value2"/>) &lt;= 0;</c>
		/// </summary>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public bool LessThanOrEqualTo(T value1, T value2)
			=> @this.Compare(value1, value2) <= 0;

		/// <summary>
		/// <c>=&gt; @this.GreaterThan(<paramref name="value1"/>, <paramref name="value2"/>) ? <paramref name="value1"/> : <paramref name="value2"/>;</c>
		/// </summary>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public T Maximum(T value1, T value2)
			=> @this.GreaterThan(value1, value2) ? value1 : value2;

		/// <summary>
		/// <c>=&gt; @this.LessThan(<paramref name="value1"/>, <paramref name="value2"/>) ? <paramref name="value1"/> : <paramref name="value2"/>;</c>
		/// </summary>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public T Minimum(T value1, T value2)
			=> @this.LessThan(value1, value2) ? value1 : value2;
	}

	extension(IComparer @this)
	{
		/// <summary>
		/// <c>=&gt; @this.Compare(<paramref name="value1"/>, <paramref name="value2"/>) == 0;</c>
		/// </summary>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public bool EqualTo(object value1, object value2)
			=> @this.Compare(value1, value2) == 0;

		/// <summary>
		/// <c>=&gt; @this.Compare(<paramref name="value1"/>, <paramref name="value2"/>) &gt; 0;</c>
		/// </summary>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public bool GreaterThan(object value1, object value2)
			=> @this.Compare(value1, value2) > 0;

		/// <summary>
		/// <c>=&gt; @this.Compare(<paramref name="value1"/>, <paramref name="value2"/>) &gt;= 0;</c>
		/// </summary>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public bool GreaterThanOrEqualTo(object value1, object value2)
			=> @this.Compare(value1, value2) >= 0;

		/// <summary>
		/// <c>=&gt; @this.Compare(<paramref name="value1"/>, <paramref name="value2"/>) &lt; 0;</c>
		/// </summary>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public bool LessThan(object value1, object value2)
			=> @this.Compare(value1, value2) < 0;

		/// <summary>
		/// <c>=&gt; @this.Compare(<paramref name="value1"/>, <paramref name="value2"/>) &lt;= 0;</c>
		/// </summary>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public bool LessThanOrEqualTo(object value1, object value2)
			=> @this.Compare(value1, value2) <= 0;

		/// <summary>
		/// <c>=&gt; @this.GreaterThan(<paramref name="value1"/>, <paramref name="value2"/>) ? <paramref name="value1"/> : <paramref name="value2"/>;</c>
		/// </summary>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public object Maximum(object value1, object value2)
			=> @this.GreaterThan(value1, value2) ? value1 : value2;

		/// <summary>
		/// <c>=&gt; @this.LessThan(<paramref name="value1"/>, <paramref name="value2"/>) ? <paramref name="value1"/> : <paramref name="value2"/>;</c>
		/// </summary>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public object Minimum(object value1, object value2)
			=> @this.LessThan(value1, value2) ? value1 : value2;
	}
}
