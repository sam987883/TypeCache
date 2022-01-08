// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using TypeCache.Extensions;
using static TypeCache.Collections.Extensions.EnumeratorExtensions;
using static TypeCache.Default;

namespace TypeCache.Collections;

public static class Enumerable<T>
{
	public static IEnumerable<T> Empty => Array<T>.Empty;

	internal static bool Any(IEnumerable<T> enumerable)
	{
		using var enumerator = enumerable.GetEnumerator();
		return enumerator.MoveNext();
	}

	internal static IEnumerable<T?> As(IEnumerable enumerable)
	{
		foreach (var item in enumerable)
			yield return item is T value ? value : default;
	}

	internal static int Count(IEnumerable<T> enumerable)
	{
		using var enumerator = enumerable.GetEnumerator();
		return enumerator.Count();
	}

	internal static void Do(IEnumerable<T> enumerable, Action<T> action)
	{
		action.AssertNotNull();

		foreach (var item in enumerable)
			action(item);
	}

	internal static void Do(IEnumerable<T> enumerable, Action<T, int> action)
	{
		action.AssertNotNull();

		var i = -1;
		foreach (var item in enumerable)
			action(item, ++i);
	}

	internal static void Do(IEnumerable<T> enumerable, Action<T> action, Action between)
	{
		action.AssertNotNull();
		between.AssertNotNull();

		using var itemEnumerator = enumerable.GetEnumerator();
		if (itemEnumerator.TryNext(out var item))
			action(item);

		while (itemEnumerator.TryNext(out item))
		{
			between();
			action(item);
		}
	}

	internal static void Do(IEnumerable<T> enumerable, Action<T, int> action, Action between)
	{
		action.AssertNotNull();
		between.AssertNotNull();

		var i = 0;

		using var itemEnumerator = enumerable.GetEnumerator();
		if (itemEnumerator.TryNext(out var item))
			action(item, i);

		while (itemEnumerator.TryNext(out item))
		{
			between();
			action(item, ++i);
		}
	}

	internal static IEnumerable<T> Each(IEnumerable<T> enumerable, Func<T, T> edit)
	{
		edit.AssertNotNull();

		foreach (var item in enumerable)
			yield return edit(item);
	}

	internal static IEnumerable<T> Each(IEnumerable<T> enumerable, Func<T, int, T> edit)
	{
		edit.AssertNotNull();

		var i = -1;
		foreach (var item in enumerable)
			yield return edit(item, ++i);
	}

	internal static T? Get(IEnumerable<T> enumerable, int index)
	{
		using var enumerator = enumerable.GetEnumerator();
		return enumerator.Get(index);
	}

	internal static IEnumerable<T> If(IEnumerable enumerable)
	{
		foreach (var item in enumerable)
			if (item is T value)
				yield return value;
	}

	internal static IEnumerable<T> If(IEnumerable<T> enumerable, Predicate<T> filter)
	{
		filter.AssertNotNull();

		foreach (var item in enumerable)
			if (filter(item))
				yield return item;
	}

	internal static IEnumerable<V> Map<V>(IEnumerable<T> enumerable, Func<T, V> map)
	{
		map.AssertNotNull();

		foreach (var item in enumerable)
			yield return map(item);
	}

	internal static IEnumerable<V> Map<V>(IEnumerable<T> enumerable, Func<T, int, V> map)
	{
		map.AssertNotNull();

		var i = -1;
		foreach (var item in enumerable)
			yield return map(item, ++i);
	}

	internal static async IAsyncEnumerable<V> MapAsync<V>(IEnumerable<T> enumerable, Func<T, Task<V>> map, [EnumeratorCancellation] CancellationToken _ = default)
	{
		foreach (var item in enumerable)
			yield return await map(item);
	}

	internal static async IAsyncEnumerable<V> MapAsync<V>(IEnumerable<T> enumerable, Func<T, ValueTask<V>> map, [EnumeratorCancellation] CancellationToken _ = default)
	{
		foreach (var item in enumerable)
			yield return await map(item);
	}

	internal static async IAsyncEnumerable<V> MapAsync<V>(IEnumerable<T> enumerable, Func<T, CancellationToken, Task<V>> map, [EnumeratorCancellation] CancellationToken token = default)
	{
		foreach (var item in enumerable)
		{
			yield return await map(item, token);
			if (token.IsCancellationRequested)
				yield break;
		}
	}

	internal static async IAsyncEnumerable<V> MapAsync<V>(IEnumerable<T> enumerable, Func<T, CancellationToken, ValueTask<V>> map, [EnumeratorCancellation] CancellationToken token = default)
	{
		foreach (var item in enumerable)
		{
			yield return await map(item, token);
			if (token.IsCancellationRequested)
				yield break;
		}
	}

	internal static bool NoGet(out T? item)
	{
		item = default;
		return false;
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

	internal static bool TryGet(IEnumerable<T> enumerable, int index, [NotNullWhen(true)] out T? item)
	{
		using var enumerator = enumerable.GetEnumerator();
		return enumerator.TryGet(index, out item);
	}

	internal static bool TrySingle(IEnumerable<T> enumerable, [NotNullWhen(true)] out T? item)
	{
		using var enumerator = enumerable.GetEnumerator();
		return enumerator.TryNext(out item) && enumerator.MoveNext();
	}
}
