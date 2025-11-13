// Copyright (c) 2021 Samuel Abraham

namespace TypeCache.Extensions;

public static class ComparableExtensions
{
	extension<T>(IComparable<T> @this)
	{
		/// <summary>
		/// <c>=&gt; @<paramref name="this"/>.CompareTo(<paramref name="value"/>) <see langword="is"/> 0;</c>
		/// </summary>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public bool EqualTo(T value)
			=> @this.CompareTo(value) is 0;

		/// <summary>
		/// <c>=&gt; @<paramref name="this"/>.CompareTo(<paramref name="value"/>) &gt; 0;</c>
		/// </summary>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public bool GreaterThan(T value)
			=> @this.CompareTo(value) > 0;

		/// <summary>
		/// <c>=&gt; @<paramref name="this"/>.CompareTo(<paramref name="value"/>) &gt;= 0;</c>
		/// </summary>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public bool GreaterThanOrEqualTo(T value)
			=> @this.CompareTo(value) >= 0;

		/// <summary>
		/// <c>=&gt; @<paramref name="this"/>.CompareTo(<paramref name="value"/>) &lt; 0;</c>
		/// </summary>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public bool LessThan(T value)
			=> @this.CompareTo(value) < 0;

		/// <summary>
		/// <c>=&gt; @<paramref name="this"/>.CompareTo(<paramref name="value"/>) &lt;= 0;</c>
		/// </summary>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public bool LessThanOrEqualTo(T value)
			=> @this.CompareTo(value) <= 0;
	}

	extension(IComparable @this)
	{
		/// <summary>
		/// <c>=&gt; @<paramref name="this"/>.CompareTo(<paramref name="value"/>) <see langword="is"/> 0;</c>
		/// </summary>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public bool EqualTo(object value)
			=> @this.CompareTo(value) is 0;

		/// <summary>
		/// <c>=&gt; @<paramref name="this"/>.CompareTo(<paramref name="value"/>) &gt; 0;</c>
		/// </summary>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public bool GreaterThan(object value)
			=> @this.CompareTo(value) > 0;

		/// <summary>
		/// <c>=&gt; @<paramref name="this"/>.CompareTo(<paramref name="value"/>) &gt;= 0;</c>
		/// </summary>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public bool GreaterThanOrEqualTo(object value)
			=> @this.CompareTo(value) >= 0;

		/// <summary>
		/// <c>=&gt; @<paramref name="this"/>.CompareTo(<paramref name="value"/>) &lt; 0;</c>
		/// </summary>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public bool LessThan(object value)
			=> @this.CompareTo(value) < 0;

		/// <summary>
		/// <c>=&gt; @<paramref name="this"/>.CompareTo(<paramref name="value"/>) &lt;= 0;</c>
		/// </summary>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public bool LessThanOrEqualTo(object value)
			=> @this.CompareTo(value) <= 0;
	}
}
