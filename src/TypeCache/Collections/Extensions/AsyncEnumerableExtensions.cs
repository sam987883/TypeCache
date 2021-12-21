// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using TypeCache.Extensions;
using static TypeCache.Default;

namespace TypeCache.Collections.Extensions;

public static class AsyncEnumerableExtensions
{
	/// <summary>
	/// <c><see cref="Task.Yield"/></c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	private static async IAsyncEnumerable<T> Empty<T>()
	{
		await Task.Yield();
		yield break;
	}

	public static async ValueTask<T?> AggregateAsync<T>(this IAsyncEnumerable<T>? @this, T initialValue, Func<T, T, T> aggregator, CancellationToken token = default)
	{
		aggregator.AssertNotNull();

		var result = initialValue;
		await @this.DoAsync(item => result = aggregator(result!, item), token: token);
		return result;
	}

	public static async ValueTask<bool> AllAsync<T>(this IAsyncEnumerable<T>? @this, Predicate<T> filter, CancellationToken token = default)
	{
		filter.AssertNotNull();

		return !await @this.IfAsync(item => !filter(item), token).AnyAsync(token);
	}

	public static async IAsyncEnumerable<T> AndAsync<T>(this IAsyncEnumerable<T>? @this, IAsyncEnumerable<IAsyncEnumerable<T>?>? itemSets, [EnumeratorCancellation] CancellationToken token = default)
	{
		if (@this is not null)
			await foreach (var item in @this)
				yield return item;

		if (itemSets is not null)
		{
			await foreach (var itemSet in itemSets)
			{
				if (itemSet is not null)
				{
					await foreach (var item in itemSet)
						yield return item;
				}
			}
		}
	}

	public static async IAsyncEnumerable<T> AndAsync<T>(this IAsyncEnumerable<T>? @this, IAsyncEnumerable<T>? items, [EnumeratorCancellation] CancellationToken token = default)
	{
		if (@this is not null)
			await foreach (var item in @this)
				yield return item;

		if (items is not null)
			await foreach (var item in items)
				yield return item;
	}

	public static async ValueTask<bool> AnyAsync<T>([NotNullWhen(true)] this IAsyncEnumerable<T>? @this, CancellationToken token = default)
	{
		if (@this is null)
			return false;

		await using var enumerator = @this.GetAsyncEnumerator(token);
		return await enumerator.MoveNextAsync();
	}

	/// <summary>
	/// <c>await @<paramref name="this"/>.IfAsync(<paramref name="filter"/>).AnyAsync(<paramref name="token"/>)</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static async Task<bool> AnyAsync<T>([NotNullWhen(true)] this IAsyncEnumerable<T>? @this, Predicate<T> filter, CancellationToken token = default)
		=> await @this.IfAsync(filter).AnyAsync(token);

	public static async ValueTask<int> CountAsync<T>(this IAsyncEnumerable<T>? @this, CancellationToken token = default)
	{
		if (@this is null)
			return 0;

		await using var enumerator = @this.GetAsyncEnumerator(token);
		return await enumerator.CountAsync();
	}

	public static async ValueTask DoAsync<T>(this IAsyncEnumerable<T>? @this, Action<T> action, Action? between = null, CancellationToken token = default)
	{
		action.AssertNotNull();

		if (@this is null)
			return;

		await using var enumerator = @this.GetAsyncEnumerator(token);
		if (await enumerator.MoveNextAsync())
		{
			action(enumerator.Current);
			if (between is not null)
			{
				while (await enumerator.MoveNextAsync())
				{
					between();
					action(enumerator.Current);
				}
			}
			else
			{
				while (await enumerator.MoveNextAsync())
					action(enumerator.Current);
			}
		}
	}

	public static async ValueTask DoAsync<T>(this IAsyncEnumerable<T>? @this, Action<T, int> action, Action? between = null, CancellationToken token = default)
	{
		action.AssertNotNull();

		if (@this is null)
			return;

		await using var enumerator = @this.GetAsyncEnumerator(token);
		if (await enumerator.MoveNextAsync())
		{
			var i = 0;
			action(enumerator.Current, i);
			if (between is not null)
			{
				while (await enumerator.MoveNextAsync())
				{
					between();
					action(enumerator.Current, ++i);
				}
			}
			else
			{
				while (await enumerator.MoveNextAsync())
					action(enumerator.Current, ++i);
			}
		}
	}

