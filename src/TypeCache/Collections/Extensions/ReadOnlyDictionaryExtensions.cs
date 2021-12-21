// Copyright (c) 2021 Samuel Abraham

using System.Collections.Generic;
using System.Runtime.CompilerServices;
using static TypeCache.Default;

namespace TypeCache.Collections.Extensions;

public static class ReadOnlyDictionaryExtensions
{
	/// <summary>
	/// <code>
	/// <see langword="if"/> (!<paramref name="keys"/>.Any())<br/>
	/// <see langword="    yield break"/>;<br/>
	/// <br/>
	/// <see langword="for"/> (<see langword="var"/> i = 0; i &lt; <paramref name="keys"/>.Length; ++i)<br/>
	/// <see langword="    if"/> (@<paramref name="this"/>.TryGetValue(<paramref name="keys"/>[i], <see langword="out var"/> value))<br/>
	/// <see langword="        yield return"/> value;
	/// </code>
	/// </summary>
	public static IEnumerable<V> Get<K, V>(this IReadOnlyDictionary<K, V> @this, params K[] keys)
		where K : notnull
	{
		if (!keys.Any())
			yield break;

		for (var i = 0; i < keys.Length; ++i)
			if (@this.TryGetValue(keys[i], out var value))
				yield return value;
	}

	/// <summary>
	/// <code>
	/// <see langword="if"/> (!<paramref name="keys"/>.Any())<br/>
	/// <see langword="    yield break"/>;<br/>
	/// <br/>
	/// <see langword="for"/> (<see langword="var"/> key <see langword="in"/> <paramref name="keys"/>)<br/>
	/// <see langword="    if"/> (@<paramref name="this"/>.TryGetValue(key, <see langword="out var"/> value))<br/>
	/// <see langword="        yield return"/> value;
	/// </code>
	/// </summary>
	public static IEnumerable<V> Get<K, V>(this IReadOnlyDictionary<K, V> @this, IEnumerable<K> keys)
		where K : notnull
	{
		if (!keys.Any())
			yield break;

		foreach (var key in keys)
			if (@this.TryGetValue(key, out var value))
				yield return value;
	}
}
