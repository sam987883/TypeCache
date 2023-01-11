// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using TypeCache.Collections;
using TypeCache.Extensions;
using TypeCache.Reflection;
using static System.StringComparison;
using static System.Reflection.BindingFlags;

namespace TypeCache.Extensions;

public static class TypeExtensions
{
	private const BindingFlags BINDING_FLAGS = FlattenHierarchy | Instance | NonPublic | Public | Static;
	private const BindingFlags INSTANCE_BINDING_FLAGS = FlattenHierarchy | Instance | NonPublic | Public;
	private const BindingFlags STATIC_BINDING_FLAGS = FlattenHierarchy | NonPublic | Public | Static;

	/// <exception cref="MissingMethodException"></exception>
	public static object? Create(this Type @this, params object?[]? parameters)
		=> @this.FindConstructor(parameters) switch
		{
			null when @this.IsValueType && parameters?.Any() is not true => TypeStore.DefaultValueTypeConstructorInvokes[@this.TypeHandle].Invoke(),
			null => throw new MissingMethodException(@this.Name(), @this.Name),
			var constructorInfo => constructorInfo.InvokeMethod(parameters)
		};

	public static ConstructorInfo? FindConstructor(this Type @this, params object?[]? arguments)
		=> @this.GetConstructors(INSTANCE_BINDING_FLAGS)
			.FirstOrDefault(constructor => constructor.GetParameters().IsCallableWith(arguments));

	public static MethodInfo? FindMethod(this Type @this, string name, Type[] argumentTypes, bool nameIgnoreCase = false)
		=> @this.GetMethod(name, nameIgnoreCase ? INSTANCE_BINDING_FLAGS | IgnoreCase : INSTANCE_BINDING_FLAGS, argumentTypes);

	public static MethodInfo? FindMethod(this Type @this, string name, object?[]? arguments, bool nameIgnoreCase = false)
		=> @this.GetMethods(INSTANCE_BINDING_FLAGS).FirstOrDefault(method =>
			method.Name().Is(name, nameIgnoreCase ? OrdinalIgnoreCase : Ordinal) && method.GetParameters().IsCallableWith(arguments));

	public static MethodInfo? FindStaticMethod(this Type @this, string name, object?[]? arguments, bool nameIgnoreCase = false)
		=> @this.GetMethods(STATIC_BINDING_FLAGS).FirstOrDefault(method =>
			method.Name().Is(name, nameIgnoreCase ? OrdinalIgnoreCase : Ordinal) && method.GetParameters().IsCallableWith(arguments));

	public static object? GetFieldValue(this Type @this, string name, object instance, bool nameIgnoreCase = false)
		=> @this.GetField(name, nameIgnoreCase ? INSTANCE_BINDING_FLAGS | IgnoreCase : INSTANCE_BINDING_FLAGS)?
			.GetValue(instance);

	public static Kind GetKind(this Type @this)
		=> @this switch
		{
			{ IsPointer: true } => Kind.Pointer,
			{ IsInterface: true } => Kind.Interface,
			{ IsClass: true } => Kind.Class,
			{ IsValueType: true } => Kind.Struct,
			_ => throw new UnreachableException(Invariant($"{nameof(GetKind)}({nameof(Type)}): [{@this?.Name() ?? "null"}] is not supported."))
		};

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static ObjectType GetObjectType(this Type @this)
		=> TypeStore.ObjectTypes[@this.TypeHandle];

	public static object? GetPropertyValue(this Type @this, string name, object instance, bool nameIgnoreCase = false, params object?[]? index)
		=> @this.GetProperty(name, nameIgnoreCase ? INSTANCE_BINDING_FLAGS | IgnoreCase : INSTANCE_BINDING_FLAGS)?
			.GetPropertyValue(instance, index);

	public static object? GetStaticFieldValue(this Type @this, string name, bool nameIgnoreCase = false)
		=> @this.GetField(name, nameIgnoreCase ? STATIC_BINDING_FLAGS | IgnoreCase : STATIC_BINDING_FLAGS)?
			.GetFieldValue(null);

	public static object? GetStaticPropertyValue(this Type @this, string name, bool nameIgnoreCase = false, params object?[]? index)
		=> @this.GetProperty(name, nameIgnoreCase ? STATIC_BINDING_FLAGS | IgnoreCase : STATIC_BINDING_FLAGS)?
			.GetPropertyValue(null, index);

