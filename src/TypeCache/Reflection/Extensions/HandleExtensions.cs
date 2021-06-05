﻿// Copyright (c) 2021 Samuel Abraham

using System;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace TypeCache.Reflection.Extensions
{
	public static class HandleExtensions
	{
		/// <summary>
		/// <c><see cref="MemberCache.Types"/>[@<paramref name="this"/>]</c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static TypeMember GetTypeMember(this RuntimeTypeHandle @this)
			=> MemberCache.Types[@this];

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
		public static MethodBase? ToMethodInfo(this RuntimeMethodHandle @this)
			=> MethodBase.GetMethodFromHandle(@this);

		/// <summary>
		/// <c><see cref="Type.GetTypeFromHandle(RuntimeTypeHandle)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Type ToType(this RuntimeTypeHandle @this)
			=> Type.GetTypeFromHandle(@this);
	}
}
