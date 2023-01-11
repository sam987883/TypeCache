// Copyright (c) 2021 Samuel Abraham

using System.Collections.Concurrent;
using System.Reflection;
using TypeCache.Attributes;
using TypeCache.Collections;
using TypeCache.Reflection;

namespace TypeCache.Extensions;

public static class MemberInfoExtensions
{
	private const char GENERIC_TICKMARK = '`';

	/// <inheritdoc cref="FieldInfo.GetValue(object?)"/>
	/// <remarks>
	/// <b>The code to get the field value is built once and used subsequently.<br/>
	/// This is much faster than late binding.<br/>
	/// In the case of a constant, <c><see cref="FieldInfo.GetRawConstantValue"/></c> is used instead.</b>
	/// </remarks>
	/// <param name="instance">Pass <c><see langword="null"/></c> if this is a static field.</param>
	public static object? GetFieldValue(this FieldInfo @this, object? instance)
		=> @this.IsLiteral
			? @this.GetRawConstantValue()
			: TypeStore.FieldGetInvokes.GetOrAdd(@this.FieldHandle, handle => @this.FieldGetInvoke().Compile())(instance);

	/// <param name="instance">Pass <c><see langword="null"/></c> if this is a static property getter.</param>
	/// <param name="index">Property <b>indexer</b> arguments; otherwise <c><see langword="null"/></c> if not an indexer.</param>
	public static object? GetPropertyValue(this PropertyInfo @this, object? instance, params object?[]? index)
		=> @this.CanRead ? @this.GetMethod?.InvokeMethod(instance switch
		{
			null when index?.Any() is true => index,
			null => Array<object>.Empty,
			_ when index?.Any() is true => new[] { instance }.Concat(index).ToArray(),
			_ => new[] { instance }
		}) : null;

	/// <remarks>
	/// <c>@<paramref name="this"/>.GetCustomAttribute&lt;<typeparamref name="T"/>&gt;(<paramref name="inherit"/>) <see langword="is not null"/>;</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool HasCustomAttribute<T>(this MemberInfo @this, bool inherit = true)
		where T : Attribute
		=> @this.GetCustomAttribute<T>(inherit) is not null;

	/// <remarks>
	/// <c>@<paramref name="this"/>.GetCustomAttribute&lt;<typeparamref name="T"/>&gt;(<paramref name="inherit"/>) <see langword="is not null"/>;</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool HasCustomAttribute<T>(this ParameterInfo @this, bool inherit = true)
		where T : Attribute
		=> @this.GetCustomAttribute<T>(inherit) is not null;

	/// <remarks>
	/// <c>@<paramref name="this"/>.GetCustomAttribute(<paramref name="attributeType"/>, <paramref name="inherit"/>) <see langword="is not null"/>;</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool HasCustomAttribute(this MemberInfo @this, Type attributeType, bool inherit = true)
		=> @this.GetCustomAttribute(attributeType, inherit) is not null;

	/// <remarks>
	/// <c>@<paramref name="this"/>.GetCustomAttribute(<paramref name="attributeType"/>, <paramref name="inherit"/>) <see langword="is not null"/>;</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool HasCustomAttribute(this ParameterInfo @this, Type attributeType, bool inherit = true)
		=> @this.GetCustomAttribute(attributeType, inherit) is not null;

	/// <remarks>
	/// <c>@<paramref name="this"/>.ReturnType.IsAny(<see langword="typeof"/>(Task), <see langword="typeof"/>(ValueTask), <see langword="typeof"/>(void));</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool HasNoReturnValue(this MethodInfo @this)
		=> @this.ReturnType.IsAny(typeof(Task), typeof(ValueTask), typeof(void));

	/// <param name="instance">Pass <c><see langword="null"/></c> if this is a static method.</param>
	/// <param name="arguments">The method arguments.</param>
	public static object? InvokeMethod(this MethodBase @this, params object?[]? arguments)
	{
		@this.DeclaringType.AssertNotNull();
		var constructor = TypeStore.MethodInvokes[(@this.DeclaringType.TypeHandle, @this.MethodHandle)];
		return constructor.Invoke(arguments);
	}

