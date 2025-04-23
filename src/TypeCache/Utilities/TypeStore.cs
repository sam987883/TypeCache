// Copyright (c) 2021 Samuel Abraham

using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Frozen;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Data;
using System.Numerics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using TypeCache.Extensions;

namespace TypeCache.Utilities;

public static class TypeStore
{
	static TypeStore()
	{
		CollectionTypes = new LazyDictionary<RuntimeTypeHandle, CollectionType>(handle =>
			handle.ToType() switch
			{
				{ IsArray: true } => CollectionType.Array,
				var type when type.Implements(typeof(ArrayList)) => CollectionType.ArrayList,
				var type when type.Implements(typeof(BitArray)) => CollectionType.BitArray,
				var type when type.Implements(typeof(BlockingCollection<>)) => CollectionType.BlockingCollection,
				var type when type.Implements(typeof(ConcurrentBag<>)) => CollectionType.ConcurrentBag,
				var type when type.Implements(typeof(ConcurrentDictionary<,>)) => CollectionType.ConcurrentDictionary,
				var type when type.Implements(typeof(ConcurrentQueue<>)) => CollectionType.ConcurrentQueue,
				var type when type.Implements(typeof(ConcurrentStack<>)) => CollectionType.ConcurrentStack,
				var type when type.Implements(typeof(FrozenDictionary<,>)) => CollectionType.FrozenDictionary,
				var type when type.Implements(typeof(FrozenSet<>)) => CollectionType.FrozenSet,
				var type when type.Implements(typeof(Hashtable)) => CollectionType.Hashtable,
				var type when type.Implements(typeof(HybridDictionary)) => CollectionType.HybridDictionary,
				var type when type.Implements(typeof(ImmutableArray<>)) => CollectionType.ImmutableArray,
				var type when type.Implements(typeof(ImmutableSortedDictionary<,>)) => CollectionType.ImmutableSortedDictionary,
				var type when type.Implements(typeof(IImmutableDictionary<,>)) => CollectionType.ImmutableDictionary,
				var type when type.Implements(typeof(ImmutableSortedSet<>)) => CollectionType.ImmutableSortedSet,
				var type when type.Implements(typeof(IImmutableSet<>)) => CollectionType.ImmutableSet,
				var type when type.Implements(typeof(IImmutableList<>)) => CollectionType.ImmutableList,
				var type when type.Implements(typeof(IImmutableQueue<>)) => CollectionType.ImmutableQueue,
				var type when type.Implements(typeof(IImmutableStack<>)) => CollectionType.ImmutableStack,
				var type when type.Implements(typeof(KeyedCollection<,>)) => CollectionType.KeyedCollection,
				var type when type.Implements(typeof(LinkedList<>)) => CollectionType.LinkedList,
				var type when type.Implements(typeof(ListDictionary)) => CollectionType.ListDictionary,
				var type when type.Implements(typeof(NameObjectCollectionBase)) => CollectionType.NameObjectCollection,
				var type when type.Implements(typeof(ObservableCollection<>)) => CollectionType.ObservableCollection,
				var type when type.Implements(typeof(IOrderedDictionary)) => CollectionType.OrderedDictionary,
				var type when type.Implements(typeof(PriorityQueue<,>)) => CollectionType.PriorityQueue,
				var type when type.Implements(typeof(Queue<>)) => CollectionType.Queue,
				var type when type.Implements(typeof(Queue)) => CollectionType.Queue,
				var type when type.Implements(typeof(ReadOnlyObservableCollection<>)) => CollectionType.ReadOnlyObservableCollection,
				var type when type.Implements(typeof(ReadOnlyCollection<>)) => CollectionType.ReadOnlyCollection,
				var type when type.Implements(typeof(ReadOnlyCollectionBase)) => CollectionType.ReadOnlyCollection,
				var type when type.Implements(typeof(SortedDictionary<,>)) => CollectionType.SortedDictionary,
				var type when type.Implements(typeof(SortedList<,>)) => CollectionType.SortedList,
				var type when type.Implements(typeof(SortedList)) => CollectionType.SortedList,
				var type when type.Implements(typeof(SortedSet<>)) => CollectionType.SortedSet,
				var type when type.Implements(typeof(Stack<>)) => CollectionType.Stack,
				var type when type.Implements(typeof(Stack)) => CollectionType.Stack,
				var type when type.Implements(typeof(StringCollection)) => CollectionType.StringCollection,
				var type when type.Implements(typeof(Collection<>)) => CollectionType.Collection,
				var type when type.Implements(typeof(CollectionBase)) => CollectionType.Collection,
				var type when type.Implements(typeof(IReadOnlySet<>)) => CollectionType.ReadOnlySet,
				var type when type.Implements(typeof(ISet<>)) => CollectionType.Set,
				var type when type.Implements(typeof(IDictionary<,>)) => CollectionType.Dictionary,
				var type when type.Implements(typeof(IReadOnlyDictionary<,>)) => CollectionType.ReadOnlyDictionary,
				var type when type.Implements(typeof(IList<>)) => CollectionType.List,
				var type when type.Implements(typeof(IReadOnlyList<>)) => CollectionType.ReadOnlyList,
				var type when type.Implements(typeof(IReadOnlyCollection<>)) => CollectionType.ReadOnlyCollection,
				var type when type.Implements(typeof(ICollection<>)) => CollectionType.Collection,
				_ => CollectionType.None
			});
		ConstructorArrayFuncs = new LazyDictionary<(RuntimeTypeHandle TypeHandle, RuntimeMethodHandle MethodHandle), Func<object?[]?, object>>(_ =>
			((ConstructorInfo)_.MethodHandle.ToMethodBase(_.TypeHandle)).ToArrayFuncExpression().Compile());
		ConstructorTupleFuncs = new LazyDictionary<(RuntimeTypeHandle TypeHandle, RuntimeMethodHandle MethodHandle), Func<ITuple?, object>>(_ =>
			((ConstructorInfo)_.MethodHandle.ToMethodBase(_.TypeHandle)).ToTupleFuncExpression().Compile());
		ScalarTypes = new Dictionary<RuntimeTypeHandle, ScalarType>(29)
		{
			{ typeof(BigInteger).TypeHandle, ScalarType.BigInteger },
			{ typeof(bool).TypeHandle, ScalarType.Boolean },
			{ typeof(byte).TypeHandle, ScalarType.Byte },
			{ typeof(char).TypeHandle, ScalarType.Char },
			{ typeof(DateOnly).TypeHandle, ScalarType.DateOnly },
			{ typeof(DateTime).TypeHandle, ScalarType.DateTime },
			{ typeof(DateTimeOffset).TypeHandle, ScalarType.DateTimeOffset },
			{ typeof(decimal).TypeHandle, ScalarType.Decimal },
			{ typeof(double).TypeHandle, ScalarType.Double },
			{ typeof(Enum).TypeHandle, ScalarType.Enum },
			{ typeof(Guid).TypeHandle, ScalarType.Guid },
			{ typeof(Half).TypeHandle, ScalarType.Half },
			{ typeof(Index).TypeHandle, ScalarType.Index },
			{ typeof(Int128).TypeHandle, ScalarType.Int128 },
			{ typeof(short).TypeHandle, ScalarType.Int16 },
			{ typeof(int).TypeHandle, ScalarType.Int32 },
			{ typeof(long).TypeHandle, ScalarType.Int64 },
			{ typeof(IntPtr).TypeHandle, ScalarType.IntPtr },
			{ typeof(sbyte).TypeHandle, ScalarType.SByte },
			{ typeof(float).TypeHandle, ScalarType.Single },
			{ typeof(string).TypeHandle, ScalarType.String },
			{ typeof(TimeOnly).TypeHandle, ScalarType.TimeOnly },
			{ typeof(TimeSpan).TypeHandle, ScalarType.TimeSpan },
			{ typeof(UInt128).TypeHandle, ScalarType.UInt128 },
			{ typeof(ushort).TypeHandle, ScalarType.UInt16 },
			{ typeof(uint).TypeHandle, ScalarType.UInt32 },
			{ typeof(ulong).TypeHandle, ScalarType.UInt64 },
			{ typeof(UIntPtr).TypeHandle, ScalarType.UIntPtr },
			{ typeof(Uri).TypeHandle, ScalarType.Uri }
		}.ToFrozenDictionary();
		DefaultValueTypeConstructorFuncs = new LazyDictionary<RuntimeTypeHandle, Func<object>>(handle =>
			handle.ToType().ToNewExpression().As<object>().Lambda<Func<object>>().Compile());
		Delegates = new LazyDictionary<(RuntimeTypeHandle TypeHandle, RuntimeMethodHandle MethodHandle), Delegate>(_ =>
			_.MethodHandle.ToMethodBase(_.TypeHandle) switch
			{
				ConstructorInfo constructorInfo => constructorInfo.ToDelegateExpression().Compile(),
				MethodInfo methodInfo => methodInfo.ToDelegateExpression().Compile(),
				_ => throw new UnreachableException("Unable to store Delegate MethodBase type.")
			});
		FieldGetFuncs = new LazyDictionary<RuntimeFieldHandle, Func<object, object?>>(handle =>
			handle.ToFieldInfo().ToFuncExpression().Compile());
		FieldSetActions = new LazyDictionary<RuntimeFieldHandle, Action<object, object?>>(handle =>
			handle.ToFieldInfo().ToActionExpression().Compile());
		MethodArrayActions = new LazyDictionary<(RuntimeTypeHandle TypeHandle, RuntimeMethodHandle MethodHandle), Action<object, object?[]?>>(_ =>
			((MethodInfo)_.MethodHandle.ToMethodBase(_.TypeHandle)).ToArrayActionExpression().Compile());
		MethodTupleActions = new LazyDictionary<(RuntimeTypeHandle TypeHandle, RuntimeMethodHandle MethodHandle), Action<object, ITuple?>>(_ =>
			((MethodInfo)_.MethodHandle.ToMethodBase(_.TypeHandle)).ToTupleActionExpression().Compile());
		MethodArrayFuncs = new LazyDictionary<(RuntimeTypeHandle TypeHandle, RuntimeMethodHandle MethodHandle), Func<object, object?[]?, object?>>(_ =>
			((MethodInfo)_.MethodHandle.ToMethodBase(_.TypeHandle)).ToArrayFuncExpression().Compile());
		MethodTupleFuncs = new LazyDictionary<(RuntimeTypeHandle TypeHandle, RuntimeMethodHandle MethodHandle), Func<object, ITuple?, object?>>(_ =>
			((MethodInfo)_.MethodHandle.ToMethodBase(_.TypeHandle)).ToTupleFuncExpression().Compile());
		ObjectTypes = new LazyDictionary<RuntimeTypeHandle, ObjectType>(handle =>
			handle.ToType() switch
			{
				{ IsPointer: true } => ObjectType.Pointer,
				{ IsPrimitive: true } => ObjectType.ScalarType,
				var type when type.GetScalarType() is not ScalarType.None => ObjectType.ScalarType,
				var type when type == typeof(Action) => ObjectType.Action,
				var type when type == typeof(Action<>) => ObjectType.Action,
				var type when type == typeof(Action<,>) => ObjectType.Action,
				var type when type == typeof(Action<,,>) => ObjectType.Action,
				var type when type == typeof(Action<,,,>) => ObjectType.Action,
				var type when type == typeof(Action<,,,,>) => ObjectType.Action,
				var type when type == typeof(Action<,,,,,>) => ObjectType.Action,
				var type when type == typeof(Action<,,,,,,>) => ObjectType.Action,
				var type when type == typeof(Action<,,,,,,,>) => ObjectType.Action,
				var type when type == typeof(Action<,,,,,,,,>) => ObjectType.Action,
				var type when type == typeof(Action<,,,,,,,,,>) => ObjectType.Action,
				var type when type == typeof(Action<,,,,,,,,,,>) => ObjectType.Action,
				var type when type == typeof(Action<,,,,,,,,,,,>) => ObjectType.Action,
				var type when type == typeof(Action<,,,,,,,,,,,,>) => ObjectType.Action,
				var type when type == typeof(Action<,,,,,,,,,,,,,>) => ObjectType.Action,
				var type when type == typeof(Action<,,,,,,,,,,,,,,>) => ObjectType.Action,
				var type when type == typeof(Action<,,,,,,,,,,,,,,,>) => ObjectType.Action,
				var type when type.Implements(typeof(IAsyncEnumerable<>)) => ObjectType.AsyncEnumerable,
				var type when type.Implements(typeof(IAsyncEnumerator<>)) => ObjectType.AsyncEnumerator,
				var type when type.Implements(typeof(Attribute)) => ObjectType.Attribute,
				var type when type.Implements(typeof(DataColumn)) => ObjectType.DataColumn,
				var type when type.Implements(typeof(DataRow)) => ObjectType.DataRow,
				var type when type.Implements(typeof(DataRowView)) => ObjectType.DataRowView,
				var type when type.Implements(typeof(DataSet)) => ObjectType.DataSet,
				var type when type.Implements(typeof(DataTable)) => ObjectType.DataTable,
				var type when type.Implements(typeof(IEnumerator)) => ObjectType.Enumerator,
				var type when type.Implements(typeof(Exception)) => ObjectType.Exception,
				var type when type == typeof(Func<>) => ObjectType.Func,
				var type when type == typeof(Func<,>) => ObjectType.Func,
				var type when type == typeof(Func<,,>) => ObjectType.Func,
				var type when type == typeof(Func<,,,>) => ObjectType.Func,
				var type when type == typeof(Func<,,,,>) => ObjectType.Func,
				var type when type == typeof(Func<,,,,,>) => ObjectType.Func,
				var type when type == typeof(Func<,,,,,,>) => ObjectType.Func,
				var type when type == typeof(Func<,,,,,,,>) => ObjectType.Func,
				var type when type == typeof(Func<,,,,,,,,>) => ObjectType.Func,
				var type when type == typeof(Func<,,,,,,,,,>) => ObjectType.Func,
				var type when type == typeof(Func<,,,,,,,,,,>) => ObjectType.Func,
				var type when type == typeof(Func<,,,,,,,,,,,>) => ObjectType.Func,
				var type when type == typeof(Func<,,,,,,,,,,,,>) => ObjectType.Func,
				var type when type == typeof(Func<,,,,,,,,,,,,,>) => ObjectType.Func,
				var type when type == typeof(Func<,,,,,,,,,,,,,,>) => ObjectType.Func,
				var type when type == typeof(Func<,,,,,,,,,,,,,,,>) => ObjectType.Func,
				var type when type == typeof(Func<,,,,,,,,,,,,,,,,>) => ObjectType.Func,
				var type when type.Implements(typeof(Delegate)) => ObjectType.Delegate,
				var type when type == typeof(JsonArray) => ObjectType.JsonArray,
				var type when type == typeof(JsonDocument) => ObjectType.JsonDocument,
				var type when type == typeof(JsonElement) => ObjectType.JsonElement,
				var type when type == typeof(JsonObject) => ObjectType.JsonObject,
				var type when type.Implements(typeof(JsonValue)) => ObjectType.JsonValue,
				var type when type.Implements(typeof(Lazy<>)) => ObjectType.Lazy,
				var type when type == typeof(Memory<>) => ObjectType.Memory,
				var type when type == typeof(Nullable<>) => ObjectType.Nullable,
				var type when type == typeof(object) => ObjectType.Object,
				var type when type.Implements(typeof(IObservable<>)) => ObjectType.Observable,
				var type when type.Implements(typeof(IObserver<>)) => ObjectType.Observer,
				var type when type == typeof(Range) => ObjectType.Range,
				var type when type == typeof(ReadOnlyMemory<>) => ObjectType.ReadOnlyMemory,
				var type when type == typeof(ReadOnlySpan<>) => ObjectType.ReadOnlySpan,
				var type when type == typeof(Span<>) => ObjectType.Span,
				var type when type.Implements(typeof(Stream)) => ObjectType.Stream,
				var type when type.Implements(typeof(StringBuilder)) => ObjectType.StringBuilder,
				var type when type.Implements(typeof(Task)) => ObjectType.Task,
				var type when type.Implements(typeof(IAsyncResult)) => ObjectType.AsyncResult,
				{ IsClass: true } type when type.Implements(typeof(ITuple)) => ObjectType.Tuple,
				var type when type.Implements(typeof(Type)) => ObjectType.Type,
				var type when type == typeof(ValueTask) => ObjectType.ValueTask,
				var type when type == typeof(ValueTask<>) => ObjectType.ValueTask,
				{ IsValueType: true } type when type.Implements(typeof(ITuple)) => ObjectType.ValueTuple,
				var type when type == typeof(void) => ObjectType.Void,
				var type when type.Implements(typeof(WeakReference)) => ObjectType.WeakReference,
				var type when type.Implements(typeof(WeakReference<>)) => ObjectType.WeakReference,
				var type when type.Implements(typeof(IEnumerable)) => ObjectType.Enumerable,
				_ => ObjectType.Unknown
			});
		StaticFieldGetFuncs = new LazyDictionary<RuntimeFieldHandle, Func<object?>>(handle =>
			handle.ToFieldInfo().ToStaticFuncExpression().Compile());
		StaticFieldSetActions = new LazyDictionary<RuntimeFieldHandle, Action<object?>>(handle =>
			handle.ToFieldInfo().ToStaticActionExpression().Compile());
		StaticMethodArrayActions = new LazyDictionary<(RuntimeTypeHandle TypeHandle, RuntimeMethodHandle MethodHandle), Action<object?[]?>>(_ =>
			((MethodInfo)_.MethodHandle.ToMethodBase(_.TypeHandle)).ToStaticArrayActionExpression().Compile());
		StaticMethodTupleActions = new LazyDictionary<(RuntimeTypeHandle TypeHandle, RuntimeMethodHandle MethodHandle), Action<ITuple?>>(_ =>
			((MethodInfo)_.MethodHandle.ToMethodBase(_.TypeHandle)).ToStaticTupleActionExpression().Compile());
		StaticMethodArrayFuncs = new LazyDictionary<(RuntimeTypeHandle TypeHandle, RuntimeMethodHandle MethodHandle), Func<object?[]?, object?>>(_ =>
			((MethodInfo)_.MethodHandle.ToMethodBase(_.TypeHandle)).ToStaticArrayFuncExpression().Compile());
		StaticMethodTupleFuncs = new LazyDictionary<(RuntimeTypeHandle TypeHandle, RuntimeMethodHandle MethodHandle), Func<ITuple?, object?>>(_ =>
			((MethodInfo)_.MethodHandle.ToMethodBase(_.TypeHandle)).ToStaticTupleFuncExpression().Compile());
	}

