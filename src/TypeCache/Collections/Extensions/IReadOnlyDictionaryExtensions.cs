// Copyright (c) 2021 Samuel Abraham

using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace TypeCache.Collections.Extensions
{
	public static class IReadOnlyDictionaryExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static IEnumerable<T> Empty<T>()
		{
			yield break;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static V? Get<K, V>(this IReadOnlyDictionary<K, V> @this, K key)
			where K : notnull
			where V : class
			=> @this.TryGetValue(key, out var value) ? value : null;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static V? GetValue<K, V>(this IReadOnlyDictionary<K, V> @this, K key)
			where K : notnull
			where V : struct
			=> @this.TryGetValue(key, out var value) ? (V?)value : null;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IEnumerable<V> GetValues<K, V>(this IReadOnlyDictionary<K, V> @this, params K[] keys)
			where K : notnull
			=> keys.Any() ? @this.GetValues((IEnumerable<K>)keys) : Empty<V>();

		public static IEnumerable<V> GetValues<K, V>(this IReadOnlyDictionary<K, V> @this, IEnumerable<K> keys)
			where K : notnull
		{
			if (keys.Any())
			{
				foreach (var key in keys.ToCustomEnumerable())
					if (@this.TryGetValue(key, out var value))
						yield return value;
			}
		}
	}
}
