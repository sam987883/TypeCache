// Copyright (c) 2021 Samuel Abraham

using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Numerics;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using TypeCache.Attributes;
using TypeCache.Extensions;
using TypeCache.Reflection;

namespace TypeCache.Extensions;

public static class TypeExtensions
{
	private const char GENERIC_TICKMARK = '`';

	private static readonly IReadOnlyDictionary<RuntimeTypeHandle, SystemType> SystemTypes;

	static TypeExtensions()
	{
		SystemTypes = new Dictionary<RuntimeTypeHandle, SystemType>(141)
		{
			{ typeof(Action).TypeHandle, SystemType.Action },
			{ typeof(Action<>).TypeHandle, SystemType.Action },
			{ typeof(Action<,>).TypeHandle, SystemType.Action },
			{ typeof(Action<,,>).TypeHandle, SystemType.Action },
			{ typeof(Action<,,,>).TypeHandle, SystemType.Action },
			{ typeof(Action<,,,,>).TypeHandle, SystemType.Action },
			{ typeof(Action<,,,,,>).TypeHandle, SystemType.Action },
			{ typeof(Action<,,,,,,>).TypeHandle, SystemType.Action },
			{ typeof(Action<,,,,,,,>).TypeHandle, SystemType.Action },
			{ typeof(Action<,,,,,,,,>).TypeHandle, SystemType.Action },
			{ typeof(Action<,,,,,,,,,>).TypeHandle, SystemType.Action },
			{ typeof(Action<,,,,,,,,,,>).TypeHandle, SystemType.Action },
			{ typeof(Action<,,,,,,,,,,,>).TypeHandle, SystemType.Action },
			{ typeof(Action<,,,,,,,,,,,,>).TypeHandle, SystemType.Action },
			{ typeof(Action<,,,,,,,,,,,,,>).TypeHandle, SystemType.Action },
			{ typeof(Action<,,,,,,,,,,,,,,>).TypeHandle, SystemType.Action },
			{ typeof(Action<,,,,,,,,,,,,,,,>).TypeHandle, SystemType.Action },
			{ typeof(bool).TypeHandle, SystemType.Boolean },
			{ typeof(byte).TypeHandle, SystemType.Byte },
			{ typeof(char).TypeHandle, SystemType.Char },
			{ typeof(DateOnly).TypeHandle, SystemType.DateOnly },
			{ typeof(DateTime).TypeHandle, SystemType.DateTime },
			{ typeof(DateTimeOffset).TypeHandle, SystemType.DateTimeOffset },
			{ typeof(DBNull).TypeHandle, SystemType.DBNull },
			{ typeof(decimal).TypeHandle, SystemType.Decimal },
			{ typeof(double).TypeHandle, SystemType.Double },
			{ typeof(Func<>).TypeHandle, SystemType.Func },
			{ typeof(Func<,>).TypeHandle, SystemType.Func },
			{ typeof(Func<,,>).TypeHandle, SystemType.Func },
			{ typeof(Func<,,,>).TypeHandle, SystemType.Func },
			{ typeof(Func<,,,,>).TypeHandle, SystemType.Func },
			{ typeof(Func<,,,,,>).TypeHandle, SystemType.Func },
			{ typeof(Func<,,,,,,>).TypeHandle, SystemType.Func },
			{ typeof(Func<,,,,,,,>).TypeHandle, SystemType.Func },
			{ typeof(Func<,,,,,,,,>).TypeHandle, SystemType.Func },
			{ typeof(Func<,,,,,,,,,>).TypeHandle, SystemType.Func },
			{ typeof(Func<,,,,,,,,,,>).TypeHandle, SystemType.Func },
			{ typeof(Func<,,,,,,,,,,,>).TypeHandle, SystemType.Func },
			{ typeof(Func<,,,,,,,,,,,,>).TypeHandle, SystemType.Func },
			{ typeof(Func<,,,,,,,,,,,,,>).TypeHandle, SystemType.Func },
			{ typeof(Func<,,,,,,,,,,,,,,>).TypeHandle, SystemType.Func },
			{ typeof(Func<,,,,,,,,,,,,,,,>).TypeHandle, SystemType.Func },
			{ typeof(Func<,,,,,,,,,,,,,,,,>).TypeHandle, SystemType.Func },
			{ typeof(Guid).TypeHandle, SystemType.Guid },
			{ typeof(Half).TypeHandle, SystemType.Half },
			{ typeof(Index).TypeHandle, SystemType.Index },
			{ typeof(Int128).TypeHandle, SystemType.Int128 },
			{ typeof(short).TypeHandle, SystemType.Int16 },
			{ typeof(int).TypeHandle, SystemType.Int32 },
			{ typeof(long).TypeHandle, SystemType.Int64 },
			{ typeof(IntPtr).TypeHandle, SystemType.IntPtr },
			{ typeof(Lazy<>).TypeHandle, SystemType.Lazy },
			{ typeof(Lazy<,>).TypeHandle, SystemType.Lazy },
			{ typeof(Memory<>).TypeHandle, SystemType.Memory },
			{ typeof(Nullable<>).TypeHandle, SystemType.Nullable },
			{ typeof(object).TypeHandle, SystemType.Object },
			{ typeof(Range).TypeHandle, SystemType.Range },
			{ typeof(ReadOnlyMemory<>).TypeHandle, SystemType.ReadOnlyMemory },
			{ typeof(ReadOnlySpan<>).TypeHandle, SystemType.ReadOnlySpan },
			{ typeof(sbyte).TypeHandle, SystemType.SByte },
			{ typeof(float).TypeHandle, SystemType.Single },
			{ typeof(Span<>).TypeHandle, SystemType.Span },
			{ typeof(string).TypeHandle, SystemType.String },
			{ typeof(TimeOnly).TypeHandle, SystemType.TimeOnly },
			{ typeof(TimeSpan).TypeHandle, SystemType.TimeSpan },
			{ typeof(Tuple).TypeHandle, SystemType.Tuple },
			{ typeof(Tuple<>).TypeHandle, SystemType.Tuple },
			{ typeof(Tuple<,>).TypeHandle, SystemType.Tuple },
			{ typeof(Tuple<,,>).TypeHandle, SystemType.Tuple },
			{ typeof(Tuple<,,,>).TypeHandle, SystemType.Tuple },
			{ typeof(Tuple<,,,,>).TypeHandle, SystemType.Tuple },
			{ typeof(Tuple<,,,,,>).TypeHandle, SystemType.Tuple },
			{ typeof(Tuple<,,,,,,>).TypeHandle, SystemType.Tuple },
			{ typeof(Tuple<,,,,,,,>).TypeHandle, SystemType.Tuple },
			{ typeof(Type).TypeHandle, SystemType.Type },
			{ typeof(UInt128).TypeHandle, SystemType.UInt128 },
			{ typeof(ushort).TypeHandle, SystemType.UInt16 },
			{ typeof(uint).TypeHandle, SystemType.UInt32 },
			{ typeof(ulong).TypeHandle, SystemType.UInt64 },
			{ typeof(UIntPtr).TypeHandle, SystemType.UIntPtr },
			{ typeof(Uri).TypeHandle, SystemType.Uri },
			{ typeof(ValueTuple).TypeHandle, SystemType.ValueTuple },
			{ typeof(ValueTuple<>).TypeHandle, SystemType.ValueTuple },
			{ typeof(ValueTuple<,>).TypeHandle, SystemType.ValueTuple },
			{ typeof(ValueTuple<,,>).TypeHandle, SystemType.ValueTuple },
			{ typeof(ValueTuple<,,,>).TypeHandle, SystemType.ValueTuple },
			{ typeof(ValueTuple<,,,,>).TypeHandle, SystemType.ValueTuple },
			{ typeof(ValueTuple<,,,,,>).TypeHandle, SystemType.ValueTuple },
			{ typeof(ValueTuple<,,,,,,>).TypeHandle, SystemType.ValueTuple },
			{ typeof(ValueTuple<,,,,,,,>).TypeHandle, SystemType.ValueTuple },
			{ typeof(void).TypeHandle, SystemType.Void },
			{ typeof(WeakReference).TypeHandle, SystemType.WeakReference },
			{ typeof(WeakReference<>).TypeHandle, SystemType.WeakReference },
			{ typeof(ArrayList).TypeHandle, SystemType.ArrayList },
			{ typeof(BitArray).TypeHandle, SystemType.BitArray },
			{ typeof(BlockingCollection<>).TypeHandle, SystemType.BlockingCollection },
			{ typeof(Collection<>).TypeHandle, SystemType.Collection },
			{ typeof(ConcurrentBag<>).TypeHandle, SystemType.ConcurrentBag },
			{ typeof(ConcurrentDictionary<,>).TypeHandle, SystemType.ConcurrentDictionary },
			{ typeof(ConcurrentQueue<>).TypeHandle, SystemType.ConcurrentQueue },
			{ typeof(Dictionary<,>).TypeHandle, SystemType.Dictionary },
			{ typeof(HashSet<>).TypeHandle, SystemType.HashSet },
			{ typeof(Hashtable).TypeHandle, SystemType.Hashtable },
			{ typeof(HybridDictionary).TypeHandle, SystemType.HybridDictionary },
			{ typeof(ImmutableArray<>).TypeHandle, SystemType.ImmutableArray },
			{ typeof(ImmutableDictionary<,>).TypeHandle, SystemType.ImmutableDictionary },
			{ typeof(ImmutableHashSet<>).TypeHandle, SystemType.ImmutableHashSet },
			{ typeof(ImmutableList<>).TypeHandle, SystemType.ImmutableList },
			{ typeof(ImmutableQueue<>).TypeHandle, SystemType.ImmutableQueue },
			{ typeof(ImmutableSortedDictionary<,>).TypeHandle, SystemType.ImmutableSortedDictionary },
			{ typeof(ImmutableSortedSet<>).TypeHandle, SystemType.ImmutableSortedSet },
			{ typeof(ImmutableStack<>).TypeHandle, SystemType.ImmutableStack },
			{ typeof(KeyedCollection<,>).TypeHandle, SystemType.KeyedCollection },
			{ typeof(LinkedList<>).TypeHandle, SystemType.LinkedList },
			{ typeof(List<>).TypeHandle, SystemType.List },
			{ typeof(ListDictionary).TypeHandle, SystemType.ListDictionary },
			{ typeof(NameObjectCollectionBase).TypeHandle, SystemType.NameObjectCollectionBase },
			{ typeof(NameValueCollection).TypeHandle, SystemType.NameValueCollection },
			{ typeof(ObservableCollection<>).TypeHandle, SystemType.ObservableCollection },
			{ typeof(OrderedDictionary).TypeHandle, SystemType.OrderedDictionary },
			{ typeof(PriorityQueue<,>).TypeHandle, SystemType.PriorityQueue },
			{ typeof(Queue<>).TypeHandle, SystemType.Queue },
			{ typeof(ReadOnlyCollection<>).TypeHandle, SystemType.ReadOnlyCollection },
			{ typeof(ReadOnlyDictionary<,>).TypeHandle, SystemType.ReadOnlyDictionary },
			{ typeof(ReadOnlyObservableCollection<>).TypeHandle, SystemType.ReadOnlyObservableCollection },
			{ typeof(SortedDictionary<,>).TypeHandle, SystemType.SortedDictionary },
			{ typeof(SortedList<,>).TypeHandle, SystemType.SortedList },
			{ typeof(SortedSet<>).TypeHandle, SystemType.SortedSet },
			{ typeof(Stack<>).TypeHandle, SystemType.Stack },
			{ typeof(StringCollection).TypeHandle, SystemType.StringCollection },
			{ typeof(StringDictionary).TypeHandle, SystemType.StringDictionary },
			{ typeof(BigInteger).TypeHandle, SystemType.BigInteger },
			{ typeof(JsonDocument).TypeHandle, SystemType.JsonDocument },
			{ typeof(JsonElement).TypeHandle, SystemType.JsonElement },
			{ typeof(JsonArray).TypeHandle, SystemType.JsonArray },
			{ typeof(JsonObject).TypeHandle, SystemType.JsonObject },
			{ typeof(JsonValue).TypeHandle, SystemType.JsonValue },
			{ typeof(Task).TypeHandle, SystemType.Task },
			{ typeof(Task<>).TypeHandle, SystemType.Task },
			{ typeof(ValueTask).TypeHandle, SystemType.ValueTask },
			{ typeof(ValueTask<>).TypeHandle, SystemType.ValueTask },
		}.ToImmutableDictionary();
	}