	public static bool IsCallableWith(this ParameterInfo[] @this, params object?[]? arguments)
	{
		if (@this.Any(_ => _.IsOut))
			return false;

		if (arguments?.Any() is not true)
			return !@this.Any() || @this.All(parameterInfo => parameterInfo!.HasDefaultValue || parameterInfo.IsOptional);

		var argumentEnumerator = arguments.GetEnumerator();
		return @this
			.All(parameterInfo => argumentEnumerator.IfNext(out var argument) switch
			{
				true when argument is not null => argument.GetType().IsAssignableTo(parameterInfo.ParameterType),
				true when argument is null => parameterInfo.ParameterType.IsNullable(),
				_ => parameterInfo.HasDefaultValue || parameterInfo.IsOptional
			}) && !argumentEnumerator.MoveNext();
	}

	[DebuggerHidden]
	private static bool IsInvokable(this ParameterInfo[] @this)
		=> @this.All(parameterInfo => !parameterInfo.IsOut && parameterInfo.ParameterType.IsInvokable());

	/// <remarks>
	/// <c>=&gt; ((<see cref="MethodBase"/>)@<paramref name="this"/>).IsInvokable();</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	internal static bool IsInvokable(this ConstructorInfo @this)
		=> @this.GetParameters().IsInvokable();

	/// <remarks>
	/// <c>=&gt; ((<see cref="MethodBase"/>)@<paramref name="this"/>).IsInvokable() &amp;&amp; @<paramref name="this"/>.ReturnType.IsInvokable();</c>
	/// </remarks>
	[DebuggerHidden]
	internal static bool IsInvokable(this MethodInfo @this)
		=> @this.GetParameters().IsInvokable() && @this.ReturnType.IsInvokable();

	/// <inheritdoc cref="MethodInfo.MakeGenericMethod(Type[])"/>
	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.MakeGenericMethod(<see langword="typeof"/>(<typeparamref name="T1"/>));</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static MethodInfo MakeGenericMethod<T1>(this MethodInfo @this)
		=> @this.MakeGenericMethod(typeof(T1));