	public static async ValueTask<T> FirstAsync<T>(this IAsyncEnumerable<T>? @this, CancellationToken _ = default)
	{
		if (@this is null)
			return await Task.FromException<T>(new ArgumentNullException(nameof(@this), $"{nameof(IAsyncEnumerable<T>)} argument is null."));

		await foreach (var item in @this)
			return item;

		return await Task.FromException<T>(new IndexOutOfRangeException($"{nameof(FirstAsync)} called on {nameof(IAsyncEnumerable<T>)} which has no values."));
	}

	/// <summary>
	/// <c>await @<paramref name="this"/>.IfAsync(<paramref name="filter"/>).FirstAsync()</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static async ValueTask<T> FirstAsync<T>(this IAsyncEnumerable<T>? @this, Predicate<T> filter)
		=> await @this.IfAsync(filter).FirstAsync();

	/// <summary>
	/// <c>Empty&lt;<typeparamref name="T"/>&gt;().AndAsync(@<paramref name="this"/>)</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static IAsyncEnumerable<T> GatherAsync<T>(this IAsyncEnumerable<IAsyncEnumerable<T>?>? @this)
		=> Empty<T>().AndAsync(@this);

	public static async Task<T> GetAsync<T>(this IAsyncEnumerable<T>? @this, Index index, CancellationToken token = default)
	{
		if (@this is null)
			return await Task.FromException<T>(new ArgumentNullException(nameof(@this), $"{nameof(IAsyncEnumerable<T>)} argument is null."));

		if (index.IsFromEnd)
			index = index.FromStart(await @this.CountAsync(token));

		if (index.Value >= 0)
		{
			await using var enumerator = @this.GetAsyncEnumerator();
			return await enumerator.SkipAsync(index.Value);
		}

		return await Task.FromException<T>(new IndexOutOfRangeException($"Argument {nameof(index)} with normalized value {index.Value} it out of range."));
	}

	public static async IAsyncEnumerable<T> GetAsync<T>(this IAsyncEnumerable<T>? @this, Range range, [EnumeratorCancellation] CancellationToken token = default)
	{
		if (@this is null)
			yield break;

		if (range.Start.IsFromEnd || range.End.IsFromEnd)
			range = range.Normalize(await @this.CountAsync());

		if (range.Start.Value < 0 || range.End.Value < 0)
			range = new Range(range.Start.Value >= 0 ? range.Start : Index.Start, range.End.Value >= 0 ? range.End : Index.Start);

		await using var enumerator = @this.GetAsyncEnumerator(token);
		if (range.IsReverse() is false)
		{
			if (range.Start.Value > 0)
				yield return await enumerator.SkipAsync(range.Start.Value);
			var count = range.End.Value - range.Start.Value;
			while (await enumerator.MoveNextAsync() && --count >= 0)
				yield return enumerator.Current;
		}
		else
		{
			if (range.End.Value > 0)
				yield return await enumerator.SkipAsync(range.End.Value);
			var count = range.Start.Value - range.End.Value;
			while (await enumerator.MoveNextAsync() && --count >= 0)
				yield return enumerator.Current;
		}
	}

	public static async IAsyncEnumerable<T> GetAsync<T>(this IAsyncEnumerable<T>? @this, Func<T, IAsyncEnumerable<T>?> map, [EnumeratorCancellation] CancellationToken _ = default)
	{
		map.AssertNotNull();

		if (@this is null)
			yield break;

		await foreach (var source in @this)
		{
			var items = map(source);
			if (items is null)
				continue;

			await foreach (var item in items)
				yield return item;
		}
	}

	/// <summary>
	/// <c>await @<paramref name="this"/>.IfAsync(_ =&gt; _ is not null &amp;&amp; _.Equals(<paramref name="value"/>)).AnyAsync()</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static async ValueTask<bool> HasAsync<T>([NotNullWhen(true)] this IAsyncEnumerable<T>? @this, T value)
		=> await @this.IfAsync(_ => _ is not null && _.Equals(value)).AnyAsync();

