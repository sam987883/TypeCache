// Copyright (c) 2021 Samuel Abraham

using System;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace TypeCache.Reflection.Extensions
{
	public static class HandleExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static TypeMember GetTypeMember(this RuntimeTypeHandle @this)
			=> MemberCache.Types[@this];

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Is<T>(this RuntimeTypeHandle @this)
			=> @this.Equals(typeof(T).TypeHandle);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Is(this RuntimeTypeHandle @this, Type type)
			=> @this.Equals(type.TypeHandle) || (type.IsGenericTypeDefinition && @this.ToGenericType() == type);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FieldInfo ToFieldInfo(this RuntimeFieldHandle @this)
			=> FieldInfo.GetFieldFromHandle(@this);

		public static Type? ToGenericType(this RuntimeTypeHandle @this)
			=> @this.ToType() switch
			{
				Type type when type.IsGenericType => type.GetGenericTypeDefinition(),
				_ => null
			};

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static MethodBase? ToMethodInfo(this RuntimeMethodHandle @this)
			=> MethodBase.GetMethodFromHandle(@this);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Type ToType(this RuntimeTypeHandle @this)
			=> Type.GetTypeFromHandle(@this);
	}
}
