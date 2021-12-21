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
	private static bool IsInvokable(this ConstructorInfo @this)
		=> @this.GetParameters().All(_ => !_.IsOut && _.ParameterType.IsInvokable());

	private static bool IsInvokable(this MethodInfo @this)
		=> @this.GetParameters().All(_ => !_.IsOut && _.ParameterType.IsInvokable()) && !@this.ReturnType.IsByRef && !@this.ReturnType.IsByRefLike;

	private static bool IsInvokable(this Type @this)
		=> !@this.IsPointer && !@this.IsByRef && !@this.IsByRefLike;

	/// <summary>
	/// <c>@<paramref name="this"/>.ToType().GetConstructors(<see cref="INSTANCE_BINDING_FLAGS"/>)<br/>
	///		.If(constructorInfo =&gt; constructorInfo.IsInvokable())<br/>
	///		.To(constructorInfo =&gt; constructorInfo.MethodHandle.GetConstructorMember(@<paramref name="this"/>))<br/>
	///		.ToArray()</c>
	/// </summary>
	/// <param name="this">The <see cref="RuntimeTypeHandle"/> of the type to get the constructors from.</param>
	public static ConstructorMember[] CreateConstructorMembers(this RuntimeTypeHandle @this)
		=> @this.ToType().GetConstructors(INSTANCE_BINDING_FLAGS)
			.If(constructorInfo => constructorInfo.IsInvokable())
			.To(constructorInfo => constructorInfo.MethodHandle.GetConstructorMember(@this))
			.ToArray();

	/// <summary>
	/// <c>@<paramref name="this"/>.ToType().GetEvents(<see cref="BINDING_FLAGS"/>)<br/>
	///		.To(eventInfo =&gt; <see cref="KeyValuePair"/>.Create(eventInfo.Name, <see langword="new"/> <see cref="EventMember"/>(eventInfo)))<br/>
	///		.ToDictionary(<see cref="NAME_STRING_COMPARISON"/>)</c>
	/// </summary>
	/// <param name="this">The <see cref="RuntimeTypeHandle"/> of the type to get the events from.</param>
	public static IDictionary<string, EventMember> CreateEventMembers(this RuntimeTypeHandle @this)
		=> @this.ToType().GetEvents(BINDING_FLAGS)
			.To(eventInfo => KeyValuePair.Create(eventInfo.Name, new EventMember(eventInfo)))
			.ToDictionary(NAME_STRING_COMPARISON);

	/// <summary>
	/// <c>@<paramref name="this"/>.ToType().GetFields(<see cref="BINDING_FLAGS"/>)<br/>
	///		.If(fieldInfo =&gt; !fieldInfo.IsLiteral &amp;&amp; !fieldInfo.FieldType.IsByRefLike)<br/>
	///		.To(fieldInfo =&gt; <see cref="KeyValuePair"/>.Create(fieldInfo.Name, fieldInfo.FieldHandle.GetFieldMember(@<paramref name="this"/>)))<br/>
	///		.ToDictionary(<see cref="NAME_STRING_COMPARISON"/>)</c>
	/// </summary>
	/// <param name="this">The <see cref="RuntimeTypeHandle"/> of the type to get the fields from.</param>
	public static IDictionary<string, FieldMember> CreateFieldMembers(this RuntimeTypeHandle @this)
		=> @this.ToType().GetFields(BINDING_FLAGS)
			.If(fieldInfo => !fieldInfo.IsLiteral && !fieldInfo.FieldType.IsByRefLike)
			.To(fieldInfo => KeyValuePair.Create(fieldInfo.Name, fieldInfo.FieldHandle.GetFieldMember(@this)))
			.ToDictionary(NAME_STRING_COMPARISON);

	/// <summary>
	/// <c>@<paramref name="this"/>.ToType().GetMethods(<see cref="BINDING_FLAGS"/>)<br/>
	///		.If(methodInfo =&gt; !methodInfo.IsSpecialName &amp;&amp; methodInfo.IsInvokable())<br/>
	///		.To(methodInfo =&gt; methodInfo.MethodHandle.GetMethodMember(@<paramref name="this"/>))<br/>
	///		.Group(method =&gt; method.Name, <see cref="NAME_STRING_COMPARISON"/>.ToStringComparer())<br/>
	///		.ToDictionary(_ =&gt; _.Key, _ =&gt; _.Value.ToArray(), <see cref="NAME_STRING_COMPARISON"/>)</c>
	/// </summary>
	/// <param name="this">The <see cref="RuntimeTypeHandle"/> of the type to get the methods from.</param>
	public static IDictionary<string, MethodMember[]> CreateMethodMembers(this RuntimeTypeHandle @this)
		=> @this.ToType().GetMethods(BINDING_FLAGS)
			.If(methodInfo => !methodInfo.IsSpecialName && methodInfo.IsInvokable())
			.To(methodInfo => methodInfo.MethodHandle.GetMethodMember(@this))
			.Group(method => method.Name, NAME_STRING_COMPARISON.ToStringComparer())
			.ToDictionary(_ => _.Key, _ => _.Value.ToArray(), NAME_STRING_COMPARISON);

	/// <summary>
	/// <c>@<paramref name="this"/>.ToType().GetFields(<see cref="BINDING_FLAGS"/>)<br/>
	///		.If(propertyInfo =&gt; propertyInfo.PropertyType.IsInvokable())<br/>
	///		.To(propertyInfo =&gt; <see cref="KeyValuePair"/>.Create(propertyInfo.Name, <see langword="new"/> PropertyMember(propertyInfo)))<br/>
	///		.ToDictionary(<see cref="NAME_STRING_COMPARISON"/>)</c>
	/// </summary>
	/// <param name="this">The <see cref="RuntimeTypeHandle"/> of the type to get the properties from.</param>
	public static IDictionary<string, PropertyMember> CreatePropertyMembers(this RuntimeTypeHandle @this)
		=> @this.ToType().GetProperties(BINDING_FLAGS)
			.If(propertyInfo => propertyInfo.PropertyType.IsInvokable())
			.To(propertyInfo => KeyValuePair.Create(propertyInfo.Name, new PropertyMember(propertyInfo)))
			.ToDictionary(NAME_STRING_COMPARISON);

	/// <summary>
	/// <c><see cref="ConstructorMember.Cache"/>[(@<paramref name="this"/>, <paramref name="typeHandle"/>)]</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static ConstructorMember GetConstructorMember(this RuntimeMethodHandle @this, RuntimeTypeHandle typeHandle)
		=> ConstructorMember.Cache[(@this, typeHandle)];

	/// <summary>
	/// <c><see cref="FieldMember.Cache"/>[(@<paramref name="this"/>, <paramref name="typeHandle"/>)]</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static FieldMember GetFieldMember(this RuntimeFieldHandle @this, RuntimeTypeHandle typeHandle)
		=> FieldMember.Cache[(@this, typeHandle)];

	/// <summary>
	/// <c><see cref="MethodMember.Cache"/>[(@<paramref name="this"/>, <paramref name="typeHandle"/>)]</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static MethodMember GetMethodMember(this RuntimeMethodHandle @this, RuntimeTypeHandle typeHandle)
		=> MethodMember.Cache[(@this, typeHandle)];

	/// <summary>
	/// <c><see cref="TypeMember.Cache"/>[@<paramref name="this"/>]</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static TypeMember GetTypeMember(this RuntimeTypeHandle @this)
		=> TypeMember.Cache[@this];

	/// <summary>
	/// <c>@<paramref name="this"/>.Equals(<see langword="typeof"/>(<typeparamref name="T"/>).TypeHandle)</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static bool Is<T>(this RuntimeTypeHandle @this)
		=> @this.Equals(typeof(T).TypeHandle);

	/// <summary>
	/// <c>@<paramref name="this"/>.Equals(<paramref name="type"/>.TypeHandle)
	/// || (<paramref name="type"/>.IsGenericTypeDefinition &amp;&amp; @<paramref name="this"/>.ToGenericType() == <paramref name="type"/>)</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static bool Is(this RuntimeTypeHandle @this, Type type)
		=> @this.Equals(type.TypeHandle) || (type.IsGenericTypeDefinition && @this.ToGenericType() == type);

	/// <summary>
	/// <c><see cref="FieldInfo"/>.GetFieldFromHandle(@<paramref name="this"/>)</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static FieldInfo ToFieldInfo(this RuntimeFieldHandle @this)
		=> FieldInfo.GetFieldFromHandle(@this);

	/// <summary>
	/// <c><see cref="FieldInfo"/>.GetFieldFromHandle(<paramref name="fieldHandle"/>, @<paramref name="this"/>)</c>
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
	/// <c><see cref="MethodBase"/>.GetMethodFromHandle(@<paramref name="this"/>)</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static MethodBase? ToMethodBase(this RuntimeMethodHandle @this)
		=> MethodBase.GetMethodFromHandle(@this);

	/// <summary>
	/// <c><see cref="MethodBase"/>.GetMethodFromHandle(<paramref name="methodHandle"/>, @<paramref name="this"/>)</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static MethodBase? ToMethodBase(this RuntimeTypeHandle @this, RuntimeMethodHandle methodHandle)
		=> MethodBase.GetMethodFromHandle(methodHandle, @this);

	/// <summary>
	/// <c><see cref="Type"/>.GetTypeFromHandle(@<paramref name="this"/>)</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static Type ToType(this RuntimeTypeHandle @this)
		=> Type.GetTypeFromHandle(@this);
}
