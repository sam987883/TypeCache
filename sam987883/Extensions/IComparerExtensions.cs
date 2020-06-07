// Copyright (c) 2020 Samuel Abraham

using sam987883.Common;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace sam987883.Extensions
{
	public static class IComparerExtensions
    {
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Equal(this IComparer @this, object value1, object value2) =>
			@this.Compare(value1, value2) == 0;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Equal<T>(this IComparer<T> @this, T value1, T value2) =>
			@this.Compare(value1, value2) == 0;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool LessThan(this IComparer @this, object value1, object value2) =>
			@this.Compare(value1, value2) < 0;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool LessThan<T>(this IComparer<T> @this, T value1, T value2) =>
			@this.Compare(value1, value2) < 0;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool LessThanOrEqual(this IComparer @this, object value1, object value2) =>
			@this.Compare(value1, value2) <= 0;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool LessThanOrEqual<T>(this IComparer<T> @this, T value1, T value2) =>
			@this.Compare(value1, value2) <= 0;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool MoreThan(this IComparer @this, object value1, object value2) =>
			@this.Compare(value1, value2) > 0;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool MoreThan<T>(this IComparer<T> @this, T value1, T value2) =>
			@this.Compare(value1, value2) > 0;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool MoreThanOrEqual(this IComparer @this, object value1, object value2) =>
			@this.Compare(value1, value2) >= 0;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool MoreThanOrEqual<T>(this IComparer<T> @this, T value1, T value2) =>
			@this.Compare(value1, value2) >= 0;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static object Maximum(this IComparer @this, object value1, object value2) =>
			@this.MoreThan(value1, value2) ? value1 : value2;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T Maximum<T>(this IComparer<T> @this, T value1, T value2) =>
			@this.MoreThan(value1, value2) ? value1 : value2;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static object Minimum(this IComparer @this, object value1, object value2) =>
			@this.LessThan(value1, value2) ? value1 : value2;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T Minimum<T>(this IComparer<T> @this, T value1, T value2) =>
			@this.LessThan(value1, value2) ? value1 : value2;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IEqualityComparer<T> ToEqualityComparer<T>(this IComparer<T> @this) =>
			new CustomEqualityComparer<T>(@this.Equal);
	}
}