	public static IReadOnlyDictionary<RuntimeTypeHandle, CollectionType> CollectionTypes { get; }

	public static IReadOnlyDictionary<(RuntimeTypeHandle, RuntimeMethodHandle), Func<object?[]?, object>> ConstructorArrayFuncs { get; }

	public static IReadOnlyDictionary<(RuntimeTypeHandle, RuntimeMethodHandle), Func<ITuple?, object>> ConstructorTupleFuncs { get; }

	public static IReadOnlyDictionary<RuntimeTypeHandle, Func<object>> DefaultValueTypeConstructorFuncs { get; }

	public static IReadOnlyDictionary<(RuntimeTypeHandle, RuntimeMethodHandle), Delegate> Delegates { get; }

	public static IReadOnlyDictionary<RuntimeFieldHandle, Func<object, object?>> FieldGetFuncs { get; }

	public static IReadOnlyDictionary<RuntimeFieldHandle, Action<object, object?>> FieldSetActions { get; }

	public static IReadOnlyDictionary<(RuntimeTypeHandle, RuntimeMethodHandle), Action<object, object?[]?>> MethodArrayActions { get; }

	public static IReadOnlyDictionary<(RuntimeTypeHandle, RuntimeMethodHandle), Action<object, ITuple?>> MethodTupleActions { get; }

