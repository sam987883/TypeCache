// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using TypeCache.Extensions;

namespace TypeCache.Collections.Extensions
{
	public static class ArrayExtensions
	{
		/// <summary>
		/// <c><see cref="Task.WhenAll(Task[])"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async ValueTask AllAsync<T>(this Task[] @this)
			=> await Task.WhenAll(@this);

		/// <summary>
		/// <c><see cref="Task.WhenAll{TResult}(Task{TResult}[])"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async ValueTask<T[]> AllAsync<T>(this Task<T>[]? @this)
			=> @this.Any() ? await Task.WhenAll(@this) : await Task.FromResult(Array.Empty<T>());

		/// <summary>
		/// <c><see cref="Task.WhenAny(Task[])"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async ValueTask AnyAsync<T>(this Task[] @this)
			=> await Task.WhenAny(@this);

		/// <summary>
		/// <c><see cref="Task.WhenAny{TResult}(Task{TResult}[])"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async ValueTask<Task<T>> AnyAsync<T>(this Task<T>[] @this)
			=> await Task.WhenAny(@this);

		/// <summary>
		/// <c><see cref="Array.Clear(Array, int, int)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Clear<T>(this T[] @this, int start = 0, int length = 0)
			=> Array.Clear(@this, start, length == 0 ? @this.Length : length);

		/// <summary>
		/// <c><see cref="Array.Copy(Array, Array, long)"/></c>
		/// </summary>
		/// <remarks>Throws <see cref="ArgumentNullException"/>.</remarks>
		public static void CopyTo<T>(this T[] @this, T[] target, long length = 0)
		{
			target.AssertNotNull(nameof(target));

			Array.Copy(@this, target, length < 1 ? @this.Length : length);
		}

		/// <summary>
		/// <c><see cref="Array.Copy(Array, long, Array, long, long)"/></c>
		/// </summary>
		/// <remarks>Throws <see cref="ArgumentNullException"/>.</remarks>
		public static void CopyTo<T>(this T[] @this, long sourceIndex, T[] target, long targetIndex, long length = 0)
		{
			target.AssertNotNull(nameof(target));

			Array.Copy(@this, sourceIndex, target, targetIndex, length < 1 ? @this.Length : length);
		}

		public static void Deconstruct<T>(this T[]? @this, out T? first, out T[] rest)
			where T : struct
		{
			first = @this?.Length > 0 ? @this[0] : null;
			rest = @this?.Length > 1 ? @this[1..] : Array.Empty<T>();
		}

		public static void Deconstruct<T>(this T[]? @this, out T? first, out T? second, out T[] rest)
			where T : struct
		{
			first = @this?.Length > 0 ? @this[0] : null;
			second = @this?.Length > 1 ? @this[1] : null;
			rest = @this?.Length > 2 ? @this[2..] : Array.Empty<T>();
		}

		public static void Deconstruct<T>(this T[]? @this, out T? first, out T? second, out T? third, out T[] rest)
			where T : struct
		{
			first = @this?.Length > 0 ? @this[0] : null;
			second = @this?.Length > 1 ? @this[1] : null;
			third = @this?.Length > 2 ? @this[2] : null;
			rest = @this?.Length > 3 ? @this[3..] : Array.Empty<T>();
		}

		public static void Deconstruct<T>(this T[]? @this, out T? first, out T[] rest)
			where T : class
		{
			first = @this?.Length > 0 ? @this[0] : null;
			rest = @this?.Length > 1 ? @this[1..] : Array.Empty<T>();
		}

		public static void Deconstruct<T>(this T[] @this, out T? first, out T? second, out T[] rest)
			where T : class
		{
			first = @this?.Length > 0 ? @this[0] : null;
			second = @this?.Length > 1 ? @this[1] : null;
			rest = @this?.Length > 2 ? @this[2..] : Array.Empty<T>();
		}

