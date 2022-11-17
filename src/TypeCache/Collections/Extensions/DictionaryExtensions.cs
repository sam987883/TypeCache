// Copyright (c) 2021 Samuel Abraham

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using static TypeCache.Default;

namespace TypeCache.Collections.Extensions;

public static class DictionaryExtensions
{
	/// <summary>
	/// <code>
	/// {<br/>
	/// <see langword="    if"/> (!<paramref name="keys"/>.Any())<br/>
	/// <see langword="        yield break;"/> value;<br/>
	/// <br/>
	/// <see langword="    var"/> count = <paramref name="keys"/>.Length;<br/>
	/// <see langword="    for"/> (<see langword="var"/> i = 0; i &lt; count; ++i)<br/>
	/// <see langword="        if"/> (@<paramref name="this"/>.TryGetValue(<paramref name="keys"/>[i], <see langword="out var"/> value))<br/>
	/// <see langword="            yield return"/> value;<br/>
	/// }
	/// </code>
	/// </summary>
	public static IEnumerable<V> Get<K, V>(this IDictionary<K, V> @this, params K[] keys)
		where K : notnull
	{
		if (!keys.Any())
			yield break;

		var count = keys.Length;
		for (var i = 0; i < count; ++i)
			if (@this.TryGetValue(keys[i], out var value))
				yield return value;
	}

	/// <summary>
	/// <code>
	/// {<br/>
	/// <see langword="    if"/> (!<paramref name="keys"/>.Any())<br/>
	/// <see langword="        yield break;"/> value;<br/>
	/// <br/>
	/// <see langword="    foreach"/> (<see langword="var"/> key <see langword="in"/> <paramref name="keys"/>)<br/>
	/// <see langword="        if"/> (@<paramref name="this"/>.TryGetValue(key, <see langword="out var"/> value))<br/>
	/// <see langword="            yield return"/> value;
	/// }
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
	/// <code>
	/// {<br/>
	/// <see langword="    foreach"/> (<see langword="var"/> pair <see langword="in"/> <paramref name="pairs"/>)<br/>
	/// <see langword="        if"/> (@<paramref name="this"/>.TryGetValue(pair.Key, <see langword="out var"/> value))<br/>
	/// <see langword="            yield return"/> (pair.Key, value, pair.Value);<br/>
	/// }
	/// </code>
	/// </summary>
	public static IEnumerable<(K Key, V1 Value1, V2 Value2)> Match<K, V1, V2>(this IDictionary<K, V1> @this, IEnumerable<KeyValuePair<K, V2>> pairs)
		where K : notnull
	{
		foreach (var pair in pairs)
			if (@this.TryGetValue(pair.Key, out var value))
				yield return (pair.Key, value, pair.Value);
	}

	/// <summary>
	/// <c>=&gt; <see langword="new"/> ReadOnlyDictionary&lt;<typeparamref name="K"/>, <typeparamref name="V"/>&gt;(@<paramref name="this"/>);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static IReadOnlyDictionary<K, V> ToReadOnly<K, V>(this IDictionary<K, V> @this)
		where K : notnull
		=> new ReadOnlyDictionary<K, V>(@this);
}
