// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection;
using TypeCache.Extensions;
using TypeCache.Reflection.Members;

namespace TypeCache.Reflection.Cache
{
	internal static class PropertyCache<T>
	{
		internal static IImmutableDictionary<string, IPropertyMember> Properties { get; }

		static PropertyCache()
		{
			Properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
				.If(propertyInfo => !propertyInfo.GetIndexParameters().Any())
				.To(propertyInfo => KeyValuePair.Create(propertyInfo.Name, (IPropertyMember)new PropertyMember(propertyInfo)))
				.ToImmutable(StringComparer.Ordinal);
		}
	}
}
