// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using TypeCache.Extensions;
using static TypeCache.Default;

namespace TypeCache.Collections.Extensions;

public static class DictionaryExtensions
{
	/// <summary>
	/// <code>
	/// <see langword="if"/> (!<paramref name="keys"/>.Any())<br/>
	/// <see langword="    yield break;"/> value;<br/>
	/// <br/>
	/// <see langword="for"/> (<see langword="var"/> i = 0; i &lt; <paramref name="keys"/>.Length; ++i)<br/>
	/// <see langword="    if"/> (@<paramref name="this"/>.TryGetValue(<paramref name="keys"/>[i], <see langword="out var"/> value))<br/>
	/// <see langword="        yield return"/> value;
	/// </code>
	/// </summary>
	public static IEnumerable<V> Get<K, V>(this IDictionary<K, V> @this, params K[] keys)
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
	/// <see langword="    yield break;"/> value;<br/>
	/// <br/>
	/// <see langword="for"/> (<see langword="var"/> i = 0; i &lt; <paramref name="keys"/>.Length; ++i)<br/>
	/// <see langword="    if"/> (@<paramref name="this"/>.TryGetValue(<paramref name="keys"/>[i], <see langword="out var"/> value))<br/>
	/// <see langword="        yield return"/> value;
	/// </code>
	/// </summary>
	public static IEnumerable<V> Get<K, V>(this IDictionary<K, V> @this, IEnumerable<K> keys)
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
	public static IDictionary<K, (V1, V2)> Match<K, V1, V2>(this (IDictionary<K, V1>, IDictionary<K, V2>) @this, IEqualityComparer<K>? comparer)
		where K : notnull
		=> @this.Item1.Keys.Match(@this.Item2.Keys, comparer).To(key => KeyValuePair.Create(key, (@this.Item1[key], @this.Item2[key]))).ToDictionary(comparer);

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.Match(<paramref name="comparison"/>.ToStringComparer());</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static IDictionary<string, (V1, V2)> Match<V1, V2>(this (IDictionary<string, V1>, IDictionary<string, V2>) @this, StringComparison comparison)
		=> @this.Match(comparison.ToStringComparer());

	/// <summary>
	/// <c>=&gt; <see langword="new"/> ReadOnlyDictionary&lt;<typeparamref name="K"/>, <typeparamref name="V"/>&gt;(@<paramref name="this"/>);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static IReadOnlyDictionary<K, V> ToReadOnly<K, V>(this IDictionary<K, V> @this)
		where K : notnull
		=> new ReadOnlyDictionary<K, V>(@this);
}
