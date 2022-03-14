// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using TypeCache.Extensions;
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

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.Item1.Keys.Match(@<paramref name="this"/>.Item2.Keys, <paramref name="comparer"/>).To(key =&gt; <see cref="KeyValuePair"/>.Create(key, (@<paramref name="this"/>.Item1[key], @<paramref name="this"/>.Item2[key]))).ToDictionary(<paramref name="comparer"/>);</c>
	/// </summary>
	public static IDictionary<K, (V1, V2)> Match<K, V1, V2>(this (IReadOnlyDictionary<K, V1>, IReadOnlyDictionary<K, V2>) @this, IEqualityComparer<K>? comparer)
		where K : notnull
		=> @this.Item1.Keys.Match(@this.Item2.Keys, comparer).Map(key => KeyValuePair.Create(key, (@this.Item1[key], @this.Item2[key]))).ToDictionary(comparer);

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.Match(<paramref name="comparison"/>.ToStringComparer());</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static IDictionary<string, (V1, V2)> Match<V1, V2>(this (IReadOnlyDictionary<string, V1>, IReadOnlyDictionary<string, V2>) @this, StringComparison comparison)
		=> @this.Match(comparison.ToStringComparer());
}
