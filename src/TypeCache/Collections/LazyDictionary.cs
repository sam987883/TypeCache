﻿// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Threading;
using TypeCache.Collections.Extensions;
using TypeCache.Extensions;
using static TypeCache.Default;

namespace TypeCache.Collections;

public class LazyDictionary<K, V> : IReadOnlyDictionary<K, V>
	where K : notnull
{
	private readonly ConcurrentDictionary<K, Lazy<V>> _Dictionary;
	private readonly Func<K, Lazy<V>> _CreateValue;

	/// <exception cref="ArgumentNullException"/>
	public LazyDictionary(Func<K, V> createValue, IEqualityComparer<K>? comparer = null)
	{
		createValue.AssertNotNull();

		this._CreateValue = key => new Lazy<V>(() => createValue(key), LazyThreadSafetyMode.ExecutionAndPublication);
		this._Dictionary = new(comparer);
	}

	/// <exception cref="ArgumentNullException"/>
	public LazyDictionary(Func<K, V> createValue, int concurrencyLevel, int capacity, IEqualityComparer<K>? comparer = null)
	{
		createValue.AssertNotNull();

		this._CreateValue = key => new Lazy<V>(() => createValue(key), LazyThreadSafetyMode.ExecutionAndPublication);
		this._Dictionary = new(concurrencyLevel, capacity, comparer);
	}

	public V this[K key] => this._Dictionary.GetOrAdd(key, this._CreateValue).Value;

	public IEnumerable<K> Keys => this._Dictionary.Keys;

	public IEnumerable<V> Values => this._Dictionary.Values.Map(_ => _.Value);

	public int Count => this._Dictionary.Count;

	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public bool ContainsKey(K key)
		=> this._Dictionary.ContainsKey(key);

	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public IEnumerator<KeyValuePair<K, V>> GetEnumerator()
		=> this._Dictionary.Map(pair => KeyValuePair.Create(pair.Key, pair.Value.Value)).GetEnumerator();

	[MethodImpl(METHOD_IMPL_OPTIONS)]
	IEnumerator IEnumerable.GetEnumerator()
		=> this._Dictionary.Map(pair => KeyValuePair.Create(pair.Key, pair.Value.Value)).GetEnumerator();

	bool IReadOnlyDictionary<K, V>.TryGetValue(K key, [MaybeNullWhen(false)] out V value)
	{
		if (this._Dictionary.TryGetValue(key, out var lazy) || this._Dictionary.TryAdd(key, lazy = this._CreateValue(key)))
		{
			value = lazy.Value;
			return true;
		}
		value = default;
		return false;
	}
}
