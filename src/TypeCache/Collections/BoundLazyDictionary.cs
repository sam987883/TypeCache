// Copyright (c) 2021 Samuel Abraham

using System.Collections;
using System.Collections.Frozen;

namespace TypeCache.Collections;

public static class BoundLazyDictionary
{
	public static BoundLazyDictionary<K, V> Create<K, V>(
		IEnumerable<K> keys,
		Func<K, V> createValue,
		int concurrencyLevel = -1,
		IEqualityComparer<K>? comparer = null)
		where K : notnull
	{
		var keySet = keys.ToFrozenSet(comparer);
		var lazyDictionary = LazyDictionary.Create(createValue, keySet.Count, -1, comparer);

		return new(lazyDictionary, keySet);
	}

	public static BoundLazyDictionary<K, V> CreateThreadSafe<K, V>(
		IEnumerable<K> keys,
		Func<K, V> createValue,
		int concurrencyLevel = -1,
		IEqualityComparer<K>? comparer = null)
		where K : notnull
	{
		var keySet = keys.ToFrozenSet(comparer);
		var lazyDictionary = LazyDictionary.CreateThreadSafe(createValue, keySet.Count, -1, comparer);

		return new(lazyDictionary, keySet);
	}
}

public sealed class BoundLazyDictionary<K, V>(LazyDictionary<K, V> lazyDictionary, FrozenSet<K> keys) : IReadOnlyDictionary<K, V>
	where K : notnull
{
	public V this[K key] => keys.Contains(key) ? lazyDictionary[key] : this.NoValue!;

	public IEnumerable<K> Keys => keys;

	public V? NoValue { get; init; }

	public IEnumerable<V> Values => keys.Select(key => lazyDictionary[key]);

	public int Count => keys.Count;

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public bool ContainsKey(K key)
		=> keys.Contains(key);

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public IEnumerator<KeyValuePair<K, V>> GetEnumerator()
		=> keys.Select(key => KeyValuePair.Create(key, lazyDictionary[key])).GetEnumerator();

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	IEnumerator IEnumerable.GetEnumerator()
		=> this.GetEnumerator();

	bool IReadOnlyDictionary<K, V>.TryGetValue(K key, [MaybeNullWhen(false)] out V value)
	{
		if (keys.Contains(key))
			return ((IReadOnlyDictionary<K, V>)lazyDictionary).TryGetValue(key, out value);

		value = default;
		return false;
	}
}
