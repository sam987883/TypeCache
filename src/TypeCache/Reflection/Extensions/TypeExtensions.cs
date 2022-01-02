// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Numerics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Tasks;
using TypeCache.Collections.Extensions;
using TypeCache.Extensions;
using static TypeCache.Default;

namespace TypeCache.Reflection.Extensions;

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
			{ typeof(DateOnly).TypeHandle, SystemType.DateOnly},
			{ typeof(DateTime).TypeHandle, SystemType.DateTime},
			{ typeof(DateTimeOffset).TypeHandle, SystemType.DateTimeOffset},
			{ typeof(TimeOnly).TypeHandle, SystemType.TimeOnly},
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
	/// <c>=&gt; <paramref name="types"/>.Any(@<paramref name="this"/>.Is);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static bool Any(this Type? @this, params Type[] types)
		=> types.Any(@this.Is);

	/// <summary>
	/// <code>
	/// =&gt; @<paramref name="this"/> <see langword="switch"/><br/>
	/// {<br/>
	/// <see langword="    "/>_ <see langword="when"/> @<paramref name="this"/> == <see langword="typeof"/>(<see cref="string"/>) =&gt; <see cref="Kind.Class"/>,<br/>
	/// <see langword="    "/>{ IsPointer: <see langword="true"/> } =&gt; <see cref="Kind.Pointer"/>,<br/>
	/// <see langword="    "/>{ IsEnum: <see langword="true"/> } =&gt; <see cref="Kind.Enum"/>,<br/>
	/// <see langword="    "/>{ IsArray: <see langword="true"/> } =&gt; <see cref="Kind.Collection"/>,<br/>
	/// <see langword="    "/>_ <see langword="when"/> <see langword="typeof"/>(<see cref="Delegate"/>).IsAssignableFrom(@<paramref name="this"/>.BaseType) =&gt; <see cref="Kind.Delegate"/>,<br/>
	/// <see langword="    "/>_ <see langword="when"/> @<paramref name="this"/>.IsEnumerable() =&gt; <see cref="Kind.Collection"/>,<br/>
	/// <see langword="    "/>{ IsInterface: <see langword="true"/> } =&gt; <see cref="Kind.Interface"/>,<br/>
	/// <see langword="    "/>{ IsValueType: <see langword="true"/> } =&gt; <see cref="Kind.Struct"/>,<br/>
	/// <see langword="    "/>_ =&gt; <see cref="Kind.Class"/><br/>
	/// };
	/// </code>
	/// </summary>
	public static Kind GetKind(this Type @this)
		=> @this switch
		{
			_ when @this == typeof(string) => Kind.Class,
			{ IsPointer: true } => Kind.Pointer,
			{ IsEnum: true } => Kind.Enum,
			{ IsArray: true } => Kind.Collection,
			_ when typeof(Delegate).IsAssignableFrom(@this.BaseType) => Kind.Delegate,
			_ when @this.IsEnumerable() => Kind.Collection,
			{ IsInterface: true } => Kind.Interface,
			{ IsValueType: true } => Kind.Struct,
			_ => Kind.Class,
		};

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.GetCustomAttribute&lt;<see cref="NameAttribute"/>&gt;()?.Name
	///		?? (@<paramref name="this"/>.Name.Contains('`') ? @<paramref name="this"/>.Name.Left(@<paramref name="this"/>.Name.IndexOf('`')) : @<paramref name="this"/>.Name);</c>
	/// </summary>
	public static string GetName(this MemberInfo @this)
		=> @this.GetCustomAttribute<NameAttribute>()?.Name ?? (@this.Name.Contains('`') ? @this.Name.Left(@this.Name.IndexOf('`')) : @this.Name);

	/// <summary>
	/// <code>
	/// =&gt; @<paramref name="this"/> <see langword="switch"/><br/>
	/// {<br/>
	/// <see langword="    "/>_ <see langword="when"/> <see cref="SystemTypes"/>.TryGetValue(@<paramref name="this"/>.ToGenericType()?.TypeHandle ?? @<paramref name="this"/>.TypeHandle, <see langword="out var"/> systemType) =&gt; systemType,<br/>
	/// <see langword="    "/>{ IsEnum: <see langword="true"/> } =&gt; @<paramref name="this"/>.GetEnumUnderlyingType().GetSystemType(),<br/>
	/// <see langword="    "/>{ IsArray: <see langword="true"/> } =&gt; <see cref="SystemType.Array"/>,<br/>
	/// <see langword="    "/>_ <see langword="when"/> @<paramref name="this"/>.IsEnumerable() =&gt; <see cref="SystemType.Enumerable"/>,<br/>
	/// <see langword="    "/>_ =&gt; <see cref="SystemType.Unknown"/><br/>
	/// };
	/// </code>
	/// </summary>
	public static SystemType GetSystemType(this Type @this)
		=> @this switch
		{
			_ when SystemTypes.TryGetValue(@this.ToGenericType()?.TypeHandle ?? @this.TypeHandle, out var systemType) => systemType,
			{ IsEnum: true } => @this.GetEnumUnderlyingType().GetSystemType(),
			{ IsArray: true } => SystemType.Array,
			_ when @this.IsEnumerable() => SystemType.Enumerable,
			_ => SystemType.Unknown
		};

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.Implements(<see langword="typeof"/>(<typeparamref name="T"/>));</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static bool Implements<T>(this Type @this)
		=> @this.Implements(typeof(T));

	/// <summary>
	/// <code>
	/// <see langword="if"/> (<paramref name="type"/>.IsInterface)<br/>
	/// {<br/>
	/// <see langword="    return"/> <paramref name="type"/>.IsGenericTypeDefinition<br/>
	/// <see langword="        "/>? @<paramref name="this"/>.GetInterfaces().Any(_ =&gt; _.ToGenericType()?.TypeHandle.Equals(<paramref name="type"/>.TypeHandle) <see langword="is true"/>)<br/>
	/// <see langword="        "/>: @<paramref name="this"/>.GetInterfaces().Any(_ =&gt; _.TypeHandle.Equals(<paramref name="type"/>.TypeHandle));<br/>
	/// }<br/>
	/// <see langword="else if"/> (<paramref name="type"/>.IsGenericTypeDefinition)<br/>
	/// {<br/>
	/// <see langword="    var"/> baseType = @<paramref name="this"/>.BaseType;<br/>
	/// <see langword="    while"/> (baseType <see langword="is not null"/>)<br/>
	/// <see langword="    "/>{<br/>
	/// <see langword="        if"/> (baseType.ToGenericType()?.TypeHandle.Equals(<paramref name="type"/>.TypeHandle) <see langword="is true"/>)<br/>
	/// <see langword="             return true"/>;<br/>
	/// <see langword="        "/>baseType = baseType.BaseType;<br/>
	/// <see langword="    "/>}<br/>
	/// }<br/>
	/// <see langword="else"/><br/>
	/// {<br/>
	/// <see langword="    var"/> baseType = @<paramref name="this"/>.BaseType;<br/>
	/// <see langword="    while"/> (baseType <see langword="is not null"/>)<br/>
	/// <see langword="    "/>{<br/>
	/// <see langword="        if"/> (baseType.TypeHandle.Equals(<paramref name="type"/>.TypeHandle))<br/>
	/// <see langword="             return true"/>;<br/>
	/// <see langword="        "/>baseType = baseType.BaseType;<br/>
	/// <see langword="    "/>}<br/>
	/// }<br/>
	/// <see langword="return false"/>;
	/// </code>
	/// </summary>
	public static bool Implements(this Type @this, Type type)
	{
		if (type.IsInterface)
		{
			return type.IsGenericTypeDefinition
				? @this.GetInterfaces().Any(_ => _.ToGenericType()?.TypeHandle.Equals(type.TypeHandle) is true)
				: @this.GetInterfaces().Any(_ => _.TypeHandle.Equals(type.TypeHandle));
		}
		else if (type.IsGenericTypeDefinition)
		{
			var baseType = @this.BaseType;
			while (baseType is not null)
			{
				if (baseType.ToGenericType()?.TypeHandle.Equals(type.TypeHandle) is true)
					return true;
				baseType = baseType.BaseType;
			}
		}
		else
		{
			var baseType = @this.BaseType;
			while (baseType is not null)
			{
				if (baseType.TypeHandle.Equals(type.TypeHandle))
					return true;
				baseType = baseType.BaseType;
			}
		}
		return false;
	}

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/> == <see langword="typeof"/>(<typeparamref name="T"/>);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static bool Is<T>(this Type? @this)
		=> @this == typeof(T);

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/> == <paramref name="type"/>
	///		|| (<paramref name="type"/>.IsGenericTypeDefinition &amp;&amp; <paramref name="type"/> == @<paramref name="this"/>.ToGenericType());</c>
	/// </summary>
	public static bool Is(this Type? @this, Type type)
		=> @this == type || (type.IsGenericTypeDefinition && type == @this.ToGenericType());

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.Is&lt;<see cref="IEnumerable"/>&gt;() || @<paramref name="this"/>.Implements&lt;<see cref="IEnumerable"/>&gt;();</c>
	/// </summary>
	public static bool IsEnumerable(this Type @this)
		=> @this.Is<IEnumerable>() || @this.Implements<IEnumerable>();

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.Is&lt;<see cref="IEnumerable{T}"/>&gt;() || @<paramref name="this"/>.Implements&lt;<see cref="IEnumerable{T}"/>&gt;();</c>
	/// </summary>
	public static bool IsEnumerableOf<T>(this Type @this)
		=> @this.Is<IEnumerable<T>>() || @this.Implements<IEnumerable<T>>();

	/// <summary>
	/// <code>
	/// =&gt; @<paramref name="this"/> <see langword="switch"/><br/>
	/// {<br/>
	///	<see langword="    null"/> =&gt; <see langword="null"/>,<br/>
	///	<see langword="    "/>_ <see langword="when"/> @<paramref name="this"/>.IsGenericTypeDefinition =&gt; @<paramref name="this"/>,<br/>
	///	<see langword="    "/>_ <see langword="when"/> @<paramref name="this"/>.IsGenericType =&gt; @<paramref name="this"/>.GetGenericTypeDefinition(),<br/>
	///	<see langword="    "/>_ =&gt; <see langword="null"/><br/>
	/// };
	/// </code>
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
