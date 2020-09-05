// Copyright (c) 2020 Samuel Abraham

using Sam987883.Common;
using Sam987883.Reflection.Members;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using static Sam987883.Common.Extensions.IEnumerableExtensions;
using static Sam987883.Common.Extensions.IReadOnlyDictionaryExtensions;

namespace Sam987883.Reflection.Caches
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

		public (D? Method, bool Exists) GetMethod<D>(string name) where D : Delegate =>
			this.Methods
				.Get(name).Value
				.To(staticMethodMember => staticMethodMember.Method)
				.If<Delegate, D>()
				.First();

		public IImmutableDictionary<string, IImmutableList<IStaticMethodMember>> Methods { get; }
	}
}