	public static Kind GetKind(this Type @this)
		=> @this switch
		{
			{ IsPointer: true } => Kind.Pointer,
			{ IsInterface: true } => Kind.Interface,
			{ IsClass: true } => Kind.Class,
			{ IsValueType: true } => Kind.Struct,
			_ => throw new UnreachableException(Invariant($"{nameof(Type)} [{@this?.Name ?? "null"}] is not supported."))
		};

	internal static ObjectType GetObjectType(this Type @this)
		=> @this switch
		{
			{ IsArray: true } => ObjectType.Array,
			{ IsEnum: true } => ObjectType.Enum,
			_ when @this.Implements<IAsyncResult>() => ObjectType.AsyncResult,
			{ IsClass: true } when @this.IsAssignableTo(typeof(Attribute)) => ObjectType.Attribute,
			{ IsClass: true } when @this.IsAssignableTo(typeof(Delegate)) => ObjectType.Delegate,
			{ IsClass: true } when @this.IsAssignableTo(typeof(Exception)) => ObjectType.Exception,
			{ IsClass: true } when @this.IsAssignableTo(typeof(JsonNode)) => ObjectType.JsonNode,
			{ IsClass: true } when @this.IsAssignableTo(typeof(OrderedDictionary)) => ObjectType.OrderedDictionary,
			{ IsClass: true } when @this.IsAssignableTo(typeof(Stream)) => ObjectType.Stream,
			{ IsGenericType: true } or { IsGenericTypeDefinition: true } => @this.ToGenericType()! switch
			{
				var genericType when genericType.IsOrImplements(typeof(IImmutableDictionary<,>)) => ObjectType.ImmutableDictionary,
				var genericType when genericType.IsOrImplements(typeof(IImmutableSet<>)) => ObjectType.ImmutableSet,
				var genericType when genericType.IsOrImplements(typeof(IImmutableList<>)) => ObjectType.ImmutableList,
				var genericType when genericType.IsOrImplements(typeof(IImmutableQueue<>)) => ObjectType.ImmutableQueue,
				var genericType when genericType.IsOrImplements(typeof(IImmutableStack<>)) => ObjectType.ImmutableStack,
				var genericType when genericType.IsOrImplements(typeof(IReadOnlyDictionary<,>)) => ObjectType.ReadOnlyDictionary,
				var genericType when genericType.IsOrImplements(typeof(IReadOnlySet<>)) => ObjectType.ReadOnlySet,
				var genericType when genericType.IsOrImplements(typeof(IReadOnlyList<>)) => ObjectType.ReadOnlyList,
				var genericType when genericType.IsOrImplements(typeof(IReadOnlyCollection<>)) => ObjectType.ReadOnlyCollection,
				var genericType when genericType.IsOrImplements(typeof(IDictionary<,>)) => ObjectType.Dictionary,
				var genericType when genericType.IsOrImplements(typeof(ISet<>)) => ObjectType.Set,
				var genericType when genericType.IsOrImplements(typeof(IAsyncEnumerable<>)) => ObjectType.AsyncEnumerable,
				var genericType when genericType.IsOrImplements(typeof(IAsyncEnumerator<>)) => ObjectType.AsyncEnumerator,
				var genericType when genericType.IsOrImplements(typeof(IList<>)) => ObjectType.List,
				var genericType when genericType.IsOrImplements(typeof(ICollection<>)) => ObjectType.Collection,
				var genericType when genericType.IsOrImplements(typeof(IEnumerable<>)) => ObjectType.Enumerable,
				var genericType when genericType.IsOrImplements(typeof(IEnumerator<>)) => ObjectType.Enumerator,
				var genericType when genericType.IsOrImplements(typeof(IObservable<>)) => ObjectType.Observable,
				var genericType when genericType.IsOrImplements(typeof(IObserver<>)) => ObjectType.Observer,
				_ => ObjectType.Unknown
			},
			_ => ObjectType.Unknown
		};

