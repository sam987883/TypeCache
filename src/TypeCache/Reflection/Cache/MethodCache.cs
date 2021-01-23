// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection;
using TypeCache.Extensions;
using TypeCache.Reflection.Members;

namespace TypeCache.Reflection.Cache
{
	internal static class MethodCache<T>
	{
		internal static IImmutableDictionary<string, IImmutableList<IMethodMember>> Methods { get; }

		static MethodCache()
		{
			var methodInfos = typeof(T).GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
				.If(methodInfo => !methodInfo.ContainsGenericParameters && !methodInfo.IsSpecialName)
				.ToArray();
			Methods = methodInfos
				.To(methodInfo => methodInfo.Name)
				.ToHashSet(StringComparer.Ordinal)
				.To(name => KeyValuePair.Create(name, methodInfos
					.If(methodInfo => StringComparer.Ordinal.Equals(methodInfo.Name, name))
					.To(methodInfo => (IMethodMember)new MethodMember(methodInfo))
					.ToImmutable()))
				.ToImmutable(StringComparer.Ordinal);
		}
	}
}
