﻿// Copyright (c) 2021 Samuel Abraham

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;

namespace TypeCache.Collections.Extensions
{
	public static class IDictionaryExtensions
	{
		/// <summary>
		/// <c><see cref="IDictionary{K, V}.TryGetValue(K, out V)"/> ? value : null</c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static V? Get<K, V>(this IDictionary<K, V> @this, K key)
			where K : notnull
			where V : class
			=> @this.TryGetValue(key, out var value) ? value : null;

		/// <summary>
		/// <c><see cref="IDictionary{K, V}.TryGetValue(K, out V)"/> ? value : null</c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
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

		/// <summary>
		/// <c>new <see cref="ReadOnlyDictionary{K, V}"/>(@<paramref name="this"/>)</c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IReadOnlyDictionary<K, V> ToReadOnly<K, V>(this IDictionary<K, V> @this)
			where K : notnull
			=> new ReadOnlyDictionary<K, V>(@this);
	}
}