		public static void Deconstruct<T>(this T[] @this, out T? first, out T? second, out T? third, out T[] rest)
			where T : class
		{
			first = @this?.Length > 0 ? @this[0] : null;
			second = @this?.Length > 1 ? @this[1] : null;
			third = @this?.Length > 2 ? @this[2] : null;
			rest = @this?.Length > 3 ? @this[3..] : Array.Empty<T>();
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
		/// Can modify the contents of the array and the looping index.<br/>
		/// index = 0 restarts the loop, --index repeats the current item and ++index skips the next item.
		/// </summary>
		/// <remarks>Throws <see cref="ArgumentNullException"/>.</remarks>
		public static void Do<T>(this T[]? @this, ActionRef<T, int> action)
		{
			action.AssertNotNull(nameof(action));

			var count = @this?.Length ?? 0;
			for (var index = 0; index < count; ++index)
				action(ref @this![index], ref index);
		}

		/// <remarks>Throws <see cref="ArgumentNullException"/>.</remarks>
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

		/// <remarks>Throws <see cref="ArgumentNullException"/>.</remarks>
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

		/// <summary>
		/// <c><see cref="Array.Resize{T}(ref T[], int)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Resize<T>(this T[] @this, int size)
			=> Array.Resize(ref @this, size);

		/// <summary>
		/// <c><see cref="Array.Reverse{T}(T[])"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Reverse<T>(this T[] @this)
			=> Array.Reverse(@this);

		/// <summary>
		/// <c><see cref="Array.BinarySearch{T}(T[], T, IComparer{T}?)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int Search<T>(this T[] @this, T value, IComparer<T>? comparer = null)
			=> Array.BinarySearch(@this, value, comparer);

		/// <summary>
		/// <c><see cref="Array.BinarySearch{T}(T[], int, int, T, IComparer{T}?)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int Search<T>(this T[] @this, T value, int start, int length = 0, IComparer<T>? comparer = null)
			=> Array.BinarySearch(@this, start, length > 0 ? length : @this.Length, value, comparer);

		/// <summary>
		/// <c><see cref="Array.Sort{T}(T[], IComparer{T}?)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Sort<T>(this T[] @this, IComparer<T>? comparer = null)
			=> Array.Sort(@this, comparer);

		/// <summary>
		/// <c><see cref="Array.Sort{T}(T[], Comparison{T})"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Sort<T>(this T[] @this, Comparison<T> comparison)
			=> Array.Sort(@this, comparison);

		/// <summary>
		/// <c><see cref="Array.Sort{T}(T[], int, int, IComparer{T}?)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Sort<T>(this T[] @this, int start, int length = 0, IComparer<T>? comparer = null)
			=> Array.Sort(@this, start, length > 0 ? length : @this.Length, comparer);

		/// <remarks>Throws <see cref="IndexOutOfRangeException"/>.</remarks>
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

		/// <summary>
		/// <c><see cref="ImmutableArray.Create{T}(T[])"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ImmutableArray<T> ToImmutableArray<T>(this T[]? @this)
			where T : notnull
			=> ImmutableArray.Create(@this);

		/// <summary>
		/// <c><see cref="ImmutableHashSet.Create{T}(IEqualityComparer{T}?, T[])"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ImmutableHashSet<string> ToImmutableHashSet(this string[]? @this, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
			=> ImmutableHashSet.Create(comparison.ToStringComparer(), @this ?? Array.Empty<string>());

		/// <summary>
		/// <c><see cref="ImmutableHashSet.Create{T}(IEqualityComparer{T}?, T[])"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ImmutableHashSet<T> ToImmutableHashSet<T>(this T[]? @this, IEqualityComparer<T>? comparer)
			where T : notnull
			=> ImmutableHashSet.Create(comparer, @this ?? Array.Empty<T>());

		/// <summary>
		/// <c><see cref="ImmutableList.Create{T}(T[])"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ImmutableList<T> ToImmutableList<T>(this T[]? @this)
			where T : notnull
			=> ImmutableList.Create(@this ?? Array.Empty<T>());

		/// <summary>
		/// <c><see cref="ImmutableQueue.Create{T}(T[])"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ImmutableQueue<T> ToImmutableQueue<T>(this T[]? @this)
			where T : notnull
			=> ImmutableQueue.Create(@this ?? Array.Empty<T>());

		/// <summary>
		/// <c><see cref="ImmutableSortedSet.Create{T}(IComparer{T}?, T[])"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ImmutableSortedSet<string> ToImmutableSortedSet(this string[]? @this, StringComparison comparison = StringComparison.Ordinal)
			=> ImmutableSortedSet.Create(comparison.ToStringComparer(), @this ?? Array.Empty<string>());

		/// <summary>
		/// <c><see cref="ImmutableSortedSet.Create{T}(IComparer{T}?, T[])"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ImmutableSortedSet<T> ToImmutableSortedSet<T>(this T[]? @this, IComparer<T>? comparer)
			where T : notnull
			=> ImmutableSortedSet.Create(comparer, @this ?? Array.Empty<T>());

		/// <summary>
		/// <c><see cref="ImmutableStack.Create{T}(T[])"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ImmutableStack<T> ToImmutableStack<T>(this T[]? @this)
			where T : notnull
			=> ImmutableStack.Create(@this ?? Array.Empty<T>());

		/// <summary>
		/// <c><see cref="Task.WaitAll(Task[], CancellationToken)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void WaitForAll<T>(this Task[] @this, CancellationToken cancellationToken = default)
			=> Task.WaitAll(@this, cancellationToken);

		/// <summary>
		/// <c><see cref="Task.WaitAll(Task[], int, CancellationToken)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void WaitForAll<T>(this Task[] @this, int milliseconds, CancellationToken cancellationToken = default)
			=> Task.WaitAll(@this, milliseconds, cancellationToken);

		/// <summary>
		/// <c><see cref="Task.WaitAny(Task[], CancellationToken)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void WaitForAny<T>(this Task[] @this, CancellationToken cancellationToken = default)
			=> Task.WaitAny(@this, cancellationToken);

		/// <summary>
		/// <c><see cref="Task.WaitAny(Task[], CancellationToken)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void WaitForAny<T>(this Task[] @this, int milliseconds, CancellationToken cancellationToken = default)
			=> Task.WaitAny(@this, milliseconds, cancellationToken);
	}
}
