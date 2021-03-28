// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection;
using System.Runtime.CompilerServices;
using TypeCache.Collections.Extensions;

namespace TypeCache.Reflection.Extensions
{
	public static class TypeExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Any(this Type? @this, params Type[] types)
			=> types.Any(@this.Is!);

		public static object Create(this Type @this, params object[] parameters)
			=> @this.GetConstructorCache().First(constructor => constructor!.IsCallableWith(parameters))?.Create!(parameters)
				?? throw new ArgumentException($"Create instance of class {@this.Name} failed with {parameters?.Length ?? 0} parameters.");

		public static TypeMember CreateMember(this Type @this)
		{
			var interfaces = @this.GetInterfaces();
			var kind = @this.GetKind();
			var systemType = @this.GetSystemType();
			var isNullable = kind == Kind.Class || kind == Kind.Delegate || kind == Kind.Interface || systemType == SystemType.Nullable;
			var attributes = IEnumerableExtensions.ToImmutableArray(@this.GetCustomAttributes<Attribute>(true));
			var genericTypeHandles = IEnumerableExtensions.ToImmutableArray(@this.GenericTypeArguments.To(_ => _.TypeHandle));
			var interfaceTypeHandles = IEnumerableExtensions.ToImmutableArray(interfaces.To(_ => _.TypeHandle));

			return new TypeMember(@this.GetName(), attributes, !@this.IsVisible, @this.IsPublic, kind, systemType, @this.TypeHandle, @this.BaseType?.TypeHandle,
				genericTypeHandles, interfaceTypeHandles, @this.IsEnumerable(), isNullable, @this.IsPointer, @this.IsByRef || @this.IsByRefLike);
		}

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
		{
			return @this switch
			{
				_ when MemberCache.SystemTypeMap.TryGetValue(@this.TypeHandle, out var systemType) => systemType,
				_ when @this.GetKind() == Kind.Enum => @this.GetEnumUnderlyingType().GetSystemType(),
				_ when @this.IsArray => SystemType.Array,
				_ when @this.IsEnumerable() => SystemType.Enumerable,
				_ => SystemType.Unknown
			};
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string GetName(this MemberInfo @this)
			=> @this.GetCustomAttribute<NameAttribute>()?.Name ?? @this.Name;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string GetName(this ParameterInfo @this)
			=> @this.GetCustomAttribute<NameAttribute>()?.Name ?? @this.Name!;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IImmutableList<ConstructorMember> GetConstructorCache(this Type @this)
			=> MemberCache.Constructors[@this.TypeHandle];

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IImmutableDictionary<string, FieldMember> GetFieldMembers(this Type @this)
			=> MemberCache.Fields[@this.TypeHandle];

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IImmutableList<IndexerMember> GetIndexerMembers(this Type @this)
			=> MemberCache.Indexers[@this.TypeHandle];

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IImmutableDictionary<string, IImmutableList<MethodMember>> GetMethodMembers(this Type @this)
			=> MemberCache.Methods[@this.TypeHandle];

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IImmutableDictionary<string, PropertyMember> GetPropertyMembers(this Type @this)
			=> MemberCache.Properties[@this.TypeHandle];

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IImmutableDictionary<string, StaticFieldMember> GetStaticFieldMembers(this Type @this)
			=> MemberCache.StaticFields[@this.TypeHandle];

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IImmutableDictionary<string, IImmutableList<StaticMethodMember>> GetStaticMethodMembers(this Type @this)
			=> MemberCache.StaticMethods[@this.TypeHandle];

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IImmutableDictionary<string, StaticPropertyMember> GetStaticPropertyMembers(this Type @this)
			=> MemberCache.StaticProperties[@this.TypeHandle];

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
				|| @this.Implements(typeof(IAsyncDisposable))
				|| @this.Implements(typeof(IAsyncEnumerable<>))
				|| @this.Is(typeof(IAsyncDisposable))
				|| @this.Is(typeof(IAsyncEnumerable<>));
		}

		public static bool IsEnumerable(this Type @this)
			=> @this.Is<IEnumerable>() || @this.ToGenericType() == typeof(IEnumerable);

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
	}
}
