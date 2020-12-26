// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Immutable;
using TypeCache.Extensions;
using TypeCache.Reflection.Members;

namespace TypeCache.Reflection.Caches
{
	internal sealed class ConstructorCache<T> : IConstructorCache<T>
		where T : class
	{
		public ConstructorCache()
		{
			this.Constructors = typeof(T).GetConstructors(TypeCache.INSTANCE_BINDING)
				.To(constructorInfo => (IConstructorMember)new ConstructorMember(constructorInfo))
				.ToImmutable();
		}

		public IImmutableList<IConstructorMember> Constructors { get; }

		public T Create(params object[] parameters)
		{
			var constructorMember = this.Constructors.First(constructorMember => constructorMember.IsCallableWith(parameters));
			return constructorMember != null
				? (T)constructorMember.Invoke(parameters)
				: throw new ArgumentException($"Create instance of {typeof(T).Name} failed with {parameters?.Length ?? 0} parameters.");
		}
	}
}
