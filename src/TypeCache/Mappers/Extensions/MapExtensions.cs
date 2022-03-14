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

public static class MapExtensions
{
	private static IEnumerable<K> Map<K, V>(this IEnumerable<KeyValuePair<K, V>> @this, IDictionary<K, V> to)
	{
		@this.AssertNotNull();
		to.AssertNotNull();

		foreach (var pair in @this)
		{
			if (to.ContainsKey(pair.Key))
			{
				to[pair.Key] = pair.Value;
				yield return pair.Key;
			}
		}
	}

	/// <exception cref="ArgumentNullException"/>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static K[] Map<K, V>(this (IReadOnlyDictionary<K, V> From, IDictionary<K, V> To) @this)
		=> @this.From.Map(@this.To).ToArray();

	/// <exception cref="ArgumentNullException"/>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static K[] Map<K, V>(this (IDictionary<K, V> From, IDictionary<K, V> To) @this)
		=> @this.From.Map(@this.To).ToArray();

	/// <exception cref="ArgumentNullException"/>
	public static string[] MapFields(this (IReadOnlyDictionary<string, object?> From, object To) @this, StringComparison nameComparison = NAME_STRING_COMPARISON)
	{
		@this.From.AssertNotNull();
		@this.To.AssertNotNull();

		return mapFields(@this.From, @this.To, nameComparison.ToStringComparer()).ToArray();

		static IEnumerable<string> mapFields(IReadOnlyDictionary<string, object?> from, object to, StringComparer comparer)
		{
			var toFields = (IReadOnlyDictionary<string, FieldMember>)to.GetTypeMember().Fields;

			foreach (var match in (from, toFields).Match(comparer))
			{
				if (match.Value.Item2.Setter is not null
					&& ((match.Value.Item1 is null && match.Value.Item2.FieldType.Nullable)
						|| (match.Value.Item1 is not null && match.Value.Item2.FieldType.Supports(match.Value.Item1.GetType()))))
				{
					match.Value.Item2.SetValue(to, match.Value.Item1);
					yield return match.Key;
				}
			}
		}
	}

	/// <exception cref="ArgumentNullException"/>
	public static string[] MapFields(this (IDictionary<string, object?> From, object To) @this, StringComparison nameComparison = NAME_STRING_COMPARISON)
	{
		@this.From.AssertNotNull();
		@this.To.AssertNotNull();

		return mapFields(@this.From, @this.To, nameComparison.ToStringComparer()).ToArray();

		static IEnumerable<string> mapFields(IDictionary<string, object?> from, object to, StringComparer comparer)
		{
			var toFields = (IDictionary<string, FieldMember>)to.GetTypeMember().Fields;

			foreach (var match in (from, toFields).Match(comparer))
			{
				if (match.Value.Item2.Setter is not null
					&& ((match.Value.Item1 is null && match.Value.Item2.FieldType.Nullable)
						|| (match.Value.Item1 is not null && match.Value.Item2.FieldType.Supports(match.Value.Item1.GetType()))))
				{
					match.Value.Item2.SetValue(to, match.Value.Item1);
					yield return match.Key;
				}
			}
		}
	}

	/// <exception cref="ArgumentNullException"/>
	public static string[] MapFields(this (object From, IDictionary<string, object?> To) @this)
	{
		@this.From.AssertNotNull();
		@this.To.AssertNotNull();

		var fields = @this.From.GetTypeMember().Fields;
		fields.Values.Do(field => @this.To[field.Name] = field.GetValue(@this.From));
		return fields.Keys.ToArray();
	}

	/// <exception cref="ArgumentException"/>
	/// <exception cref="ArgumentNullException"/>
	public static string[] MapFields(this (object From, object To) @this, StringComparison nameComparison = NAME_STRING_COMPARISON)
	{
		@this.From.AssertNotNull();
		@this.To.AssertNotNull();
		@this.AssertNotSame();

		return @this switch
		{
			(IReadOnlyDictionary<string, object?> from, object to) => (from, to).MapFields(nameComparison),
			(IDictionary<string, object?> from, object to) => (from, to).MapFields(nameComparison),
			(object from, IDictionary<string, object?> to) => (from, to).MapFields(nameComparison),
			_ => mapFields(@this.From, @this.To, nameComparison).ToArray()
		};

		static IEnumerable<string> mapFields(object from, object to, StringComparison nameComparison)
		{
			var fromFields = from.GetTypeMember().Fields;
			var toFields = to.GetTypeMember().Fields;

			foreach (var match in (fromFields, toFields).Match(nameComparison))
			{
				if (match.Value.Item1.Getter is not null
					&& match.Value.Item2.Setter is not null
					&& match.Value.Item2.FieldType.Supports(match.Value.Item1.FieldType))
				{
					match.Value.Item2.SetValue(to, match.Value.Item1.GetValue(from));
					yield return match.Key;
				}
			}
		}
	}