	/// <summary>
	/// <c>await @<paramref name="this"/>.IfAsync(_ =&gt; <paramref name="comparer"/>.Equals(_, <paramref name="value"/>)).AnyAsync()</c>
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	public static async ValueTask<bool> HasAsync<T>([NotNullWhen(true)] this IAsyncEnumerable<T>? @this, T value, IEqualityComparer<T> comparer)
	{
		comparer.AssertNotNull();

		return await @this.IfAsync(_ => comparer.Equals(_, value)).AnyAsync();
	}

	public static async ValueTask<bool> HasAsync<T>([NotNullWhen(true)] this IAsyncEnumerable<T>? @this, IAsyncEnumerable<T>? values)
	{
		if (@this is null)
			return false;

		if (values is null)
			return true;

		await foreach (var value in values)
			if (!await @this.HasAsync(value))
				return false;

		return true;
	}

	/// <exception cref="ArgumentNullException"/>
	public static async ValueTask<bool> HasAsync<T>([NotNullWhen(true)] this IAsyncEnumerable<T>? @this, IAsyncEnumerable<T>? values, IEqualityComparer<T> comparer)
	{
		comparer.AssertNotNull();

		if (@this is null)
			return false;

		if (values is null)
			return true;

		await foreach (var value in values)
			if (!await @this.HasAsync(value, comparer))
				return false;

		return true;
	}

	/// <exception cref="ArgumentNullException"/>
	public static async IAsyncEnumerable<T> IfAsync<T>(this IAsyncEnumerable<T>? @this, Predicate<T> filter, [EnumeratorCancellation] CancellationToken _ = default)
	{
		filter.AssertNotNull();

		if (@this is null)
			yield break;

		await foreach (var item in @this)
			if (filter(item))
				yield return item;
	}

	public static async IAsyncEnumerable<R> IfAsync<T, R>(this IAsyncEnumerable<T>? @this, [EnumeratorCancellation] CancellationToken _ = default)
	{
		if (@this is null)
			yield break;

		await foreach (var item in @this)
		{
			if (item is R value)
				yield return value;
		}
	}

	public static async IAsyncEnumerable<T> IfNotNullAsync<T>(this IAsyncEnumerable<T?>? @this, [EnumeratorCancellation] CancellationToken _ = default) where T : class
	{
		if (@this is null)
			yield break;

		await foreach (var item in @this)
			if (item is not null)
				yield return item;
	}

	public static async IAsyncEnumerable<T> IfNotNullAsync<T>(this IAsyncEnumerable<T?>? @this, [EnumeratorCancellation] CancellationToken _ = default) where T : struct
	{
		if (@this is null)
			yield break;

		await foreach (var item in @this)
			if (item.HasValue)
				yield return item.Value;
	}

	/// <summary>
	/// <c>await @<paramref name="this"/>.AggregateAsync(default(T), (x, y) =&gt; x.MoreThan(y) ? x : y)</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static async ValueTask<T?> MaximumAsync<T>(this IAsyncEnumerable<T>? @this) where T : IComparable<T>
		=> await @this.AggregateAsync(default(T), (x, y) => x!.GreaterThan(y) ? x : y);

	/// <summary>
	/// <c>await @<paramref name="this"/>.AggregateAsync(default, <paramref name="comparer"/>.Maximum)</c>
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	public static async ValueTask<T?> MaximumAsync<T>(this IAsyncEnumerable<T>? @this, IComparer<T> comparer)
	{
		comparer.AssertNotNull();

		return await @this.AggregateAsync(default, comparer!.Maximum);
	}

	/// <summary>
	/// <c>await @<paramref name="this"/>.AggregateAsync(default, (x, y) =&gt; x.LessThan(y) ? x : y)</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static async ValueTask<T?> MinimumAsync<T>(this IAsyncEnumerable<T>? @this) where T : IComparable<T>
		=> await @this.AggregateAsync(default, (x, y) => x!.LessThan(y) ? x : y);

	/// <summary>
	/// <c>await @<paramref name="this"/>.AggregateAsync(default, <paramref name="comparer"/>.Minimum)</c>
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	public static async ValueTask<T?> MinimumAsync<T>(this IAsyncEnumerable<T>? @this, IComparer<T> comparer)
	{
		comparer.AssertNotNull();

		return await @this.AggregateAsync(default, comparer!.Minimum);
	}

