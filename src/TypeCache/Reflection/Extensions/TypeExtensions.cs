// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Tasks;
using TypeCache.Collections.Extensions;

namespace TypeCache.Reflection.Extensions
{
	public static class TypeExtensions
	{
		static TypeExtensions()
		{
			SystemTypes = new Dictionary<RuntimeTypeHandle, SystemType>
			{
				{ typeof(bool).TypeHandle, SystemType.Boolean},
				{ typeof(sbyte).TypeHandle, SystemType.SByte},
				{ typeof(byte).TypeHandle, SystemType.Byte},
				{ typeof(short).TypeHandle, SystemType.Int16},
				{ typeof(ushort).TypeHandle, SystemType.UInt16},
				{ typeof(int).TypeHandle, SystemType.Int32},
				{ typeof(uint).TypeHandle, SystemType.UInt32},
				{ typeof(long).TypeHandle, SystemType.Int64},
				{ typeof(ulong).TypeHandle, SystemType.UInt64},
				{ typeof(IntPtr).TypeHandle, SystemType.IntPtr},
				{ typeof(UIntPtr).TypeHandle, SystemType.UIntPtr},
				{ typeof(BigInteger).TypeHandle, SystemType.BigInteger},
				{ typeof(float).TypeHandle, SystemType.Single},
				{ typeof(double).TypeHandle, SystemType.Double},
				{ typeof(Half).TypeHandle, SystemType.Half},
				{ typeof(decimal).TypeHandle, SystemType.Decimal},
				{ typeof(char).TypeHandle, SystemType.Char},
				{ typeof(DateTime).TypeHandle, SystemType.DateTime},
				{ typeof(DateTimeOffset).TypeHandle, SystemType.DateTimeOffset},
				{ typeof(TimeSpan).TypeHandle, SystemType.TimeSpan},
				{ typeof(Guid).TypeHandle, SystemType.Guid},
				{ typeof(Index).TypeHandle, SystemType.Index},
				{ typeof(Range).TypeHandle, SystemType.Range},
				{ typeof(JsonElement).TypeHandle, SystemType.JsonElement},
				{ typeof(string).TypeHandle, SystemType.String},
				{ typeof(Uri).TypeHandle, SystemType.Uri},
				{ typeof(DBNull).TypeHandle, SystemType.DBNull},
				{ typeof(void).TypeHandle, SystemType.Void},
				{ typeof(Span<>).TypeHandle, SystemType.Span},
				{ typeof(Memory<>).TypeHandle, SystemType.Memory},
				{ typeof(ReadOnlySpan<>).TypeHandle, SystemType.ReadOnlySpan},
				{ typeof(ReadOnlyMemory<>).TypeHandle, SystemType.ReadOnlyMemory},
				{ typeof(Lazy<>).TypeHandle, SystemType.Lazy},
				{ typeof(Lazy<,>).TypeHandle, SystemType.Lazy},
				{ typeof(Nullable<>).TypeHandle, SystemType.Nullable},
				{ typeof(Task).TypeHandle, SystemType.Task},
				{ typeof(Task<>).TypeHandle, SystemType.Task},
				{ typeof(ValueTask).TypeHandle, SystemType.ValueTask},
				{ typeof(ValueTask<>).TypeHandle, SystemType.ValueTask},
				{ typeof(HashSet<>).TypeHandle, SystemType.HashSet},
				{ typeof(ImmutableArray<>).TypeHandle, SystemType.ImmutableArray},
				{ typeof(ImmutableDictionary<,>).TypeHandle, SystemType.ImmutableDictionary},
				{ typeof(ImmutableHashSet<>).TypeHandle, SystemType.ImmutableHashSet},
				{ typeof(ImmutableList<>).TypeHandle, SystemType.ImmutableList},
				{ typeof(ImmutableQueue<>).TypeHandle, SystemType.ImmutableQueue},
				{ typeof(ImmutableSortedDictionary<,>).TypeHandle, SystemType.ImmutableSortedDictionary},
				{ typeof(ImmutableSortedSet<>).TypeHandle, SystemType.ImmutableSortedSet},
				{ typeof(ImmutableStack<>).TypeHandle, SystemType.ImmutableStack},
				{ typeof(LinkedList<>).TypeHandle, SystemType.LinkedList},
				{ typeof(List<>).TypeHandle, SystemType.List},
				{ typeof(Queue<>).TypeHandle, SystemType.Queue},
				{ typeof(SortedDictionary<,>).TypeHandle, SystemType.SortedDictionary},
				{ typeof(SortedList<,>).TypeHandle, SystemType.SortedList},
				{ typeof(SortedSet<>).TypeHandle, SystemType.SortedSet},
				{ typeof(Stack<>).TypeHandle, SystemType.Stack},
				{ typeof(Tuple).TypeHandle, SystemType.Tuple},
				{ typeof(Tuple<>).TypeHandle, SystemType.Tuple},
				{ typeof(Tuple<,>).TypeHandle, SystemType.Tuple},
				{ typeof(Tuple<,,>).TypeHandle, SystemType.Tuple},
				{ typeof(Tuple<,,,>).TypeHandle, SystemType.Tuple},
				{ typeof(Tuple<,,,,>).TypeHandle, SystemType.Tuple},
				{ typeof(Tuple<,,,,,>).TypeHandle, SystemType.Tuple},
				{ typeof(Tuple<,,,,,,>).TypeHandle, SystemType.Tuple},
				{ typeof(Tuple<,,,,,,,>).TypeHandle, SystemType.Tuple},
				{ typeof(ValueTuple).TypeHandle, SystemType.ValueTuple},
				{ typeof(ValueTuple<>).TypeHandle, SystemType.ValueTuple},
				{ typeof(ValueTuple<,>).TypeHandle, SystemType.ValueTuple},
				{ typeof(ValueTuple<,,>).TypeHandle, SystemType.ValueTuple},
				{ typeof(ValueTuple<,,,>).TypeHandle, SystemType.ValueTuple},
				{ typeof(ValueTuple<,,,,>).TypeHandle, SystemType.ValueTuple},
				{ typeof(ValueTuple<,,,,,>).TypeHandle, SystemType.ValueTuple},
				{ typeof(ValueTuple<,,,,,,>).TypeHandle, SystemType.ValueTuple},
				{ typeof(ValueTuple<,,,,,,,>).TypeHandle, SystemType.ValueTuple},
				{ typeof(Action).TypeHandle, SystemType.Action},
				{ typeof(Action<>).TypeHandle, SystemType.Action},
				{ typeof(Action<,>).TypeHandle, SystemType.Action},
				{ typeof(Action<,,>).TypeHandle, SystemType.Action},
				{ typeof(Action<,,,>).TypeHandle, SystemType.Action},
				{ typeof(Action<,,,,>).TypeHandle, SystemType.Action},
				{ typeof(Action<,,,,,>).TypeHandle, SystemType.Action},
				{ typeof(Action<,,,,,,>).TypeHandle, SystemType.Action},
				{ typeof(Action<,,,,,,,>).TypeHandle, SystemType.Action},
				{ typeof(Action<,,,,,,,,>).TypeHandle, SystemType.Action},
				{ typeof(Action<,,,,,,,,,>).TypeHandle, SystemType.Action},
				{ typeof(Action<,,,,,,,,,,>).TypeHandle, SystemType.Action},
				{ typeof(Action<,,,,,,,,,,,>).TypeHandle, SystemType.Action},
				{ typeof(Action<,,,,,,,,,,,,>).TypeHandle, SystemType.Action},
				{ typeof(Action<,,,,,,,,,,,,,>).TypeHandle, SystemType.Action},
				{ typeof(Action<,,,,,,,,,,,,,,>).TypeHandle, SystemType.Action},
				{ typeof(Action<,,,,,,,,,,,,,,,>).TypeHandle, SystemType.Action},
				{ typeof(Func<>).TypeHandle, SystemType.Func},
				{ typeof(Func<,>).TypeHandle, SystemType.Func},
				{ typeof(Func<,,>).TypeHandle, SystemType.Func},
				{ typeof(Func<,,,>).TypeHandle, SystemType.Func},
				{ typeof(Func<,,,,>).TypeHandle, SystemType.Func},
				{ typeof(Func<,,,,,>).TypeHandle, SystemType.Func},
				{ typeof(Func<,,,,,,>).TypeHandle, SystemType.Func},
				{ typeof(Func<,,,,,,,>).TypeHandle, SystemType.Func},
				{ typeof(Func<,,,,,,,,>).TypeHandle, SystemType.Func},
				{ typeof(Func<,,,,,,,,,>).TypeHandle, SystemType.Func},
				{ typeof(Func<,,,,,,,,,,>).TypeHandle, SystemType.Func},
				{ typeof(Func<,,,,,,,,,,,>).TypeHandle, SystemType.Func},
				{ typeof(Func<,,,,,,,,,,,,>).TypeHandle, SystemType.Func},
				{ typeof(Func<,,,,,,,,,,,,,>).TypeHandle, SystemType.Func},
				{ typeof(Func<,,,,,,,,,,,,,,>).TypeHandle, SystemType.Func},
				{ typeof(Func<,,,,,,,,,,,,,,,>).TypeHandle, SystemType.Func},
				{ typeof(Func<,,,,,,,,,,,,,,,,>).TypeHandle, SystemType.Func},
				{ typeof(WeakReference).TypeHandle, SystemType.WeakReference },
				{ typeof(WeakReference<>).TypeHandle, SystemType.WeakReference },
				{ typeof(Type).TypeHandle, SystemType.Type },
			}.ToImmutableDictionary();
		}

