// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TypeCache.Collections.Extensions;
using TypeCache.Extensions;
using TypeCache.Reflection;
using TypeCache.Reflection.Extensions;
using static TypeCache.Default;

namespace TypeCache.Mappers.Extensions;

public static partial class MapExtensions
{
	/// <exception cref="ArgumentNullException"/>
	private static IEnumerable<K> Map<K, V>(this IDictionary<K, V> @this, IEnumerable<KeyValuePair<K, V>> from, MapBehavior behavior)
	{
		@this.AssertNotNull();
		from.AssertNotNull();

		return from.If(pair => @this.ContainsKey(pair.Key)).Map(pair =>
		{
			@this[pair.Key] = pair.Value;
			return pair.Key;
		});
	}

	/// <exception cref="ArgumentNullException"/>
	public static IEnumerable<string> MapFields(this object @this, IEnumerable<KeyValuePair<string, object?>> from)
	{
		@this.AssertNotNull();
		from.AssertNotNull();

		return @this.GetTypeMember().Fields.Match(from)
			.If(pair => pair.Value.Item1.Setter is not null
				&& ((pair.Value.Item2 is null && pair.Value.Item1.FieldType.Nullable)
					|| (pair.Value.Item2 is not null && pair.Value.Item1.FieldType.Supports(pair.Value.Item2.GetType()))))
			.Map(pair =>
			{
				pair.Value.Item1.SetValue(@this, pair.Value.Item2);
				return pair.Key;
			});
	}

	/// <exception cref="ArgumentNullException"/>
	public static IDictionary<string, object?> MapFields(this object @this)
	{
		@this.AssertNotNull();

		return @this.GetTypeMember().Fields.ToDictionary(pair => pair.Key, pair => pair.Value.GetValue(@this), NAME_STRING_COMPARISON);
	}

	/// <exception cref="ArgumentException"/>
	/// <exception cref="ArgumentNullException"/>
	public static IEnumerable<string> MapFields(this object @this, object from)
	{
		@this.AssertNotNull();
		from.AssertNotNull();
		(@this, from).AssertNotSame();

		return from.GetTypeMember().Fields.Match(@this.GetTypeMember().Fields)
			.If(pair => pair.Value.Item1.Getter is not null
				&& pair.Value.Item2.Setter is not null
				&& pair.Value.Item2.FieldType.Supports(pair.Value.Item1.FieldType))
			.Map(pair =>
			{
				pair.Value.Item2.SetValue(@this, pair.Value.Item1.GetValue(from));
				return pair.Key;
			});
	}

	/// <exception cref="ArgumentNullException"/>
	public static IEnumerable<string> MapProperties(this object @this, IEnumerable<KeyValuePair<string, object?>> from)
	{
		@this.AssertNotNull();
		from.AssertNotNull();

		return @this.GetTypeMember().Properties.Match(from)
			.If(pair => pair.Value.Item1.Setter is not null
				&& ((pair.Value.Item2 is null && pair.Value.Item1.PropertyType.Nullable)
					|| (pair.Value.Item2 is not null && pair.Value.Item1.PropertyType.Supports(pair.Value.Item2.GetType()))))
			.Map(pair =>
			{
				pair.Value.Item1.SetValue(@this, pair.Value.Item2);
				return pair.Key;
			});
	}

	/// <exception cref="ArgumentNullException"/>
	public static IDictionary<string, object?> MapProperties(this object @this)
	{
		@this.AssertNotNull();

		return @this.GetTypeMember().Properties.ToDictionary(pair => pair.Key, pair => pair.Value.GetValue(@this), NAME_STRING_COMPARISON);
	}

	/// <exception cref="ArgumentException"/>
	/// <exception cref="ArgumentNullException"/>
	public static IEnumerable<string> MapProperties(this object @this, object from)
	{
		@this.AssertNotNull();
		from.AssertNotNull();
		(@this, from).AssertNotSame();

		return from.GetTypeMember().Properties.Match(@this.GetTypeMember().Properties)
			.If(pair => pair.Value.Item1.Getter is not null
				&& pair.Value.Item2.Setter is not null
				&& pair.Value.Item2.PropertyType.Supports(pair.Value.Item1.PropertyType))
			.Map(pair =>
			{
				pair.Value.Item2.SetValue(@this, pair.Value.Item1.GetValue(from));
				return pair.Key;
			});
	}
}
