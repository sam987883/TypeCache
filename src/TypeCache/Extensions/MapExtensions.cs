// Copyright (c) 2021 Samuel Abraham

using System.Reflection;
using TypeCache.Extensions;
using static System.Reflection.BindingFlags;

namespace TypeCache.Extensions;

public static partial class MapExtensions
{
	private const BindingFlags FIELD_BINDING_FLAGS = FlattenHierarchy | Instance | Public | NonPublic;
	private const BindingFlags PROPERTY_BINDING_FLAGS = FlattenHierarchy | Instance | Public;

	/// <exception cref="ArgumentNullException"/>
	private static IDictionary<K, V> Map<K, V>(this IDictionary<K, V> @this, IEnumerable<KeyValuePair<K, V>> source)
	{
		@this.AssertNotNull();
		source.AssertNotNull();

		foreach (var pair in source)
			@this[pair.Key] = pair.Value;

		return @this;
	}

	/// <exception cref="ArgumentNullException"/>
	private static IDictionary<K, V> MapBy<K, V>(this IDictionary<K, V> @this, IEnumerable<KeyValuePair<K, V>> source, bool match = true)
	{
		@this.AssertNotNull();
		source.AssertNotNull();

		foreach (var pair in source.Where(pair => @this.ContainsKey(pair.Key) == match))
			@this[pair.Key] = pair.Value;

		return @this;
	}

	/// <exception cref="ArgumentException"/>
	/// <exception cref="ArgumentNullException"/>
	public static T MapFields<T>(this T @this, IEnumerable<KeyValuePair<string, object?>> source, StringComparison nameComparison = StringComparison.Ordinal)
		where T : notnull
	{
		@this.AssertNotNull();
		source.AssertNotNull();

		var targetFieldMap = @this.GetType()!.GetFields(FIELD_BINDING_FLAGS)
			.Where(fieldInfo => !fieldInfo.IsInitOnly && !fieldInfo.IsLiteral)
			.ToDictionary(fieldInfo => fieldInfo.Name(), fieldInfo => fieldInfo, nameComparison.ToStringComparer());
		foreach (var pair in source)
		{
			if (targetFieldMap.TryGetValue(pair.Key, out var fieldInfo)
				&& ((pair.Value is null && fieldInfo.FieldType.IsNullable())
					|| (pair.Value is not null && pair.Value.GetType().IsAssignableTo(fieldInfo.FieldType))))
				fieldInfo.SetFieldValue(@this, pair.Value);
		}

		return @this;
	}

	/// <exception cref="ArgumentException"/>
	/// <exception cref="ArgumentNullException"/>
	public static T MapFields<T>(this T @this, object source, StringComparison nameComparison = StringComparison.Ordinal)
		where T : notnull
	{
		@this.AssertNotNull();
		source.AssertNotNull();
		(@this, source).AssertNotSame();

		var comparer = nameComparison.ToStringComparer();
		var targetFieldMap = @this.GetType().GetFields(FIELD_BINDING_FLAGS)
			.Where(fieldInfo => !fieldInfo.IsInitOnly && !fieldInfo.IsLiteral)
			.ToDictionary(fieldInfo => fieldInfo.Name(), fieldInfo => fieldInfo, comparer);
		foreach (var sourceFieldInfo in source.GetType().GetFields(FIELD_BINDING_FLAGS))
		{
			if (targetFieldMap.TryGetValue(sourceFieldInfo.Name(), out var targetFieldInfo))
			{
				var value = sourceFieldInfo.GetFieldValue(source);
				if ((value is null && targetFieldInfo.FieldType.IsNullable())
					|| (value is not null && sourceFieldInfo.FieldType.IsAssignableTo(targetFieldInfo.FieldType)))
					targetFieldInfo.SetFieldValue(@this, value);
			}
		}

		return @this;
	}

	/// <exception cref="ArgumentException"/>
	/// <exception cref="ArgumentNullException"/>
	public static T MapFields<T>(this T @this, object source, string[] fields, StringComparison nameComparison = StringComparison.Ordinal)
		where T : notnull
	{
		@this.AssertNotNull();
		source.AssertNotNull();
		fields.AssertNotEmpty();
		(@this, source).AssertNotSame();

		var comparer = nameComparison.ToStringComparer();
		var targetFieldMap = @this.GetType().GetFields(FIELD_BINDING_FLAGS)
			.Where(fieldInfo => !fieldInfo.IsInitOnly && !fieldInfo.IsLiteral)
			.ToDictionary(fieldInfo => fieldInfo.Name(), fieldInfo => fieldInfo, comparer);
		foreach (var sourceFieldInfo in source.GetType().GetFields(FIELD_BINDING_FLAGS)
			.Where(fieldInfo => fields.Contains(fieldInfo.Name(), comparer)))
		{
			if (targetFieldMap.TryGetValue(sourceFieldInfo.Name(), out var targetFieldInfo))
			{
				var value = sourceFieldInfo.GetFieldValue(source);
				if ((value is null && targetFieldInfo.FieldType.IsNullable())
					|| (value is not null && sourceFieldInfo.FieldType.IsAssignableTo(targetFieldInfo.FieldType)))
					targetFieldInfo.SetFieldValue(@this, value);
			}
		}

		return @this;
	}

