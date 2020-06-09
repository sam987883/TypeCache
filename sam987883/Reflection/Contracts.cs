// Copyright (c) 2020 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using sam987883.Reflection.Members;

namespace sam987883.Reflection
{
	public interface IEnumCache<T> : IMember
		where T : struct, Enum
	{
		IImmutableList<IEnumMember<T>> Fields { get; }

		bool Flags { get; }

		TypeCode UnderlyingTypeCode { get; }

		RuntimeTypeHandle UnderlyingTypeHandle { get; }
	}

	public interface IFieldCache<T>
	{
		IFieldAccessor<T> CreateAccessor(T instance);

		IImmutableDictionary<string, IFieldMember<T>> Fields { get; }

		void Map(T from, T to);
	}

	public interface IMethodCache<T>
	{
		(D? Method, bool Exists) GetMethod<D>(string name) where D : Delegate;

		IImmutableDictionary<string, IImmutableList<IMethodMember<T>>> Methods { get; }
	}

	public interface IPropertyCache<T>
	{
		IPropertyAccessor<T> CreateAccessor(T instance);

		IImmutableDictionary<string, IPropertyMember<T>> Properties { get; }

		void Map(T from, T to);
	}

	public interface IStaticFieldCache<T>
	{
		IImmutableDictionary<string, IStaticFieldMember> Fields { get; }
	}

	public interface IStaticMethodCache<T>
	{
		(D? Method, bool Exists) GetMethod<D>(string name) where D : Delegate;

		IImmutableDictionary<string, IImmutableList<IStaticMethodMember>> Methods { get; }
	}

	public interface IStaticPropertyCache<T>
	{
		IImmutableDictionary<string, IStaticPropertyMember> Properties { get; }
	}

	public interface ITypeCache<T> : IMember
	{
		IImmutableList<IConstructorMember<T>> Constructors { get; }

		IImmutableList<RuntimeTypeHandle> GenericInterfaces { get; }

		IImmutableList<RuntimeTypeHandle> GenericTypes { get; }

		IImmutableList<IIndexerMember<T>> Indexers { get; }

		IImmutableList<RuntimeTypeHandle> Interfaces { get; }

		T Create(params object[] parameters);
	}

	public interface IFieldAccessor<in T> : IReadOnlyDictionary<string, object>
	{
		new object? this[string key] { get; set; }
	}

	public interface IFieldMapper<in FROM, in TO>
	{
		void Map(FROM from, TO to);
	}

	public interface IPropertyAccessor<in T> : IReadOnlyDictionary<string, object>
	{
		new object? this[string key] { get; set; }
	}

	public interface IPropertyMapper<in FROM, in TO>
	{
		void Map(FROM from, TO to);
	}
}
