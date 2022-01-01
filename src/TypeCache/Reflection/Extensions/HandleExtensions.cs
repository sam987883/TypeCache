// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using TypeCache.Collections.Extensions;
using TypeCache.Extensions;
using static TypeCache.Default;

namespace TypeCache.Reflection.Extensions;

public static class HandleExtensions
{
	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.GetParameters().All(_ =&gt; !_.IsOut &amp;&amp; _.ParameterType.IsInvokable());</c>
	/// </summary>
	internal static bool IsInvokable(this ConstructorInfo @this)
		=> @this.GetParameters().All(_ => !_.IsOut && _.ParameterType.IsInvokable());

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.GetParameters().All(_ =&gt; !_.IsOut &amp;&amp; _.ParameterType.IsInvokable()) &amp;&amp; !@<paramref name="this"/>.ReturnType.IsByRef &amp;&amp; !@<paramref name="this"/>.ReturnType.IsByRefLike;</c>
	/// </summary>
	internal static bool IsInvokable(this MethodInfo @this)
		=> @this.GetParameters().All(_ => !_.IsOut && _.ParameterType.IsInvokable()) && !@this.ReturnType.IsByRef && !@this.ReturnType.IsByRefLike;

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.IsPointer &amp;&amp; !@<paramref name="this"/>.IsByRef &amp;&amp; !@<paramref name="this"/>.IsByRefLike;</c>
	/// </summary>
	internal static bool IsInvokable(this Type @this)
		=> !@this.IsPointer && !@this.IsByRef && !@this.IsByRefLike;

	/// <summary>
	/// <c><see cref="TypeMember.Cache"/>[@<paramref name="this"/>]</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static TypeMember GetTypeMember(this RuntimeTypeHandle @this)
		=> TypeMember.Cache[@this];

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.Equals(<see langword="typeof"/>(<typeparamref name="T"/>).TypeHandle);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static bool Is<T>(this RuntimeTypeHandle @this)
		=> @this.Equals(typeof(T).TypeHandle);

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.Equals(<paramref name="type"/>.TypeHandle)
	/// || (<paramref name="type"/>.IsGenericTypeDefinition &amp;&amp; @<paramref name="this"/>.ToGenericType() == <paramref name="type"/>);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static bool Is(this RuntimeTypeHandle @this, Type type)
		=> @this.Equals(type.TypeHandle) || (type.IsGenericTypeDefinition && @this.ToGenericType() == type);

	/// <summary>
	/// <c>=&gt; <see cref="FieldInfo"/>.GetFieldFromHandle(@<paramref name="this"/>);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static FieldInfo ToFieldInfo(this RuntimeFieldHandle @this)
		=> FieldInfo.GetFieldFromHandle(@this);

	/// <summary>
	/// <c>=&gt; <see cref="FieldInfo"/>.GetFieldFromHandle(<paramref name="fieldHandle"/>, @<paramref name="this"/>);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static FieldInfo ToFieldInfo(this RuntimeTypeHandle @this, RuntimeFieldHandle fieldHandle)
		=> FieldInfo.GetFieldFromHandle(fieldHandle, @this);

	/// <summary>
	/// <code>
	/// var type = @this.ToType();<br/>
	/// <see langword="return"/> type?.IsGenericType <see langword="is true"/> ? type.GetGenericTypeDefinition() : <see langword="null"/>;
	/// </code>
	/// </summary>
	public static Type? ToGenericType(this RuntimeTypeHandle @this)
	{
		var type = @this.ToType();
		return type?.IsGenericType is true ? type.GetGenericTypeDefinition() : null;
	}

	/// <summary>
	/// <c>=&gt; <see cref="MethodBase"/>.GetMethodFromHandle(@<paramref name="this"/>);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static MethodBase? ToMethodBase(this RuntimeMethodHandle @this)
		=> MethodBase.GetMethodFromHandle(@this);

	/// <summary>
	/// <c>=&gt; <see cref="MethodBase"/>.GetMethodFromHandle(<paramref name="methodHandle"/>, @<paramref name="this"/>);</c>
	/// </summary>
	/// <param name="this"><see cref="RuntimeTypeHandle"/> is needed when <paramref name="methodHandle"/> is a method using a generic parameter of its declared type.</param>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static MethodBase? ToMethodBase(this RuntimeTypeHandle @this, RuntimeMethodHandle methodHandle)
		=> MethodBase.GetMethodFromHandle(methodHandle, @this);

	/// <summary>
	/// <c>=&gt; <see cref="Type"/>.GetTypeFromHandle(@<paramref name="this"/>);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static Type ToType(this RuntimeTypeHandle @this)
		=> Type.GetTypeFromHandle(@this);
}