	/// <exception cref="ArgumentException"/>
	/// <exception cref="ArgumentNullException"/>
	public static T MapProperties<T>(this T @this, IEnumerable<KeyValuePair<string, object?>> source, StringComparison nameComparison = StringComparison.Ordinal)
		where T : notnull
	{
		@this.AssertNotNull();
		source.AssertNotNull();

		var comparer = nameComparison.ToStringComparer();
		var targetPropertyMap = @this.GetType().GetProperties(PROPERTY_BINDING_FLAGS)
			.Where(propertyInfo => propertyInfo.CanWrite && propertyInfo.SetMethod!.IsPublic)
			.ToDictionary(propertyInfo => propertyInfo.Name(), propertyInfo => propertyInfo, comparer);
		foreach (var pair in source)
		{
			if (targetPropertyMap.TryGetValue(pair.Key, out var propertyInfo)
				&& ((pair.Value is null && propertyInfo.PropertyType.IsNullable())
					|| (pair.Value is not null && pair.Value.GetType().IsAssignableTo(propertyInfo.PropertyType))))
				propertyInfo.SetPropertyValue(@this, pair.Value);
		}

		return @this;
	}

	/// <exception cref="ArgumentException"/>
	/// <exception cref="ArgumentNullException"/>
	public static T MapProperties<T>(this T @this, object source, StringComparison nameComparison = StringComparison.Ordinal)
		where T : notnull
	{
		@this.AssertNotNull();
		source.AssertNotNull();
		(@this, source).AssertNotSame();

		var comparer = nameComparison.ToStringComparer();
		var targetPropertyMap = @this.GetType().GetProperties(PROPERTY_BINDING_FLAGS)
			.Where(propertyInfo => propertyInfo.CanWrite && propertyInfo.SetMethod!.IsPublic)
			.ToDictionary(propertyInfo => propertyInfo.Name(), propertyInfo => propertyInfo, comparer);
		foreach (var sourcePropertyInfo in source.GetType().GetProperties(PROPERTY_BINDING_FLAGS)
			.Where(propertyInfo => propertyInfo.CanRead))
		{
			if (targetPropertyMap.TryGetValue(sourcePropertyInfo.Name(), out var targetPropertyInfo))
			{
				var value = sourcePropertyInfo.GetPropertyValue(source);
				if ((value is null && targetPropertyInfo.PropertyType.IsNullable())
					|| (value is not null && sourcePropertyInfo.PropertyType.IsAssignableTo(targetPropertyInfo.PropertyType)))
					targetPropertyInfo.SetPropertyValue(@this, value);
			}
		}

		return @this;
	}

	/// <exception cref="ArgumentException"/>
	/// <exception cref="ArgumentNullException"/>
	public static T MapProperties<T>(this T @this, object source, string[] properties, StringComparison nameComparison = StringComparison.Ordinal)
		where T : notnull
	{
		@this.AssertNotNull();
		source.AssertNotNull();
		(@this, source).AssertNotSame();

		var comparer = nameComparison.ToStringComparer();
		var targetPropertyMap = @this.GetType().GetProperties(PROPERTY_BINDING_FLAGS)
			.Where(propertyInfo => propertyInfo.CanWrite && propertyInfo.SetMethod!.IsPublic)
			.ToDictionary(propertyInfo => propertyInfo.Name(), propertyInfo => propertyInfo, comparer);
		foreach (var sourcePropertyInfo in source.GetType().GetProperties(PROPERTY_BINDING_FLAGS)
			.Where(propertyInfo => propertyInfo.CanRead && properties.Contains(propertyInfo.Name(), comparer)))
		{
			if (targetPropertyMap.TryGetValue(sourcePropertyInfo.Name(), out var targetPropertyInfo))
			{
				var value = sourcePropertyInfo.GetPropertyValue(source);
				if ((value is null && targetPropertyInfo.PropertyType.IsNullable())
					|| (value is not null && sourcePropertyInfo.PropertyType.IsAssignableTo(targetPropertyInfo.PropertyType)))
					targetPropertyInfo.SetPropertyValue(@this, value);
			}
		}

		return @this;
	}
}
