﻿// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using TypeCache.Collections.Extensions;
using TypeCache.Extensions;
using static TypeCache.Default;

namespace TypeCache.Collections
{
	public static class Enumerable<T>
	{
		public static IEnumerable<T> Empty => Array<T>.Empty;

		internal static IEnumerable<T?> As(IEnumerable enumerable)
		{
			foreach (var item in enumerable)
				yield return item is T value ? value : default;
		}

		internal static void Do(IEnumerable<T> enumerable, Action<T> action)
		{
			action.AssertNotNull(nameof(action));

			foreach (var item in enumerable)
				action(item);
		}

		internal static void Do(IEnumerable<T> enumerable, Action<T, int> action)
		{
			action.AssertNotNull(nameof(action));

			var i = -1;
			foreach (var item in enumerable)
				action(item, ++i);
		}

		internal static void Do(IEnumerable<T> enumerable, Action<T> action, Action between)
		{
			action.AssertNotNull(nameof(action));
			between.AssertNotNull(nameof(between));

			using var itemEnumerator = enumerable.GetEnumerator();
			if (itemEnumerator.MoveNext())
				action(itemEnumerator.Current);

			while (itemEnumerator.MoveNext())
			{
				between();
				action(itemEnumerator.Current);
			}
		}

		internal static void Do(IEnumerable<T> enumerable, Action<T, int> action, Action between)
		{
			action.AssertNotNull(nameof(action));
			between.AssertNotNull(nameof(between));

			using var itemEnumerator = enumerable.GetEnumerator();
			if (itemEnumerator.MoveNext())
				action(itemEnumerator.Current, 0);

			var i = 0;
			while (itemEnumerator.MoveNext())
			{
				between();
				action(itemEnumerator.Current, ++i);
			}
		}

		internal static IEnumerable<T> Each(IEnumerable<T> enumerable, Func<T, T> edit)
		{
			edit.AssertNotNull(nameof(edit));

			foreach (var item in enumerable)
				yield return edit(item);
		}

		internal static IEnumerable<T> Each(IEnumerable<T> enumerable, Func<T, int, T> edit)
		{
			edit.AssertNotNull(nameof(edit));

			var i = -1;
			foreach (var item in enumerable)
				yield return edit(item, ++i);
		}

		internal static IEnumerable<T> If(IEnumerable enumerable)
		{
			foreach (var item in enumerable)
				if (item is T value)
					yield return value;
		}

		internal static IEnumerable<T> If(IEnumerable<T> enumerable, Predicate<T> filter)
		{
			filter.AssertNotNull(nameof(filter));

			foreach (var item in enumerable)
				if (filter(item))
					yield return item;
		}

		internal static IEnumerable<V> To<V>(IEnumerable<T> enumerable, Func<T, V> map)
		{
			map.AssertNotNull(nameof(map));

			foreach (var item in enumerable)
				yield return map(item);
		}

		internal static IEnumerable<V> To<V>(IEnumerable<T> enumerable, Func<T, int, V> map)
		{
			map.AssertNotNull(nameof(map));

			var i = -1;
			foreach (var item in enumerable)
				yield return map(item, ++i);
		}

		[MethodImpl(METHOD_IMPL_OPTIONS)]
		internal static T[] ToArray(IEnumerable<T> enumerable)
			=> new Queue<T>(enumerable).ToArray();

		internal static IEnumerable<int> ToIndex(IEnumerable<T> enumerable, Predicate<T> filter)
		{
			var i = 0;
			foreach (var item in enumerable)
			{
				if (filter(item))
					yield return i;
				++i;
			}
		}
	}
}