	/// <summary>
	/// <c>=&gt; <paramref name="types"/>.Any(@<paramref name="this"/>.Is);</c>
	/// </summary>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool IsAny(this Type @this, params Type[] types)
		=> types.Any(@this.Is);

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.GetSystemType().IsEnumUnderlyingType();</c>
	/// </summary>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool IsEnumUnderlyingType(this Type @this)
		=> @this.GetSystemType().IsEnumUnderlyingType();

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.GetCustomAttribute&lt;<see cref="NameAttribute"/>&gt;()?.Name ?? @<paramref name="this"/>.Name.Left(@<paramref name="this"/>.Name.IndexOf('`'));</c>
	/// </summary>
	[DebuggerHidden]
	public static string Name(this MemberInfo @this)
		=> @this.GetCustomAttribute<NameAttribute>()?.Name ?? @this.Name.Left(@this.Name.IndexOf(GENERIC_TICKMARK));

	[DebuggerHidden]
	public static SystemType GetSystemType(this Type @this)
		=> @this switch
		{
			{ IsArray: true } => SystemType.Array,
			{ IsEnum: true } => SystemTypes[@this.GetEnumUnderlyingType().TypeHandle],
			_ when SystemTypes.TryGetValue(@this.ToGenericType()?.TypeHandle ?? @this.TypeHandle, out var systemType) => systemType,
			_ => SystemType.None
		};

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.TypeHandle.GetTypeMember();<br/></c>
	/// </summary>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static TypeMember GetTypeMember(this Type @this)
		=> @this.TypeHandle.GetTypeMember();

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.Implements(<see langword="typeof"/>(<typeparamref name="T"/>));</c>
	/// </summary>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool Implements<T>(this Type @this)
		=> @this.Implements(typeof(T));

