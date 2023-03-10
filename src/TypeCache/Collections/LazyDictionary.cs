// Copyright (c) 2021 Samuel Abraham

using System.Collections;
using System.Collections.Concurrent;
using TypeCache.Extensions;

namespace TypeCache.Collections;

public sealed class LazyDictionary<K, V> : IReadOnlyDictionary<K, V>
	where K : notnull
{
	private readonly ConcurrentDictionary<K, Lazy<V>> _Dictionary;
	private readonly Func<K, Lazy<V>> _CreateValue;

	/// <exception cref="ArgumentNullException"/>
	public LazyDictionary(Func<K, V> createValue, LazyThreadSafetyMode mode = LazyThreadSafetyMode.PublicationOnly, IEqualityComparer<K>? comparer = null)
	{
		createValue.AssertNotNull();
		this._CreateValue = key => new Lazy<V>(() => createValue(key), mode);
		this._Dictionary = new(comparer);
	}

	/// <exception cref="ArgumentNullException"/>
	public LazyDictionary(Func<K, V> createValue, int concurrencyLevel, int capacity, LazyThreadSafetyMode mode = LazyThreadSafetyMode.PublicationOnly, IEqualityComparer<K>? comparer = null)
	{
		createValue.AssertNotNull();

		this._CreateValue = key => new Lazy<V>(() => createValue(key), mode);
		this._Dictionary = new(concurrencyLevel, capacity, comparer);
	}

	public V this[K key] => this._Dictionary.GetOrAdd(key, this._CreateValue).Value;

	public IEnumerable<K> Keys => this._Dictionary.Keys;

	public IEnumerable<V> Values => this._Dictionary.Values.Select(_ => _.Value);

	public int Count => this._Dictionary.Count;

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public bool ContainsKey(K key)
		=> this._Dictionary.ContainsKey(key);

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public IEnumerator<KeyValuePair<K, V>> GetEnumerator()
		=> this._Dictionary.Select(pair => KeyValuePair.Create(pair.Key, pair.Value.Value)).GetEnumerator();

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	IEnumerator IEnumerable.GetEnumerator()
		=> this._Dictionary.Select(pair => KeyValuePair.Create(pair.Key, pair.Value.Value)).GetEnumerator();

	bool IReadOnlyDictionary<K, V>.TryGetValue(K key, [MaybeNullWhen(false)] out V value)
	{
		var success = this._Dictionary.TryGetValue(key, out var lazy)
			|| this._Dictionary.TryAdd(key, lazy = this._CreateValue(key));
		value = success ? lazy.Value : default;
		return success;
	}
}
