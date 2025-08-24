// Copyright (c) 2021 Samuel Abraham

using System.Collections;

namespace TypeCache.Collections;

public sealed class LazyReadOnlyDictionary<K, V>(IReadOnlyDictionary<K, Lazy<V>> dictionary) : IReadOnlyDictionary<K, V>
	where K : notnull
{
	public V this[K key] => dictionary[key].Value;

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
		=> this.GetEnumerator();

	bool IReadOnlyDictionary<K, V>.TryGetValue(K key, [MaybeNullWhen(false)] out V value)
	{
		var success = dictionary.TryGetValue(key, out var lazy);
		value = success ? lazy!.Value : default;
		return success;
	}
}
