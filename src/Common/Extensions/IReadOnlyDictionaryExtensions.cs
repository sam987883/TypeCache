// Copyright (c) 2020 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Sam987883.Common.Extensions
{
	public static class IReadOnlyDictionaryExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static (V Value, bool Exists) Get<K, V>(this IReadOnlyDictionary<K, V> @this, K key) where K : notnull
			=> @this.TryGetValue(key, out var value) ? (value, true) : default;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static (T Value, bool Exists) Get<K, V, T>(this IReadOnlyDictionary<K, V> @this, K key, Func<V, T> map) where K : notnull
			=> @this.TryGetValue(key, out var value) ? (map(value), true) : default;

		public static IEnumerable<V> GetValues<K, V>(this IReadOnlyDictionary<K, V> @this, params K[] keys) where K : notnull
		{
			if (keys.Any())
			{
				for (var i = 0; i < keys.Length; ++i)
					if (@this.TryGetValue(keys[i], out var value))
						yield return value;
			}
		}

		public static IEnumerable<V> GetValues<K, V>(this IReadOnlyDictionary<K, V> @this, IEnumerable<K> keys) where K : notnull
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