	public static SystemType GetSystemType(this Type @this)
		=> @this switch
		{
			{ IsArray: true } => SystemType.Array,
			{ IsEnum: true } => TypeStore.SystemTypes[@this.GetEnumUnderlyingType().TypeHandle],
			_ when TypeStore.SystemTypes.TryGetValue(@this.ToGenericType()?.TypeHandle ?? @this.TypeHandle, out var systemType) => systemType,
			_ => SystemType.None
		};

	public static bool Implements(this Type @this, Type type)
	{
		return type switch
		{
			{ IsGenericTypeDefinition: false } => @this.IsAssignableTo(type),
			{ IsInterface: true } => @this.GetInterfaces().Any(_ => type.Is(_.ToGenericType())),
			_ => isDescendantOf(@this.BaseType, type)
		};

		static bool isDescendantOf(Type? baseType, Type type)
		{
			while (baseType is not null)
			{
				if (baseType.Is(type))
					return true;
				baseType = baseType.BaseType;
			}
			return false;
		}
	}

	public static object? InvokeMethod(this Type @this, string name, object instance, params object?[]? arguments)
		=> @this.FindMethod(name, arguments)?
			.InvokeMethod(arguments?.Any() is true ? new[] { instance }.Concat(arguments).ToArray() : new[] { instance });

	public static object? InvokeMethod(this Type @this, string name, Type[] genericTypes, object instance, params object?[]? arguments)
		=> @this.FindMethod(name, arguments)?
			.MakeGenericMethod(genericTypes)
			.InvokeMethod(arguments?.Any() is true ? new[] { instance }.Concat(arguments).ToArray() : new[] { instance });

	public static object? InvokeMethod<T1>(this Type @this, string name, object instance, params object?[]? arguments)
		=> @this.FindMethod(name, arguments)?
			.MakeGenericMethod<T1>()
			.InvokeMethod(arguments?.Any() is true ? new[] { instance }.Concat(arguments).ToArray() : new[] { instance });

	public static object? InvokeMethod<T1, T2>(this Type @this, string name, object instance, params object?[]? arguments)
		=> @this.FindMethod(name, arguments)?
			.MakeGenericMethod<T1, T2>()
			.InvokeMethod(arguments?.Any() is true ? new[] { instance }.Concat(arguments).ToArray() : new[] { instance });

	public static object? InvokeMethod<T1, T2, T3>(this Type @this, string name, object instance, params object?[]? arguments)
		=> @this.FindMethod(name, arguments)?
			.MakeGenericMethod<T1, T2, T3>()
			.InvokeMethod(arguments?.Any() is true ? new[] { instance }.Concat(arguments).ToArray() : new[] { instance });

	public static object? InvokeMethod<T1, T2, T3, T4>(this Type @this, string name, object instance, params object?[]? arguments)
		=> @this.FindMethod(name, arguments)?
			.MakeGenericMethod<T1, T2, T3, T4>()
			.InvokeMethod(arguments?.Any() is true ? new[] { instance }.Concat(arguments).ToArray() : new[] { instance });

	public static object? InvokeMethod<T1, T2, T3, T4, T5>(this Type @this, string name, object instance, params object?[]? arguments)
		=> @this.FindMethod(name, arguments)?
			.MakeGenericMethod<T1, T2, T3, T4, T5>()
			.InvokeMethod(arguments?.Any() is true ? new[] { instance }.Concat(arguments).ToArray() : new[] { instance });

	public static object? InvokeMethod<T1, T2, T3, T4, T5, T6>(this Type @this, string name, object instance, params object?[]? arguments)
		=> @this.FindMethod(name, arguments)?
			.MakeGenericMethod<T1, T2, T3, T4, T5, T6>()
			.InvokeMethod(arguments?.Any() is true ? new[] { instance }.Concat(arguments).ToArray() : new[] { instance });

	public static object? InvokeStaticMethod(this Type @this, string name, params object?[]? arguments)
		=> @this.FindStaticMethod(name, arguments)?
			.InvokeMethod(null, arguments);

	public static object? InvokeStaticMethod(this Type @this, string name, Type[] genericTypes, params object?[]? arguments)
		=> @this.FindMethod(name, arguments)?
			.MakeGenericMethod(genericTypes)
			.InvokeMethod(null, arguments);

	public static object? InvokeStaticMethod<T1>(this Type @this, string name, params object?[]? arguments)
		=> @this.FindStaticMethod(name, arguments)?
			.MakeGenericMethod<T1>()
			.InvokeMethod(null, arguments);

