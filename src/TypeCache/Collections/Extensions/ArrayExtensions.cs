// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TypeCache.Extensions;

namespace TypeCache.Collections.Extensions
{
	public static class ArrayExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Clear<T>(this T[] @this, int start = 0, int length = 0)
			=> Array.Clear(@this, start, length == 0 ? @this.Length : length);

		public static void CopyTo<T>(this T[] @this, T[] target, int length = 0)
		{
			target.AssertNotNull(nameof(target));

			Array.Copy(@this, target, length < 1 ? @this.Length : length);
		}

		public static void CopyTo<T>(this T[] @this, int sourceIndex, T[] target, int targetIndex, int length = 0)
		{
			target.AssertNotNull(nameof(target));

			Array.Copy(@this, sourceIndex, target, targetIndex, length < 1 ? @this.Length : length);
		}

		public static void Do<T>(this T[]? @this, Action<T> action)
		{
			action.AssertNotNull(nameof(action));

			var count = @this?.Length ?? 0;
			for (var i = 0; i < count; ++i)
				action(@this![i]);
		}

		public static void Do<T>(this T[]? @this, Action<T, int> action)
		{
			action.AssertNotNull(nameof(action));

			var count = @this?.Length ?? 0;
			for (var i = 0; i < count; ++i)
				action(@this![i], i);
		}

		public static void Do<T>(this T[]? @this, Action<T> action, Action between)
		{
			action.AssertNotNull(nameof(action));
			between.AssertNotNull(nameof(between));

			var count = @this?.Length ?? 0;
			if (count > 0)
			{
				action(@this![0]);
				for (var i = 1; i < count; ++i)
				{
					between();
					action(@this[i]);
				}
			}
		}

		public static void Do<T>(this T[]? @this, Action<T, int> action, Action between)
		{
			action.AssertNotNull(nameof(action));
			between.AssertNotNull(nameof(between));

			var count = @this?.Length ?? 0;
			if (count > 0)
			{
				var i = 0;
				action(@this![0], i);
				for (i = 1; i < count; ++i)
				{
					between();
					action(@this[i], i);
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Resize<T>(this T[] @this, int size)
			=> Array.Resize(ref @this, size);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Reverse<T>(this T[] @this)
			=> Array.Reverse(@this);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int Search<T>(this T[] @this, T value, IComparer<T>? comparer = null)
			=> Array.BinarySearch(@this, value, comparer);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int Search<T>(this T[] @this, T value, int start, int length = 0, IComparer<T>? comparer = null)
			=> Array.BinarySearch(@this, start, length > 0 ? length : @this.Length, value, comparer);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Sort<T>(this T[] @this, IComparer<T>? comparer = null)
			=> Array.Sort(@this, comparer);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Sort<T>(this T[] @this, int start, int length = 0, IComparer<T>? comparer = null)
			=> Array.Sort(@this, start, length > 0 ? length : @this.Length, comparer);

		public static T[] Subarray<T>(this T[] @this, int sourceIndex, int length = 0)
		{
			if (sourceIndex + length > @this.Length)
				throw new IndexOutOfRangeException($"{nameof(Subarray)}: last index {sourceIndex + length} is more than array length {@this.Length}.");

			var array = new T[length > 0 ? length : @this.Length];
			Array.Copy(@this, sourceIndex, array, 0, array.Length);
			return array;
		}
	}
}
