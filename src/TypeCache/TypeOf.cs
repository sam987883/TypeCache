// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using TypeCache.Reflection;
using TypeCache.Reflection.Extensions;
using static TypeCache.Default;

namespace TypeCache;

public static class TypeOf<T>
{
	public static TypeMember Member { get; } = typeof(T).GetTypeMember()!;

	public static IReadOnlyList<Attribute> Attributes => Member.Attributes;

	public static TypeMember? BaseType => Member.BaseType;

	public static TypeMember? ElementType => Member.ElementType;

	public static RuntimeTypeHandle? GenericHandle => Member.GenericHandle;

	public static IReadOnlyList<TypeMember> GenericTypes => Member.GenericTypes;

	/// <inheritdoc cref="Type.TypeHandle"/>
	public static RuntimeTypeHandle Handle => Member.Handle;

	public static IReadOnlyList<TypeMember> InterfaceTypes => Member.InterfaceTypes;

	/// <inheritdoc cref="Type.IsVisible"/>
	public static bool Internal => Member.Internal;

	public static Kind Kind => Member.Kind;

	public static string Name => Member.Name;

	/// <inheritdoc cref="Type.Namespace"/>
	public static string Namespace => Member.Namespace;

	public static bool Nullable => Member.Nullable;

	/// <inheritdoc cref="Type.IsPublic"/>
	public static bool Public => Member.Public;

	public static SystemType SystemType => Member.SystemType;

	public static IReadOnlyList<ConstructorMember> Constructors => Member.Constructors;

	public static IReadOnlyList<FieldMember> Fields => Member.Fields;

	public static IReadOnlyList<MethodMember> Methods => Member.Methods;

	public static IReadOnlyList<PropertyMember> Properties => Member.Properties;

	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static T? Create(params object?[]? parameters)
		=> (T?)Member.Create(parameters);

	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static bool Implements<V>()
		where V : class
		=> Member.Implements<V>();

	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static bool Implements(Type type)
		=> Member.Implements(type);

	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static object? InvokeMethod(string name, params object?[]? parameters)
		=> Member.InvokeMethod(name, parameters);

	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static object? InvokeGenericMethod<T1>(string name, params object?[]? parameters)
		=> Member.InvokeGenericMethod(name, new[] { typeof(T1) }, parameters);

	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static object? InvokeGenericMethod<T1, T2>(string name, params object?[]? parameters)
		=> Member.InvokeGenericMethod(name, new[] { typeof(T1), typeof(T2) }, parameters);

	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static object? InvokeGenericMethod<T1, T2, T3>(string name, params object?[]? parameters)
		=> Member.InvokeGenericMethod(name, new[] { typeof(T1), typeof(T2), typeof(T3) }, parameters);

	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static object? InvokeGenericMethod<T1, T2, T3, T4>(string name, params object?[]? parameters)
		=> Member.InvokeGenericMethod(name, new[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4) }, parameters);

	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static object? InvokeGenericMethod<T1, T2, T3, T4, T5>(string name, params object?[]? parameters)
		=> Member.InvokeGenericMethod(name, new[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5) }, parameters);

	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static object? InvokeGenericMethod<T1, T2, T3, T4, T5, T6>(string name, params object?[]? parameters)
		=> Member.InvokeGenericMethod(name, new[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6) }, parameters);

	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static object? InvokeGenericMethod(string name, Type[] genericTypes, params object?[]? parameters)
		=> Member.InvokeGenericMethod(name, genericTypes, parameters);

	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static bool Is<V>()
		=> Member.Is<V>();

	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static bool Is(Type type)
		=> Member.Is(type);

	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static D? GetConstructor<D>()
		where D : Delegate
		=> Member.GetConstructor<D>();

	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static D? GetMethod<D>(string name, bool isStatic = false)
		where D : Delegate
		=> Member.GetMethod<D>(name, isStatic);
}