	/// <exception cref="ArgumentNullException"/>
	public static async IAsyncEnumerable<V> ToAsync<T, V>(this IAsyncEnumerable<T>? @this, Func<T, V> map, [EnumeratorCancellation] CancellationToken _ = default)
	{
		map.AssertNotNull();

		if (@this is null)
			yield break;

		await foreach (var item in @this)
			yield return map(item);
	}

	/// <summary>
	/// <c>Empty&lt;<typeparamref name="V"/>&gt;().AndAsync(@<paramref name="this"/>.ToAsync&lt;<typeparamref name="T"/>, IAsyncEnumerable&lt;<typeparamref name="V"/>&gt;&gt;(<paramref name="map"/>))</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static IAsyncEnumerable<V> ToAsync<T, V>(this IAsyncEnumerable<T>? @this, Func<T, IAsyncEnumerable<V>> map)
		=> Empty<V>().AndAsync(@this.ToAsync<T, IAsyncEnumerable<V>>(map));

	/// <exception cref="ArgumentNullException"/>
	public static async ValueTask<ConcurrentDictionary<K, V>> ToDictionaryAsync<K, V>(this IAsyncEnumerable<K>? @this, Func<K, V> valueFactory, int concurrencyLevel, CancellationToken token = default) where K : notnull
	{
		valueFactory.AssertNotNull();

		var dictionary = new ConcurrentDictionary<K, V>(concurrencyLevel, await @this.CountAsync(token));
		if (@this is not null)
			await @this.DoAsync(key => dictionary.TryAdd(key, valueFactory(key)), token: token);
		return dictionary;
	}

	/// <exception cref="ArgumentNullException"/>
	public static async ValueTask<ConcurrentDictionary<K, V>> ToDictionaryAsync<K, V>(this IAsyncEnumerable<K>? @this, Func<K, V> valueFactory, IEqualityComparer<K> comparer, int concurrencyLevel, CancellationToken token = default) where K : notnull
	{
		valueFactory.AssertNotNull();
		comparer.AssertNotNull();

		var dictionary = new ConcurrentDictionary<K, V>(concurrencyLevel, await @this.CountAsync(token), comparer);
		if (@this is not null)
			await @this.DoAsync(key => dictionary.TryAdd(key, valueFactory(key)), token: token);
		return dictionary;
	}

	/// <exception cref="ArgumentNullException"/>
	public static async ValueTask<ConcurrentDictionary<K, V>> ToDictionaryAsync<T, K, V>(this IAsyncEnumerable<T>? @this, Func<T, K> keyFactory, Func<T, V> valueFactory, int concurrencyLevel, CancellationToken token = default) where K : notnull
	{
		keyFactory.AssertNotNull();
		valueFactory.AssertNotNull();

		var dictionary = new ConcurrentDictionary<K, V>(concurrencyLevel, await @this.CountAsync(token));
		if (@this is not null)
			await @this.DoAsync(value => dictionary.TryAdd(keyFactory(value), valueFactory(value)), token: token);
		return dictionary;
	}

	/// <exception cref="ArgumentNullException"/>
	public static async ValueTask<ConcurrentDictionary<K, V>> ToDictionaryAsync<T, K, V>(this IAsyncEnumerable<T>? @this, Func<T, K> keyFactory, Func<T, V> valueFactory, IEqualityComparer<K> comparer, int concurrencyLevel, CancellationToken token = default) where K : notnull
	{
		keyFactory.AssertNotNull();
		valueFactory.AssertNotNull();
		comparer.AssertNotNull();

		var dictionary = new ConcurrentDictionary<K, V>(concurrencyLevel, await @this.CountAsync(token), comparer);
		if (@this is not null)
			await @this.DoAsync(value => dictionary.TryAdd(keyFactory(value), valueFactory(value)), token: token);
		return dictionary;
	}

	public static async ValueTask<List<T>> ToListAsync<T>(this IAsyncEnumerable<T>? @this, CancellationToken token = default)
	{
		var list = new List<T>();
		if (@this is not null)
		{
			await using var enumerator = @this.GetAsyncEnumerator();
			while (await enumerator.MoveNextAsync())
				list.Add(enumerator.Current);
		}
		return list;
	}
}
