// Copyright (c) 2020 Samuel Abraham

using Sam987883.Common.Extensions;
using Sam987883.Common.Models;
using Sam987883.Reflection.Accessors;
using Sam987883.Reflection.Extensions;
using Sam987883.Reflection.Members;
using System;
using System.Collections.Immutable;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Sam987883.Reflection.Caches
{
	internal class TypeCache : ITypeCache
	{
		public const BindingFlags INSTANCE_BINDING = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
		public const BindingFlags STATIC_BINDING = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;

		public static StringComparer NameComparer { get; } = StringComparer.Ordinal;

		private readonly IServiceProvider ServiceProvider;

		public TypeCache(IServiceProvider serviceProvider) =>
			this.ServiceProvider = serviceProvider;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public T Create<T>() where T : class, new() =>
			this.ServiceProvider.GetRequiredService<IConstructorCache<T>>().Create();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public IMemberAccessor CreateFieldAccessor<T>(T instance) where T : class, new() =>
			new FieldAccessor<T>(this.ServiceProvider.GetRequiredService<IFieldCache<T>>(), instance);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public IMemberAccessor CreatePropertyAccessor<T>(T instance) where T : class, new() =>
			new PropertyAccessor<T>(this.ServiceProvider.GetRequiredService<IPropertyCache<T>>(), instance);

		public T[] Map<T>(RowSet rowSet) where T : class, new()
		{
			var items = new T[rowSet.Rows.Length];
			var item = this.Create<T>();
			var accessor = this.CreatePropertyAccessor(item);
			var members = rowSet.Columns.Match(accessor.SetNames, TypeCache.NameComparer);

			rowSet.Rows.Do((row, rowIndex) =>
			{
				item = this.Create<T>();
				accessor = this.CreatePropertyAccessor(item);
				members.Do((member, columnIndex) => accessor[member] = row[columnIndex]);
				items[rowIndex] = item;
			});

			return items;
		}

		public RowSet Map<T>(T[] items, params string[] columns) where T : class, new()
		{
			if (items.Any())
			{
				var accessor = this.CreatePropertyAccessor(items.First().Value);

				var rowSet = new RowSet
				{
					Columns = columns.Any()
						? columns.Match(accessor.GetNames, TypeCache.NameComparer).ToArray()
						: accessor.GetNames.ToArrayOf(accessor.GetNames.Count),
					Rows = new object?[items.Length][]
				};

				items.Do((item, rowIndex) =>
				{
					var row = new object?[rowSet.Columns.Length];
					var accessor = this.CreatePropertyAccessor(item);
					rowSet.Columns.Do((member, columnIndex) => row[columnIndex] = accessor[member]);
					rowSet.Rows[rowIndex] = row;
				});

				return rowSet;
			}

			return new RowSet
			{
				Columns = columns,
				Rows = new object?[0][]
			};
		}
	}

	internal sealed class TypeCache<T> : Member, ITypeCache<T>
		where T : class
	{
		private readonly IServiceProvider _ServiceProvider;

		public TypeCache(IServiceProvider serviceProvider)
			: base(typeof(T))
		{
			this._ServiceProvider = serviceProvider;
			var type = typeof(T);

			var interfaces = type.GetInterfaces();
			this.GenericInterfaceHandles = interfaces
				.If(_ => _.IsGenericType)
				.To(_ => _.GetGenericTypeDefinition().TypeHandle)
				.ToImmutable();

			this.GenericTypeHandles = type.IsGenericType
				? type.GenericTypeArguments
					.To(genericType => genericType.TypeHandle)
					.ToImmutable(type.GenericTypeArguments.Length)
				: ImmutableArray<RuntimeTypeHandle>.Empty;

			this.GenericTypes = type.IsGenericType
				? type.GenericTypeArguments
					.To(genericType => genericType.ToNativeType())
					.ToImmutable(type.GenericTypeArguments.Length)
				: ImmutableArray<NativeType>.Empty;

			this.InterfaceHandles = interfaces
				.To(_ => _.TypeHandle)
				.ToImmutable();
		}

		public IImmutableList<RuntimeTypeHandle> GenericInterfaceHandles { get; }

		public IImmutableList<RuntimeTypeHandle> GenericTypeHandles { get; }

		public IImmutableList<NativeType> GenericTypes { get; }

		public IImmutableList<RuntimeTypeHandle> InterfaceHandles { get; }

		public IConstructorCache<T> ConstructorCache =>
			this._ServiceProvider.GetRequiredService<IConstructorCache<T>>();

		public IFieldCache<T> FieldCache =>
			this._ServiceProvider.GetRequiredService<IFieldCache<T>>();

		public IMethodCache<T> MethodCache =>
			this._ServiceProvider.GetRequiredService<IMethodCache<T>>();

		public IIndexerCache<T> IndexerCache =>
			this._ServiceProvider.GetRequiredService<IIndexerCache<T>>();

		public IPropertyCache<T> PropertyCache =>
			this._ServiceProvider.GetRequiredService<IPropertyCache<T>>();

		public IStaticFieldCache<T> StaticFieldCache =>
			this._ServiceProvider.GetRequiredService<IStaticFieldCache<T>>();

		public IStaticMethodCache<T> StaticMethodCache =>
			this._ServiceProvider.GetRequiredService<IStaticMethodCache<T>>();

		public IStaticPropertyCache<T> StaticPropertyCache =>
			this._ServiceProvider.GetRequiredService<IStaticPropertyCache<T>>();
	}
}
