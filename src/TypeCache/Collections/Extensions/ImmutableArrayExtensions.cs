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
	public static class ImmutableArrayExtensions
	{
		public static void Deconstruct<T>(this ImmutableArray<T> @this, out T? first, out IEnumerable<T> rest)
			where T : struct
		{
			first = @this.Length > 0 ? @this[0] : null;
			rest = @this.Length > 1 ? @this.Skip(1) : Enumerable<T>.Empty;
		}

		public static void Deconstruct<T>(this ImmutableArray<T> @this, out T? first, out T? second, out IEnumerable<T> rest)
			where T : struct
		{
			first = @this.Length > 0 ? @this[0] : null;
			second = @this.Length > 1 ? @this[1] : null;
			rest = @this.Length > 2 ? @this.Skip(2) : Enumerable<T>.Empty;
		}

		public static void Deconstruct<T>(this ImmutableArray<T> @this, out T? first, out T? second, out T? third, out IEnumerable<T> rest)
			where T : struct
		{
			first = @this.Length > 0 ? @this[0] : null;
			second = @this.Length > 1 ? @this[1] : null;
			third = @this.Length > 2 ? @this[2] : null;
			rest = @this.Length > 3 ? @this.Skip(3) : Enumerable<T>.Empty;
		}

		public static void Deconstruct<T>(this ImmutableArray<T> @this, out T? first, out IEnumerable<T> rest)
			where T : class
		{
			first = @this.Length > 0 ? @this[0] : null;
			rest = @this.Length > 1 ? @this.Skip(1) : Enumerable<T>.Empty;
		}

		public static void Deconstruct<T>(this ImmutableArray<T> @this, out T? first, out T? second, out IEnumerable<T> rest)
			where T : class
		{
			first = @this.Length > 0 ? @this[0] : null;
			second = @this.Length > 1 ? @this[1] : null;
			rest = @this.Length > 2 ? @this.Skip(2) : Enumerable<T>.Empty;
		}

		public static void Deconstruct<T>(this ImmutableArray<T> @this, out T? first, out T? second, out T? third, out IEnumerable<T> rest)
			where T : class
		{
			first = @this.Length > 0 ? @this[0] : null;
			second = @this.Length > 1 ? @this[1] : null;
			third = @this.Length > 2 ? @this[2] : null;
			rest = @this.Length > 3 ? @this.Skip(3) : Enumerable<T>.Empty;
		}

		public static void Do<T>(this ImmutableArray<T> @this, Action<T> action)
		{
			action.AssertNotNull(nameof(action));

			var count = @this.Length;
			for (var i = 0; i < count; ++i)
				action(@this[i]);
		}

		public static void Do<T>(this ImmutableArray<T> @this, Action<T, int> action)
		{
			action.AssertNotNull(nameof(action));

			var count = @this.Length;
			for (var i = 0; i < count; ++i)
				action(@this[i], i);
		}

		public static void Do<T>(this ImmutableArray<T> @this, Action<T> action, Action between)
		{
			action.AssertNotNull(nameof(action));
			between.AssertNotNull(nameof(between));

			var count = @this.Length;
			if (count > 0)
			{
				action(@this[0]);
				for (var i = 1; i < count; ++i)
				{
					between();
					action(@this[i]);
				}
			}
		}

		public static void Do<T>(this ImmutableArray<T> @this, Action<T, int> action, Action between)
		{
			action.AssertNotNull(nameof(action));
			between.AssertNotNull(nameof(between));

			var count = @this.Length;
			if (count > 0)
			{
				var i = 0;
				action(@this[0], i);
				for (i = 1; i < count; ++i)
				{
					between();
					action(@this[i], i);
				}
			}
		}

		/// <exception cref="ArgumentNullException"/>
		public static IEnumerable<T> Each<T>(this ImmutableArray<T> @this, Func<T, T> edit)
		{
			edit.AssertNotNull(nameof(edit));

			var count = @this.Length;
			for (var i = 0; i < count; ++i)
				yield return edit(@this[i]);
		}

		/// <exception cref="ArgumentNullException"/>
		public static IEnumerable<T> Each<T>(this ImmutableArray<T> @this, Func<T, int, T> edit)
		{
			edit.AssertNotNull(nameof(edit));

			var count = @this.Length;
			for (var i = 0; i < count; ++i)
				yield return edit(@this[i], i);
		}

		/// <exception cref="IndexOutOfRangeException" />
		public static IEnumerable<T> Get<T>(this ImmutableArray<T> @this, Range range)
		{
			if (range.Start.IsFromEnd || range.End.IsFromEnd)
				range = range.Normalize(@this.Length);

			return range.Values().To(i => @this[i]);
		}

		/// <exception cref="ArgumentNullException"/>
		public static IEnumerable<T> If<T>(this ImmutableArray<T> @this, Predicate<T> filter)
		{
			filter.AssertNotNull(nameof(filter));

			var count = @this.Length;
			for (var i = 0; i < count; ++i)
			{
				var item = @this[i];
				if (filter(item))
					yield return item;
			}
		}

		/// <exception cref="ArgumentNullException"/>
		public static async IAsyncEnumerable<T> IfAsync<T>(this ImmutableArray<T> @this, PredicateAsync<T> filter, [EnumeratorCancellation] CancellationToken _ = default)
		{
			filter.AssertNotNull(nameof(filter));

			var count = @this.Length;
			for (var i = 0; i < count; ++i)
			{
				var item = @this[i];
				if (await filter(item))
					yield return item;
			}
		}

		/// <exception cref="ArgumentNullException"/>
		public static IEnumerable<V> To<T, V>(this ImmutableArray<T> @this, Func<T, V> map)
		{
			map.AssertNotNull(nameof(map));

			var count = @this.Length;
			for (var i = 0; i < count; ++i)
				yield return map(@this[i]);
		}

		/// <exception cref="ArgumentNullException"/>
		public static IEnumerable<V> To<T, V>(this ImmutableArray<T> @this, Func<T, int, V> map)
		{
			map.AssertNotNull(nameof(map));

			var count = @this.Length;
			for (var i = 0; i < count; ++i)
				yield return map(@this[i], i);
		}

		/// <exception cref="ArgumentNullException"/>
		internal static async IAsyncEnumerable<V> ToAsync<T, V>(ImmutableArray<T> @this, Func<T, Task<V>> map, [EnumeratorCancellation] CancellationToken _ = default)
		{
			map.AssertNotNull(nameof(map));

			var count = @this.Length;
			for (var i = 0; i < count; ++i)
				yield return await map(@this[i]);
		}

		/// <exception cref="ArgumentNullException"/>
		public static IEnumerable<int> ToIndex<T>(this ImmutableArray<T> @this, Predicate<T> filter)
		{
			filter.AssertNotNull(nameof(filter));

			return 0.Range(@this.Length).If(i => filter(@this[i]));
		}
	}
}
