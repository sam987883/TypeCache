// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
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

		/// <summary>
		/// Can modify the items in the array.
		/// </summary>
		public static void Do<T>(this T[]? @this, ActionRef<T> action)
		{
			action.AssertNotNull(nameof(action));

			var count = @this?.Length ?? 0;
			for (var i = 0; i < count; ++i)
				action(ref @this![i]);
		}

		/// <summary>
		/// Can modify the contents of the array and the looping index.
		/// </summary>
		/// <remarks>index = 0 restarts the loop, --index repeats the current item and ++iindex skips the next item</remarks>
		public static void Do<T>(this T[]? @this, ActionRef<T, int> action)
		{
			action.AssertNotNull(nameof(action));

			var count = @this?.Length ?? 0;
			for (var index = 0; index < count; ++index)
				action(ref @this![index], ref index);
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

		public static V[] ToArray<T, V>(this T[]? @this, Func<T, V> map)
		{
			if (!@this.Any())
				return Array.Empty<V>();

			var array = new V[@this.Length];
			@this.Do((item, index) => array[index] = map(item));
			return array;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ImmutableArray<T> ToImmutableArray<T>(this T[]? @this)
			where T : notnull
			=> ImmutableArray.Create(@this);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ImmutableHashSet<string> ToImmutableHashSet(this string[]? @this, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
			=> ImmutableHashSet.Create(comparison.ToStringComparer(), @this ?? Array.Empty<string>());

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ImmutableHashSet<T> ToImmutableHashSet<T>(this T[]? @this, IEqualityComparer<T>? comparer = null)
			where T : notnull
			=> ImmutableHashSet.Create(comparer, @this ?? Array.Empty<T>());

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ImmutableList<T> ToImmutableList<T>(this T[]? @this)
			where T : notnull
			=> ImmutableList.Create(@this ?? Array.Empty<T>());

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ImmutableQueue<T> ToImmutableQueue<T>(this T[]? @this)
			where T : notnull
			=> ImmutableQueue.Create(@this ?? Array.Empty<T>());

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ImmutableSortedSet<string> ToImmutableSortedSet(this string[]? @this, StringComparison comparison = StringComparison.Ordinal)
			=> ImmutableSortedSet.Create(comparison.ToStringComparer(), @this ?? Array.Empty<string>());

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ImmutableSortedSet<T> ToImmutableSortedSet<T>(this T[]? @this, IComparer<T>? comparer = null)
			where T : notnull
			=> ImmutableSortedSet.Create(comparer, @this ?? Array.Empty<T>());

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ImmutableStack<T> ToImmutableStack<T>(this T[]? @this)
			where T : notnull
			=> ImmutableStack.Create(@this ?? Array.Empty<T>());
	}
}
