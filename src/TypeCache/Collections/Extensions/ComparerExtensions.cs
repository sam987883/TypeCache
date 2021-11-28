// Copyright (c) 2021 Samuel Abraham

using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using static TypeCache.Default;

namespace TypeCache.Collections.Extensions
{
	public static class ComparerExtensions
	{
		/// <summary>
		/// <c><see cref="IComparer.Compare(object?, object?)"/> == 0</c>
		/// </summary>
		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static bool EqualTo(this IComparer @this, object value1, object value2)
			=> @this.Compare(value1, value2) == 0;

		/// <summary>
		/// <c><see cref="IComparer{T}.Compare(T, T)"/> == 0</c>
		/// </summary>
		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static bool EqualTo<T>(this IComparer<T> @this, T value1, T value2)
			=> @this.Compare(value1, value2) == 0;

		/// <summary>
		/// <c><see cref="IComparer.Compare(object?, object?)"/> &lt; 0</c>
		/// </summary>
		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static bool LessThan(this IComparer @this, object value1, object value2)
			=> @this.Compare(value1, value2) < 0;

		/// <summary>
		/// <c><see cref="IComparer{T}.Compare(T, T)"/> &lt; 0</c>
		/// </summary>
		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static bool LessThan<T>(this IComparer<T> @this, T value1, T value2)
			=> @this.Compare(value1, value2) < 0;

		/// <summary>
		/// <c><see cref="IComparer.Compare(object?, object?)"/> &lt;= 0</c>
		/// </summary>
		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static bool LessThanOrEqualTo(this IComparer @this, object value1, object value2)
			=> @this.Compare(value1, value2) <= 0;

		/// <summary>
		/// <c><see cref="IComparer{T}.Compare(T, T)"/> &lt;= 0</c>
		/// </summary>
		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static bool LessThanOrEqualTo<T>(this IComparer<T> @this, T value1, T value2)
			=> @this.Compare(value1, value2) <= 0;

		/// <summary>
		/// <c><see cref="IComparer.Compare(object?, object?)"/> &gt; 0</c>
		/// </summary>
		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static bool MoreThan(this IComparer @this, object value1, object value2)
			=> @this.Compare(value1, value2) > 0;

		/// <summary>
		/// <c><see cref="IComparer{T}.Compare(T, T)"/> &gt; 0</c>
		/// </summary>
		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static bool MoreThan<T>(this IComparer<T> @this, T value1, T value2)
			=> @this.Compare(value1, value2) > 0;

		/// <summary>
		/// <c><see cref="IComparer.Compare(object?, object?)"/> &gt;= 0</c>
		/// </summary>
		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static bool MoreThanOrEqualTo(this IComparer @this, object value1, object value2)
			=> @this.Compare(value1, value2) >= 0;

		/// <summary>
		/// <c><see cref="IComparer{T}.Compare(T, T)"/> &gt;= 0</c>
		/// </summary>
		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static bool MoreThanOrEqualTo<T>(this IComparer<T> @this, T value1, T value2)
			=> @this.Compare(value1, value2) >= 0;

		/// <summary>
		/// <c><see cref="IComparer.Compare(object?, object?)"/> &gt; 0 ? <paramref name="value1"/> : <paramref name="value2"/></c>
		/// </summary>
		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static object Maximum(this IComparer @this, object value1, object value2)
			=> @this.MoreThan(value1, value2) ? value1 : value2;

		/// <summary>
		/// <c><see cref="IComparer{T}.Compare(T, T)"/> &gt; 0 ? <paramref name="value1"/> : <paramref name="value2"/></c>
		/// </summary>
		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static T Maximum<T>(this IComparer<T> @this, T value1, T value2)
			=> @this.MoreThan(value1, value2) ? value1 : value2;

		/// <summary>
		/// <c><see cref="IComparer.Compare(object?, object?)"/> &lt; 0 ? <paramref name="value1"/> : <paramref name="value2"/></c>
		/// </summary>
		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static object Minimum(this IComparer @this, object value1, object value2)
			=> @this.LessThan(value1, value2) ? value1 : value2;

		/// <summary>
		/// <c><see cref="IComparer{T}.Compare(T, T)"/> &lt; 0 ? <paramref name="value1"/> : <paramref name="value2"/></c>
		/// </summary>
		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static T Minimum<T>(this IComparer<T> @this, T value1, T value2)
			=> @this.LessThan(value1, value2) ? value1 : value2;
	}
}