	/// <inheritdoc cref="MethodInfo.MakeGenericMethod(Type[])"/>
	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.MakeGenericMethod(<see langword="typeof"/>(<typeparamref name="T1"/>),
	/// <see langword="typeof"/>(<typeparamref name="T2"/>));</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static MethodInfo MakeGenericMethod<T1, T2>(this MethodInfo @this)
		=> @this.MakeGenericMethod(typeof(T1), typeof(T2));

	/// <inheritdoc cref="MethodInfo.MakeGenericMethod(Type[])"/>
	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.MakeGenericMethod(<see langword="typeof"/>(<typeparamref name="T1"/>),
	/// <see langword="typeof"/>(<typeparamref name="T2"/>),
	/// <see langword="typeof"/>(<typeparamref name="T3"/>));</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static MethodInfo MakeGenericMethod<T1, T2, T3>(this MethodInfo @this)
		=> @this.MakeGenericMethod(typeof(T1), typeof(T2), typeof(T3));

	/// <inheritdoc cref="MethodInfo.MakeGenericMethod(Type[])"/>
	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.MakeGenericMethod(<see langword="typeof"/>(<typeparamref name="T1"/>),
	/// <see langword="typeof"/>(<typeparamref name="T2"/>),
	/// <see langword="typeof"/>(<typeparamref name="T3"/>),
	/// <see langword="typeof"/>(<typeparamref name="T4"/>));</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static MethodInfo MakeGenericMethod<T1, T2, T3, T4>(this MethodInfo @this)
		=> @this.MakeGenericMethod(typeof(T1), typeof(T2), typeof(T3), typeof(T4));

	/// <inheritdoc cref="MethodInfo.MakeGenericMethod(Type[])"/>
	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.MakeGenericMethod(<see langword="typeof"/>(<typeparamref name="T1"/>),
	/// <see langword="typeof"/>(<typeparamref name="T2"/>),
	/// <see langword="typeof"/>(<typeparamref name="T3"/>),
	/// <see langword="typeof"/>(<typeparamref name="T4"/>),
	/// <see langword="typeof"/>(<typeparamref name="T5"/>));</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static MethodInfo MakeGenericMethod<T1, T2, T3, T4, T5>(this MethodInfo @this)
		=> @this.MakeGenericMethod(typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5));

	/// <inheritdoc cref="MethodInfo.MakeGenericMethod(Type[])"/>
	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.MakeGenericMethod(<see langword="typeof"/>(<typeparamref name="T1"/>),
	/// <see langword="typeof"/>(<typeparamref name="T2"/>),
	/// <see langword="typeof"/>(<typeparamref name="T3"/>),
	/// <see langword="typeof"/>(<typeparamref name="T4"/>),
	/// <see langword="typeof"/>(<typeparamref name="T5"/>),
	/// <see langword="typeof"/>(<typeparamref name="T6"/>));</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static MethodInfo MakeGenericMethod<T1, T2, T3, T4, T5, T6>(this MethodInfo @this)
		=> @this.MakeGenericMethod(typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6));

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.GetCustomAttribute&lt;<see cref="NameAttribute"/>&gt;()?.Name<br/>
	/// <see langword="    "/>?? @<paramref name="this"/>.Name.Left(@<paramref name="this"/>.Name.IndexOf('`'));</c>
	/// </summary>
	[DebuggerHidden]
	public static string Name(this MemberInfo @this)
		=> @this.GetCustomAttribute<NameAttribute>()?.Name
			?? @this.Name.Left(@this.Name.IndexOf(GENERIC_TICKMARK));

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.GetCustomAttribute&lt;<see cref="NameAttribute"/>&gt;()?.Name<br/>
	/// <see langword="    "/>?? @<paramref name="this"/>.Name.Left(@<paramref name="this"/>.Name.IndexOf('`'))<br/>
	/// <see langword="    "/>?? Invariant($"Parameter{@<paramref name="this"/>.Position}");</c>
	/// </summary>
	[DebuggerHidden]
	public static string Name(this ParameterInfo @this)
		=> @this.GetCustomAttribute<NameAttribute>()?.Name
			?? @this.Name?.Left(@this.Name.IndexOf(GENERIC_TICKMARK))
			?? Invariant($"Parameter{@this.Position}");

	/// <inheritdoc cref="FieldInfo.SetValue(object, object)"/>
	/// <remarks>
	/// <b>The code to set the field is built once and used subsequently.<br/>
	/// This is much faster than late binding.</b>
	/// </remarks>
	/// <param name="instance">Pass <c><see langword="null"/></c> if this is a static field.</param>
	public static void SetFieldValue(this FieldInfo @this, object? instance, object? value)
	{
		if (!@this.IsInitOnly && !@this.IsLiteral)
			TypeStore.FieldSetInvokes.GetOrAdd(@this.FieldHandle, handle => @this.FieldSetInvoke().Compile())(instance, value);
	}

	/// <param name="instance">Pass <c><see langword="null"/></c> if this is a static property setter.</param>
	/// <param name="index">Property <b>indexer</b> arguments; otherwise <c><see langword="null"/></c> if not an indexer.</param>
	public static void SetPropertyValue(this PropertyInfo @this, object? instance, object? value, params object?[]? index)
	{
		if (@this.CanWrite)
			@this.SetMethod?.InvokeMethod(instance switch
			{
				null when index?.Any() is true => index.Append(value).ToArray(),
				null => new[] { value },
				_ when index?.Any() is true => new[] { instance }.Concat(index).Append(value).ToArray(),
				_ => new[] { instance }.Append(value).ToArray()
			});
	}
}
