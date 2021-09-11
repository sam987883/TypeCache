// Copyright (c) 2021 Samuel Abraham

using System.Collections.Generic;
using System.Runtime.CompilerServices;
using static TypeCache.Default;

namespace TypeCache.Collections.Extensions
{
	public static class IReadOnlyDictionaryExtensions
	{
		/// <summary>
		/// <c><see cref="IReadOnlyDictionary{K, V}.TryGetValue(K, out V)"/> ? value : null</c>
		/// </summary>
		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static V? Get<K, V>(this IReadOnlyDictionary<K, V> @this, K key)
			where K : notnull
			where V : class
			=> @this.TryGetValue(key, out var value) ? value : null;

		/// <summary>
		/// <c><see cref="IReadOnlyDictionary{K, V}.TryGetValue(K, out V)"/> ? value : null</c>
		/// </summary>
		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static V? GetValue<K, V>(this IReadOnlyDictionary<K, V> @this, K key)
			where K : notnull
			where V : struct
			=> @this.TryGetValue(key, out var value) ? (V?)value : null;

		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static IEnumerable<V> GetValues<K, V>(this IReadOnlyDictionary<K, V> @this, params K[] keys)
			where K : notnull
		{
			if (keys.Any())
			{
				for (var i = 0; i < keys.Length; ++i)
					if (@this.TryGetValue(keys[i], out var value))
						yield return value;
			}
		}

		public static IEnumerable<V> GetValues<K, V>(this IReadOnlyDictionary<K, V> @this, IEnumerable<K> keys)
			where K : notnull
		{
			if (keys.Any())
			{
				foreach (var key in keys)
					if (@this.TryGetValue(key, out var value))
						yield return value;
			}
		}
	}
}
