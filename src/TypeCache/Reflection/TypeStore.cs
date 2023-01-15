// Copyright (c) 2021 Samuel Abraham

using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Data;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using TypeCache.Collections;
using TypeCache.Extensions;

namespace TypeCache.Reflection;

internal static class TypeStore
{
	static TypeStore()
	{
		DefaultValueTypeConstructorInvokes = new LazyDictionary<RuntimeTypeHandle, Func<object>>(handle =>
			handle.ToType().ToNewExpression().As<object>().Lambda<Func<object>>().Compile());
		FieldGetInvokes = new();
		FieldSetInvokes = new();
		MethodInvokes = new LazyDictionary<(RuntimeTypeHandle TypeHandle, RuntimeMethodHandle MethodHandle), Func<object?[]?, object?>>(_ =>
			_.MethodHandle.ToMethodBase(_.TypeHandle) switch
			{
				MethodInfo methodInfo => methodInfo.ToInvokeLambdaExpression().Compile(),
				ConstructorInfo constructorInfo => constructorInfo.ToInvokeLambdaExpression().Compile(),
				_ => throw new UnreachableException("Method or Constructor not found.")
			});
		ObjectTypes = new LazyDictionary<RuntimeTypeHandle, ObjectType>(handle => handle.ToType() switch
		{
			var type when type == typeof(object) => ObjectType.Object,
			var type when type == typeof(string) => ObjectType.String,
			var type when type == typeof(StringBuilder) => ObjectType.StringBuilder,
			{ IsPrimitive: true } => ObjectType.Primitive,
			{ IsArray: true } => ObjectType.Array,
			{ IsEnum: true } => ObjectType.Enum,
			{ IsClass: true } type when type.IsAssignableTo<Attribute>() => ObjectType.Attribute,
			{ IsClass: true } type when type.IsAssignableTo<DataColumn>() => ObjectType.DataColumn,
			{ IsClass: true } type when type.IsAssignableTo<DataRow>() => ObjectType.DataRow,
			{ IsClass: true } type when type.IsAssignableTo<DataRowView>() => ObjectType.DataRowView,
			{ IsClass: true } type when type.IsAssignableTo<DataSet>() => ObjectType.DataSet,
			{ IsClass: true } type when type.IsAssignableTo<DataTable>() => ObjectType.DataTable,
			{ IsClass: true } type when type.IsAssignableTo<Delegate>() => ObjectType.Delegate,
			{ IsClass: true } type when type.IsAssignableTo<Exception>() => ObjectType.Exception,
			{ IsClass: true } type when type.IsAssignableTo<JsonNode>() => ObjectType.JsonNode,
			{ IsClass: true } type when type.IsAssignableTo<OrderedDictionary>() => ObjectType.OrderedDictionary,
			{ IsClass: true } type when type.IsAssignableTo<Stream>() => ObjectType.Stream,
			var type when type.IsAssignableTo<IAsyncResult>() => ObjectType.AsyncResult,
			var type when type.IsGenericType || type.IsGenericTypeDefinition => type.ToGenericType()! switch
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
		});
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

	public static IReadOnlyDictionary<RuntimeTypeHandle, Func<object>> DefaultValueTypeConstructorInvokes { get; }

	public static ConcurrentDictionary<RuntimeFieldHandle, Func<object?, object?>> FieldGetInvokes { get; }

	public static ConcurrentDictionary<RuntimeFieldHandle, Action<object?, object?>> FieldSetInvokes { get; }

	public static IReadOnlyDictionary<(RuntimeTypeHandle, RuntimeMethodHandle), Func<object?[]?, object?>> MethodInvokes { get; }

	public static IReadOnlyDictionary<RuntimeTypeHandle, ObjectType> ObjectTypes { get; }

	public static IReadOnlyDictionary<RuntimeTypeHandle, SystemType> SystemTypes { get; }
}
