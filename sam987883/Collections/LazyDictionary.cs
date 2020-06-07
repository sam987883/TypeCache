// Copyright (c) 2020 Samuel Abraham

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace sam987883.Collections
{
	public class LazyDictionary<K, V> : IReadOnlyDictionary<K, V> where K : notnull
	{
		private readonly ConcurrentDictionary<K, V> _Dictionary;
		private readonly Func<K, V> _CreateValue;

		public LazyDictionary(Func<K, V> createValue, int concurrencyLevel, int capacity, IEqualityComparer<K>? comparer = null)
		{
			this._CreateValue = createValue;
			this._Dictionary = new ConcurrentDictionary<K, V>(concurrencyLevel, capacity, comparer ?? EqualityComparer<K>.Default);
		}

		public LazyDictionary(Func<K, V> createValue, IEqualityComparer<K>? comparer = null)
		{
			this._CreateValue = createValue;
			this._Dictionary = new ConcurrentDictionary<K, V>(comparer ?? EqualityComparer<K>.Default);
		}

		V IReadOnlyDictionary<K, V>.this[K key] => this._Dictionary.GetOrAdd(key, this._CreateValue);
		IEnumerable<K> IReadOnlyDictionary<K, V>.Keys => this._Dictionary.Keys;
		IEnumerable<V> IReadOnlyDictionary<K, V>.Values => this._Dictionary.Values;
		int IReadOnlyCollection<KeyValuePair<K, V>>.Count => this._Dictionary.Count;
		bool IReadOnlyDictionary<K, V>.ContainsKey(K key) => this._Dictionary.ContainsKey(key);
		IEnumerator<KeyValuePair<K, V>> IEnumerable<KeyValuePair<K, V>>.GetEnumerator() => this._Dictionary.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => this._Dictionary.GetEnumerator();
		bool IReadOnlyDictionary<K, V>.TryGetValue(K key, out V value) => this._Dictionary.TryGetValue(key, out value) || this._Dictionary.TryAdd(key, value = this._CreateValue(key));
	}
}
