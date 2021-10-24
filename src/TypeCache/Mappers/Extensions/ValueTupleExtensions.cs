// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using TypeCache.Collections.Extensions;
using TypeCache.Extensions;
using TypeCache.Reflection;
using TypeCache.Reflection.Extensions;
using static TypeCache.Default;

namespace TypeCache.Mappers.Extensions
{
	public static class ValueTupleExtensions
	{
		/// <exception cref="ArgumentNullException"/>
		public static K[] Map<K, V>(this (IDictionary<K, V> From, IDictionary<K, V> To) @this)
		{
			@this.From.AssertNotNull(nameof(@this.From));
			@this.To.AssertNotNull(nameof(@this.To));

			return map(@this.From, @this.To).ToArray();

			static IEnumerable<K> map(IDictionary<K, V> from, IDictionary<K, V> to)
			{
				foreach (var pair in from)
				{
					if (to.ContainsKey(pair.Key))
					{
						to[pair.Key] = pair.Value;
						yield return pair.Key;
					}
				}
			}
		}

		/// <exception cref="ArgumentNullException"/>
		public static string[] MapFields(this (IDictionary<string, object?> From, object To) @this, StringComparison nameComparison = NAME_STRING_COMPARISON)
		{
			@this.From.AssertNotNull(nameof(@this.From));
			@this.To.AssertNotNull(nameof(@this.To));

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
			@this.From.AssertNotNull(nameof(@this.From));
			@this.To.AssertNotNull(nameof(@this.To));

			var fields = @this.From.GetTypeMember().Fields;
			fields.Values.Do(field => @this.To[field.Name] = field.GetValue(@this.From));
			return fields.Keys.ToArray();
		}

		/// <exception cref="ArgumentException"/>
		/// <exception cref="ArgumentNullException"/>
		public static string[] MapFields(this (object From, object To) @this, StringComparison nameComparison = NAME_STRING_COMPARISON)
		{
			@this.From.AssertNotNull(nameof(@this.From));
			@this.To.AssertNotNull(nameof(@this.To));
			@this.AssertNotSame((nameof(@this.From), nameof(@this.To)));

			return @this switch
			{
				(IDictionary<string, object?> from, object to) => (from, to).MapFields(nameComparison),
				(object from, IDictionary<string, object?> to) => (from, to).MapFields(nameComparison),
				_ => mapFields(@this.From, @this.To, nameComparison.ToStringComparer()).ToArray()
			};

			static IEnumerable<string> mapFields(object from, object to, StringComparer comparer)
			{
				var fromFields = (IDictionary<string, FieldMember>)from.GetTypeMember().Fields;
				var toFields = (IDictionary<string, FieldMember>)to.GetTypeMember().Fields;

				foreach (var match in (fromFields, toFields).Match(comparer))
				{
					if (match.Value.Item1.Getter is not null && match.Value.Item2.Setter is not null
						&& match.Value.Item2.FieldType.Supports(match.Value.Item1.FieldType))
					{
						match.Value.Item2.SetValue(to, match.Value.Item1.GetValue(from));
						yield return match.Key;
					}
				}
			}
		}

		/// <exception cref="ArgumentNullException"/>
		public static string[] MapProperties(this (IDictionary<string, object?> From, object To) @this, StringComparison nameComparison = NAME_STRING_COMPARISON)
		{
			@this.From.AssertNotNull(nameof(@this.From));
			@this.To.AssertNotNull(nameof(@this.To));

			return mapProperties(@this.From, @this.To, nameComparison.ToStringComparer()).ToArray();

			static IEnumerable<string> mapProperties(IDictionary<string, object?> from, object to, StringComparer comparer)
			{
				var toProperties = (IDictionary<string, PropertyMember>)to.GetTypeMember().Properties;

				foreach (var match in (from, toProperties).Match(comparer))
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
			@this.From.AssertNotNull(nameof(@this.From));
			@this.To.AssertNotNull(nameof(@this.To));

			var properties = @this.From.GetTypeMember().Properties;
			properties.Values.Do(property => @this.To[property.Name] = property.GetValue(@this.From));
			return properties.Keys.ToArray();
		}

		/// <exception cref="ArgumentException"/>
		/// <exception cref="ArgumentNullException"/>
		public static string[] MapProperties(this (object From, object To) @this, StringComparison nameComparison = NAME_STRING_COMPARISON)
		{
			@this.From.AssertNotNull(nameof(@this.From));
			@this.To.AssertNotNull(nameof(@this.To));
			@this.AssertNotSame((nameof(@this.From), nameof(@this.To)));

			return @this switch
			{
				(IDictionary<string, object?> from, object to) => (from, to).MapProperties(nameComparison),
				(object from, IDictionary<string, object?> to) => (from, to).MapProperties(),
				_ => mapProperties(@this.From, @this.To, nameComparison.ToStringComparer()).ToArray()
			};

			static IEnumerable<string> mapProperties(object from, object to, StringComparer comparer)
			{
				var fromProperties = (IDictionary<string, PropertyMember>)from.GetTypeMember().Properties;
				var toProperties = (IDictionary<string, PropertyMember>)to.GetTypeMember().Properties;

				foreach (var match in (fromProperties, toProperties).Match(comparer))
				{
					if (match.Value.Item1.Getter is not null && match.Value.Item2.Setter is not null
						&& match.Value.Item2.PropertyType.Supports(match.Value.Item1.PropertyType))
					{
						match.Value.Item2.SetValue(to, match.Value.Item1.GetValue(from));
						yield return match.Key;
					}
				}
			}
		}
	}
}
