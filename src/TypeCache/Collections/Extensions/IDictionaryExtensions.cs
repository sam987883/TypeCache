// Copyright (c) 2021 Samuel Abraham

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;

namespace TypeCache.Collections.Extensions
{
	public static class IDictionaryExtensions
	{
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

		public static IEnumerable<V> GetValues<K, V>(this IDictionary<K, V> @this, params K[] keys)
			where K : notnull
		{
			if (keys != null)
			{
				for (var i = 0; i < keys.Length; ++i)
					if (@this.TryGetValue(keys[i], out var value))
						yield return value;
			}
		}

		public static IEnumerable<V> GetValues<K, V>(this IDictionary<K, V> @this, IEnumerable<K> keys)
			where K : notnull
		{
			if (keys != null)
			{
				foreach (var key in keys)
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