	public static bool Implements(this Type @this, Type type)
	{
		if (type.IsInterface)
			return @this.GetInterfaces().Any(type.Is);

		var baseType = @this.BaseType;
		while (baseType is not null)
		{
			if (baseType.Is(type))
				return true;
			baseType = baseType.BaseType;
		}

		return false;
	}

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/> == <see langword="typeof"/>(<typeparamref name="T"/>);</c>
	/// </summary>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool Is<T>(this Type? @this)
		=> @this == typeof(T);

	[DebuggerHidden]
	public static bool Is(this Type @this, Type? type)
		=> @this.IsGenericTypeDefinition || type?.IsGenericTypeDefinition is true ? @this.ToGenericType() == type.ToGenericType() : @this == type;

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.IsClass || @<paramref name="this"/>.IsPointer || <see langword="typeof"/>(Nullable<>) == @<paramref name="this"/>.ToGenericType();</c>
	/// </summary>
	[DebuggerHidden]
	public static bool IsNullable(this Type @this, Type? type)
		=> @this.IsClass || @this.IsPointer || typeof(Nullable<>) == @this.ToGenericType();

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.Is&lt;<typeparamref name="T"/>&gt;() || @<paramref name="this"/>.Implements&lt;<typeparamref name="T"/>&gt;();</c>
	/// </summary>
	[DebuggerHidden]
	public static bool IsOrImplements<T>(this Type @this)
		=> @this.Is<T>() || @this.Implements<T>();

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.Is(<paramref name="type"/>) || @<paramref name="this"/>.Implements(<paramref name="type"/>);</c>
	/// </summary>
	[DebuggerHidden]
	public static bool IsOrImplements(this Type @this, Type type)
		=> @this.Is(type) || @this.Implements(type);

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.Is&lt;<see cref="IEnumerable{T}"/>&gt;() || @<paramref name="this"/>.Implements&lt;<see cref="IEnumerable{T}"/>&gt;();</c>
	/// </summary>
	[DebuggerHidden]
	public static bool IsEnumerableOf<T>(this Type @this)
		=> @this.Is<IEnumerable<T>>() || @this.Implements<IEnumerable<T>>();

