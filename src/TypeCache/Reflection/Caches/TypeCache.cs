// Copyright (c) 2021 Samuel Abraham

using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using TypeCache.Data;
using TypeCache.Extensions;
using TypeCache.Reflection.Accessors;

namespace TypeCache.Reflection.Caches
{
	internal class TypeCache : ITypeCache
	{
		public const BindingFlags INSTANCE_BINDING = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
		public const BindingFlags STATIC_BINDING = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;

		public static StringComparer NameComparer { get; } = StringComparer.Ordinal;

		internal static ITypeCache BackDoor { get; private set; }

		private readonly IServiceProvider _ServiceProvider;

		public TypeCache(IServiceProvider serviceProvider)
		{
			this._ServiceProvider = serviceProvider;
			BackDoor = this;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public object Create(Type type)
			=> this.GetConstructorCache(type).Create();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public T Create<T>() where T : class, new()
			=> this.GetConstructorCache<T>().Create();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public IMemberAccessor CreateFieldAccessor(object instance)
			=> new FieldAccessor(this.GetFieldCache(instance.GetType()), instance);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public IMemberAccessor CreatePropertyAccessor(object instance)
			=> new PropertyAccessor(this.GetPropertyCache(instance.GetType()), instance);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public IConstructorCache<T> GetConstructorCache<T>()
			where T : class
			=> this._ServiceProvider.GetRequiredService<IConstructorCache<T>>();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public IFieldCache<T> GetFieldCache<T>()
			where T : class
			=> this._ServiceProvider.GetRequiredService<IFieldCache<T>>();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public IIndexerCache<T> GetIndexerCache<T>()
			where T : class
			=> this._ServiceProvider.GetRequiredService<IIndexerCache<T>>();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public IMethodCache<T> GetMethodCache<T>()
			where T : class
			=> this._ServiceProvider.GetRequiredService<IMethodCache<T>>();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public IPropertyCache<T> GetPropertyCache<T>()
			where T : class
			=> this._ServiceProvider.GetRequiredService<IPropertyCache<T>>();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public IStaticFieldCache<T> GetStaticFieldCache<T>()
			where T : class
			=> this._ServiceProvider.GetRequiredService<IStaticFieldCache<T>>();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public IStaticMethodCache<T> GetStaticMethodCache<T>()
			where T : class
			=> this._ServiceProvider.GetRequiredService<IStaticMethodCache<T>>();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public IStaticPropertyCache<T> GetStaticPropertyCache<T>()
			where T : class
			=> this._ServiceProvider.GetRequiredService<IStaticPropertyCache<T>>();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public IConstructorCache<object> GetConstructorCache(Type type)
			=> (IConstructorCache<object>)this._ServiceProvider.GetRequiredService(typeof(IConstructorCache<>).MakeGenericType(type));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public IFieldCache<object> GetFieldCache(Type type)
			=> (IFieldCache<object>)this._ServiceProvider.GetRequiredService(typeof(IFieldCache<>).MakeGenericType(type));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public IIndexerCache<object> GetIndexerCache(Type type)
			=> (IIndexerCache<object>)this._ServiceProvider.GetRequiredService(typeof(IIndexerCache<>).MakeGenericType(type));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public IMethodCache<object> GetMethodCache(Type type)
			=> (IMethodCache<object>)this._ServiceProvider.GetRequiredService(typeof(IMethodCache<>).MakeGenericType(type));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public IPropertyCache<object> GetPropertyCache(Type type)
			=> (IPropertyCache<object>)this._ServiceProvider.GetRequiredService(typeof(IPropertyCache<>).MakeGenericType(type));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public IStaticFieldCache<object> GetStaticFieldCache(Type type)
			=> (IStaticFieldCache<object>)this._ServiceProvider.GetRequiredService(typeof(IStaticFieldCache<>).MakeGenericType(type));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public IStaticMethodCache<object> GetStaticMethodCache(Type type)
			=> (IStaticMethodCache<object>)this._ServiceProvider.GetRequiredService(typeof(IStaticMethodCache<>).MakeGenericType(type));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public IStaticPropertyCache<object> GetStaticPropertyCache(Type type)
			=> (IStaticPropertyCache<object>)this._ServiceProvider.GetRequiredService(typeof(IStaticPropertyCache<>).MakeGenericType(type));

		public T[] Map<T>(RowSet rowSet, IEqualityComparer<string>? comparer = null)
			where T : class, new()
		{
			comparer ??= TypeCache.NameComparer;

			var items = new T[rowSet.Rows.Length];
			var propertyCache = this.GetPropertyCache<T>();
			var members = rowSet.Columns.Match(propertyCache.SetNames, comparer);

			rowSet.Rows.Do((row, rowIndex) =>
			{
				var item = this.Create<T>();
				members.Do((member, columnIndex) => propertyCache.Properties[member][item] = row[columnIndex]);
				items[rowIndex] = item;
			});

			return items;
		}

		public RowSet Map<T>(T[] items, string[] columns, IEqualityComparer<string>? comparer = null)
			where T : class, new()
		{
			comparer ??= TypeCache.NameComparer;

			var propertyCache = this.GetPropertyCache<T>();
			var rowSet = new RowSet
			{
				Columns = columns.Any()
					? columns.Match(propertyCache.GetNames, comparer).ToArray()
					: propertyCache.GetNames.ToArray(),
				Rows = new object?[items.Length][]
			};

			items.Do((item, rowIndex) =>
			{
				var row = new object?[rowSet.Columns.Length];
				rowSet.Columns.Do((member, columnIndex) => row[columnIndex] = propertyCache.Properties[member][item]);
				rowSet.Rows[rowIndex] = row;
			});

			return rowSet;
		}
	}
}
