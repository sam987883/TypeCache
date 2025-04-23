// Copyright (c) 2021 Samuel Abraham

using System.Collections;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using TypeCache.Extensions;
using TypeCache.Utilities;
using static System.Reflection.BindingFlags;

namespace TypeCache.Extensions;

public partial class ReflectionExtensions
{
	/// <exception cref="MissingMethodException"></exception>
	public static object? Create(this Type @this, object?[]? arguments = null)
		=> @this.FindConstructor(arguments) switch
		{
			null when @this.IsValueType && (arguments is null || arguments.Length is 0) => TypeStore.DefaultValueTypeConstructorFuncs[@this.TypeHandle].Invoke(),
			null => throw new MissingMethodException(@this.Name, "Constructor"),
			var constructorInfo => constructorInfo.InvokeFunc(arguments)
		};

	/// <exception cref="MissingMethodException"></exception>
	public static object? Create(this Type @this, ITuple? arguments)
		=> @this.FindConstructor(arguments) switch
		{
			null when @this.IsValueType && (arguments is null || arguments.Length is 0) => TypeStore.DefaultValueTypeConstructorFuncs[@this.TypeHandle].Invoke(),
			null => throw new MissingMethodException(@this.Name, "Constructor"),
			var constructorInfo => constructorInfo.InvokeFunc(arguments)
		};

	public static ConstructorInfo? FindConstructor(this Type @this, object?[]? arguments)
		=> @this.GetConstructors(INSTANCE_BINDING_FLAGS)
			.FirstOrDefault(constructor => constructor.IsCallableWith(arguments));

	public static ConstructorInfo? FindConstructor(this Type @this, ITuple? arguments)
		=> @this.GetConstructors(INSTANCE_BINDING_FLAGS)
			.FirstOrDefault(constructor => constructor.IsCallableWith(arguments));

	public static MethodInfo? FindMethod(this Type @this, string name, Type[] argumentTypes)
		=> @this.GetMethodInfos(name).FirstOrDefault(_ => _.IsCallableWith(argumentTypes));

	public static MethodInfo? FindMethod(this Type @this, string name, object?[]? arguments)
		=> @this.GetMethodInfos(name).FirstOrDefault(_ => _.IsCallableWith(arguments));

	public static MethodInfo? FindMethod(this Type @this, string name, ITuple? arguments)
		=> @this.GetMethodInfos(name).FirstOrDefault(_ => _.IsCallableWith(arguments));

	/// <exception cref="ArgumentNullException"/>
	/// <exception cref="ArgumentException"/>
	/// <exception cref="InvalidOperationException"/>
	/// <exception cref="NotSupportedException"/>
	public static MethodInfo? FindMethod(this Type @this, string name, Type[] genericTypes, object?[]? arguments)
	{
		var methodInfos = @this.GetMethodInfos(name)
			.Where(_ => _.IsGenericMethod && _.GetGenericArguments().Length == genericTypes.Length)
			.Select(_ => _.MakeGenericMethod(genericTypes))
			.ToArray();
		if (methodInfos.Length is 0)
			return null;

		return methodInfos.FirstOrDefault(_ => _.IsCallableWith(arguments));
	}

	/// <exception cref="ArgumentNullException"/>
	/// <exception cref="ArgumentException"/>
	/// <exception cref="InvalidOperationException"/>
	/// <exception cref="NotSupportedException"/>
	public static MethodInfo? FindMethod(this Type @this, string name, Type[] genericTypes, ITuple? arguments)
	{
		var methodInfos = @this.GetMethodInfos(name)
			.Where(_ => _.IsGenericMethod && _.GetGenericArguments().Length == genericTypes.Length)
			.Select(_ => _.MakeGenericMethod(genericTypes))
			.ToArray();
		if (methodInfos.Length is 0)
			return null;

		return methodInfos.FirstOrDefault(_ => _.IsCallableWith(arguments));
	}

