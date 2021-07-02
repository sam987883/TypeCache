// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Reflection;
using TypeCache.Collections.Extensions;
using TypeCache.Extensions;

namespace TypeCache.Reflection.Extensions
{
	public static class ObjectExtensions
	{
		public static TypeMember GetTypeMember<T>(this T @this)
			where T : class
			=> @this switch
			{
				Type type => type.TypeHandle.GetTypeMember(),
				MemberInfo memberInfo => memberInfo.DeclaringType!.TypeHandle.GetTypeMember(),
				_ => @this.GetType().TypeHandle.GetTypeMember()
			};

		public static ISet<string> MapFields<T, FROM>(this T @this, FROM source)
			where T : class
			where FROM : class
		{
			source.AssertNotNull(nameof(source));

			if (@this is not null && source is not null)
			{
				var fromFields = source.GetTypeMember().Fields;
				var toFields = @this.GetTypeMember().Fields;
				var fromNames = fromFields.If(_ => _.Value.Getter is not null).To(_ => _.Key);
				var toNames = toFields.If(_ => _.Value.Setter is not null).To(_ => _.Key);
				var names = fromNames.Match(toNames);
				names.Do(name => toFields[name].SetValue(@this, fromFields[name].GetValue(source)));
				return names;
			}
			return new HashSet<string>(0);
		}

		public static ISet<string> MapProperties<T, FROM>(this T @this, FROM source)
			where T : class
			where FROM : class
		{
			source.AssertNotNull(nameof(source));

			if (@this is not null && source is not null)
			{
				var fromProperties = source.GetTypeMember().Properties;
				var toProperties = @this.GetTypeMember().Properties;
				var fromNames = fromProperties.If(_ => _.Value.Getter is not null).To(_ => _.Key);
				var toNames = toProperties.If(_ => _.Value.Setter is not null).To(_ => _.Key);
				var names = fromNames.Match(toNames);
				names.Do(name => toProperties[name].SetValue(@this, fromProperties[name].GetValue(source)));
				return names;
			}
			return new HashSet<string>(0);
		}

		public static void ReadFields<T>(this T @this, IDictionary<string, object?> source)
			where T : class
		{
			source.AssertNotNull(nameof(source));

			@this?.GetType().GetTypeMember().Fields.Do(_ =>
			{
				if (_.Value.Setter is not null && source.TryGetValue(_.Key, out var value))
					_.Value.SetValue(@this, value);
			});
		}

		public static void ReadProperties<T>(this T @this, IDictionary<string, object?> source)
			where T : class
		{
			source.AssertNotNull(nameof(source));

			@this?.GetType().GetTypeMember().Properties.Do(_ =>
			{
				if (_.Value.Setter is not null && source.TryGetValue(_.Key, out var value))
					_.Value.SetValue(@this, value);
			});
		}

		public static IDictionary<string, object?> WriteFields<T>(this T @this, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
			where T : class
			=> @this.GetTypeMember().Fields.If(_ => _.Value.Getter is not null).ToDictionary(_ => _.Key, _ => _.Value.GetValue(@this), comparison);

		public static IDictionary<string, object?> WriteProperties<T>(this T @this, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
			where T : class
			=> @this.GetTypeMember().Properties.If(_ => _.Value.Getter is not null).ToDictionary(_ => _.Key, _ => _.Value.GetValue(@this), comparison);
	}
}
