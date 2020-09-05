// Copyright (c) 2020 Samuel Abraham

using System;
using System.Collections.Immutable;

namespace Sam987883.Reflection
{
	public interface ITypeCache<T> : IMember
		where T : class
	{
		IImmutableList<RuntimeTypeHandle> GenericInterfaces { get; }

		IImmutableList<RuntimeTypeHandle> GenericTypes { get; }

		IImmutableList<RuntimeTypeHandle> Interfaces { get; }

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
