// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using TypeCache.Common;
using TypeCache.Extensions;
using TypeCache.Reflection.Members;

namespace TypeCache.Reflection.Caches
{
	internal sealed class StaticMethodCache<T> : IStaticMethodCache<T>
		where T : class
	{
		public StaticMethodCache()
		{
			var methodInfos = typeof(T).GetMethods(TypeCache.STATIC_BINDING)
				.If(methodInfo => !methodInfo.ContainsGenericParameters && !methodInfo.IsSpecialName)
				.ToList();
			var valueComparer = new CustomEqualityComparer<IImmutableList<IStaticMethodMember>>((x, y) => x == y);
			this.Methods = methodInfos
				.To(methodInfo => methodInfo.Name)
				.ToHashSet(TypeCache.NameComparer)
				.To(name => KeyValuePair.Create(name, methodInfos
					.If(methodInfo => TypeCache.NameComparer.Equals(methodInfo.Name, name))
					.To(methodInfo => (IStaticMethodMember)new StaticMethodMember(methodInfo))
					.ToImmutable()))
				.ToImmutable(TypeCache.NameComparer, valueComparer);
		}

		public D? GetMethod<D>(string name) where D : Delegate
			=> this.Methods
				.Get(name)
				.To(staticMethodMember => staticMethodMember.Method)
				.If<Delegate, D>()
				.First();

		public IImmutableDictionary<string, IImmutableList<IStaticMethodMember>> Methods { get; }
	}
}
