// Copyright (c) 2021 Samuel Abraham

using System.Collections.ObjectModel;

namespace TypeCache.Extensions;

public static class DictionaryExtensions
{
	/// <exception cref="ArgumentNullException"/>
	public static bool If<K, V>(this IDictionary<K, V> @this, K key, Action action)
		where K : notnull
	{
		@this.AssertNotNull();

		var success = @this.ContainsKey(key);
		if (success)
			action();

		return success;
	}

	/// <exception cref="ArgumentNullException"/>
	public static bool If<K, V>(this IDictionary<K, V> @this, K key, Action<V> action)
		where K : notnull
	{
		@this.AssertNotNull();

		var success = @this.TryGetValue(key, out var value);
		if (success)
			action(value!);

		return success;
	}

	/// <exception cref="ArgumentNullException"/>
	public static bool If<K, V>(this IDictionary<K, V> @this, K key, Action<K, V> action)
		where K : notnull
	{
		@this.AssertNotNull();

		var success = @this.TryGetValue(key, out var value);
		if (success)
			action(key, value!);

		return success;
	}

	/// <inheritdoc cref="ReadOnlyDictionary{TKey, TValue}.ReadOnlyDictionary(IDictionary{TKey, TValue})"/>
	/// <remarks>
	/// <c>=&gt; <see langword="new"/> ReadOnlyDictionary&lt;<typeparamref name="K"/>, <typeparamref name="V"/>&gt;(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static IReadOnlyDictionary<K, V> ToReadOnly<K, V>(this IDictionary<K, V> @this)
		where K : notnull
		=> new ReadOnlyDictionary<K, V>(@this);
}
