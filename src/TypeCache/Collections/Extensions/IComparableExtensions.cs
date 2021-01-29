// Copyright (c) 2021 Samuel Abraham

using System;
using System.Runtime.CompilerServices;

namespace TypeCache.Collections.Extensions
{
	public static class CompareExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Between(this IComparable @this, object value1, object value2)
			=> @this.MoreThanOrEqualTo(value1) && @this.LessThanOrEqualTo(value2);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Between<T>(this IComparable<T> @this, T value1, T value2)
			=> @this.MoreThanOrEqualTo(value1) && @this.LessThanOrEqualTo(value2);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool EqualTo(this IComparable @this, object value)
			=> @this.CompareTo(value) == 0;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool EqualTo<T>(this IComparable<T> @this, T value)
			=> @this.CompareTo(value) == 0;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool InBetween(this IComparable @this, object value1, object value2)
			=> @this.MoreThan(value1) && @this.LessThan(value2);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool InBetween<T>(this IComparable<T> @this, T value1, T value2)
			=> @this.MoreThan(value1) && @this.LessThan(value2);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool LessThan(this IComparable @this, object value)
			=> @this.CompareTo(value) < 0;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool LessThan<T>(this IComparable<T> @this, T value)
			=> @this.CompareTo(value) < 0;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool LessThanOrEqualTo(this IComparable @this, object value)
			=> @this.CompareTo(value) <= 0;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool LessThanOrEqualTo<T>(this IComparable<T> @this, T value)
			=> @this.CompareTo(value) <= 0;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool MoreThan(this IComparable @this, object value)
			=> @this.CompareTo(value) > 0;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool MoreThan<T>(this IComparable<T> @this, T value)
			=> @this.CompareTo(value) > 0;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool MoreThanOrEqualTo(this IComparable @this, object value)
			=> @this.CompareTo(value) >= 0;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool MoreThanOrEqualTo<T>(this IComparable<T> @this, T value)
			=> @this.CompareTo(value) >= 0;
	}
}
