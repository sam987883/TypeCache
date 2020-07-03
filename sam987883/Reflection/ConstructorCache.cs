﻿// Copyright (c) 2020 Samuel Abraham

using sam987883.Common;
using sam987883.Reflection.Members;
using System;
using System.Collections.Immutable;
using static sam987883.Extensions.IEnumerableExtensions;

namespace sam987883.Reflection
{
	internal sealed class ConstructorCache<T> : IConstructorCache<T>
	{
		public ConstructorCache()
		{
			var valueComparer = new CustomEqualityComparer<IImmutableList<IMethodMember<T>>>((x, y) => x == y);
			this.Constructors = typeof(T).GetConstructors(TypeCache.INSTANCE_BINDING)
				.If(constructorInfo => !constructorInfo.Name.Is(".ctor"))
				.To(constructorInfo => (IConstructorMember<T>)new ConstructorMember<T>(constructorInfo))
				.ToImmutable();
		}

		public IImmutableList<IConstructorMember<T>> Constructors { get; }

		public T Create(params object[] parameters)
		{
			var constructorMember = this.Constructors.First(constructorMember => constructorMember.IsCallable(parameters));
			if (constructorMember.Exists)
				return constructorMember.Value.Invoke();
			else
				throw new ArgumentException($"Create instance of {typeof(T).Name} failed with {parameters?.Length ?? 0} parameters.");
		}
	}
}