	[DebuggerHidden]
	public static Type? ToGenericType(this Type? @this)
		=> @this switch
		{
			{ IsGenericTypeDefinition: true } => @this,
			{ IsGenericType: true } => @this.GetGenericTypeDefinition(),
			_ => null
		};

	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.GetParameters().All(_ =&gt; !_.IsOut &amp;&amp; _.ParameterType.IsInvokable());</c>
	/// </remarks>
	[DebuggerHidden]
	private static bool IsInvokable(this MethodBase @this)
		=> @this.GetParameters().All(_ => !_.IsOut && _.ParameterType.IsInvokable());

	/// <remarks>
	/// <c>=&gt; ((<see cref="MethodBase"/>)@<paramref name="this"/>).IsInvokable();</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	internal static bool IsInvokable(this ConstructorInfo @this)
		=> ((MethodBase)@this).IsInvokable();

	/// <remarks>
	/// <c>=&gt; ((<see cref="MethodBase"/>)@<paramref name="this"/>).IsInvokable() &amp;&amp; @<paramref name="this"/>.ReturnType.IsInvokable();</c>
	/// </remarks>
	[DebuggerHidden]
	internal static bool IsInvokable(this MethodInfo @this)
		=> ((MethodBase)@this).IsInvokable() && @this.ReturnType.IsInvokable();

	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.IsPointer &amp;&amp; !@<paramref name="this"/>.IsByRef &amp;&amp; !@<paramref name="this"/>.IsByRefLike;</c>
	/// </remarks>
	[DebuggerHidden]
	internal static bool IsInvokable(this Type @this)
		=> !@this.IsPointer && !@this.IsByRef && !@this.IsByRefLike;
}