	/// <exception cref="ArgumentNullException"/>
	/// <exception cref="ArgumentException"/>
	/// <exception cref="InvalidOperationException"/>
	/// <exception cref="NotSupportedException"/>
	public static MethodInfo? FindStaticMethod(this Type @this, string name, Type[] argumentTypes)
		=> @this.GetStaticMethodInfos(name).FirstOrDefault(_ => _.IsCallableWith(argumentTypes));

	/// <exception cref="ArgumentNullException"/>
	/// <exception cref="ArgumentException"/>
	/// <exception cref="InvalidOperationException"/>
	/// <exception cref="NotSupportedException"/>
	public static MethodInfo? FindStaticMethod(this Type @this, string name, object?[]? arguments)
		=> @this.GetStaticMethodInfos(name).FirstOrDefault(_ => _.IsCallableWith(arguments));

	/// <exception cref="ArgumentNullException"/>
	/// <exception cref="ArgumentException"/>
	/// <exception cref="InvalidOperationException"/>
	/// <exception cref="NotSupportedException"/>
	public static MethodInfo? FindStaticMethod(this Type @this, string name, ITuple? arguments)
		=> @this.GetStaticMethodInfos(name).FirstOrDefault(_ => _.IsCallableWith(arguments));

	/// <exception cref="ArgumentNullException"/>
	/// <exception cref="ArgumentException"/>
	/// <exception cref="InvalidOperationException"/>
	/// <exception cref="NotSupportedException"/>
	public static MethodInfo? FindStaticMethod(this Type @this, string name, Type[] genericTypes, object?[]? arguments)
	{
		var methodInfos = @this.GetStaticMethodInfos(name)
			.Where(_ => _.IsGenericMethod && _.GetGenericArguments().Length == genericTypes.Length)
			.Select(_ => _.MakeGenericMethod(genericTypes))
			.ToArray();
		if (methodInfos.Length is 0)
			return null;

		return methodInfos.FirstOrDefault(_ => _.IsCallableWith(arguments));
	}

	/// <exception cref="ArgumentNullException"/>
	/// <exception cref="ArgumentException"/>
	/// <exception cref="InvalidOperationException"/>
	/// <exception cref="NotSupportedException"/>
	public static MethodInfo? FindStaticMethod(this Type @this, string name, Type[] genericTypes, ITuple? arguments)
	{
		var methodInfos = @this.GetStaticMethodInfos(name)
			.Where(_ => _.IsGenericMethod && _.GetGenericArguments().Length == genericTypes.Length)
			.Select(_ => _.MakeGenericMethod(genericTypes))
			.ToArray();
		if (methodInfos.Length is 0)
			return null;

		return methodInfos.FirstOrDefault(_ => _.IsCallableWith(arguments));
	}

	public static CollectionType GetCollectionType(this Type @this)
		=> TypeStore.CollectionTypes[@this.IsGenericType ? @this.GetGenericTypeDefinition().TypeHandle : @this.TypeHandle];

	/// <inheritdoc cref="Type.GetField(string, BindingFlags)"/>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static object? GetFieldValue(this Type @this, string name, object instance)
		=> @this.GetFieldInfo(name)?.GetValueEx(instance);

	/// <inheritdoc cref="Type.GetField(string, BindingFlags)"/>
	/// <remarks>
	/// <b>Does a case-insensitive search for the field by <paramref name="name"/>.<br/>
	/// If multiple fields are found with the same name, then a case-sensitive search is performed instead.</b>
	/// </remarks>
	/// <exception cref="ArgumentNullException"/>
	public static FieldInfo? GetFieldInfo(this Type @this, string name)
	{
		name.ThrowIfBlank();

		return @this.GetFields(INSTANCE_BINDING_FLAGS).GetMemberInfo(name);
	}

