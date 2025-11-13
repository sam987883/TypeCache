// Copyright (c) 2021 Samuel Abraham

using System.Collections.ObjectModel;
using TypeCache.Collections;
using TypeCache.Reflection;

namespace TypeCache.Extensions;

public static class DictionaryExtensions
{
	extension<K, V>(IDictionary<K, V> @this) where K : notnull
	{
		/// <exception cref="ArgumentNullException"/>
		public bool If(K key, Action action)
		{
			@this.ThrowIfNull();

			var success = @this.ContainsKey(key);
			if (success)
				action();

			return success;
		}

		/// <exception cref="ArgumentNullException"/>
		public bool If(K key, Action<K, V> action)
		{
			@this.ThrowIfNull();

			var success = @this.TryGetValue(key, out var value);
			if (success)
				action(key, value!);

			return success;
		}

		/// <inheritdoc cref="ReadOnlyDictionary{TKey, TValue}.ReadOnlyDictionary(IDictionary{TKey, TValue})"/>
		/// <remarks>
		/// <c>=&gt; <see langword="new"/> ReadOnlyDictionary&lt;<typeparamref name="K"/>, <typeparamref name="V"/>&gt;(@this);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public IReadOnlyDictionary<K, V> ToReadOnly()
			=> new ReadOnlyDictionary<K, V>(@this);
	}

	extension(IDictionary<string, object?> @this)
	{
		/// <exception cref="ArgumentNullException"/>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public IDictionary<string, object?> Map(IDictionary<string, object?> target)
			=> (@this as IEnumerable<KeyValuePair<string, object?>>).Map(target);

		/// <exception cref="ArgumentNullException"/>
		/// <exception cref="ArgumentOutOfRangeException"/>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public void MapFields(object target)
			=> (@this as IEnumerable<KeyValuePair<string, object?>>).MapFields(target);

		/// <exception cref="ArgumentNullException"/>
		/// <exception cref="ArgumentOutOfRangeException"/>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public void MapProperties(object target)
			=> (@this as IEnumerable<KeyValuePair<string, object?>>).MapProperties(target);
	}

	extension(IEnumerable<KeyValuePair<string, object?>> @this)
	{
		/// <exception cref="ArgumentNullException"/>
		public IDictionary<string, object?> Map(IDictionary<string, object?> target)
		{
			@this.ThrowIfNull();
			target.ThrowIfNull();

			foreach (var pair in @this)
				if (pair.Value is not null)
					target[pair.Key] = pair.Value;

			return target;
		}

		/// <exception cref="ArgumentNullException"/>
		/// <exception cref="ArgumentOutOfRangeException"/>
		public void MapFields(object target)
		{
			@this.ThrowIfNull();
			target.ThrowIfNull();

			target.GetType().CollectionType.ThrowIfNotEqual(CollectionType.None,
				() => Invariant($"{nameof(MapFields)}: Cannot map to a collection - {target.GetType().CollectionType.Name}"));

			var targetFields = target.GetType().Fields;
			foreach (var pair in @this)
			{
				if (!targetFields.TryGetValue(pair.Key, out var field))
					continue;

				if (pair.Value is null && !field.FieldType.IsNullable)
					continue;

				field.SetValue(target, pair.Value);
			}
		}

		/// <exception cref="ArgumentNullException"/>
		/// <exception cref="ArgumentOutOfRangeException"/>
		public void MapProperties(object target)
		{
			@this.ThrowIfNull();
			target.ThrowIfNull();

			target.GetType().CollectionType.ThrowIfNotEqual(CollectionType.None,
				() => Invariant($"{nameof(MapProperties)}: Cannot map to a collection - {target.GetType().CollectionType.Name}"));

			var targetProperties = target.GetType().Properties;
			foreach (var pair in @this)
			{
				if (!targetProperties.TryGetValue(pair.Key, out var property))
					continue;

				if (!property.CanWrite)
					continue;

				if (pair.Value is null && !property.PropertyType.IsNullable)
					continue;

				property.SetValue(target, pair.Value);
			}
		}
	}

	extension<K, V>(IReadOnlyDictionary<K, V> @this) where K : notnull
	{
		/// <remarks>
		/// <c>=&gt; @this.TryGetValue(<paramref name="key"/>, <see langword="out var"/> value) ? value : <paramref name="defaultValue"/>;</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public V? GetValue(K key, V? defaultValue)
			=> @this.TryGetValue(key, out var value) ? value : defaultValue;
	}

	extension<K, V>(IReadOnlyDictionary<K, Lazy<V>> @this) where K : notnull
	{
		/// <inheritdoc cref="ReadOnlyDictionary{TKey, TValue}.ReadOnlyDictionary(IDictionary{TKey, TValue})"/>
		/// <remarks>
		/// <c>=&gt; <see langword="new"/> ReadOnlyDictionary&lt;<typeparamref name="K"/>, <typeparamref name="V"/>&gt;(@this);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public IReadOnlyDictionary<K, V> ToLazyReadOnly()
			=> new LazyReadOnlyDictionary<K, V>(@this);
	}
}
