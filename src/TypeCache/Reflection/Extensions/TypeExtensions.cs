﻿// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection;
using System.Runtime.CompilerServices;
using TypeCache.Collections.Extensions;
using TypeCache.Extensions;

namespace TypeCache.Reflection.Extensions
{
	public static class TypeExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Any(this Type? @this, params Type[] types)
			=> types.Any(@this.Is!);

		public static Kind GetKind(this Type @this)
			=> @this switch
			{
				_ when typeof(Delegate).IsAssignableFrom(@this.BaseType) => Kind.Delegate,
				_ when @this.IsEnum => Kind.Enum,
				_ when @this.IsInterface => Kind.Interface,
				_ when @this.IsValueType => Kind.Struct,
				_ => Kind.Class,
			};

		public static SystemType GetSystemType(this Type @this)
			=> @this switch
			{
				_ when MemberCache.SystemTypeMap.TryGetValue(@this.ToGenericType()?.TypeHandle ?? @this.TypeHandle, out var systemType) => systemType,
				_ when @this.GetKind() == Kind.Enum => @this.GetEnumUnderlyingType().GetSystemType(),
				_ when @this.IsArray => SystemType.Array,
				_ when @this.IsEnumerable() => SystemType.Enumerable,
				_ => SystemType.Unknown
			};

		public static string GetName(this MemberInfo @this)
			=> @this.GetCustomAttribute<NameAttribute>()?.Name ?? (@this.Name.Contains('`') ? @this.Name.Left(@this.Name.IndexOf('`')) : @this.Name);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string GetName(this ParameterInfo @this)
			=> @this.GetCustomAttribute<NameAttribute>()?.Name ?? @this.Name!;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Implements<T>(this Type @this)
			where T : class
			=> @this.Implements(typeof(T));

		public static bool Implements(this Type @this, Type type)
			=> @this.BaseType.Is(type) || (type.IsInterface && @this.GetInterfaces().Any(_ => _.Is(type)));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Is<T>(this Type? @this)
			=> @this == typeof(T);

		public static bool Is(this Type? @this, Type type)
			=> type == @this || (type.IsGenericTypeDefinition && type == @this.ToGenericType());

		public static bool IsAsync(this Type @this)
		{
			var systemType = @this.GetSystemType();
			return systemType == SystemType.Task
				|| systemType == SystemType.ValueTask
				|| @this.Is(typeof(IAsyncDisposable))
				|| @this.Is(typeof(IAsyncEnumerable<>))
				|| @this.Implements(typeof(IAsyncDisposable))
				|| @this.Implements(typeof(IAsyncEnumerable<>));
		}

		public static bool IsEnumerable(this Type @this)
			=> @this.Is<IEnumerable>() || @this.Implements(typeof(IEnumerable));

		public static bool IsInvokable(this Type @this)
			=> !@this.IsPointer && !@this.IsByRef && !@this.IsByRefLike;

		public static Type? ToGenericType(this Type? @this)
			=> @this switch
			{
				null => null,
				_ when @this.IsGenericTypeDefinition => @this,
				_ when @this.IsGenericType => @this.GetGenericTypeDefinition(),
				_ => null
			};

		public static TypeMember ToMember(this Type @this)
		{
			var attributes = @this.GetCustomAttributes<Attribute>(true).ToImmutableArray();
			var kind = @this.GetKind();
			var systemType = @this.GetSystemType();
			var baseTypeHandle = @this.BaseType?.TypeHandle ?? typeof(object).TypeHandle;
			var interfaces = @this.GetInterfaces();
			var enclosedTypeHandle = systemType switch
			{
				_ when @this.HasElementType => @this.GetElementType()!.TypeHandle,
				SystemType.Dictionary or SystemType.ImmutableDictionary or SystemType.ImmutableSortedDictionary or SystemType.SortedDictionary
					=> typeof(KeyValuePair<,>).MakeGenericType(@this.GenericTypeArguments).TypeHandle,
				_ when @this.GenericTypeArguments.Length == 1 => @this.GenericTypeArguments[0].TypeHandle,
				_ => (RuntimeTypeHandle?)null
			};
			var genericTypeHandles = @this.GenericTypeArguments.To(_ => _.TypeHandle).ToImmutableArray();
			var interfaceTypeHandles = interfaces.To(_ => _.TypeHandle).ToImmutableArray();

			return new TypeMember(@this.GetName(), attributes, !@this.IsVisible, @this.IsPublic, kind, systemType, @this.TypeHandle, baseTypeHandle,
				enclosedTypeHandle, genericTypeHandles, interfaceTypeHandles, @this.IsEnumerable(), @this.IsPointer, @this.IsByRef || @this.IsByRefLike);
		}
	}
}
