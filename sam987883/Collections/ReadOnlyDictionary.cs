// Copyright (c) 2020 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using static sam987883.Extensions.IEnumerableExtensions;

namespace sam987883.Collections
{
	public class ReadOnlyDictionary<K, V> : ReadOnlyCollection<KeyValuePair<K, V>>, IReadOnlyDictionary<K, V> where K : notnull
	{
		private delegate bool TryGetValueDelegate(K key, out V value);

		private readonly Func<K, bool> _ContainsKey;
		private readonly Func<K, V> _GetValue;
		private readonly TryGetValueDelegate _TryGetValue;

		public ReadOnlyDictionary(IDictionary<K, V> dictionary) : base(dictionary)
		{
			this._ContainsKey = dictionary.ContainsKey;
			this._GetValue = key => dictionary[key];
			this._TryGetValue = dictionary.TryGetValue;
		}

		protected ReadOnlyDictionary(IReadOnlyDictionary<K, V> dictionary) : base(dictionary)
		{
			this._ContainsKey = dictionary.ContainsKey;
			this._GetValue = key => dictionary[key];
			this._TryGetValue = dictionary.TryGetValue;
		}

		public V this[K key] =>
			this._GetValue(key);

		public IEnumerable<K> Keys =>
			this.To(pair => pair.Key);

		public IEnumerable<V> Values =>
			this.To(pair => pair.Value);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool ContainsKey(K key) =>
			this._ContainsKey(key);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool TryGetValue(K key, out V value) =>
			this._TryGetValue(key, out value);
	}
}
