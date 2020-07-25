// Copyright (c) 2020 Samuel Abraham

using Microsoft.Extensions.DependencyInjection;
using sam987883.Common.Extensions;
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
		private readonly IServiceProvider _ServiceProvider;

		public TypeCache(IServiceProvider serviceProvider)
			: base(typeof(T))
		{
			this._ServiceProvider = serviceProvider;
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
		}

		public IImmutableList<RuntimeTypeHandle> GenericInterfaces { get; }

		public IImmutableList<RuntimeTypeHandle> GenericTypes { get; }

		public IImmutableList<RuntimeTypeHandle> Interfaces { get; }

		public IConstructorCache<T> ConstructorCache => this._ServiceProvider.GetRequiredService<IConstructorCache<T>>();

		public IFieldCache<T> FieldCache => this._ServiceProvider.GetRequiredService<IFieldCache<T>>();

		public IMethodCache<T> MethodCache => this._ServiceProvider.GetRequiredService<IMethodCache<T>>();

		public IIndexerCache<T> IndexerCache => this._ServiceProvider.GetRequiredService<IIndexerCache<T>>();

		public IPropertyCache<T> PropertyCache => this._ServiceProvider.GetRequiredService<IPropertyCache<T>>();

		public IStaticFieldCache<T> StaticFieldCache => this._ServiceProvider.GetRequiredService<IStaticFieldCache<T>>();

		public IStaticMethodCache<T> StaticMethodCache => this._ServiceProvider.GetRequiredService<IStaticMethodCache<T>>();

		public IStaticPropertyCache<T> StaticPropertyCache => this._ServiceProvider.GetRequiredService<IStaticPropertyCache<T>>();
	}
}
