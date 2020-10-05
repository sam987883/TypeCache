// Copyright (c) 2020 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;

namespace Sam987883.Common.Extensions
{
	public static class IDictionaryExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static (V Value, bool Exists) Get<K, V>(this IDictionary<K, V> @this, K key) where K : notnull
			=> @this.TryGetValue(key, out var value) ? (value, true) : default;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static (T Value, bool Exists) Get<K, V, T>(this IDictionary<K, V> @this, K key, Func<V, T> map) where K : notnull
			=> @this.TryGetValue(key, out var value) ? (map(value), true) : default;

		public static IEnumerable<V> GetValues<K, V>(this IDictionary<K, V> @this, params K[] keys) where K : notnull
		{
			if (keys.Any())
			{
				for (var i = 0; i < keys.Length; ++i)
				{
					if (@this.TryGetValue(keys[i], out var value))
						yield return value;
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IReadOnlyDictionary<K, V> ToReadOnly<K, V>(this IDictionary<K, V> @this) where K : notnull
			=> new ReadOnlyDictionary<K, V>(@this);
	}
}
