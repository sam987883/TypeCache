// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using TypeCache.Collections.Extensions;
using TypeCache.Reflection.Extensions;

namespace TypeCache.Extensions;

public static partial class MapExtensions
{
	/// <exception cref="ArgumentNullException"/>
	private static void Map<K, V>(this IDictionary<K, V> @this, IEnumerable<KeyValuePair<K, V>> source, MapBehavior behavior)
	{
		@this.AssertNotNull();
		source.AssertNotNull();

		var pairs = behavior switch
		{
			MapBehavior.Matched => source.If(pair => @this.ContainsKey(pair.Key)),
			MapBehavior.Unmatched => source.If(pair => !@this.ContainsKey(pair.Key)),
			_ => source
		};

		pairs.Do(pair => @this[pair.Key] = pair.Value);
	}

	/// <exception cref="ArgumentException"/>
	/// <exception cref="ArgumentNullException"/>
	public static void MapFields(this object @this, IEnumerable<KeyValuePair<string, object?>> source, StringComparison nameComparison = StringComparison.Ordinal)
	{
		@this.AssertNotNull();
		source.AssertNotNull();

		@this.GetTypeMember().Fields
			.ToDictionary(_ => _.Name, _ => _, nameComparison)
			.Match(source.ToDictionary(nameComparison))
			.If(match => match.Value1.Setter is not null
				&& match.Value1.FieldType.Supports(match.Value2.GetTypeMember()))
			.Do(match =>
			{
				if (match.Value2 is not null || match.Value1.FieldType.Nullable)
					match.Value1.SetValue(@this, match.Value2);
			});
	}

	/// <exception cref="ArgumentException"/>
	/// <exception cref="ArgumentNullException"/>
	public static void MapFields(this object @this, object source, StringComparison nameComparison = StringComparison.Ordinal)
	{
		@this.AssertNotNull();
		source.AssertNotNull();
		(@this, source).AssertNotSame();

		var targetFieldMap = @this.GetTypeMember().Fields
			.ToDictionary(_ => _.Name, _ => _, nameComparison);
		var sourceFieldMap = source.GetTypeMember().Fields
			.ToDictionary(_ => _.Name, _ => _, nameComparison);

		targetFieldMap
			.Match(sourceFieldMap)
			.If(match => match.Value1.Setter is not null
				&& match.Value2.Getter is not null
				&& match.Value1.FieldType.Supports(match.Value2.FieldType))
			.Do(match =>
			{
				var value = match.Value2.GetValue(source);
				if (value is not null || match.Value1.FieldType.Nullable)
					match.Value1.SetValue(@this, value);
			});
	}

	/// <exception cref="ArgumentException"/>
	/// <exception cref="ArgumentNullException"/>
	public static void MapFields(this object @this, object source, string[] fields, StringComparison nameComparison = StringComparison.Ordinal)
	{
		@this.AssertNotNull();
		source.AssertNotNull();
		(@this, source).AssertNotSame();

		var targetFieldMap = @this.GetTypeMember().Fields
			.If(_ => fields.Has(_.Name, nameComparison))
			.ToDictionary(_ => _.Name, _ => _, nameComparison);
		var sourceFieldMap = source.GetTypeMember().Fields
			.If(_ => fields.Has(_.Name, nameComparison))
			.ToDictionary(_ => _.Name, _ => _, nameComparison);

		targetFieldMap
			.Match(sourceFieldMap)
			.If(match => match.Value1.Setter is not null
				&& match.Value2.Getter is not null
				&& match.Value1.FieldType.Supports(match.Value2.FieldType))
			.Do(match =>
			{
				var value = match.Value2.GetValue(source);
				if (value is not null || match.Value1.FieldType.Nullable)
					match.Value1.SetValue(@this, value);
			});
	}

	/// <exception cref="ArgumentException"/>
	/// <exception cref="ArgumentNullException"/>
	public static void MapProperties(this object @this, IEnumerable<KeyValuePair<string, object?>> source, StringComparison nameComparison = StringComparison.Ordinal)
	{
		@this.AssertNotNull();
		source.AssertNotNull();

		@this.GetTypeMember().Properties
			.ToDictionary(_ => _.Name, _ => _, nameComparison)
			.Match(source.ToDictionary(nameComparison))
			.If(match => match.Value1.Setter is not null
				&& match.Value1.PropertyType.Supports(match.Value2.GetTypeMember()))
			.Do(match =>
			{
				if (match.Value2 is not null || match.Value1.PropertyType.Nullable)
					match.Value1.SetValue(@this, match.Value2);
			});
	}

	/// <exception cref="ArgumentException"/>
	/// <exception cref="ArgumentNullException"/>
	public static void MapProperties(this object @this, object source, StringComparison nameComparison = StringComparison.Ordinal)
	{
		@this.AssertNotNull();
		source.AssertNotNull();
		(@this, source).AssertNotSame();

		var targetPropertyMap = @this.GetTypeMember().Properties
			.ToDictionary(_ => _.Name, _ => _, nameComparison);
		var sourcePropertyMap = source.GetTypeMember().Properties
			.ToDictionary(_ => _.Name, _ => _, nameComparison);

		targetPropertyMap
			.Match(sourcePropertyMap)
			.If(match => match.Value1.Setter is not null
				&& match.Value2.Getter is not null
				&& match.Value1.PropertyType.Supports(match.Value2.PropertyType))
			.Do(match =>
			{
				var value = match.Value2.GetValue(source);
				if (value is not null || match.Value1.PropertyType.Nullable)
					match.Value1.SetValue(@this, value);
			});
	}

	/// <exception cref="ArgumentException"/>
	/// <exception cref="ArgumentNullException"/>
	public static void MapProperties(this object @this, object source, string[] properties, StringComparison nameComparison = StringComparison.Ordinal)
	{
		@this.AssertNotNull();
		source.AssertNotNull();
		(@this, source).AssertNotSame();

		var targetPropertyMap = @this.GetTypeMember().Properties
			.If(_ => properties.Has(_.Name, nameComparison))
			.ToDictionary(_ => _.Name, _ => _, nameComparison);
		var sourcePropertyMap = source.GetTypeMember().Properties
			.If(_ => properties.Has(_.Name, nameComparison))
			.ToDictionary(_ => _.Name, _ => _, nameComparison);

		targetPropertyMap
			.Match(sourcePropertyMap)
			.If(match => match.Value1.Setter is not null
				&& match.Value2.Getter is not null
				&& match.Value1.PropertyType.Supports(match.Value2.PropertyType))
			.Do(match =>
			{
				var value = match.Value2.GetValue(source);
				if (value is not null || match.Value1.PropertyType.Nullable)
					match.Value1.SetValue(@this, value);
			});
	}
}
