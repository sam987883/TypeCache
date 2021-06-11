// Copyright (c) 2021 Samuel Abraham

using System;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace TypeCache.Reflection.Extensions
{
	public static class HandleExtensions
	{
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
