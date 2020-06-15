// Copyright (c) 2020 Samuel Abraham

using Microsoft.Extensions.DependencyInjection;
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
		public TypeCache(IConstructorCache<T> constructorCache
			, IFieldCache<T> fieldCache
			, IMethodCache<T> methodCache
			, IIndexerCache<T> indexerCache
			, IPropertyCache<T> propertyCache
			, IStaticFieldCache<T> staticFieldCache
			, IStaticMethodCache<T> staticMethodCache
			, IStaticPropertyCache<T> staticPropertyCache)
			: base(typeof(T))
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

			this.ConstructorCache = constructorCache;
			this.FieldCache = fieldCache;
			this.MethodCache = methodCache;
			this.IndexerCache = indexerCache;
			this.PropertyCache = propertyCache;
			this.StaticFieldCache = staticFieldCache;
			this.StaticMethodCache = staticMethodCache;
			this.StaticPropertyCache = staticPropertyCache;
		}

		public IImmutableList<RuntimeTypeHandle> GenericInterfaces { get; }

		public IImmutableList<RuntimeTypeHandle> GenericTypes { get; }

		public IImmutableList<RuntimeTypeHandle> Interfaces { get; }

		public IConstructorCache<T> ConstructorCache { get; }

		public IFieldCache<T> FieldCache { get; }

		public IMethodCache<T> MethodCache { get; }

		public IIndexerCache<T> IndexerCache { get; }

		public IPropertyCache<T> PropertyCache { get; }

		public IStaticFieldCache<T> StaticFieldCache { get; }

		public IStaticMethodCache<T> StaticMethodCache { get; }

		public IStaticPropertyCache<T> StaticPropertyCache { get; }
	}
}
