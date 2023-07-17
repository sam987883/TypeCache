// Copyright (c) 2021 Samuel Abraham

using System.Reflection;
using TypeCache.Attributes;
using TypeCache.Extensions;
using static System.Reflection.BindingFlags;

namespace TypeCache.Extensions;

public static partial class MapExtensions
{
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static T Map<T>(this bool @this, T trueValue, T falseValue)
		=> @this ? trueValue : falseValue;

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static T Map<T>(this bool @this, Func<T> trueValue, T falseValue)
		=> @this ? trueValue() : falseValue;

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static T Map<T>(this bool @this, Func<T> trueValue, Func<T> falseValue)
		=> @this ? trueValue() : falseValue();

	/// <exception cref="ArgumentNullException"/>
	public static T? Map<T, K, V>(this IDictionary<K, V> @this, K key, Func<V, T> map, T? keyNotFoundValue = default)
		where K : notnull
	{
		@this.AssertNotNull();

		return @this.TryGetValue(key, out var value) ? map(value) : keyNotFoundValue;
	}

	/// <exception cref="ArgumentNullException"/>
	public static T? Map<T, K, V>(this IDictionary<K, V> @this, K key, Func<K, V, T> map, T? keyNotFoundValue = default)
		where K : notnull
	{
		@this.AssertNotNull();

		return @this.TryGetValue(key, out var value) ? map(key, value) : keyNotFoundValue;
	}

	/// <exception cref="ArgumentNullException"/>
	public static T MapTo<T>(this IEnumerable<KeyValuePair<string, object?>> @this, T target, bool ignoreCase = true)
		where T : notnull
	{
		@this.AssertNotNull();
		target.AssertNotNull();

		var targetType = target.GetType();
		var bindings = ignoreCase switch
		{
			true => FlattenHierarchy | Instance | Public | IgnoreCase,
			_ => FlattenHierarchy | Instance | Public
		};

		foreach (var pair in @this)
		{
			var targetPropertyInfo = targetType.GetProperty(pair.Key, bindings);
			if (targetPropertyInfo?.CanWrite is not true)
				continue;

			if (pair.Value is null && !targetPropertyInfo.PropertyType.IsNullable())
				continue;

			if (pair.Value is not null && !pair.Value.GetType().IsConvertibleTo(targetPropertyInfo.PropertyType))
				continue;

			targetPropertyInfo.SetPropertyValue(target, pair.Value);
		}

		return target;
	}

	/// <exception cref="ArgumentException"/>
	/// <exception cref="ArgumentNullException"/>
	/// <exception cref="ArgumentOutOfRangeException"/>
	public static T MapTo<T>(this T @this, T target)
		where T : notnull
	{
		@this.AssertNotNull();
		target.AssertNotNull();
		(target, @this).AssertNotSame();

		var properties = typeof(T).GetProperties(FlattenHierarchy | Instance | Public);
		foreach (var propertyInfo in properties)
		{
			if (!propertyInfo.CanRead)
				continue;

			var mapAttributes = propertyInfo.GetCustomAttributes<MapAttribute>()
				.Where(_ => _.Type == typeof(T))
				.ToArray();
			var ignoreAttributes = propertyInfo.GetCustomAttributes<MapIgnoreAttribute>()
				.Where(_ => _.Type is null || _.Type == typeof(T))
				.ToArray();
			if (!mapAttributes.Any() && !ignoreAttributes.Any())
			{
				var value = propertyInfo.GetPropertyValue(@this);
				if (propertyInfo.CanWrite)
					propertyInfo!.SetPropertyValue(target, value);
			}
			else if (mapAttributes.Any() && !ignoreAttributes.Any(_ => _.Type is not null))
			{
				var value = propertyInfo.GetPropertyValue(@this);
				foreach (var attribute in mapAttributes)
				{
					var targetPropertyInfo = properties.FirstOrDefault(propertyInfo => propertyInfo.Name.Is(attribute.Property));
					if (targetPropertyInfo?.CanWrite is true)
						targetPropertyInfo.SetPropertyValue(target, value);
				}
			}
		}

		return target;
	}

