// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace TypeCache.Collections
{
	public class LazyDictionary<K, V> : IReadOnlyDictionary<K, V>
		where K : notnull
	{
		private readonly ConcurrentDictionary<K, V> _Dictionary;
		private readonly Func<K, V> _CreateValue;

		public LazyDictionary(Func<K, V> createValue)
		{
			this._CreateValue = createValue;
			this._Dictionary = new ConcurrentDictionary<K, V>();
		}

		public LazyDictionary(Func<K, V> createValue, IEqualityComparer<K> comparer)
		{
			this._CreateValue = createValue;
			this._Dictionary = new ConcurrentDictionary<K, V>(comparer);
		}

		public LazyDictionary(Func<K, V> createValue, int concurrencyLevel, int capacity)
		{
			this._CreateValue = createValue;
			this._Dictionary = new ConcurrentDictionary<K, V>(concurrencyLevel, capacity);
		}

		public LazyDictionary(Func<K, V> createValue, int concurrencyLevel, int capacity, IEqualityComparer<K> comparer)
		{
			this._CreateValue = createValue;
			this._Dictionary = new ConcurrentDictionary<K, V>(concurrencyLevel, capacity, comparer);
		}

		public V this[K key] => this._Dictionary.GetOrAdd(key, this._CreateValue);

		public IEnumerable<K> Keys => this._Dictionary.Keys;

		public IEnumerable<V> Values => this._Dictionary.Values;

		public int Count => this._Dictionary.Count;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool ContainsKey(K key)
			=> this._Dictionary.ContainsKey(key);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public IEnumerator<KeyValuePair<K, V>> GetEnumerator()
			=> this._Dictionary.GetEnumerator();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		IEnumerator IEnumerable.GetEnumerator()
			=> this._Dictionary.GetEnumerator();

		bool IReadOnlyDictionary<K, V>.TryGetValue(K key, [MaybeNullWhen(false)] out V value)
			=> this._Dictionary.TryGetValue(key, out value) || this._Dictionary.TryAdd(key, value = this._CreateValue(key));
	}
}