	public static IReadOnlyDictionary<(RuntimeTypeHandle, RuntimeMethodHandle), Func<object, object?[]?, object?>> MethodArrayFuncs { get; }

	public static IReadOnlyDictionary<(RuntimeTypeHandle, RuntimeMethodHandle), Func<object, ITuple?, object?>> MethodTupleFuncs { get; }

	public static IReadOnlyDictionary<RuntimeTypeHandle, ObjectType> ObjectTypes { get; }

	public static IReadOnlyDictionary<RuntimeTypeHandle, ScalarType> ScalarTypes { get; }

	public static IReadOnlyDictionary<RuntimeFieldHandle, Func<object?>> StaticFieldGetFuncs { get; }

	public static IReadOnlyDictionary<RuntimeFieldHandle, Action<object?>> StaticFieldSetActions { get; }

	public static IReadOnlyDictionary<(RuntimeTypeHandle, RuntimeMethodHandle), Action<object?[]?>> StaticMethodArrayActions { get; }

	public static IReadOnlyDictionary<(RuntimeTypeHandle, RuntimeMethodHandle), Action<ITuple?>> StaticMethodTupleActions { get; }

	public static IReadOnlyDictionary<(RuntimeTypeHandle, RuntimeMethodHandle), Func<object?[]?, object?>> StaticMethodArrayFuncs { get; }

	public static IReadOnlyDictionary<(RuntimeTypeHandle, RuntimeMethodHandle), Func<ITuple?, object?>> StaticMethodTupleFuncs { get; }
}
