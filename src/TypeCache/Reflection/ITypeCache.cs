// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using TypeCache.Data;

namespace TypeCache.Reflection
{
	public interface ITypeCache
	{
		object Create(Type type);

		T Create<T>() where T : class, new();

		IMemberAccessor CreateFieldAccessor(object instance);

		IMemberAccessor CreatePropertyAccessor(object instance);

		IConstructorCache<T> GetConstructorCache<T>() where T : class;

		IFieldCache<T> GetFieldCache<T>() where T : class;

		IIndexerCache<T> GetIndexerCache<T>() where T : class;

		IMethodCache<T> GetMethodCache<T>() where T : class;

		IPropertyCache<T> GetPropertyCache<T>() where T : class;

		IStaticFieldCache<T> GetStaticFieldCache<T>() where T : class;

		IStaticMethodCache<T> GetStaticMethodCache<T>() where T : class;

		IStaticPropertyCache<T> GetStaticPropertyCache<T>() where T : class;

		IConstructorCache<object> GetConstructorCache(Type type);

		IFieldCache<object> GetFieldCache(Type type);

		IIndexerCache<object> GetIndexerCache(Type type);

		IMethodCache<object> GetMethodCache(Type type);

		IPropertyCache<object> GetPropertyCache(Type type);

		IStaticFieldCache<object> GetStaticFieldCache(Type type);

		IStaticMethodCache<object> GetStaticMethodCache(Type type);

		IStaticPropertyCache<object> GetStaticPropertyCache(Type type);

		T[] Map<T>(RowSet rowSet, IEqualityComparer<string>? comparer = null) where T : class, new();

		RowSet Map<T>(T[] items, string[] columns, IEqualityComparer<string>? comparer = null) where T : class, new();
	}
}
