﻿// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using TypeCache.Collections.Extensions;

namespace TypeCache.Reflection.Extensions
{
	public static class HandleExtensions
	{
		private const BindingFlags BINDINGS = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;

		public static ConstructorMember[] CreateConstructorMembers(this RuntimeTypeHandle @this)
			=> @this.ToType().GetConstructors(BINDINGS)
				.If(constructorInfo => !constructorInfo.IsStatic && constructorInfo.IsInvokable())
				.To(constructorInfo => constructorInfo.MethodHandle.GetConstructorMember(@this))
				.ToArray();

		public static IDictionary<string, EventMember> CreateEventMembers(this RuntimeTypeHandle @this)
			=> @this.ToType().GetEvents(BINDINGS)
				.To(eventInfo => KeyValuePair.Create(eventInfo.Name, new EventMember(eventInfo)))
				.ToDictionary(StringComparison.Ordinal);

		public static IDictionary<string, FieldMember> CreateFieldMembers(this RuntimeTypeHandle @this)
			=> @this.ToType().GetFields(BINDINGS)
				.If(fieldInfo => !fieldInfo.IsLiteral && !fieldInfo.FieldType.IsByRefLike)
				.To(fieldInfo => KeyValuePair.Create(fieldInfo.Name, fieldInfo.FieldHandle.GetFieldMember(@this)))
				.ToDictionary(StringComparison.Ordinal);

		public static IDictionary<string, MethodMember[]> CreateMethodMembers(this RuntimeTypeHandle @this)
			=> @this.ToType().GetMethods(BINDINGS)
				.If(methodInfo => !methodInfo.IsSpecialName && methodInfo.IsInvokable())
				.To(methodInfo => methodInfo.MethodHandle.GetMethodMember(@this))
				.Group(method => method.Name, StringComparer.Ordinal)
				.ToDictionary(_ => _.Key, _ => _.Value.ToArray(), StringComparison.Ordinal);

		public static IDictionary<string, PropertyMember> CreatePropertyMembers(this RuntimeTypeHandle @this)
			=> @this.ToType().GetProperties(BINDINGS)
				.If(propertyInfo => propertyInfo.PropertyType.IsInvokable())
				.To(propertyInfo => KeyValuePair.Create(propertyInfo.Name, new PropertyMember(propertyInfo)))
				.ToDictionary(StringComparison.Ordinal);

		/// <summary>
		/// <c><see cref="ConstructorMember.Cache"/>[@<paramref name="this"/>]</c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ConstructorMember GetConstructorMember(this RuntimeMethodHandle @this, RuntimeTypeHandle typeHandle)
			=> ConstructorMember.Cache[(@this, typeHandle)];

		/// <summary>
		/// <c><see cref="FieldMember.Cache"/>[@<paramref name="this"/>]</c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FieldMember GetFieldMember(this RuntimeFieldHandle @this, RuntimeTypeHandle typeHandle)
			=> FieldMember.Cache[(@this, typeHandle)];

		/// <summary>
		/// <c><see cref="MethodMember.Cache"/>[@<paramref name="this"/>]</c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static MethodMember GetMethodMember(this RuntimeMethodHandle @this, RuntimeTypeHandle typeHandle)
			=> MethodMember.Cache[(@this, typeHandle)];

		/// <summary>
		/// <c><see cref="TypeMember.Cache"/>[@<paramref name="this"/>]</c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static TypeMember GetTypeMember(this RuntimeTypeHandle @this)
			=> TypeMember.Cache[@this];

		/// <summary>
		/// <c><see cref="RuntimeTypeHandle.Equals(RuntimeTypeHandle)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Is<T>(this RuntimeTypeHandle @this)
			=> @this.Equals(typeof(T).TypeHandle);

		/// <summary>
		/// <code>
		/// <see cref="RuntimeTypeHandle.Equals(RuntimeTypeHandle)"/>
		/// || (<paramref name="type"/>.IsGenericTypeDefinition &amp;&amp; @<paramref name="this"/>.ToGenericType() == <paramref name="type"/>)
		/// </code>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Is(this RuntimeTypeHandle @this, Type type)
			=> @this.Equals(type.TypeHandle) || (type.IsGenericTypeDefinition && @this.ToGenericType() == type);

		/// <summary>
		/// <c><see cref="FieldInfo.GetFieldFromHandle(RuntimeFieldHandle)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FieldInfo ToFieldInfo(this RuntimeFieldHandle @this)
			=> FieldInfo.GetFieldFromHandle(@this);

		/// <summary>
		/// <c><see cref="FieldInfo.GetFieldFromHandle(RuntimeFieldHandle, RuntimeTypeHandle)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FieldInfo ToFieldInfo(this RuntimeTypeHandle @this, RuntimeFieldHandle fieldHandle)
			=> FieldInfo.GetFieldFromHandle(fieldHandle, @this);

		/// <summary>
		/// <c><see cref="Type.GetGenericTypeDefinition"/></c>
		/// </summary>
		public static Type? ToGenericType(this RuntimeTypeHandle @this)
		{
			var type = @this.ToType();
			return type?.IsGenericType is true ? type.GetGenericTypeDefinition() : null;
		}

		/// <summary>
		/// <c><see cref="MethodBase.GetMethodFromHandle(RuntimeMethodHandle)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static MethodBase? ToMethodBase(this RuntimeMethodHandle @this)
			=> MethodBase.GetMethodFromHandle(@this);

		/// <summary>
		/// <c><see cref="MethodBase.GetMethodFromHandle(RuntimeMethodHandle, RuntimeTypeHandle)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static MethodBase? ToMethodBase(this RuntimeTypeHandle @this, RuntimeMethodHandle methodHandle)
			=> MethodBase.GetMethodFromHandle(methodHandle, @this);

		/// <summary>
		/// <c><see cref="Type.GetTypeFromHandle(RuntimeTypeHandle)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Type ToType(this RuntimeTypeHandle @this)
			=> Type.GetTypeFromHandle(@this);
	}
}
