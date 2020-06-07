// Copyright (c) 2020 Samuel Abraham

using sam987883.Extensions;
using sam987883.Reflection.Members;
using System;
using System.Collections.Immutable;
using System.Reflection;

namespace sam987883.Reflection
{
	public static class TypeCache
	{
		public const BindingFlags INSTANCE_BINDING = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
		public const BindingFlags STATIC_BINDING = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;

		internal static StringComparer NameComparer { get; } = StringComparer.Ordinal;
	}

	internal sealed class TypeCache<T> : Member, ITypeCache<T>
	{
		public TypeCache() : base(typeof(T))
		{
			var type = typeof(T);

			var interfaces = type.GetInterfaces();
			this.GenericInterfaces = interfaces
				.If(_ => _.IsGenericType)
				.To(_ => _.GetGenericTypeDefinition().TypeHandle)
				.ToImmutable(interfaces.Length);

			this.GenericTypes = type.IsGenericType
				? type.GenericTypeArguments
					.To(genericType => genericType.TypeHandle)
					.ToImmutable(type.GenericTypeArguments.Length)
				: ImmutableArray<RuntimeTypeHandle>.Empty;

			this.Interfaces = interfaces
				.To(_ => _.TypeHandle)
				.ToImmutable(interfaces.Length);

			this.Constructors = type.GetConstructors(TypeCache.INSTANCE_BINDING)
				.If(constructorInfo => !constructorInfo.Name.Is(".ctor"))
				.To(constructorInfo => (IConstructorMember<T>)new ConstructorMember<T>(constructorInfo))
				.ToImmutable();

			this.Indexers = type.GetProperties(TypeCache.INSTANCE_BINDING)
				.If(propertyInfo => propertyInfo.GetIndexParameters().Any())
				.To(propertyInfo => (IIndexerMember<T>)new IndexerMember<T>(propertyInfo))
				.ToImmutable();
		}

		public IImmutableList<IConstructorMember<T>> Constructors { get; }

		public IImmutableList<RuntimeTypeHandle> GenericInterfaces { get; }

		public IImmutableList<RuntimeTypeHandle> GenericTypes { get; }

		public IImmutableList<IIndexerMember<T>> Indexers { get; }

		public IImmutableList<RuntimeTypeHandle> Interfaces { get; }

		public T Create(params object[] parameters)
		{
			var constructorMember = this.Constructors.First(constructorMember => constructorMember.IsCallable(parameters));
			if (constructorMember.Exists)
				return constructorMember.Value.Invoke();
			else
				throw new ArgumentException($"Create instance of {this.Name} failed with {parameters?.Length ?? 0} parameters.");
		}
	}
}
