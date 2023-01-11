// Copyright (c) 2021 Samuel Abraham

using System.Reflection;
using TypeCache.Extensions;
using TypeCache.Reflection;
using static System.Reflection.BindingFlags;

namespace TypeCache;

public static class TypeOf<T>
	where T : notnull
{
	public static Kind Kind { get; } = typeof(T).GetKind();

	public static string Name { get; } = typeof(T).Name();

	public static bool Nullable { get; } = typeof(T).IsNullable();

	public static ObjectType ObjectType { get; } = typeof(T).GetObjectType();

	public static SystemType SystemType { get; } = typeof(T).GetSystemType();

	/// <summary>
	/// <c>=&gt; <see langword="typeof"/>(<typeparamref name="T"/>).GetConstructors(<see cref="Public"/>);</c>
	/// </summary>
	public static ConstructorInfo[] Constructors => typeof(T).GetConstructors(Public);

	/// <summary>
	/// <c>=&gt; <see langword="typeof"/>(<typeparamref name="T"/>).GetEvents(<see cref="FlattenHierarchy"/> | <see cref="Instance"/> | <see cref="Public"/>);</c>
	/// </summary>
	public static EventInfo[] Events => typeof(T).GetEvents(FlattenHierarchy | Instance | Public);

	/// <summary>
	/// <c>=&gt; <see langword="typeof"/>(<typeparamref name="T"/>).GetFields(<see cref="FlattenHierarchy"/> | <see cref="Instance"/> | <see cref="Public"/>);</c>
	/// </summary>
	public static FieldInfo[] Fields => typeof(T).GetFields(FlattenHierarchy | Instance | Public);

	/// <summary>
	/// <c>=&gt; <see langword="typeof"/>(<typeparamref name="T"/>).GetMethods(<see cref="FlattenHierarchy"/> | <see cref="Instance"/> | <see cref="Public"/>);</c>
	/// </summary>
	public static MethodInfo[] Methods => typeof(T).GetMethods(FlattenHierarchy | Instance | Public);

	/// <summary>
	/// <c>=&gt; <see langword="typeof"/>(<typeparamref name="T"/>).GetProperties(<see cref="FlattenHierarchy"/> | <see cref="Instance"/> | <see cref="Public"/>);</c>
	/// </summary>
	public static PropertyInfo[] Properties => typeof(T).GetProperties(FlattenHierarchy | Instance | Public);

	/// <summary>
	/// <c>=&gt; <see langword="typeof"/>(<typeparamref name="T"/>).GetFields(<see cref="FlattenHierarchy"/> | <see cref="Public"/> | <see cref="Static"/>);</c>
	/// </summary>
	public static FieldInfo[] StaticFields => typeof(T).GetFields(FlattenHierarchy | Public | Static);

	/// <summary>
	/// <c>=&gt; <see langword="typeof"/>(<typeparamref name="T"/>).GetMethods(<see cref="FlattenHierarchy"/> | <see cref="Public"/> | <see cref="Static"/>);</c>
	/// </summary>
	public static MethodInfo[] StaticMethods => typeof(T).GetMethods(FlattenHierarchy | Public | Static);

	/// <summary>
	/// <c>=&gt; <see langword="typeof"/>(<typeparamref name="T"/>).GetProperties(<see cref="FlattenHierarchy"/> | <see cref="Public"/> | <see cref="Static"/>);</c>
	/// </summary>
	public static PropertyInfo[] StaticProperties => typeof(T).GetProperties(FlattenHierarchy | Public | Static);

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static T? Create(params object?[]? arguments)
		=> (T?)typeof(T).Create(arguments);

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static object? InvokeMethod(string name, T instance, params object?[]? arguments)
		=> typeof(T).InvokeMethod(name, instance, arguments);

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static object? InvokeMethod(string name, Type[] genericTypes, T instance, params object?[]? arguments)
		=> typeof(T).InvokeMethod(name, genericTypes, instance, arguments);

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static object? InvokeMethod<T1>(string name, T instance, params object?[]? arguments)
		=> typeof(T).InvokeMethod<T1>(name, instance, arguments);

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static object? InvokeMethod<T1, T2>(string name, T instance, params object?[]? arguments)
		=> typeof(T).InvokeMethod<T1, T2>(name, instance, arguments);

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static object? InvokeMethod<T1, T2, T3>(string name, T instance, params object?[]? arguments)
		=> typeof(T).InvokeMethod<T1, T2, T3>(name, instance, arguments);

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static object? InvokeMethod<T1, T2, T3, T4>(string name, T instance, params object?[]? arguments)
		=> typeof(T).InvokeMethod<T1, T2, T3, T4>(name, instance, arguments);

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static object? InvokeMethod<T1, T2, T3, T4, T5>(string name, T instance, params object?[]? arguments)
		=> typeof(T).InvokeMethod<T1, T2, T3, T4, T5>(name, instance, arguments);

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static object? InvokeMethod<T1, T2, T3, T4, T5, T6>(string name, T instance, params object?[]? arguments)
		=> typeof(T).InvokeMethod<T1, T2, T3, T4, T5, T6>(name, instance, arguments);

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static object? InvokeStaticMethod(string name, params object?[]? arguments)
		=> typeof(T).InvokeStaticMethod(name, arguments);

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static object? InvokeStaticMethod(string name, Type[] genericTypes, params object?[]? arguments)
		=> typeof(T).InvokeStaticMethod(name, genericTypes, arguments);

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static object? InvokeStaticMethod<T1>(string name, params object?[]? arguments)
		=> typeof(T).InvokeStaticMethod<T1>(name, arguments);

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static object? InvokeStaticMethod<T1, T2>(string name, params object?[]? arguments)
		=> typeof(T).InvokeStaticMethod<T1, T2>(name, arguments);

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static object? InvokeStaticMethod<T1, T2, T3>(string name, params object?[]? arguments)
		=> typeof(T).InvokeStaticMethod<T1, T2, T3>(name, arguments);

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static object? InvokeStaticMethod<T1, T2, T3, T4>(string name, params object?[]? arguments)
		=> typeof(T).InvokeStaticMethod<T1, T2, T3, T4>(name, arguments);

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static object? InvokeStaticMethod<T1, T2, T3, T4, T5>(string name, params object?[]? arguments)
		=> typeof(T).InvokeStaticMethod<T1, T2, T3, T4, T5>(name, arguments);

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static object? InvokeStaticMethod<T1, T2, T3, T4, T5, T6>(string name, params object?[]? arguments)
		=> typeof(T).InvokeStaticMethod<T1, T2, T3, T4, T5, T6>(name, arguments);

	//[MethodImpl(AggressiveInlining), DebuggerHidden]
	//public static D? GetConstructor<D>()
	//	where D : Delegate
	//	=> Member.GetConstructor<D>();

	//[MethodImpl(AggressiveInlining), DebuggerHidden]
	//public static D? GetMethod<D>(string name, bool isStatic = false)
	//	where D : Delegate
	//	=> Member.GetMethod<D>(name, isStatic);
}
