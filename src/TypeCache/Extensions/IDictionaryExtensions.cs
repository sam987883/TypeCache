// Copyright (c) 2021 Samuel Abraham

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;

namespace TypeCache.Extensions
{
	public static class IDictionaryExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static IEnumerable<T> Empty<T>()
		{
			yield break;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static V? Get<K, V>(this IDictionary<K, V> @this, K key)
			where K : notnull
			where V : class
			=> @this.TryGetValue(key, out var value) ? value : null;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static V? GetValue<K, V>(this IDictionary<K, V> @this, K key)
			where K : notnull
			where V : struct
			=> @this.TryGetValue(key, out var value) ? (V?)value : null;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IEnumerable<V> GetValues<K, V>(this IDictionary<K, V> @this, params K[] keys)
			where K : notnull
			=> keys.Any() ? @this.GetValues((IEnumerable<K>)keys) : Empty<V>();

		public static IEnumerable<V> GetValues<K, V>(this IDictionary<K, V> @this, IEnumerable<K> keys)
			where K : notnull
		{
			if (keys.Any())
			{
				foreach (var key in keys.ToCustomEnumerable())
					if (@this.TryGetValue(key, out var value))
						yield return value;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IReadOnlyDictionary<K, V> ToReadOnly<K, V>(this IDictionary<K, V> @this)
			where K : notnull
			=> new ReadOnlyDictionary<K, V>(@this);
	}
}
