// Copyright (c) 2020 Samuel Abraham

using Sam987883.Common.Models;
using System;
using System.Collections.Immutable;

namespace Sam987883.Reflection
{
	public interface ITypeCache
	{
		T? Create<T>() where T : class, new();

		IMemberAccessor CreateFieldAccessor<T>(T instance) where T : class, new();

		IMemberAccessor CreatePropertyAccessor<T>(T instance) where T : class, new();

		T[] Map<T>(RowSet rowSet) where T : class, new();

		RowSet Map<T>(T[] items, params string[] columns) where T : class, new();
	}

	public interface ITypeCache<T> : IMember
		where T : class
	{
		IImmutableList<RuntimeTypeHandle> GenericInterfaceHandles { get; }

		IImmutableList<RuntimeTypeHandle> GenericTypeHandles { get; }

		IImmutableList<NativeType> GenericTypes { get; }

		IImmutableList<RuntimeTypeHandle> InterfaceHandles { get; }

		IConstructorCache<T> ConstructorCache { get; }

		IFieldCache<T> FieldCache { get; }

		IMethodCache<T> MethodCache { get; }

		IIndexerCache<T> IndexerCache { get; }

		IPropertyCache<T> PropertyCache { get; }

		IStaticFieldCache<T> StaticFieldCache { get; }

		IStaticMethodCache<T> StaticMethodCache { get; }

		IStaticPropertyCache<T> StaticPropertyCache { get; }
	}
}
