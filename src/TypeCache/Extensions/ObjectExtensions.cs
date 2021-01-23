// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using TypeCache.Reflection;

namespace TypeCache.Extensions
{
	public static class ObjectExtensions
	{
		public static void Assert<T>([AllowNull] this T @this, string name, [AllowNull] T value, [CallerMemberName] string caller = null)
		{
			name.AssertNotBlank(nameof(name));

			switch (@this)
			{
				case IEquatable<T> equatable when !equatable.Equals(value):
					throw new ArgumentException($"{caller} -> {nameof(Assert)}: [{@this}] <> [{value?.ToString() ?? "null"}].", name);
				case null when value != null:
					throw new ArgumentException($"{caller} -> {nameof(Assert)}: null <> [{value}].", name);
				case IEquatable<T> _:
				case null:
					return;
				default:
					if (!object.Equals(@this, value))
						throw new ArgumentException($"{caller} -> {nameof(Assert)}: [{@this}] <> [{value?.ToString() ?? "null"}].", name);
					return;
			}
		}

		public static void Assert<T>([AllowNull] this T @this, string name, [AllowNull] T value, IEqualityComparer<T> comparer, [CallerMemberName] string caller = null)
		{
			name.AssertNotBlank(nameof(name));
			comparer.AssertNotNull(nameof(comparer));

			if (!comparer.Equals(@this, value))
				throw new ArgumentException($"{caller} -> {nameof(Assert)}: {(@this != null ? $"[{@this}]" : "null")} <> {(value != null ? $"[{value}]" : "null")}.", name);
		}

		public static void AssertNotNull<T>([AllowNull] this T @this, string name, [CallerMemberName] string caller = null)
			where T : class
		{
			if (@this == null)
				throw new ArgumentNullException($"{caller} -> {nameof(AssertNotNull)}: [{name}] is null.");
		}

		public static IImmutableDictionary<string, IFieldMember> GetClassFields<T>(this T @this)
			where T : class
		{
			@this.AssertNotNull(nameof(@this));

			return typeof(T) == typeof(object) ? @this.GetType().GetClassFields() : Class<T>.Fields;
		}

		public static IImmutableList<IIndexerMember> GetClassIndexers<T>(this T @this)
			where T : class
		{
			@this.AssertNotNull(nameof(@this));

			return typeof(T) == typeof(object) ? @this.GetType().GetClassIndexers() : Class<T>.Indexers;
		}

		public static IImmutableDictionary<string, IPropertyMember> GetClassProperties<T>(this T @this)
			where T : class
		{
			@this.AssertNotNull(nameof(@this));

			return typeof(T) == typeof(object) ? @this.GetType().GetClassProperties() : Class<T>.Properties;
		}

		public static void MapFields<T>(this T @this, T source)
			where T : class
		{
			source.AssertNotNull(nameof(source));

			@this.GetClassFields()
				.If(_ => _.Value.Public && _.Value.Getter != null && _.Value.Setter != null)
				.Do(_ => _.Value[@this] = _.Value[source]);
		}

		public static ISet<string> MapFields<T, FROM>(this T @this, FROM source, bool compareCase = false)
			where T : class
			where FROM : class
		{
			var fromFields = source.GetClassFields();
			var toFields = @this.GetClassFields();
			var fromNames = fromFields.If(_ => _.Value.Getter != null).To(_ => _.Key);
			var toNames = toFields.If(_ => _.Value.Setter != null).To(_ => _.Key);
			var names = fromNames.Match(toNames, compareCase);
			names.Do(name => toFields[name][@this] = fromFields[name][source]);
			return names;
		}

		public static void MapFields<T>(this T @this, IDictionary<string, object?> source)
			where T : class
		{
			source.AssertNotNull(nameof(source));

			@this.GetClassFields().Do(_ =>
			{
				if (_.Value.Setter != null && source.TryGetValue(_.Key, out var value))
					_.Value[@this] = value;
			});
		}

		public static void MapProperties<T>(this T @this, T source)
			where T : class
		{
			source.AssertNotNull(nameof(source));

			@this.GetClassProperties()
				.If(_ => _.Value.Public && _.Value.Getter != null && _.Value.Setter != null)
				.Do(_ => _.Value[@this] = _.Value[source]);
		}

		public static ISet<string> MapProperties<T, FROM>(this T @this, FROM source, bool compareCase = false)
			where T : class
			where FROM : class
		{
			var fromProperties = source.GetClassProperties();
			var toProperties = @this.GetClassProperties();
			var fromNames = fromProperties.If(_ => _.Value.Getter != null).To(_ => _.Key);
			var toNames = toProperties.If(_ => _.Value.Setter != null).To(_ => _.Key);
			var names = fromNames.Match(toNames, compareCase);
			names.Do(name => toProperties[name][@this] = fromProperties[name][source]);
			return names;
		}

		public static void MapProperties<T>(this T @this, IDictionary<string, object?> source)
			where T : class
		{
			source.AssertNotNull(nameof(source));

			@this.GetClassProperties().Do(_ =>
			{
				if (_.Value.Setter != null && source.TryGetValue(_.Key, out var value))
					_.Value[@this] = value;
			});
		}

		public static IDictionary<string, object?>? ToFieldDictionary<T>(this T @this, bool compareCase = false)
			where T : class
			=> @this.GetClassFields().If(_ => _.Value.Getter != null).ToDictionary(_ => _.Key, _ => _.Value[@this], compareCase ? StringComparer.Ordinal : StringComparer.OrdinalIgnoreCase);

		public static IDictionary<string, object?>? ToPropertyDictionary<T>(this T @this, bool compareCase = false)
			where T : class
			=> @this.GetClassProperties().If(_ => _.Value.Getter != null).ToDictionary(_ => _.Key, _ => _.Value[@this], compareCase ? StringComparer.Ordinal : StringComparer.OrdinalIgnoreCase);
	}
}
