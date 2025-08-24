// Copyright (c) 2021 Samuel Abraham

using System.Collections.ObjectModel;
using TypeCache.Collections;
using TypeCache.Reflection;

namespace TypeCache.Extensions;

public static class DictionaryExtensions
{
	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.TryGetValue(<paramref name="key"/>, <see langword="out var"/> value) ? value : <paramref name="defaultValue"/>;</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static V? GetValue<K, V>(this IReadOnlyDictionary<K, V> @this, K key, V? defaultValue)
		where K : notnull
		=> @this.TryGetValue(key, out var value) ? value : defaultValue;

	/// <exception cref="ArgumentNullException"/>
	public static bool If<K, V>(this IDictionary<K, V> @this, K key, Action action)
		where K : notnull
	{
		@this.ThrowIfNull();

		var success = @this.ContainsKey(key);
		if (success)
			action();

		return success;
	}

	/// <exception cref="ArgumentNullException"/>
	public static bool If<K, V>(this IDictionary<K, V> @this, K key, Action<KeyValuePair<K, V>> action)
		where K : notnull
	{
		@this.ThrowIfNull();

		var success = @this.TryGetValue(key, out var value);
		if (success)
			action(KeyValuePair.Create(key, value!));

		return success;
	}

	/// <exception cref="ArgumentNullException"/>
	public static IDictionary<string, object?> Map(this IEnumerable<KeyValuePair<string, object?>> @this, IDictionary<string, object?> target)
	{
		@this.ThrowIfNull();
		target.ThrowIfNull();

		foreach (var pair in @this)
			if (pair.Value is not null)
				target[pair.Key] = pair.Value;

		return target;
	}

	/// <exception cref="ArgumentNullException"/>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static IDictionary<string, object?> Map(this IDictionary<string, object?> @this, IDictionary<string, object?> target)
		=> (@this as IEnumerable<KeyValuePair<string, object?>>).Map(target);

	/// <exception cref="ArgumentNullException"/>
	/// <exception cref="ArgumentOutOfRangeException"/>
	public static void MapFields(this IEnumerable<KeyValuePair<string, object?>> @this, object target)
	{
		@this.ThrowIfNull();
		target.ThrowIfNull();

		target.GetType().CollectionType().ThrowIfNotEqual(CollectionType.None, Invariant($"{nameof(MapFields)}: Cannot map to a collection - {target.GetType().CollectionType().Name()}"));

		var targetFields = target.GetType().Fields();
		foreach (var pair in @this)
		{
			if (!targetFields.TryGetValue(pair.Key, out var field))
				continue;

			if (pair.Value is null && !field.FieldType.IsNullable())
				continue;

			field.SetValue(target, pair.Value);
		}
	}

	/// <exception cref="ArgumentNullException"/>
	/// <exception cref="ArgumentOutOfRangeException"/>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static void MapFields(this IDictionary<string, object?> @this, object target)
		=> (@this as IEnumerable<KeyValuePair<string, object?>>).MapFields(target);

	/// <exception cref="ArgumentNullException"/>
	/// <exception cref="ArgumentOutOfRangeException"/>
	public static void MapProperties(this IEnumerable<KeyValuePair<string, object?>> @this, object target)
	{
		@this.ThrowIfNull();
		target.ThrowIfNull();

		target.GetType().CollectionType().ThrowIfNotEqual(CollectionType.None, Invariant($"{nameof(MapProperties)}: Cannot map to a collection - {target.GetType().CollectionType().Name()}"));

		var targetProperties = target.GetType().Properties();
		foreach (var pair in @this)
		{
			if (!targetProperties.TryGetValue(pair.Key, out var property))
				continue;

			if (!property.CanWrite)
				continue;

			if (pair.Value is null && !property.PropertyType.IsNullable())
				continue;

			property.SetValue(target, pair.Value);
		}
	}

	/// <exception cref="ArgumentNullException"/>
	/// <exception cref="ArgumentOutOfRangeException"/>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static void MapProperties(this IDictionary<string, object?> @this, object target)
		=> (@this as IEnumerable<KeyValuePair<string, object?>>).MapProperties(target);

	/// <inheritdoc cref="ReadOnlyDictionary{TKey, TValue}.ReadOnlyDictionary(IDictionary{TKey, TValue})"/>
	/// <remarks>
	/// <c>=&gt; <see langword="new"/> ReadOnlyDictionary&lt;<typeparamref name="K"/>, <typeparamref name="V"/>&gt;(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static IReadOnlyDictionary<K, V> ToLazyReadOnly<K, V>(this IReadOnlyDictionary<K, Lazy<V>> @this)
		where K : notnull
		=> new LazyReadOnlyDictionary<K, V>(@this);

	/// <inheritdoc cref="ReadOnlyDictionary{TKey, TValue}.ReadOnlyDictionary(IDictionary{TKey, TValue})"/>
	/// <remarks>
	/// <c>=&gt; <see langword="new"/> ReadOnlyDictionary&lt;<typeparamref name="K"/>, <typeparamref name="V"/>&gt;(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static IReadOnlyDictionary<K, V> ToReadOnly<K, V>(this IDictionary<K, V> @this)
		where K : notnull
		=> new ReadOnlyDictionary<K, V>(@this);
}
