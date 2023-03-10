// Copyright (c) 2021 Samuel Abraham

using System.Reflection;
using TypeCache.Attributes;
using TypeCache.Extensions;
using static System.Reflection.BindingFlags;

namespace TypeCache.Extensions;

public static partial class MapExtensions
{
	/// <exception cref="ArgumentException"/>
	/// <exception cref="ArgumentNullException"/>
	/// <exception cref="ArgumentOutOfRangeException"/>
	public static T MapTo<S, T>(this S @this, T target, bool ignoreCase = true, params string[] properties)
		where S : notnull
		where T : notnull
	{
		@this.AssertNotNull();
		target.AssertNotNull();
		((object)target, (object)@this).AssertNotSame();

		var sourceType = @this.GetType();
		var targetType = target.GetType();
		var bindings = FlattenHierarchy | Instance | Public;
		if (ignoreCase)
			bindings |= IgnoreCase;

		if (@this is IEnumerable<KeyValuePair<string, object?>> pairs)
		{
			if (target is IDictionary<string, object?> dictionary)
				pairs.MapToDictionary(dictionary);
			else
			{
				foreach (var pair in pairs)
				{
					var propertyInfo = targetType.GetProperty(pair.Key, bindings);
					if (propertyInfo?.CanWrite is true)
						propertyInfo!.SetPropertyValue(target, pair.Value);
				}
			}
		}
		else if (target is IDictionary<string, object?> dictionary)
		{
			foreach (var propertyInfo in sourceType.GetProperties(bindings)
				.Where(propertyInfo => propertyInfo.CanRead))
			{
				var mapAttributes = propertyInfo.GetCustomAttributes<MapToAttribute>()
					.Where(attribute => attribute.Type.IsAssignableTo<IDictionary<string, object?>>());
				var value = propertyInfo.GetPropertyValue(@this);

				if (!mapAttributes.Any())
				{
					dictionary[propertyInfo.Name] = value;
					continue;
				}

				foreach (var attribute in mapAttributes)
				{
					if (attribute.Type.IsAssignableTo<IDictionary<string, object?>>())
						dictionary[attribute.Member] = value;
				}
			}
		}
		else if (sourceType == targetType)
		{
			if (properties?.Any() is true)
			{
				foreach (var property in properties)
				{
					var propertyInfo = sourceType.GetProperty(property, bindings);
					if (propertyInfo?.CanRead is not true)
						continue;

					var mapAttributes = propertyInfo.GetCustomAttributes<MapToAttribute<T>>();
					var value = propertyInfo!.GetPropertyValue(@this);

					if (!mapAttributes.Any())
					{
						if (propertyInfo.CanWrite)
							propertyInfo!.SetPropertyValue(target, value);

						continue;
					}

					foreach (var attribute in mapAttributes)
					{
						var targetPropertyInfo = targetType.GetProperty(attribute.Member, bindings);
						if (targetPropertyInfo?.CanWrite is true)
							targetPropertyInfo.SetPropertyValue(target, value);
					}
				}
			}
			else
			{
				foreach (var propertyInfo in sourceType.GetProperties(bindings)
					.Where(propertyInfo => propertyInfo.CanRead))
				{
					var mapAttributes = propertyInfo.GetCustomAttributes<MapToAttribute<T>>();
					var value = propertyInfo.GetPropertyValue(@this);

					if (!mapAttributes.Any())
					{
						if (propertyInfo.CanWrite)
							propertyInfo!.SetPropertyValue(target, value);

						continue;
					}

					foreach (var attribute in mapAttributes)
					{
						var targetPropertyInfo = targetType.GetProperty(attribute.Member, bindings);
						if (targetPropertyInfo?.CanWrite is true)
							targetPropertyInfo.SetPropertyValue(target, value);
					}
				}
			}
		}
		else if (properties?.Any() is true)
		{
			foreach (var property in properties)
			{
				var sourcePropertyInfo = sourceType.GetProperty(property, bindings);
				if (sourcePropertyInfo?.CanRead is not true)
					continue;

				var mapAttributes = sourcePropertyInfo.GetCustomAttributes<MapToAttribute<T>>();
				if (!mapAttributes.Any())
				{
					var targetPropertyInfo = targetType.GetProperty(property, bindings);
					if (targetPropertyInfo?.CanWrite is not true)
						continue;

					var value = sourcePropertyInfo.GetPropertyValue(@this);
					if (value is null && !targetPropertyInfo.PropertyType.IsNullable())
						continue;

					if (value is not null && !value.GetType().IsAssignableTo(targetPropertyInfo.PropertyType))
						continue;

					targetPropertyInfo.SetPropertyValue(target, value);
				}
				else
				{
					var value = sourcePropertyInfo.GetPropertyValue(@this);
					foreach (var attribute in mapAttributes)
					{
						var targetPropertyInfo = targetType.GetProperty(attribute.Member, bindings);
						if (targetPropertyInfo?.CanWrite is true)
							targetPropertyInfo.SetPropertyValue(target, value);
					}
				}
			}
		}
		else
		{
			foreach (var sourcePropertyInfo in sourceType.GetProperties(bindings)
				.Where(propertyInfo => propertyInfo.CanRead))
			{
				var mapAttributes = sourcePropertyInfo.GetCustomAttributes<MapToAttribute<T>>();
				if (!mapAttributes.Any())
				{
					var targetPropertyInfo = targetType.GetProperty(sourcePropertyInfo.Name, bindings);
					if (targetPropertyInfo?.CanWrite is not true)
						continue;

					var value = sourcePropertyInfo.GetPropertyValue(@this);
					if (value is null && !targetPropertyInfo.PropertyType.IsNullable())
						continue;

					if (value is not null && !value.GetType().IsAssignableTo(targetPropertyInfo.PropertyType))
						continue;

					targetPropertyInfo.SetPropertyValue(target, value);
				}
				else
				{
					var value = sourcePropertyInfo.GetPropertyValue(@this);
					foreach (var attribute in mapAttributes)
					{
						var targetPropertyInfo = targetType.GetProperty(attribute.Member, bindings);
						if (targetPropertyInfo?.CanWrite is true)
							targetPropertyInfo.SetPropertyValue(target, value);
					}
				}
			}
		}

		return target;
	}

	/// <exception cref="ArgumentNullException"/>
	public static IDictionary<K, V?> MapToDictionary<K, V>(this IEnumerable<KeyValuePair<K, V?>> @this, IDictionary<K, V?> target)
	{
		@this.AssertNotNull();
		target.AssertNotNull();

		foreach (var pair in @this)
			target[pair.Key] = pair.Value;

		return target;
	}
}
