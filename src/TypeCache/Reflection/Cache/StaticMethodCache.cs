// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection;
using TypeCache.Extensions;
using TypeCache.Reflection.Members;

namespace TypeCache.Reflection.Cache
{
	internal static class StaticMethodCache<T>
	{
		internal static IImmutableDictionary<string, IImmutableList<IStaticMethodMember>> Methods { get; }

		static StaticMethodCache()
		{
			var methodInfos = typeof(T).GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)
				.If(methodInfo => !methodInfo.ContainsGenericParameters && !methodInfo.IsSpecialName)
				.ToArray();
			Methods = methodInfos
				.To(methodInfo => methodInfo.Name)
				.ToHashSet(StringComparer.Ordinal)
				.To(name => KeyValuePair.Create(name, methodInfos
					.If(methodInfo => StringComparer.Ordinal.Equals(methodInfo.Name, name))
					.To(methodInfo => (IStaticMethodMember)new StaticMethodMember(methodInfo))
					.ToImmutable()))
				.ToImmutable(StringComparer.Ordinal);
		}
	}
}
