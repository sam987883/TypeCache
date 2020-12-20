// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using TypeCache.Common;
using TypeCache.Extensions;
using TypeCache.Reflection.Members;

namespace TypeCache.Reflection.Caches
{
	internal sealed class MethodCache<T> : IMethodCache<T>
	{
		public MethodCache()
		{
			var methodInfos = typeof(T).GetMethods(TypeCache.INSTANCE_BINDING)
				.If(methodInfo => !methodInfo.ContainsGenericParameters && !methodInfo.IsSpecialName)
				.ToList();
			var valueComparer = new CustomEqualityComparer<IImmutableList<IMethodMember>>((x, y) => x == y);
			this.Methods = methodInfos
				.To(methodInfo => methodInfo.Name)
				.ToHashSet(TypeCache.NameComparer)
				.To(name => KeyValuePair.Create(name, methodInfos
					.If(methodInfo => TypeCache.NameComparer.Equals(methodInfo.Name, name))
					.To(methodInfo => (IMethodMember)new MethodMember(methodInfo))
					.ToImmutable()))
				.ToImmutable(TypeCache.NameComparer, valueComparer);
		}

		public D? GetMethod<D>(string name) where D : Delegate
		{
			name.AssertNotBlank(nameof(name));

			return this.Methods.Get(name).To(_ => _.Method).If<Delegate, D>().First();
		}

		public IImmutableDictionary<string, IImmutableList<IMethodMember>> Methods { get; }
	}
}
