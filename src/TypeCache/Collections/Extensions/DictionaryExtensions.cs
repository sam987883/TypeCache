// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using TypeCache.Extensions;
using static TypeCache.Default;

namespace TypeCache.Collections.Extensions
{
	public static class DictionaryExtensions
	{
		/// <summary>
		/// <c><see cref="IDictionary{K, V}.TryGetValue(K, out V)"/> ? value : null</c>
		/// </summary>
		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static V? Get<K, V>(this IDictionary<K, V> @this, K key)
			where K : notnull
			where V : class
			=> @this.TryGetValue(key, out var value) ? value : null;

		/// <summary>
		/// <c><see cref="IDictionary{K, V}.TryGetValue(K, out V)"/> ? value : null</c>
		/// </summary>
		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static V? GetValue<K, V>(this IDictionary<K, V> @this, K key)
			where K : notnull
			where V : struct
			=> @this.TryGetValue(key, out var value) ? (V?)value : null;

		public static IEnumerable<V> GetValues<K, V>(this IDictionary<K, V> @this, params K[] keys)
			where K : notnull
		{
			if (keys is not null)
			{
				for (var i = 0; i < keys.Length; ++i)
					if (@this.TryGetValue(keys[i], out var value))
						yield return value;
			}
		}

		public static IEnumerable<V> GetValues<K, V>(this IDictionary<K, V> @this, IEnumerable<K> keys)
			where K : notnull
		{
			if (keys is not null)
			{
				foreach (var key in keys)
					if (@this.TryGetValue(key, out var value))
						yield return value;
			}
		}

		public static IDictionary<K, (V1, V2)> Match<K, V1, V2>(this (IDictionary<K, V1>, IDictionary<K, V2>) @this, IEqualityComparer<K>? comparer)
			where K : notnull
			=> @this.Item1.Keys.Match(@this.Item2.Keys, comparer).To(key => KeyValuePair.Create(key, (@this.Item1[key], @this.Item2[key]))).ToDictionary(comparer);

		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static IDictionary<string, (V1, V2)> Match<V1, V2>(this (IDictionary<string, V1>, IDictionary<string, V2>) @this, StringComparison comparison)
			=> @this.Match(comparison.ToStringComparer());

		/// <summary>
		/// <c>new <see cref="ReadOnlyDictionary{K, V}"/>(@<paramref name="this"/>)</c>
		/// </summary>
		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static IReadOnlyDictionary<K, V> ToReadOnly<K, V>(this IDictionary<K, V> @this)
			where K : notnull
			=> new ReadOnlyDictionary<K, V>(@this);
	}
}
