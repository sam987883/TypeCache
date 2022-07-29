// Copyright (c) 2021 Samuel Abraham

using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using TypeCache.Collections.Extensions;
using static TypeCache.Default;

namespace TypeCache.Reflection.Extensions;

public static class HandleExtensions
{
	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.GetParameters().All(_ =&gt; !_.IsOut &amp;&amp; _.ParameterType.IsInvokable());</c>
	/// </remarks>
	internal static bool IsInvokable(this ConstructorInfo @this)
		=> @this.GetParameters().All(_ => !_.IsOut && _.ParameterType.IsInvokable());

	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.GetParameters().All(_ =&gt; !_.IsOut &amp;&amp; _.ParameterType.IsInvokable()) &amp;&amp; !@<paramref name="this"/>.ReturnType.IsByRef &amp;&amp; !@<paramref name="this"/>.ReturnType.IsByRefLike;</c>
	/// </remarks>
	internal static bool IsInvokable(this MethodInfo @this)
		=> @this.GetParameters().All(_ => !_.IsOut && _.ParameterType.IsInvokable()) && !@this.ReturnType.IsByRef && !@this.ReturnType.IsByRefLike;

	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.IsPointer &amp;&amp; !@<paramref name="this"/>.IsByRef &amp;&amp; !@<paramref name="this"/>.IsByRefLike;</c>
	/// </remarks>
	internal static bool IsInvokable(this Type @this)
		=> !@this.IsPointer && !@this.IsByRef && !@this.IsByRefLike;

	/// <remarks>
	/// <c>=&gt; <see cref="TypeMember.Cache"/>[@<paramref name="this"/>];</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static TypeMember GetTypeMember(this RuntimeTypeHandle @this)
		=> TypeMember.Cache[@this];

	/// <inheritdoc cref="RuntimeTypeHandle.Equals(RuntimeTypeHandle)"/>
	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.Equals(<see langword="typeof"/>(<typeparamref name="T"/>).TypeHandle);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static bool Is<T>(this RuntimeTypeHandle @this)
		=> @this.Equals(typeof(T).TypeHandle);

	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.Equals(<paramref name="type"/>.TypeHandle)
	/// || (<paramref name="type"/>.IsGenericTypeDefinition &amp;&amp; @<paramref name="this"/>.ToGenericType() == <paramref name="type"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static bool Is(this RuntimeTypeHandle @this, Type type)
		=> @this.Equals(type.TypeHandle) || (type.IsGenericTypeDefinition && @this.ToGenericType() == type);

	/// <inheritdoc cref="FieldInfo.GetFieldFromHandle(RuntimeFieldHandle)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="FieldInfo"/>.GetFieldFromHandle(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static FieldInfo ToFieldInfo(this RuntimeFieldHandle @this)
		=> FieldInfo.GetFieldFromHandle(@this);

	/// <inheritdoc cref="FieldInfo.GetFieldFromHandle(RuntimeFieldHandle, RuntimeTypeHandle)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="FieldInfo"/>.GetFieldFromHandle(@<paramref name="this"/>, <paramref name="typeHandle"/>);</c>
	/// </remarks>
	/// <param name="typeHandle"><see cref="RuntimeTypeHandle"/> is needed when <see cref="RuntimeFieldHandle"/> is a field whose type is a generic parameter of its declared type.</param>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static FieldInfo ToFieldInfo(this RuntimeFieldHandle @this, RuntimeTypeHandle typeHandle)
		=> FieldInfo.GetFieldFromHandle(@this, typeHandle);

	/// <remarks>
	/// <code>
	/// var type = @this.ToType();<br/>
	/// <see langword="return"/> type?.IsGenericType <see langword="is true"/> ? type.GetGenericTypeDefinition() : <see langword="null"/>;
	/// </code>
	/// </remarks>
	public static Type? ToGenericType(this RuntimeTypeHandle @this)
	{
		var type = @this.ToType();
		return type?.IsGenericType is true ? type.GetGenericTypeDefinition() : null;
	}

	/// <inheritdoc cref="MethodBase.GetMethodFromHandle(RuntimeMethodHandle)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="MethodBase"/>.GetMethodFromHandle(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static MethodBase? ToMethodBase(this RuntimeMethodHandle @this)
		=> MethodBase.GetMethodFromHandle(@this);

	/// <inheritdoc cref="MethodBase.GetMethodFromHandle(RuntimeMethodHandle, RuntimeTypeHandle)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="MethodBase"/>.GetMethodFromHandle(@<paramref name="this"/>, <paramref name="typeHandle"/>);</c>
	/// </remarks>
	/// <param name="typeHandle"><see cref="RuntimeTypeHandle"/> is needed when <see cref="RuntimeMethodHandle"/> is a method using a generic parameter of its declared type.</param>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static MethodBase? ToMethodBase(this RuntimeMethodHandle @this, RuntimeTypeHandle typeHandle)
		=> MethodBase.GetMethodFromHandle(@this, typeHandle);

	/// <inheritdoc cref="Type.GetTypeFromHandle(RuntimeTypeHandle)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Type"/>.GetTypeFromHandle(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static Type ToType(this RuntimeTypeHandle @this)
		=> Type.GetTypeFromHandle(@this);
}