	public static object? InvokeStaticMethod<T1, T2>(this Type @this, string name, params object?[]? arguments)
		=> @this.FindStaticMethod(name, arguments)?
			.MakeGenericMethod<T1, T2>()
			.InvokeMethod(null, arguments);

	public static object? InvokeStaticMethod<T1, T2, T3>(this Type @this, string name, params object?[]? arguments)
		=> @this.FindStaticMethod(name, arguments)?
			.MakeGenericMethod<T1, T2, T3>()
			.InvokeMethod(null, arguments);

	public static object? InvokeStaticMethod<T1, T2, T3, T4>(this Type @this, string name, params object?[]? arguments)
		=> @this.FindStaticMethod(name, arguments)?
			.MakeGenericMethod<T1, T2, T3, T4>()
			.InvokeMethod(null, arguments);

	public static object? InvokeStaticMethod<T1, T2, T3, T4, T5>(this Type @this, string name, params object?[]? arguments)
		=> @this.FindStaticMethod(name, arguments)?
			.MakeGenericMethod<T1, T2, T3, T4, T5>()
			.InvokeMethod(null, arguments);

	public static object? InvokeStaticMethod<T1, T2, T3, T4, T5, T6>(this Type @this, string name, params object?[]? arguments)
		=> @this.FindStaticMethod(name, arguments)?
			.MakeGenericMethod<T1, T2, T3, T4, T5, T6>()
			.InvokeMethod(null, arguments);

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/> == <see langword="typeof"/>(<typeparamref name="T"/>);</c>
	/// </summary>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool Is<T>(this Type? @this)
		=> @this == typeof(T);

	[DebuggerHidden]
	public static bool Is(this Type @this, Type? type)
		=> @this.IsGenericTypeDefinition || type?.IsGenericTypeDefinition is true ? @this.ToGenericType() == type.ToGenericType() : @this == type;

