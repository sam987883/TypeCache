// Copyright (c) 2020 Samuel Abraham

using Sam987883.Common;
using Sam987883.Reflection.Members;
using System;
using System.Collections.Immutable;
using static Sam987883.Common.Extensions.IEnumerableExtensions;

namespace Sam987883.Reflection.Caches
{
	internal sealed class ConstructorCache<T> : IConstructorCache<T>
		where T : class
	{
		public ConstructorCache()
		{
			var valueComparer = new CustomEqualityComparer<IImmutableList<IMethodMember<T>>>((x, y) => x == y);
			this.Constructors = typeof(T).GetConstructors(TypeCache.INSTANCE_BINDING)
				.To(constructorInfo => (IConstructorMember<T>)new ConstructorMember<T>(constructorInfo))
				.ToImmutable();
		}

		public IImmutableList<IConstructorMember<T>> Constructors { get; }

		public T Create(params object[] parameters)
		{
			var constructorMember = this.Constructors.First(constructorMember => constructorMember.IsCallableWith(parameters));
			if (constructorMember.Exists)
				return constructorMember.Value.Invoke();
			else
				throw new ArgumentException($"Create instance of {typeof(T).Name} failed with {parameters?.Length ?? 0} parameters.");
		}
	}
}
