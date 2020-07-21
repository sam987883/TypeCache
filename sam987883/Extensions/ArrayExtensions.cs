// Copyright (c) 2020 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace sam987883.Extensions
{
	public static class ArrayExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Any<T>(this T[]? @this) =>
			@this?.Length > 0;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Clear<T>(this T[] @this, int start = 0, int length = 0) =>
			Array.Clear(@this, start, length == 0 ? @this.Length : length);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void CopyTo<T>(this T[] @this, T[] target, int length = 0) =>
			Array.Copy(@this, target, length < 1 ? @this.Length : length);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void CopyTo<T>(this T[] @this, int sourceIndex, T[] target, int targetIndex, int length = 0) =>
			Array.Copy(@this, sourceIndex, target, targetIndex, length < 1 ? @this.Length : length);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string Csv<T>(this T[] @this) =>
			string.Join(',', @this);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string Join<T>(this T[] @this, char delimeter) =>
			string.Join(delimeter, @this);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string Join<T>(this T[] @this, string delimeter) =>
			string.Join(delimeter, @this);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Resize<T>(this T[] @this, int size) =>
			Array.Resize(ref @this, size);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Reverse<T>(this T[] @this) =>
			Array.Reverse(@this);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int Search<T>(this T[] @this, T value, IComparer<T>? comparer = null) =>
			Array.BinarySearch(@this, value, comparer);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int Search<T>(this T[] @this, T value, int start, int length = 0, IComparer<T>? comparer = null) =>
			Array.BinarySearch(@this, start, length > 0 ? length : @this.Length, value, comparer);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Sort<T>(this T[] @this, IComparer<T>? comparer = null) =>
			Array.Sort(@this, comparer);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Sort<T>(this T[] @this, int start, int length = 0, IComparer<T>? comparer = null) =>
			Array.Sort(@this, start, length > 0 ? length : @this.Length, comparer);

		public static T[] Subarray<T>(this T[] @this, int sourceIndex, int length = 0)
		{
			if (sourceIndex + length > @this.Length)
				throw new IndexOutOfRangeException("");

			var array = new T[length > 0 ? length : @this.Length];
			Array.Copy(@this, sourceIndex, array, 0, array.Length);
			return array;
		}
	}
}
