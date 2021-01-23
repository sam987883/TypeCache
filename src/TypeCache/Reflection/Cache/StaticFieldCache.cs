// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection;
using TypeCache.Extensions;
using TypeCache.Reflection.Members;

namespace TypeCache.Reflection.Cache
{
	internal static class StaticFieldCache<T>
	{
		internal static IImmutableDictionary<string, IStaticFieldMember> Fields { get; }

		static StaticFieldCache()
		{
			Fields = typeof(T).GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)
				.If(fieldInfo => !fieldInfo.IsLiteral)
				.To(fieldInfo => KeyValuePair.Create(fieldInfo.Name, (IStaticFieldMember)new StaticFieldMember(fieldInfo)))
				.ToImmutable(StringComparer.Ordinal);
		}
	}
}
