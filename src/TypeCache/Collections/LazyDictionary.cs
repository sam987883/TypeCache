// Copyright (c) 2021 Samuel Abraham

using System.Collections;
using System.Collections.Concurrent;
using static System.Threading.LazyThreadSafetyMode;

namespace TypeCache.Collections;

public static class LazyDictionary
{
	public static LazyDictionary<K, V> Create<K, V>(Func<K, V> createValue, IEqualityComparer<K>? comparer = null)
		where K : notnull
	{
		var createLazyValue = new Func<K, Lazy<V>>(key => new Lazy<V>(() => createValue(key), PublicationOnly));
		var dictionary = new ConcurrentDictionary<K, Lazy<V>>(comparer);

		return new(dictionary, createLazyValue);
	}

	public static LazyDictionary<K, V> Create<K, V>(Func<K, V> createValue, int capacity, int concurrencyLevel = -1, IEqualityComparer<K>? comparer = null)
		where K : notnull
	{
		var createLazyValue = new Func<K, Lazy<V>>(key => new Lazy<V>(() => createValue(key), PublicationOnly));
		var dictionary = new ConcurrentDictionary<K, Lazy<V>>(concurrencyLevel, capacity, comparer);

		return new(dictionary, createLazyValue);
	}

	public static LazyDictionary<K, V> CreateThreadSafe<K, V>(Func<K, V> createValue, IEqualityComparer<K>? comparer = null)
		where K : notnull
	{
		var createLazyValue = new Func<K, Lazy<V>>(key => new Lazy<V>(() => createValue(key), ExecutionAndPublication));
		var dictionary = new ConcurrentDictionary<K, Lazy<V>>(comparer);

		return new(dictionary, createLazyValue);
	}

	public static LazyDictionary<K, V> CreateThreadSafe<K, V>(Func<K, V> createValue, int capacity, int concurrencyLevel = -1, IEqualityComparer<K>? comparer = null)
		where K : notnull
	{
		var createLazyValue = new Func<K, Lazy<V>>(key => new Lazy<V>(() => createValue(key), ExecutionAndPublication));
		var dictionary = new ConcurrentDictionary<K, Lazy<V>>(concurrencyLevel, capacity, comparer);

		return new(dictionary, createLazyValue);
	}
}

public sealed class LazyDictionary<K, V>(ConcurrentDictionary<K, Lazy<V>> dictionary, Func<K, Lazy<V>> createValue) : IReadOnlyDictionary<K, V>
	where K : notnull
{
	public V this[K key] => dictionary.GetOrAdd(key, createValue).Value;

	public IEnumerable<K> Keys => dictionary.Keys;

	public IEnumerable<V> Values => dictionary.Values.Select(_ => _.Value);

	public int Count => dictionary.Count;

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public bool ContainsKey(K key)
		=> dictionary.ContainsKey(key);

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public IEnumerator<KeyValuePair<K, V>> GetEnumerator()
		=> dictionary.Select(pair => KeyValuePair.Create(pair.Key, pair.Value.Value)).GetEnumerator();

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	IEnumerator IEnumerable.GetEnumerator()
		=> dictionary.Select(pair => KeyValuePair.Create(pair.Key, pair.Value.Value)).GetEnumerator();

	bool IReadOnlyDictionary<K, V>.TryGetValue(K key, [MaybeNullWhen(false)] out V value)
	{
		var success = dictionary.TryGetValue(key, out var lazy)
			|| dictionary.TryAdd(key, lazy = createValue(key));
		value = success ? lazy.Value : default;
		return success;
	}
}
