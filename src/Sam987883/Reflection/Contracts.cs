// Copyright (c) 2020 Samuel Abraham

using System;
using System.Collections.Immutable;
using Sam987883.Reflection.Members;

namespace Sam987883.Reflection
{
	public interface IConstructorCache<T>
		where T : class
	{
		IImmutableList<IConstructorMember<T>> Constructors { get; }

		T Create(params object[] parameters);
	}

	public interface IEnumCache<T> : IMember
		where T : struct, Enum
	{
		IImmutableList<IEnumMember<T>> Fields { get; }

		bool Flags { get; }

		NativeType UnderlyingType { get; }

		RuntimeTypeHandle UnderlyingTypeHandle { get; }
	}

	public interface IFieldCache<T>
		where T : class
	{
		void Map(T from, T to);

		IImmutableDictionary<string, IFieldMember<T>> Fields { get; }
	}

	public interface IIndexerCache<T>
		where T : class
	{
		IImmutableList<IIndexerMember<T>> Indexers { get; }
	}

	public interface IMethodCache<T>
		where T : class
	{
		(D? Method, bool Exists) GetMethod<D>(string name) where D : Delegate;

		IImmutableDictionary<string, IImmutableList<IMethodMember<T>>> Methods { get; }
	}

	public interface IPropertyCache<T>
		where T : class
	{
		void Map(T from, T to);

		IImmutableDictionary<string, IPropertyMember<T>> Properties { get; }
	}

	public interface IStaticFieldCache<T>
		where T : class
	{
		IImmutableDictionary<string, IStaticFieldMember> Fields { get; }
	}

	public interface IStaticMethodCache<T>
		where T : class
	{
		(D? Method, bool Exists) GetMethod<D>(string name) where D : Delegate;

		IImmutableDictionary<string, IImmutableList<IStaticMethodMember>> Methods { get; }
	}

	public interface IStaticPropertyCache<T>
		where T : class
	{
		IImmutableDictionary<string, IStaticPropertyMember> Properties { get; }
	}

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