	/// <inheritdoc cref="Type.GetMethod(string, BindingFlags)"/>
	/// <remarks>
	/// <b>Does a case-insensitive search for methods by <paramref name="name"/>.<br/>
	/// If multiple methods are found with the same name but different casings,<br/>
	/// then a case-sensitive search is performed instead.</b>
	/// </remarks>
	/// <exception cref="ArgumentNullException"/>
	public static MethodInfo[] GetMethodInfos(this Type @this, string name)
	{
		name.ThrowIfBlank();

		return @this.GetMethods(INSTANCE_BINDING_FLAGS).GetMethodInfos(name);
	}

	public static ObjectType GetObjectType(this Type @this)
		=> TypeStore.ObjectTypes[@this.IsGenericType ? @this.GetGenericTypeDefinition().TypeHandle : @this.TypeHandle];

	/// <inheritdoc cref="Type.GetProperty(string, BindingFlags)"/>
	/// <remarks>
	/// <b>Does a case-insensitive search for the property by <paramref name="name"/>.<br/>
	/// If multiple properties are found with the same name, then a case-sensitive search is performed instead.</b>
	/// </remarks>
	/// <exception cref="ArgumentNullException"/>
	public static PropertyInfo? GetPropertyInfo(this Type @this, string name)
	{
		name.ThrowIfBlank();

		return @this.GetProperties(INSTANCE_BINDING_FLAGS).GetMemberInfo(name);
	}