	/// <exception cref="ArgumentNullException"/>
	public static string[] MapProperties(this (IReadOnlyDictionary<string, object?> From, object To) @this, StringComparison nameComparison = NAME_STRING_COMPARISON)
	{
		@this.From.AssertNotNull();
		@this.To.AssertNotNull();

		return mapProperties(@this.From, @this.To, nameComparison).ToArray();

		static IEnumerable<string> mapProperties(IReadOnlyDictionary<string, object?> from, object to, StringComparison nameComparison)
		{
			var toProperties = (IReadOnlyDictionary<string, PropertyMember>)to.GetTypeMember().Properties;

			foreach (var match in (from, toProperties).Match(nameComparison))
			{
				if (match.Value.Item2.Setter is not null
					&& ((match.Value.Item1 is null && match.Value.Item2.PropertyType.Nullable)
						|| (match.Value.Item1 is not null && match.Value.Item2.PropertyType.Supports(match.Value.Item1.GetType()))))
				{
					match.Value.Item2.SetValue(to, match.Value.Item1);
					yield return match.Key;
				}
			}
		}
	}

	/// <exception cref="ArgumentNullException"/>
	public static string[] MapProperties(this (IDictionary<string, object?> From, object To) @this, StringComparison nameComparison = NAME_STRING_COMPARISON)
	{
		@this.From.AssertNotNull();
		@this.To.AssertNotNull();

		return mapProperties(@this.From, @this.To, nameComparison).ToArray();

		static IEnumerable<string> mapProperties(IDictionary<string, object?> from, object to, StringComparison nameComparison)
		{
			var toProperties = (IDictionary<string, PropertyMember>)to.GetTypeMember().Properties;

			foreach (var match in (from, toProperties).Match(nameComparison))
			{
				if (match.Value.Item2.Setter is not null
					&& ((match.Value.Item1 is null && match.Value.Item2.PropertyType.Nullable)
						|| (match.Value.Item1 is not null && match.Value.Item2.PropertyType.Supports(match.Value.Item1.GetType()))))
				{
					match.Value.Item2.SetValue(to, match.Value.Item1);
					yield return match.Key;
				}
			}
		}
	}

	/// <exception cref="ArgumentNullException"/>
	public static string[] MapProperties(this (object From, IDictionary<string, object?> To) @this)
	{
		@this.From.AssertNotNull();
		@this.To.AssertNotNull();

		var properties = @this.From.GetTypeMember().Properties;
		properties.Values.Do(property => @this.To[property.Name] = property.GetValue(@this.From));
		return properties.Keys.ToArray();
	}

	/// <exception cref="ArgumentException"/>
	/// <exception cref="ArgumentNullException"/>
	public static string[] MapProperties(this (object From, object To) @this, StringComparison nameComparison = NAME_STRING_COMPARISON)
	{
		@this.From.AssertNotNull();
		@this.To.AssertNotNull();
		@this.AssertNotSame();

		return @this switch
		{
			(IReadOnlyDictionary<string, object?> from, object to) => (from, to).MapProperties(nameComparison),
			(IDictionary<string, object?> from, object to) => (from, to).MapProperties(nameComparison),
			(object from, IDictionary<string, object?> to) => (from, to).MapProperties(),
			_ => mapProperties(@this.From, @this.To, nameComparison).ToArray()
		};

		static IEnumerable<string> mapProperties(object from, object to, StringComparison nameComparison)
		{
			var fromProperties = from.GetTypeMember().Properties;
			var toProperties = to.GetTypeMember().Properties;

			foreach (var match in (fromProperties, toProperties).Match(nameComparison))
			{
				if (match.Value.Item1.Getter is not null
					&& match.Value.Item2.Setter is not null
					&& match.Value.Item2.PropertyType.Supports(match.Value.Item1.PropertyType))
				{
					match.Value.Item2.SetValue(to, match.Value.Item1.GetValue(from));
					yield return match.Key;
				}
			}
		}
	}
}
