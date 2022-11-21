// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using TypeCache.Extensions;
using TypeCache.Reflection;
using static TypeCache.Default;

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
		this._CreateValue = key => Lazy.Create(() => createValue(key), mode);
		this._Dictionary = new(comparer);
	}

	/// <exception cref="ArgumentNullException"/>
	public LazyDictionary(Func<K, V> createValue, int concurrencyLevel, int capacity, LazyThreadSafetyMode mode = LazyThreadSafetyMode.PublicationOnly, IEqualityComparer<K>? comparer = null)
	{
		createValue.AssertNotNull();

		this._CreateValue = key => Lazy.Create(() => createValue(key), mode);
		this._Dictionary = new(concurrencyLevel, capacity, comparer);
	}

	public V this[K key] => this._Dictionary.GetOrAdd(key, this._CreateValue).Value;

	public IEnumerable<K> Keys => this._Dictionary.Keys;

	public IEnumerable<V> Values => this._Dictionary.Values.Select(_ => _.Value);

	public int Count => this._Dictionary.Count;

	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public bool ContainsKey(K key)
		=> this._Dictionary.ContainsKey(key);

	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public IEnumerator<KeyValuePair<K, V>> GetEnumerator()
		=> this._Dictionary.Select(pair => KeyValuePair.Create(pair.Key, pair.Value.Value)).GetEnumerator();

	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
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
