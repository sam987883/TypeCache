// Copyright (c) 2021 Samuel Abraham

using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace TypeCache.Collections.Extensions
{
	public static class IComparerExtensions
	{
		/// <summary>
		/// <c><see cref="IComparer.Compare(object?, object?)"/> == 0</c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Same(this IComparer @this, object value1, object value2)
			=> @this.Compare(value1, value2) == 0;

		/// <summary>
		/// <c><see cref="IComparer{T}.Compare(T, T)"/> == 0</c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Same<T>(this IComparer<T> @this, T value1, T value2)
			=> @this.Compare(value1, value2) == 0;

		/// <summary>
		/// <c><see cref="IComparer.Compare(object?, object?)"/> &lt; 0</c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool LessThan(this IComparer @this, object value1, object value2)
			=> @this.Compare(value1, value2) < 0;

		/// <summary>
		/// <c><see cref="IComparer{T}.Compare(T, T)"/> &lt; 0</c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool LessThan<T>(this IComparer<T> @this, T value1, T value2)
			=> @this.Compare(value1, value2) < 0;

		/// <summary>
		/// <c><see cref="IComparer.Compare(object?, object?)"/> &lt;= 0</c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool LessThanOrEqual(this IComparer @this, object value1, object value2)
			=> @this.Compare(value1, value2) <= 0;

		/// <summary>
		/// <c><see cref="IComparer{T}.Compare(T, T)"/> &lt;= 0</c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool LessThanOrEqual<T>(this IComparer<T> @this, T value1, T value2)
			=> @this.Compare(value1, value2) <= 0;

		/// <summary>
		/// <c><see cref="IComparer.Compare(object?, object?)"/> &gt; 0</c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool MoreThan(this IComparer @this, object value1, object value2)
			=> @this.Compare(value1, value2) > 0;

		/// <summary>
		/// <c><see cref="IComparer{T}.Compare(T, T)"/> &gt; 0</c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool MoreThan<T>(this IComparer<T> @this, T value1, T value2)
			=> @this.Compare(value1, value2) > 0;

		/// <summary>
		/// <c><see cref="IComparer.Compare(object?, object?)"/> &gt;= 0</c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool MoreThanOrEqual(this IComparer @this, object value1, object value2)
			=> @this.Compare(value1, value2) >= 0;

		/// <summary>
		/// <c><see cref="IComparer{T}.Compare(T, T)"/> &gt;= 0</c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool MoreThanOrEqual<T>(this IComparer<T> @this, T value1, T value2)
			=> @this.Compare(value1, value2) >= 0;

		/// <summary>
		/// <c><see cref="IComparer.Compare(object?, object?)"/> &gt; 0 ? <paramref name="value1"/> : <paramref name="value2"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static object Maximum(this IComparer @this, object value1, object value2)
			=> @this.MoreThan(value1, value2) ? value1 : value2;

		/// <summary>
		/// <c><see cref="IComparer{T}.Compare(T, T)"/> &gt; 0 ? <paramref name="value1"/> : <paramref name="value2"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T Maximum<T>(this IComparer<T> @this, T value1, T value2)
			=> @this.MoreThan(value1, value2) ? value1 : value2;

		/// <summary>
		/// <c><see cref="IComparer.Compare(object?, object?)"/> &lt; 0 ? <paramref name="value1"/> : <paramref name="value2"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static object Minimum(this IComparer @this, object value1, object value2)
			=> @this.LessThan(value1, value2) ? value1 : value2;

		/// <summary>
		/// <c><see cref="IComparer{T}.Compare(T, T)"/> &lt; 0 ? <paramref name="value1"/> : <paramref name="value2"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T Minimum<T>(this IComparer<T> @this, T value1, T value2)
			=> @this.LessThan(value1, value2) ? value1 : value2;
	}
}