	/// <exception cref="ArgumentException"/>
	/// <exception cref="ArgumentNullException"/>
	/// <exception cref="ArgumentOutOfRangeException"/>
	public static T MapTo<S, T>(this S @this, T target, bool ignoreCase = true)
		where S : notnull
		where T : notnull
		=> @this switch
		{
			IEnumerable<KeyValuePair<string, object?>> pairs when target is IDictionary<string, object?> dictionary => (T)(object)pairs.MapToDictionary(dictionary),
			IEnumerable<KeyValuePair<string, object?>> pairs => pairs.MapTo(target, ignoreCase),
			_ when target is IDictionary<string, object?> dictionary => (T)(object)@this.MapToDictionary(dictionary),
			_ when typeof(S) == typeof(T) => (T)(object)@this.MapTo((S)(object)target),
			_ when ignoreCase => MapModel(@this, target, FlattenHierarchy | Instance | Public | IgnoreCase),
			_ => MapModel(@this, target, FlattenHierarchy | Instance | Public)
		};

	/// <exception cref="ArgumentNullException"/>
	public static IDictionary<K, V?> MapToDictionary<K, V>(this IEnumerable<KeyValuePair<K, V?>> @this, IDictionary<K, V?> target)
	{
		@this.AssertNotNull();
		target.AssertNotNull();

		foreach (var pair in @this)
			target[pair.Key] = pair.Value;

		return target;
	}

	/// <exception cref="ArgumentNullException"/>
	public static IDictionary<string, object?> MapToDictionary(this object @this, IDictionary<string, object?> target)
	{
		@this.AssertNotNull();
		target.AssertNotNull();

		var sourceProperties = @this.GetType().GetProperties(FlattenHierarchy | Instance | Public)
			.Where(propertyInfo => propertyInfo.CanRead);

		foreach (var propertyInfo in sourceProperties)
		{
			var mapAttributes = propertyInfo.GetCustomAttributes<MapAttribute>()
				.Where(attribute => attribute.Type.IsAssignableTo<IDictionary<string, object?>>());
			var value = propertyInfo.GetPropertyValue(@this);

			if (!mapAttributes.Any())
			{
				target[propertyInfo.Name] = value;
				continue;
			}

			foreach (var attribute in mapAttributes)
				target[attribute.Property] = value;
		}

		return target;
	}

	/// <exception cref="ArgumentNullException"/>
	private static T MapModel<S, T>(S source, T target, BindingFlags bindings)
		where S : notnull
		where T : notnull
	{
		source.AssertNotNull();
		target.AssertNotNull();

		foreach (var sourcePropertyInfo in typeof(S).GetProperties(bindings)
			.Where(propertyInfo => propertyInfo.CanRead))
		{
			var mapAttributes = sourcePropertyInfo.GetCustomAttributes<MapAttribute>()
				.Where(_ => _.Type == typeof(T))
				.ToArray();
			var ignoreAttributes = sourcePropertyInfo.GetCustomAttributes<MapIgnoreAttribute>()
				.Where(_ => _.Type is null || _.Type == typeof(T))
				.ToArray();
			if (!mapAttributes.Any() && !ignoreAttributes.Any())
			{
				var targetPropertyInfo = typeof(T).GetProperty(sourcePropertyInfo.Name, bindings);
				if (targetPropertyInfo?.CanWrite is not true)
					continue;

				var value = sourcePropertyInfo.GetPropertyValue(source);
				if (value is null && !targetPropertyInfo.PropertyType.IsNullable())
					continue;

				if (value is not null && !value.GetType().IsConvertibleTo(targetPropertyInfo.PropertyType))
					continue;

				targetPropertyInfo.SetPropertyValue(target, value);
			}
			else if (mapAttributes.Any() && !ignoreAttributes.Any(_ => _.Type is not null))
			{
				var value = sourcePropertyInfo.GetPropertyValue(source);
				foreach (var attribute in mapAttributes)
				{
					var targetPropertyInfo = typeof(T).GetProperty(attribute.Property, bindings);
					if (targetPropertyInfo?.CanWrite is true)
						targetPropertyInfo.SetPropertyValue(target, value);
				}
			}
		}

		return target;
	}
}
