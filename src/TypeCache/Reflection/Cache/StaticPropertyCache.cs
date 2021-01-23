// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection;
using TypeCache.Extensions;
using TypeCache.Reflection.Members;

namespace TypeCache.Reflection.Cache
{
	internal static class StaticPropertyCache<T>
	{
		internal static IImmutableDictionary<string, IStaticPropertyMember> Properties { get; }

		static StaticPropertyCache()
		{
			Properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)
				.To(propertyInfo => KeyValuePair.Create(propertyInfo.Name, (IStaticPropertyMember)new StaticPropertyMember(propertyInfo)))
				.ToImmutable(StringComparer.Ordinal);
		}
	}
}
