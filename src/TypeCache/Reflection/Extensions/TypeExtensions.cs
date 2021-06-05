// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using TypeCache.Collections.Extensions;
using TypeCache.Extensions;

namespace TypeCache.Reflection.Extensions
{
	public static class TypeExtensions
	{
		/// <summary>
		/// <c><paramref name="types"/>.Any(@<paramref name="this"/>.Is)</c>
		/// </summary>
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

		/// <summary>
		/// <c>@<paramref name="this"/>.Implements(typeof(<typeparamref name="T"/>))</c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Implements<T>(this Type @this)
			where T : class
			=> @this.Implements(typeof(T));

		/// <summary>
		/// <code>
		/// @<paramref name="this"/>BaseType.Is(<paramref name="type"/>)
		/// || (<paramref name="type"/>.IsInterface &amp;&amp; @<paramref name="this"/>.GetInterfaces().Any(_ => _.Is(<paramref name="type"/>)))
		/// </code>
		/// </summary>
		public static bool Implements(this Type @this, Type type)
			=> @this.BaseType.Is(type) || (type.IsInterface && @this.GetInterfaces().Any(_ => _.Is(type)));

		/// <summary>
		/// <c>@<paramref name="this"/> == typeof(<typeparamref name="T"/>)</c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Is<T>(this Type? @this)
			=> @this == typeof(T);

		/// <summary>
		/// <code>
		/// @<paramref name="this"/> == <paramref name="type"/>
		/// || (<paramref name="type"/>.IsGenericTypeDefinition &amp;&amp; <paramref name="type"/> == @<paramref name="this"/>.ToGenericType())
		/// </code>
		/// </summary>
		public static bool Is(this Type? @this, Type type)
			=> @this == type || (type.IsGenericTypeDefinition && type == @this.ToGenericType());

		/// <summary>
		/// <code>
		/// <list type="table">
		/// <item><see cref="Task"/>, <see cref="Task{TResult}"/></item>
		/// <item><see cref="ValueTask"/>, <see cref="ValueTask{TResult}"/></item>
		/// <item><see cref="IAsyncDisposable"/></item>
		/// <item><see cref="IAsyncEnumerable{T}"/></item>
		/// </list>
		/// </code>
		/// </summary>
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

		/// <summary>
		/// <c>@<paramref name="this"/>.Is&lt;<see cref="IEnumerable"/>&gt;() || @<paramref name="this"/>.Implements(typeof(<see cref="IEnumerable"/>))</c>
		/// </summary>
		public static bool IsEnumerable(this Type @this)
			=> @this.Is<IEnumerable>() || @this.Implements(typeof(IEnumerable));

		/// <summary>
		/// <c>!@<paramref name="this"/>.IsPointer &amp;&amp; !@<paramref name="this"/>.IsByRef &amp;&amp; !@<paramref name="this"/>.IsByRefLike</c>
		/// </summary>
		public static bool IsInvokable(this Type @this)
			=> !@this.IsPointer && !@this.IsByRef && !@this.IsByRefLike;

		/// <summary>
		/// <c><see cref="Type.GetGenericTypeDefinition"/></c>
		/// </summary>
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
