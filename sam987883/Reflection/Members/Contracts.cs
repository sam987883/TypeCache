// Copyright (c) 2020 Samuel Abraham

using System;
using System.Collections.Immutable;

namespace sam987883.Reflection.Members
{
	public interface IMember
	{
		IImmutableList<RuntimeTypeHandle> ArrayTypeHandles { get; }

		IImmutableList<Attribute> Attributes { get; }

		bool Internal { get; }

		bool IsString { get; }

		bool IsValueType { get; }

		string Name { get; }

		bool IsNullable { get; }

		bool Public { get; }

		RuntimeTypeHandle TypeHandle { get; }
	}

	public interface IConstructorMember<out T> : IMember
	{
		Delegate Method { get; }

		IImmutableList<IParameter> Parameters { get; }

		T Invoke(params object[] parameters);

		bool IsCallable(params object[] arguments);
	}

	public interface IEnumMember<out T> : IMember
	{
		T Value { get; }
	}

	public interface IFieldMember<in T> : IMember
	{
		object? this[T instance] { get; set; }

		Delegate? Getter { get; }

		Func<T, object?>? GetValue { get; }

		Delegate? Setter { get; }

		Action<T, object?>? SetValue { get; }
	}

	public interface IIndexerMember<in T> : IMember
	{
		object? this[T instance, params object[] indexParameters] { get; set; }

		IMethodMember<T>? GetMethod { get; }

		IMethodMember<T>? SetMethod { get; }
	}

	public interface IMethodMember<in T> : IMember
	{
		Delegate Method { get; }

		IImmutableList<IParameter> Parameters { get; }

		bool Void { get; }

		object? Invoke(T instance, params object[]? parameters);

		bool IsCallable(params object[]? arguments);
	}

	public interface IParameter
	{
		IImmutableList<Attribute> Attributes { get; }

		object? DefaultValue { get; }

		bool HasDefaultValue { get; }

		string Name { get; }

		bool Optional { get; }

		bool Out { get; }

		RuntimeTypeHandle TypeHandle { get; }

		bool Supports(Type type);
	}

	public interface IPropertyMember<in T> : IMember
	{
		object? this[T instance] { get; set; }

		IMethodMember<T>? GetMethod { get; }

		IMethodMember<T>? SetMethod { get; }
	}

	public interface IStaticFieldMember : IMember
	{
		Delegate? Getter { get; }

		Func<object?>? GetValue { get; }

		Delegate? Setter { get; }

		Action<object?>? SetValue { get; }

		object? Value { get; set; }
	}

	public interface IStaticMethodMember : IMember
	{
		Delegate Method { get; }

		IImmutableList<IParameter> Parameters { get; }

		bool Void { get; }

		object Invoke(params object[]? parameters);

		bool IsCallable(params object[]? arguments);
	}

	public interface IStaticPropertyMember : IMember
	{
		IStaticMethodMember? GetMethod { get; }

		IStaticMethodMember? SetMethod { get; }

		object? Value { get; set; }
	}
}