	/// <inheritdoc cref="Type.GetFields(BindingFlags)"/>
	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.GetFields(<see cref="FlattenHierarchy"/> | <see cref="Public"/> | <see cref="Instance"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static FieldInfo[] GetPublicFields(this Type @this)
		=> @this.GetFields(PUBLIC_INSTANCE_BINDING_FLAGS);

	/// <inheritdoc cref="Type.GetMethods(BindingFlags)"/>
	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.GetMethods(<see cref="FlattenHierarchy"/> | <see cref="Public"/> | <see cref="Instance"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static MethodInfo[] GetPublicMethods(this Type @this)
		=> @this.GetMethods(PUBLIC_INSTANCE_BINDING_FLAGS);

	/// <inheritdoc cref="Type.GetProperties(BindingFlags)"/>
	public static PropertyInfo[] GetPublicProperties(this Type @this)
		=> @this.GetProperties(PUBLIC_INSTANCE_BINDING_FLAGS)
			.Where(propertyInfo => propertyInfo.GetMethod?.IsStatic is not true && propertyInfo.SetMethod?.IsStatic is not true)
			.ToArray();

	/// <inheritdoc cref="Type.GetFields(BindingFlags)"/>
	/// <remarks>
	/// <c><see cref="FlattenHierarchy"/> | <see cref="Public"/> | <see cref="Static"/></c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static FieldInfo[] GetPublicStaticFields(this Type @this)
		=> @this.GetFields(PUBLIC_STATIC_BINDING_FLAGS);

	/// <inheritdoc cref="Type.GetMethods(BindingFlags)"/>
	/// <remarks>
	/// <c><see cref="FlattenHierarchy"/> | <see cref="Public"/> | <see cref="Static"/></c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static MethodInfo[] GetPublicStaticMethods(this Type @this)
		=> @this.GetMethods(PUBLIC_STATIC_BINDING_FLAGS);

	/// <inheritdoc cref="Type.GetProperties(BindingFlags)"/>
	/// <remarks>
	/// <c><see cref="FlattenHierarchy"/> | <see cref="Public"/> | <see cref="Static"/></c>
	/// </remarks>
	public static PropertyInfo[] GetPublicStaticProperties(this Type @this)
		=> @this.GetProperties(PUBLIC_STATIC_BINDING_FLAGS)
			.Where(propertyInfo => propertyInfo.GetMethod?.IsStatic is true || propertyInfo.SetMethod?.IsStatic is true)
			.ToArray();

	public static object? GetPropertyValue(this Type @this, string name, object instance, ITuple? index = null)
		=> @this.GetPropertyInfo(name)?.GetValueEx(instance, index);

	public static ScalarType GetScalarType(this Type @this)
		=> @this switch
		{
			{ IsGenericTypeDefinition: true } => ScalarType.None,
			{ IsEnum: true } => ScalarType.Enum,
			{ IsArray: true } or { IsPointer: true } => @this.GetElementType()!.GetScalarType(),
			{ IsGenericType: true } when @this.Is(typeof(Nullable<>)) => @this.GenericTypeArguments[0].GetScalarType(),
			_ when TypeStore.ScalarTypes.TryGetValue(@this.TypeHandle, out var scalarType) => scalarType,
			_ => ScalarType.None
		};

	/// <inheritdoc cref="Type.GetField(string, BindingFlags)"/>
	/// <remarks>
	/// <b>Does a case-insensitive search for the static field by <paramref name="name"/>.<br/>
	/// If multiple static fields are found with the same name, then a case-sensitive search is performed instead.</b>
	/// </remarks>
	/// <exception cref="ArgumentNullException"/>
	public static FieldInfo? GetStaticFieldInfo(this Type @this, string name)
	{
		name.ThrowIfBlank();

		return @this.GetFields(STATIC_BINDING_FLAGS).GetMemberInfo(name);
	}

	public static object? GetStaticFieldValue(this Type @this, string name)
		=> @this.GetStaticFieldInfo(name)?.GetStaticValue();

	/// <inheritdoc cref="Type.GetMethod(string, BindingFlags)"/>
	/// <remarks>
	/// <b>Does a case-insensitive search for methods by <paramref name="name"/>.<br/>
	/// If multiple methods are found with the same name but different casings,<br/>
	/// then a case-sensitive search is performed instead.</b>
	/// </remarks>
	/// <exception cref="ArgumentNullException"/>
	public static MethodInfo[] GetStaticMethodInfos(this Type @this, string name)
	{
		name.ThrowIfBlank();

		return @this.GetMethods(STATIC_BINDING_FLAGS).GetMethodInfos(name);
	}

	/// <inheritdoc cref="Type.GetProperty(string, BindingFlags)"/>
	/// <remarks>
	/// <b>Does a case-insensitive search for the static property by <paramref name="name"/>.<br/>
	/// If multiple static properties are found with the same name, then a case-sensitive search is performed instead.</b>
	/// </remarks>
	/// <exception cref="ArgumentNullException"/>
	public static PropertyInfo? GetStaticPropertyInfo(this Type @this, string name)
	{
		name.ThrowIfBlank();

		return @this.GetProperties(STATIC_BINDING_FLAGS).GetMemberInfo(name);
	}

	public static object? GetStaticPropertyValue(this Type @this, string name, ITuple? index = null)
		=> @this.GetStaticPropertyInfo(name)?.GetStaticValue(index);

	/// <summary>
	/// Gets the C# name of a type, including generic type definitions. For example:
	/// <list type="bullet">
	/// <item><c>UInt64</c></item>
	/// <item><c>String</c></item>
	/// <item><c>Char*</c></item>
	/// <item><c>Int32[]</c></item>
	/// <item><c>Int32[,,]</c></item>
	/// <item><c>IList&lt;Boolean&gt;</c></item>
	/// <item><c>IDictionary&lt;String, List&lt;Int32&gt;&gt;</c></item>
	/// <item><c>IDictionary&lt;,&gt;</c></item>
	/// </list>
	/// </summary>
	public static string GetTypeName(this Type type)
		=> type switch
		{
			_ when type == typeof(string) => type.Name,
			{ IsPointer: true } => Invariant($"{type.GetElementType()!.GetTypeName()}*"),
			{ IsArray: true } => Invariant($"{type.GetElementType()!.GetTypeName()}[{string.Concat(','.Repeat(type.GetArrayRank() - 1))}]"),
			{ IsByRef: true } => Invariant($"{type.GetElementType()!.GetTypeName()}&"),
			{ IsGenericTypeDefinition: true } => Invariant($"{type.Name[0..type.Name.IndexOf(GENERIC_TICKMARK)]}<{string.Concat(','.Repeat(type.GetGenericArguments().Length - 1))}>"),
			{ IsGenericType: true } => Invariant($"{type.Name[0..type.Name.IndexOf(GENERIC_TICKMARK)]}<{string.Join(", ", type.GetGenericArguments().Select(_ => _.GetTypeName()))}>"),
			_ => type.Name
		};

	/// <inheritdoc cref="Type.IsAssignableTo(Type)"/>
	/// <remarks>
	/// Supports generic type definitions as well.  For example:
	/// <list type="table">
	/// <item><c>List&lt;Int32&gt; : IList&lt;Int32&gt; // <see langword="true"/></c></item>
	/// <item><c>List&lt;String&gt; : IList&lt;&gt; // <see langword="true"/></c></item>
	/// <item><c>List&lt;&gt; : IList&lt;Int32&gt; // <see langword="false"/></c></item>
	/// </list>
	/// </remarks>
	public static bool Implements(this Type @this, Type type)
	{
		return type switch
		{
			_ when @this.Is(type) => true,
			{ IsArray: true } or { IsPointer: true } or { IsSealed: true } or { IsValueType: true } => false,
			{ IsGenericTypeDefinition: false } => @this.IsAssignableTo(type),
			{ IsInterface: true } => type.IsAny(@this.GetInterfaces().Where(_ => _.IsGenericType).ToArray()),
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

	/// <exception cref="ArgumentNullException"/>
	/// <exception cref="ArgumentOutOfRangeException"/>
	public static void InvokeMethodAction(this Type @this, string name, object instance, object?[]? arguments)
	{
		var methodInfo = @this.FindMethod(name, arguments);
		methodInfo.ThrowIfNull();

		methodInfo.InvokeAction(instance, arguments);
	}

	/// <exception cref="ArgumentNullException"/>
	/// <exception cref="ArgumentOutOfRangeException"/>
	public static void InvokeMethodAction(this Type @this, string name, object instance, ITuple? arguments)
	{
		var methodInfo = @this.FindMethod(name, arguments);
		methodInfo.ThrowIfNull();

		methodInfo.InvokeAction(instance, arguments);
	}

	/// <exception cref="ArgumentNullException"/>
	/// <exception cref="ArgumentOutOfRangeException"/>
	public static object? InvokeMethod(this Type @this, string name, object instance, object?[]? arguments)
	{
		var methodInfo = @this.FindMethod(name, arguments);
		methodInfo.ThrowIfNull();
		methodInfo.DeclaringType.ThrowIfNull();

		var func = TypeStore.MethodArrayFuncs[(methodInfo.DeclaringType.TypeHandle, methodInfo.MethodHandle)];
		if (func is not null)
			return func(instance, arguments);

		var action = TypeStore.MethodArrayActions[(methodInfo.DeclaringType.TypeHandle, methodInfo.MethodHandle)];
		action.ThrowIfNull();

		action(instance, arguments);
		return null;
	}

	/// <exception cref="ArgumentNullException"/>
	/// <exception cref="ArgumentOutOfRangeException"/>
	/// <exception cref="InvalidOperationException"/>
	/// <exception cref="NotSupportedException"/>
	public static void InvokeMethodAction(this Type @this, string name, Type[] genericTypes, object instance, object?[]? arguments)
		=> @this.FindMethod(name, genericTypes, arguments)?
			.InvokeAction(instance, arguments);

	/// <exception cref="ArgumentNullException"/>
	/// <exception cref="ArgumentOutOfRangeException"/>
	/// <exception cref="InvalidOperationException"/>
	/// <exception cref="NotSupportedException"/>
	public static void InvokeMethodAction(this Type @this, string name, Type[] genericTypes, object instance, ITuple? arguments)
		=> @this.FindMethod(name, genericTypes, arguments)?
			.InvokeAction(instance, arguments);

	/// <exception cref="ArgumentNullException"/>
	/// <exception cref="ArgumentOutOfRangeException"/>
	/// <exception cref="InvalidOperationException"/>
	/// <exception cref="NotSupportedException"/>
	public static object? InvokeMethodFunc(this Type @this, string name, Type[] genericTypes, object instance, object?[]? arguments)
		=> @this.FindMethod(name, genericTypes, arguments)?
			.InvokeFunc(instance, arguments);

	/// <exception cref="ArgumentNullException"/>
	/// <exception cref="ArgumentOutOfRangeException"/>
	/// <exception cref="InvalidOperationException"/>
	/// <exception cref="NotSupportedException"/>
	public static object? InvokeMethodFunc(this Type @this, string name, Type[] genericTypes, object instance, ITuple? arguments)
		=> @this.FindMethod(name, genericTypes, arguments)?
			.InvokeFunc(instance, arguments);

	/// <exception cref="ArgumentNullException"/>
	/// <exception cref="ArgumentOutOfRangeException"/>
	/// <exception cref="InvalidOperationException"/>
	/// <exception cref="NotSupportedException"/>
	public static void InvokeStaticMethodAction(this Type @this, string name, Type[] genericTypes, object?[]? arguments)
		=> @this.FindStaticMethod(name, genericTypes, arguments)?
			.InvokeStaticAction(arguments);

	/// <exception cref="ArgumentNullException"/>
	/// <exception cref="ArgumentOutOfRangeException"/>
	/// <exception cref="InvalidOperationException"/>
	/// <exception cref="NotSupportedException"/>
	public static void InvokeStaticMethodAction(this Type @this, string name, Type[] genericTypes, ITuple? arguments)
		=> @this.FindStaticMethod(name, genericTypes, arguments)?
			.InvokeStaticAction(arguments);

	/// <exception cref="ArgumentNullException"/>
	/// <exception cref="ArgumentOutOfRangeException"/>
	/// <exception cref="InvalidOperationException"/>
	/// <exception cref="NotSupportedException"/>
	public static object? InvokeStaticMethodFunc(this Type @this, string name, Type[] genericTypes, object?[]? arguments)
		=> @this.FindStaticMethod(name, genericTypes, arguments)?
			.InvokeStaticFunc(arguments);

	/// <exception cref="ArgumentNullException"/>
	/// <exception cref="ArgumentOutOfRangeException"/>
	/// <exception cref="InvalidOperationException"/>
	/// <exception cref="NotSupportedException"/>
	public static object? InvokeStaticMethodFunc(this Type @this, string name, Type[] genericTypes, ITuple? arguments)
		=> @this.FindStaticMethod(name, genericTypes, arguments)?
			.InvokeStaticFunc(arguments);

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/> == <see langword="typeof"/>(<typeparamref name="T"/>);</c>
	/// </summary>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool Is<T>(this Type @this)
		=> @this == typeof(T);

	public static bool Is(this Type @this, Type type)
		=> (@this, type) switch
		{
			({ IsGenericType: true, IsGenericTypeDefinition: false }, { IsGenericTypeDefinition: true })
				=> @this.GetGenericTypeDefinition() == type,
			({ IsGenericTypeDefinition: true }, { IsGenericType: true, IsGenericTypeDefinition: false })
				=> @this == type.GetGenericTypeDefinition(),
			_ => @this == type
		};

	/// <summary>
	/// <c>=&gt; <paramref name="types"/>.Any(@<paramref name="this"/>.Is);</c>
	/// </summary>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool IsAny(this Type @this, Type[] types)
		=> types.Any(@this.Is);

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.Is([<see langword="typeof"/>(<typeparamref name="T1"/>)]);</c>
	/// </summary>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool IsAny<T1>(this Type @this)
		=> @this.Is(typeof(T1));

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.IsAny([<see langword="typeof"/>(<typeparamref name="T1"/>),
	/// <see langword="typeof"/>(<typeparamref name="T2"/>)]);</c>
	/// </summary>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool IsAny<T1, T2>(this Type @this)
		=> @this.IsAny([typeof(T1), typeof(T2)]);

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.IsAny([<see langword="typeof"/>(<typeparamref name="T1"/>),
	/// <see langword="typeof"/>(<typeparamref name="T2"/>),
	/// <see langword="typeof"/>(<typeparamref name="T3"/>)]);</c>
	/// </summary>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool IsAny<T1, T2, T3>(this Type @this)
		=> @this.IsAny([typeof(T1), typeof(T2), typeof(T3)]);

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.IsAny([<see langword="typeof"/>(<typeparamref name="T1"/>),
	/// <see langword="typeof"/>(<typeparamref name="T2"/>),
	/// <see langword="typeof"/>(<typeparamref name="T3"/>),
	/// <see langword="typeof"/>(<typeparamref name="T4"/>)]);</c>
	/// </summary>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool IsAny<T1, T2, T3, T4>(this Type @this)
		=> @this.IsAny([typeof(T1), typeof(T2), typeof(T3), typeof(T4)]);

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.IsAny([<see langword="typeof"/>(<typeparamref name="T1"/>),
	/// <see langword="typeof"/>(<typeparamref name="T2"/>),
	/// <see langword="typeof"/>(<typeparamref name="T3"/>),
	/// <see langword="typeof"/>(<typeparamref name="T4"/>),
	/// <see langword="typeof"/>(<typeparamref name="T5"/>)]);</c>
	/// </summary>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool IsAny<T1, T2, T3, T4, T5>(this Type @this)
		=> @this.IsAny([typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5)]);

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.IsAny([<see langword="typeof"/>(<typeparamref name="T1"/>),
	/// <see langword="typeof"/>(<typeparamref name="T2"/>),
	/// <see langword="typeof"/>(<typeparamref name="T3"/>),
	/// <see langword="typeof"/>(<typeparamref name="T4"/>),
	/// <see langword="typeof"/>(<typeparamref name="T5"/>),
	/// <see langword="typeof"/>(<typeparamref name="T6"/>)]);</c>
	/// </summary>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool IsAny<T1, T2, T3, T4, T5, T6>(this Type @this)
		=> @this.IsAny([typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6)]);

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

	/// <summary>
	/// Whether the type implements <c><see cref="IConvertible"/></c>.<br/>
	/// If the type is <c><see cref="Nullable{T}"/></c>, then whether the type parameter <c>T</c> implements <c><see cref="IConvertible"/></c>.
	/// </summary>
	[DebuggerHidden]
	public static bool IsConvertible(this Type @this)
		=> @this.IsAssignableTo<IConvertible>()
			|| (@this.Is(typeof(Nullable<>)) && @this.GenericTypeArguments.First().IsAssignableTo<IConvertible>());

	/// <exception cref="ArgumentNullException"/>
	public static bool IsConvertibleTo(this Type @this, Type targetType)
	{
		@this.ThrowIfNull();
		targetType.ThrowIfNull();

		return @this.GetScalarType().IsConvertibleTo(targetType.GetScalarType());
	}

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.IsAssignableTo&lt;<see cref="IEnumerable"/>&gt;();</c>
	/// </summary>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool IsEnumerable(this Type @this)
		=> @this.IsAssignableTo<IEnumerable>();

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
		=> @this.GetScalarType().IsEnumUnderlyingType();

	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.IsPointer &amp;&amp; !@<paramref name="this"/>.IsByRef &amp;&amp; !@<paramref name="this"/>.IsByRefLike;</c>
	/// </remarks>
	[DebuggerHidden]
	internal static bool IsInvokable(this Type @this)
		=> !@this.IsPointer && !@this.IsByRef && !@this.IsByRefLike;

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.IsClass || @<paramref name="this"/>.IsPointer || @<paramref name="this"/>.Is(<see langword="typeof"/>(Nullable&lt;&gt;));</c>
	/// </summary>
	[DebuggerHidden]
	public static bool IsNullable(this Type @this)
		=> @this.IsClass || @this.IsPointer || @this.Is(typeof(Nullable<>));

	public static void SetFieldValue(this Type @this, string name, object instance, object? value)
		=> @this.GetFieldInfo(name)?.SetValueEx(instance, value);

	public static void SetPropertyValue<T>(this Type @this, string name, object instance, T? value)
		=> @this.GetPropertyInfo(name)?.SetValueEx(instance, value);

	public static void SetPropertyValue(this Type @this, string name, object instance, ITuple valueAndIndex)
		=> @this.GetPropertyInfo(name)?.SetValueEx(instance, valueAndIndex);

	public static void SetStaticFieldValue(this Type @this, string name, object? value)
		=> @this.GetStaticFieldInfo(name)?.SetStaticValue(value);

	public static void SetStaticPropertyValue<T>(this Type @this, string name, T? value)
		=> @this.GetStaticPropertyInfo(name)?.SetStaticValue(value);

	public static void SetStaticPropertyValue(this Type @this, string name, ITuple valueAndIndex)
		=> @this.GetStaticPropertyInfo(name)?.SetStaticValue(valueAndIndex);

	/// <inheritdoc cref="Expression.Default(Type)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Default(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static DefaultExpression ToDefaultExpression(this Type @this)
		=> Expression.Default(@this);

	/// <inheritdoc cref="Expression.Label(Type)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Label(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static LabelTarget ToLabelTarget(this Type @this)
		=> Expression.Label(@this);

	/// <inheritdoc cref="Expression.Label(Type, string)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Label(@<paramref name="this"/>, <paramref name="name"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static LabelTarget ToLabelTarget(this Type @this, string? name)
		=> Expression.Label(@this, name);

	/// <inheritdoc cref="Expression.New(ConstructorInfo, Expression[])"/>
	public static NewExpression ToNewExpression(this Type @this, Expression[]? parameters = null)
		=> parameters switch
		{
			null or { Length: 0 } => Expression.New(@this),
			_ => @this.GetConstructor(parameters.Select(parameter => parameter.Type).ToArray())!.ToExpression(parameters)
		};

	/// <inheritdoc cref="Expression.Field(Expression, Type, string)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Field(<see langword="null"/>, @<paramref name="this"/>, <paramref name="name"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static MemberExpression ToStaticFieldExpression(this Type @this, string name)
		=> Expression.Field(null, @this, name);

	/// <inheritdoc cref="Expression.Call(Expression, string, Type[], Expression[])"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Call(@<paramref name="this"/>, <paramref name="method"/>, <see cref="Type.EmptyTypes"/>, <paramref name="arguments"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static MethodCallExpression ToStaticMethodCallExpression(this Type @this, string method, Expression[]? arguments = null)
		=> Expression.Call(@this, method, Type.EmptyTypes, arguments);

	/// <inheritdoc cref="Expression.Call(Expression, string, Type[], Expression[])"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Call(@<paramref name="this"/>, <paramref name="method"/>, <paramref name="genericTypes"/>, <paramref name="arguments"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static MethodCallExpression ToStaticMethodCallExpression(this Type @this, string method, Type[]? genericTypes, Expression[]? arguments = null)
		=> Expression.Call(@this, method, genericTypes, arguments);

	/// <inheritdoc cref="Expression.Property(Expression, Type, string)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Property(<see langword="null"/>, @<paramref name="this"/>, <paramref name="name"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static MemberExpression ToStaticPropertyExpression(this Type @this, string name)
		=> Expression.Property(null, @this, name);
}
