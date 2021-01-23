// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection;
using TypeCache.Extensions;
using TypeCache.Reflection.Members;

namespace TypeCache.Reflection.Cache
{
	internal static class FieldCache<T>
	{
		internal static IImmutableDictionary<string, IFieldMember> Fields { get; }

		static FieldCache()
		{
			Fields = typeof(T).GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
				.If(fieldInfo => !fieldInfo.IsLiteral)
				.To(fieldInfo => KeyValuePair.Create(fieldInfo.Name, (IFieldMember)new FieldMember(fieldInfo)))
				.ToImmutable(StringComparer.Ordinal);
		}
	}
}