		private static readonly IImmutableDictionary<RuntimeTypeHandle, SystemType> SystemTypes;

		/// <summary>
		/// <c><paramref name="types"/>.Any(@<paramref name="this"/>.Is)</c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Any(this Type? @this, params Type[] types)
			=> types.Any(@this.Is!);

		public static Kind GetKind(this Type @this)
			=> @this switch
			{
				_ when @this.IsPointer => Kind.Pointer,
				_ when typeof(Delegate).IsAssignableFrom(@this.BaseType) => Kind.Delegate,
				_ when @this.IsEnum => Kind.Enum,
				_ when @this != typeof(string) && @this.IsEnumerable() => Kind.Collection,
				_ when @this.IsInterface => Kind.Interface,
				_ when @this.IsValueType => Kind.Struct,
				_ => Kind.Class,
			};

		public static SystemType GetSystemType(this Type @this)
			=> @this switch
			{
				_ when SystemTypes.TryGetValue(@this.ToGenericType()?.TypeHandle ?? @this.TypeHandle, out var systemType) => systemType,
				_ when @this.IsEnum => @this.GetEnumUnderlyingType().GetSystemType(),
				_ when @this.IsArray => SystemType.Array,
				_ when @this.IsEnumerable() => SystemType.Enumerable,
				_ => SystemType.Unknown
			};

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
		/// <c>@<paramref name="this"/>.Is&lt;<see cref="IEnumerable"/>&gt;() || @<paramref name="this"/>.Implements&lt;<see cref="IEnumerable"/>&gt;()</c>
		/// </summary>
		public static bool IsEnumerable(this Type @this)
			=> @this.Is<IEnumerable>() || @this.Implements<IEnumerable>();

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
	}
}
