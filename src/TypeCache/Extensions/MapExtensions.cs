// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Linq;
using TypeCache.Extensions;
using TypeCache.Reflection.Extensions;

namespace TypeCache.Extensions;

public static partial class MapExtensions
{
	/// <exception cref="ArgumentNullException"/>
	private static void Map<K, V>(this IDictionary<K, V> @this, IEnumerable<KeyValuePair<K, V>> source)
	{
		@this.AssertNotNull();
		source.AssertNotNull();

		foreach (var pair in source)
			@this[pair.Key] = pair.Value;
	}

	/// <exception cref="ArgumentNullException"/>
	private static void MapBy<K, V>(this IDictionary<K, V> @this, IEnumerable<KeyValuePair<K, V>> source, bool match = true)
	{
		@this.AssertNotNull();
		source.AssertNotNull();

		foreach (var pair in source.Where(pair => @this.ContainsKey(pair.Key) == match))
			@this[pair.Key] = pair.Value;
	}

	/// <exception cref="ArgumentException"/>
	/// <exception cref="ArgumentNullException"/>
	public static void MapFields(this object @this, IEnumerable<KeyValuePair<string, object?>> source, StringComparison nameComparison = StringComparison.Ordinal)
	{
		@this.AssertNotNull();
		source.AssertNotNull();

		var targetFieldMap = @this.GetTypeMember()!.Fields
			.Where(field => field.SetMethod is not null)
			.ToDictionary(field => field.Name, field => field, nameComparison.ToStringComparer());
		foreach (var pair in source)
		{
			if (targetFieldMap.TryGetValue(pair.Key, out var field)
				&& ((pair.Value is null && field.FieldType.Nullable)
					|| (pair.Value is not null && field.FieldType.Supports(pair.Value.GetTypeMember()!))))
				field.SetValue!(@this, pair.Value);
		}
	}

	/// <exception cref="ArgumentException"/>
	/// <exception cref="ArgumentNullException"/>
	public static void MapFields(this object @this, object source, StringComparison nameComparison = StringComparison.Ordinal)
	{
		@this.AssertNotNull();
		source.AssertNotNull();
		(@this, source).AssertNotSame();

		var comparer = nameComparison.ToStringComparer();
		var targetFieldMap = @this.GetTypeMember()!.Fields
			.Where(field => field.SetMethod is not null)
			.ToDictionary(_ => _.Name, _ => _, comparer);
		foreach (var sourceField in source.GetTypeMember()!.Fields
			.Where(field => field.GetMethod is not null))
		{
			if (targetFieldMap.TryGetValue(sourceField.Name, out var targetField))
			{
				var value = sourceField.GetValue!(source);
				if ((value is null && targetField.FieldType.Nullable)
					|| (value is not null && targetField.FieldType.Supports(sourceField.FieldType)))
					targetField.SetValue!(@this, value);
			}
		}
	}

	/// <exception cref="ArgumentException"/>
	/// <exception cref="ArgumentNullException"/>
	public static void MapFields(this object @this, object source, string[] fields, StringComparison nameComparison = StringComparison.Ordinal)
	{
		@this.AssertNotNull();
		source.AssertNotNull();
		fields.AssertNotEmpty();
		(@this, source).AssertNotSame();

		var comparer = nameComparison.ToStringComparer();
		var targetFieldMap = @this.GetTypeMember()!.Fields
			.Where(field => field.SetMethod is not null)
			.ToDictionary(_ => _.Name, _ => _, comparer);
		foreach (var sourceField in source.GetTypeMember()!.Fields
			.Where(field => field.GetMethod is not null && fields.Contains(field.Name, comparer)))
		{
			if (targetFieldMap.TryGetValue(sourceField.Name, out var targetField))
			{
				var value = sourceField.GetValue!(source);
				if ((value is null && targetField.FieldType.Nullable)
					|| (value is not null && targetField.FieldType.Supports(sourceField.FieldType)))
					targetField.SetValue!(@this, value);
			}
		}
	}

	/// <exception cref="ArgumentException"/>
	/// <exception cref="ArgumentNullException"/>
	public static void MapProperties(this object @this, IEnumerable<KeyValuePair<string, object?>> source, StringComparison nameComparison = StringComparison.Ordinal)
	{
		@this.AssertNotNull();
		source.AssertNotNull();

		var comparer = nameComparison.ToStringComparer();
		var targetPropertyMap = @this.GetTypeMember()!.Properties
			.Where(property => property.Setter is not null)
			.ToDictionary(property => property.Name, property => property, comparer);
		foreach (var pair in source)
		{
			if (targetPropertyMap.TryGetValue(pair.Key, out var property)
				&& ((pair.Value is null && property.PropertyType.Nullable)
					|| (pair.Value is not null && property.PropertyType.Supports(pair.Value.GetTypeMember()!))))
				property.SetValue(@this, pair.Value);
		}
	}

	/// <exception cref="ArgumentException"/>
	/// <exception cref="ArgumentNullException"/>
	public static void MapProperties(this object @this, object source, StringComparison nameComparison = StringComparison.Ordinal)
	{
		@this.AssertNotNull();
		source.AssertNotNull();
		(@this, source).AssertNotSame();

		var comparer = nameComparison.ToStringComparer();
		var targetPropertyMap = @this.GetTypeMember()!.Properties
			.Where(property => property.Setter is not null)
			.ToDictionary(property => property.Name, property => property, comparer);
		foreach (var sourceProperty in source.GetTypeMember()!.Properties
			.Where(property => property.Getter is not null))
		{
			if (targetPropertyMap.TryGetValue(sourceProperty.Name, out var targetProperty))
			{
				var value = sourceProperty.GetValue(source);
				if ((value is null && targetProperty.PropertyType.Nullable)
					|| (value is not null && targetProperty.PropertyType.Supports(sourceProperty.PropertyType)))
					targetProperty.SetValue(@this, value);
			}
		}
	}

	/// <exception cref="ArgumentException"/>
	/// <exception cref="ArgumentNullException"/>
	public static void MapProperties(this object @this, object source, string[] properties, StringComparison nameComparison = StringComparison.Ordinal)
	{
		@this.AssertNotNull();
		source.AssertNotNull();
		(@this, source).AssertNotSame();

		var comparer = nameComparison.ToStringComparer();
		var targetPropertyMap = @this.GetTypeMember()!.Properties
			.Where(property => property.Setter is not null)
			.ToDictionary(property => property.Name, property => property, comparer);
		foreach (var sourceProperty in source.GetTypeMember()!.Properties
			.Where(property => property.Getter is not null && properties.Contains(property.Name, comparer)))
		{
			if (targetPropertyMap.TryGetValue(sourceProperty.Name, out var targetProperty))
			{
				var value = sourceProperty.GetValue(source);
				if ((value is null && targetProperty.PropertyType.Nullable)
					|| (value is not null && targetProperty.PropertyType.Supports(sourceProperty.PropertyType)))
					targetProperty.SetValue(@this, value);
			}
		}
	}
}
