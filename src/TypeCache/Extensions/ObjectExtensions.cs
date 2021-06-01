// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using TypeCache.Collections.Extensions;
using TypeCache.Reflection;
using TypeCache.Reflection.Extensions;

namespace TypeCache.Extensions
{
	public static class ObjectExtensions
	{
		public static void Assert<T>(this T? @this, string name, T? value, [CallerMemberName] string? caller = null)
		{
			name.AssertNotBlank(nameof(name));

			switch (@this)
			{
				case IEquatable<T> equatable when !equatable.Equals(value):
					throw new ArgumentOutOfRangeException($"{caller} -> {nameof(Assert)}: [{@this}] <> [{value?.ToString() ?? "null"}].", name);
				case null when value is not null:
					throw new ArgumentOutOfRangeException($"{caller} -> {nameof(Assert)}: null <> [{value}].", name);
				case IEquatable<T> _:
				case null:
					return;
				default:
					if (!object.Equals(@this, value))
						throw new ArgumentOutOfRangeException($"{caller} -> {nameof(Assert)}: [{@this}] <> [{value?.ToString() ?? "null"}].", name);
					return;
			}
		}

		public static void Assert<T>(this T? @this, string name, T? value, IEqualityComparer<T> comparer, [CallerMemberName] string? caller = null)
		{
			name.AssertNotBlank(nameof(name));
			comparer.AssertNotNull(nameof(comparer));

			if (!comparer.Equals(@this, value))
				throw new ArgumentOutOfRangeException($"{caller} -> {nameof(Assert)}: {(@this is not null ? $"[{@this}]" : "null")} <> {(value is not null ? $"[{value}]" : "null")}.", name);
		}

		public static void AssertNotNull<T>(this T? @this, string name, [CallerMemberName] string? caller = null)
		{
			if (@this is null)
				throw new ArgumentNullException($"{caller} -> {nameof(AssertNotNull)}: [{name}] is null.");
		}

		public static TypeMember GetTypeMember<T>(this T @this)
			where T : class
			=> @this switch
			{
				Type type => type.TypeHandle.GetTypeMember(),
				_ => @this.GetType().TypeHandle.GetTypeMember()
			};

		public static void MapFields<T>(this T @this, T source)
		{
			if (source is not null)
				@this?.GetType().GetTypeMember().Fields
					.If(_ => _.Value.IsPublic && _.Value.Getter is not null && _.Value.Setter is not null)
					.Do(_ => _.Value.SetValue!(@this, _.Value.GetValue!(source)));
		}

		public static ISet<string> MapFields<T, FROM>(this T @this, FROM source)
		{
			if (@this is not null && source is not null)
			{
				var fromFields = source.GetType().GetTypeMember().Fields;
				var toFields = @this.GetType().GetTypeMember().Fields;
				var fromNames = fromFields.If(_ => _.Value.Getter is not null).To(_ => _.Key);
				var toNames = toFields.If(_ => _.Value.Setter is not null).To(_ => _.Key);
				var names = fromNames.Match(toNames);
				names.Do(name => toFields[name].SetValue!(@this, fromFields[name].GetValue!(source)));
				return names;
			}
			return new HashSet<string>(0);
		}

		public static void MapFields<T>(this T @this, IDictionary<string, object?> source)
		{
			source.AssertNotNull(nameof(source));

			@this?.GetType().GetTypeMember().Fields.Do(_ =>
			{
				if (_.Value.SetValue is not null && source.TryGetValue(_.Key, out var value))
					_.Value.SetValue(@this, value);
			});
		}

		public static void MapProperties<T>(this T @this, T source)
		{
			if (source is not null)
				@this?.GetType().GetTypeMember().Properties
					.If(_ => _.Value.IsPublic && _.Value.GetValue is not null && _.Value.SetValue is not null)
					.Do(_ => _.Value.SetValue!(@this, _.Value.GetValue!(source)));
		}

		public static ISet<string> MapProperties<T, FROM>(this T @this, FROM source)
		{
			if (@this is not null && source is not null)
			{
				var fromProperties = source.GetType().GetTypeMember().Properties;
				var toProperties = @this.GetType().GetTypeMember().Properties;
				var fromNames = fromProperties.If(_ => _.Value.GetValue is not null).To(_ => _.Key);
				var toNames = toProperties.If(_ => _.Value.SetValue is not null).To(_ => _.Key);
				var names = fromNames.Match(toNames);
				names.Do(name => toProperties[name].SetValue!(@this, fromProperties[name].GetValue!(source)));
				return names;
			}
			return new HashSet<string>(0);
		}

		public static void MapProperties<T>(this T @this, IDictionary<string, object?> source)
		{
			source.AssertNotNull(nameof(source));

			@this?.GetType().GetTypeMember().Properties.Do(_ =>
			{
				if (_.Value.SetValue is not null && source.TryGetValue(_.Key, out var value))
					_.Value.SetValue(@this, value);
			});
		}

		public static IDictionary<string, object?>? ToFieldDictionary<T>(this T @this, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
			=> @this?.GetType().GetTypeMember().Fields.If(_ => _.Value.GetValue is not null).ToDictionary(_ => _.Key, _ => _.Value.GetValue!(@this), comparison.ToStringComparer());

		public static IDictionary<string, object?>? ToPropertyDictionary<T>(this T @this, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
			=> @this?.GetType().GetTypeMember().Properties.If(_ => _.Value.GetValue is not null).ToDictionary(_ => _.Key, _ => _.Value.GetValue!(@this), comparison.ToStringComparer());
	}
}