	/// <summary>
	/// <c>=&gt; <paramref name="types"/>.Any(@<paramref name="this"/>.Is);</c>
	/// </summary>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool IsAny(this Type @this, params Type[] types)
		=> types.Any(@this.Is);

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.IsAny(<see langword="typeof"/>(<typeparamref name="T1"/>));</c>
	/// </summary>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool IsAny<T1>(this Type @this)
		=> @this.IsAny(typeof(T1));

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.IsAny(<see langword="typeof"/>(<typeparamref name="T1"/>),
	/// <see langword="typeof"/>(<typeparamref name="T2"/>));</c>
	/// </summary>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool IsAny<T1, T2>(this Type @this)
		=> @this.IsAny(typeof(T1), typeof(T2));

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.IsAny(<see langword="typeof"/>(<typeparamref name="T1"/>),
	/// <see langword="typeof"/>(<typeparamref name="T2"/>),
	/// <see langword="typeof"/>(<typeparamref name="T3"/>));</c>
	/// </summary>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool IsAny<T1, T2, T3>(this Type @this)
		=> @this.IsAny(typeof(T1), typeof(T2), typeof(T3));

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.IsAny(<see langword="typeof"/>(<typeparamref name="T1"/>),
	/// <see langword="typeof"/>(<typeparamref name="T2"/>),
	/// <see langword="typeof"/>(<typeparamref name="T3"/>),
	/// <see langword="typeof"/>(<typeparamref name="T4"/>));</c>
	/// </summary>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool IsAny<T1, T2, T3, T4>(this Type @this)
		=> @this.IsAny(typeof(T1), typeof(T2), typeof(T3), typeof(T4));

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.IsAny(<see langword="typeof"/>(<typeparamref name="T1"/>),
	/// <see langword="typeof"/>(<typeparamref name="T2"/>),
	/// <see langword="typeof"/>(<typeparamref name="T3"/>),
	/// <see langword="typeof"/>(<typeparamref name="T4"/>),
	/// <see langword="typeof"/>(<typeparamref name="T5"/>));</c>
	/// </summary>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool IsAny<T1, T2, T3, T4, T5>(this Type @this)
		=> @this.IsAny(typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5));

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.IsAny(<see langword="typeof"/>(<typeparamref name="T1"/>),
	/// <see langword="typeof"/>(<typeparamref name="T2"/>),
	/// <see langword="typeof"/>(<typeparamref name="T3"/>),
	/// <see langword="typeof"/>(<typeparamref name="T4"/>),
	/// <see langword="typeof"/>(<typeparamref name="T5"/>),
	/// <see langword="typeof"/>(<typeparamref name="T6"/>));</c>
	/// </summary>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool IsAny<T1, T2, T3, T4, T5, T6>(this Type @this)
		=> @this.IsAny(typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6));

	/// <inheritdoc cref="Type.IsAssignableFrom(Type?)"/>
	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.IsAssignableFrom(<see langword="typeof"/>(<typeparamref name="T"/>));</c>
	/// </summary>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool IsAssignableFrom<T>(this Type @this)
		=> @this.IsAssignableFrom(typeof(T));

	/// <inheritdoc cref="Type.IsAssignableTo(Type?)"/>
	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.IsAssignableTo(<see langword="typeof"/>(<typeparamref name="T"/>));</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool IsAssignableTo<T>(this Type @this)
		=> @this.IsAssignableTo(typeof(T));

	[DebuggerHidden]
	public static bool IsConvertible(this Type @this)
		=> @this.IsAssignableTo<IConvertible>()
			|| (@this.ToGenericType() == typeof(Nullable<>) && @this.GenericTypeArguments.First().IsAssignableTo<IConvertible>());

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.IsAssignableTo&lt;<see cref="IEnumerable{T}"/>&gt;();</c>
	/// </summary>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool IsEnumerableOf<T>(this Type @this)
		=> @this.IsAssignableTo<IEnumerable<T>>();

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.GetSystemType().IsEnumUnderlyingType();</c>
	/// </summary>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool IsEnumUnderlyingType(this Type @this)
		=> @this.GetSystemType().IsEnumUnderlyingType();

	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.IsPointer &amp;&amp; !@<paramref name="this"/>.IsByRef &amp;&amp; !@<paramref name="this"/>.IsByRefLike;</c>
	/// </remarks>
	[DebuggerHidden]
	internal static bool IsInvokable(this Type @this)
		=> !@this.IsPointer && !@this.IsByRef && !@this.IsByRefLike;

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.IsClass || @<paramref name="this"/>.IsPointer || <see langword="typeof"/>(Nullable&lt;&gt;) == @<paramref name="this"/>.ToGenericType();</c>
	/// </summary>
	[DebuggerHidden]
	public static bool IsNullable(this Type @this)
		=> @this.IsClass || @this.IsPointer || typeof(Nullable<>) == @this.ToGenericType();

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.Is(<paramref name="type"/>) || @<paramref name="this"/>.Implements(<paramref name="type"/>);</c>
	/// </summary>
	[DebuggerHidden]
	public static bool IsOrImplements(this Type @this, Type type)
		=> @this.Is(type) || @this.Implements(type);

	[DebuggerHidden]
	public static void SetFieldValue(this Type @this, string name, object instance, object? value, bool nameIgnoreCase = false)
		=> @this.GetField(name, nameIgnoreCase ? INSTANCE_BINDING_FLAGS | IgnoreCase : INSTANCE_BINDING_FLAGS)?
			.SetFieldValue(instance, value);

	[DebuggerHidden]
	public static void SetPropertyValue(this Type @this, string name, object instance, object? value, bool nameIgnoreCase = false, params object?[]? index)
		=> @this.GetProperty(name, nameIgnoreCase ? INSTANCE_BINDING_FLAGS | IgnoreCase : INSTANCE_BINDING_FLAGS)?
			.SetPropertyValue(instance, value, index);

	[DebuggerHidden]
	public static void SetStaticPropertyValue(this Type @this, string name, object? value, bool nameIgnoreCase = false, params object?[]? index)
		=> @this.GetProperty(name, nameIgnoreCase ? STATIC_BINDING_FLAGS | IgnoreCase : STATIC_BINDING_FLAGS)?
			.SetPropertyValue(null, value, index);

	[DebuggerHidden]
	public static void SetStaticFieldValue(this Type @this, string name, object? value, bool nameIgnoreCase = false)
		=> @this.GetField(name, nameIgnoreCase ? STATIC_BINDING_FLAGS | IgnoreCase : STATIC_BINDING_FLAGS)?
			.SetFieldValue(null, value);

	[DebuggerHidden]
	public static Type? ToGenericType(this Type? @this)
		=> @this switch
		{
			{ IsGenericTypeDefinition: true } => @this,
			{ IsGenericType: true } => @this.GetGenericTypeDefinition(),
			_ => null
		};

	[DebuggerHidden]
	public static MethodInfo ToMethodInfo(this Delegate @this)
		=> @this.GetType().GetMethod("Invoke", INSTANCE_BINDING_FLAGS)!;
}
