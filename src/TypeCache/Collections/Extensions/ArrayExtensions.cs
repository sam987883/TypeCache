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
			=> @this.Any() ? await Task.WhenAll(@this) : await Task.FromResult(Array<T>.Empty);

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

		public static void Deconstruct<T>(this T[]? @this, out T? first, out IEnumerable<T> rest)
			where T : struct
		{
			first = @this?.Length > 0 ? @this[0] : null;
			rest = @this?.Length > 1 ? @this[1..] : Enumerable<T>.Empty;
		}

		public static void Deconstruct<T>(this T[]? @this, out T? first, out T? second, out IEnumerable<T> rest)
			where T : struct
		{
			first = @this?.Length > 0 ? @this[0] : null;
			second = @this?.Length > 1 ? @this[1] : null;
			rest = @this?.Length > 2 ? @this[2..] : Enumerable<T>.Empty;
		}

		public static void Deconstruct<T>(this T[]? @this, out T? first, out T? second, out T? third, out IEnumerable<T> rest)
			where T : struct
		{
			first = @this?.Length > 0 ? @this[0] : null;
			second = @this?.Length > 1 ? @this[1] : null;
			third = @this?.Length > 2 ? @this[2] : null;
			rest = @this?.Length > 3 ? @this[3..] : Enumerable<T>.Empty;
		}

		public static void Deconstruct<T>(this T[]? @this, out T? first, out IEnumerable<T> rest)
			where T : class
		{
			first = @this?.Length > 0 ? @this[0] : null;
			rest = @this?.Length > 1 ? @this[1..] : Enumerable<T>.Empty;
		}

		public static void Deconstruct<T>(this T[]? @this, out T? first, out T? second, out IEnumerable<T> rest)
			where T : class
		{
			first = @this?.Length > 0 ? @this[0] : null;
			second = @this?.Length > 1 ? @this[1] : null;
			rest = @this?.Length > 2 ? @this[2..] : Enumerable<T>.Empty;
		}

		public static void Deconstruct<T>(this T[]? @this, out T? first, out T? second, out T? third, out IEnumerable<T> rest)
			where T : class
		{
			first = @this?.Length > 0 ? @this[0] : null;
			second = @this?.Length > 1 ? @this[1] : null;
			third = @this?.Length > 2 ? @this[2] : null;
			rest = @this?.Length > 3 ? @this[3..] : Enumerable<T>.Empty;
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

		/// <remarks>Throws <see cref="IndexOutOfRangeException"/>.</remarks>
		public static IEnumerable<T> Get<T>(this T[] @this, Range range)
		{
			if (range.Start.IsFromEnd || range.End.IsFromEnd)
				range = range.Normalize(@this.Length);

			return range.Values().To(i => @this[i]);
		}

		/// <remarks>Throws <see cref="ArgumentNullException"/>.</remarks>
		public static IEnumerable<T> If<T>(this T[]? @this, Predicate<T> filter)
		{
			filter.AssertNotNull(nameof(filter));

			if (@this is null)
				yield break;

			var count = @this.Length;
			for (var i = 0; i < count; ++i)
			{
				var item = @this[i];
				if (filter(item))
					yield return item;
			}
		}

		/// <remarks>Throws <see cref="ArgumentNullException"/>.</remarks>
		public static async IAsyncEnumerable<T> IfAsync<T>(this T[]? @this, PredicateAsync<T> filter, [EnumeratorCancellation] CancellationToken _ = default)
		{
			filter.AssertNotNull(nameof(filter));

			if (@this is not null)
			{
				var count = @this.Length;
				for (var i = 0; i < count; ++i)
				{
					var item = @this[i];
					if (await filter(item))
						yield return item;
				}
			}
		}

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

			var array = new T[length > 0 ? length : (@this.Length - sourceIndex)];
			Array.Copy(@this, sourceIndex, array, 0, array.Length);
			return array;
		}

		/// <remarks>Throws <see cref="ArgumentNullException"/>.</remarks>
		public static IEnumerable<V> To<T, V>(this T[]? @this, Func<T, V> map)
		{
			map.AssertNotNull(nameof(map));

			if (@this is null)
				yield break;

			var count = @this.Length;
			for (var i = 0; i < count; ++i)
				yield return map(@this[i]);
		}

		/// <remarks>Throws <see cref="ArgumentNullException"/>.</remarks>
		public static IEnumerable<V> To<T, V>(this T[]? @this, Func<T, int, V> map)
		{
			map.AssertNotNull(nameof(map));

			if (@this is null)
				yield break;

			var count = @this.Length;
			for (var i = 0; i < count; ++i)
				yield return map(@this[i], i);
		}

		public static V[] ToArray<T, V>(this T[]? @this, Func<T, V> map)
		{
			if (!@this.Any())
				return Array<V>.Empty;

			var array = new V[@this.Length];
			@this.Do((item, index) => array[index] = map(item));
			return array;
		}

		/// <remarks>Throws <see cref="ArgumentNullException"/>.</remarks>
		internal static async IAsyncEnumerable<V> ToAsync<T, V>(T[]? @this, Func<T, Task<V>> map, [EnumeratorCancellation] CancellationToken _ = default)
		{
			map.AssertNotNull(nameof(map));

			if (@this is null)
				yield break;

			var count = @this.Length;
			for (var i = 0; i < count; ++i)
				yield return await map(@this[i]);
		}

		/// <summary>
		/// <c><see cref="ImmutableQueue.Create{T}(T[])"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ImmutableQueue<T> ToImmutableQueue<T>(this T[]? @this)
			where T : notnull
			=> ImmutableQueue.Create(@this ?? Array<T>.Empty);

		/// <summary>
		/// <c><see cref="ImmutableStack.Create{T}(T[])"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ImmutableStack<T> ToImmutableStack<T>(this T[]? @this)
			where T : notnull
			=> ImmutableStack.Create(@this ?? Array<T>.Empty);

		/// <remarks>Throws <see cref="ArgumentNullException"/>.</remarks>
		public static IEnumerable<int> ToIndex<T>(this T[]? @this, Predicate<T> filter)
		{
			filter.AssertNotNull(nameof(filter));

			return @this is not null ? 0.Range(@this.Length).If(i => filter(@this[i])) : Enumerable<int>.Empty;
		}

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
