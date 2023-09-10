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
using TypeCache.Collections;
using TypeCache.Extensions;

namespace TypeCache.Utilities;

internal static class TypeStore
{
	public static IReadOnlyList<(RuntimeTypeHandle Handle, CollectionType CollectionType)> CollectionTypeMap => new[]
	{
		(typeof(Array).TypeHandle, CollectionType.Array),
		(typeof(ArrayList).TypeHandle, CollectionType.ArrayList),
		(typeof(BitArray).TypeHandle, CollectionType.BitArray),
		(typeof(BlockingCollection<>).TypeHandle, CollectionType.BlockingCollection),
		(typeof(ConcurrentBag<>).TypeHandle, CollectionType.ConcurrentBag),
		(typeof(ConcurrentDictionary<,>).TypeHandle, CollectionType.ConcurrentDictionary),
		(typeof(ConcurrentQueue<>).TypeHandle, CollectionType.ConcurrentQueue),
		(typeof(ConcurrentStack<>).TypeHandle, CollectionType.ConcurrentStack),
		(typeof(Hashtable).TypeHandle, CollectionType.Hashtable),
		(typeof(HybridDictionary).TypeHandle, CollectionType.HybridDictionary),
		(typeof(ImmutableArray<>).TypeHandle, CollectionType.ImmutableArray),
		(typeof(ImmutableSortedDictionary<,>).TypeHandle, CollectionType.ImmutableSortedDictionary),
		(typeof(IImmutableDictionary<,>).TypeHandle, CollectionType.ImmutableDictionary),
		(typeof(ImmutableSortedSet<>).TypeHandle, CollectionType.ImmutableSortedSet),
		(typeof(IImmutableSet<>).TypeHandle, CollectionType.ImmutableSet),
		(typeof(IImmutableList<>).TypeHandle, CollectionType.ImmutableList),
		(typeof(IImmutableQueue<>).TypeHandle, CollectionType.ImmutableQueue),
		(typeof(IImmutableStack<>).TypeHandle, CollectionType.ImmutableStack),
		(typeof(KeyedCollection<,>).TypeHandle, CollectionType.KeyedCollection),
		(typeof(LinkedList<>).TypeHandle, CollectionType.LinkedList),
		(typeof(ListDictionary).TypeHandle, CollectionType.ListDictionary),
		(typeof(NameObjectCollectionBase).TypeHandle, CollectionType.NameObjectCollection),
		(typeof(NameValueCollection).TypeHandle, CollectionType.NameValueCollection),
		(typeof(ObservableCollection<>).TypeHandle, CollectionType.ObservableCollection),
		(typeof(IOrderedDictionary).TypeHandle, CollectionType.OrderedDictionary),
		(typeof(PriorityQueue<,>).TypeHandle, CollectionType.PriorityQueue),
		(typeof(Queue<>).TypeHandle, CollectionType.Queue),
		(typeof(Queue).TypeHandle, CollectionType.Queue),
		(typeof(ReadOnlyObservableCollection<>).TypeHandle, CollectionType.ReadOnlyObservableCollection),
		(typeof(ReadOnlyCollection<>).TypeHandle, CollectionType.ReadOnlyCollection),
		(typeof(ReadOnlyCollectionBase).TypeHandle, CollectionType.ReadOnlyCollection),
		(typeof(SortedDictionary<,>).TypeHandle, CollectionType.SortedDictionary),
		(typeof(SortedList<,>).TypeHandle, CollectionType.SortedList),
		(typeof(SortedList).TypeHandle, CollectionType.SortedList),
		(typeof(SortedSet<>).TypeHandle, CollectionType.SortedSet),
		(typeof(Stack<>).TypeHandle, CollectionType.Stack),
		(typeof(Stack).TypeHandle, CollectionType.Stack),
		(typeof(StringCollection).TypeHandle, CollectionType.StringCollection),
		(typeof(Collection<>).TypeHandle, CollectionType.Collection),
		(typeof(CollectionBase).TypeHandle, CollectionType.Collection),
		(typeof(IReadOnlySet<>).TypeHandle, CollectionType.ReadOnlySet),
		(typeof(ISet<>).TypeHandle, CollectionType.Set),
		(typeof(IDictionary<,>).TypeHandle, CollectionType.Dictionary),
		(typeof(IReadOnlyDictionary<,>).TypeHandle, CollectionType.ReadOnlyDictionary),
		(typeof(IList<>).TypeHandle, CollectionType.List),
		(typeof(IReadOnlyList<>).TypeHandle, CollectionType.ReadOnlyList),
		(typeof(IReadOnlyCollection<>).TypeHandle, CollectionType.ReadOnlyCollection),
		(typeof(ICollection<>).TypeHandle, CollectionType.Collection)
	}.ToImmutableArray();

	public static IReadOnlyList<(RuntimeTypeHandle Handle, ObjectType ObjectType)> ObjectTypeMap => new[]
	{
		(typeof(Attribute).TypeHandle, ObjectType.Attribute),
		(typeof(DataColumn).TypeHandle, ObjectType.DataColumn),
		(typeof(DataRow).TypeHandle, ObjectType.DataRow),
		(typeof(DataRowView).TypeHandle, ObjectType.DataRowView),
		(typeof(DataSet).TypeHandle, ObjectType.DataSet),
		(typeof(DataTable).TypeHandle, ObjectType.DataTable),
		(typeof(Delegate).TypeHandle, ObjectType.Delegate),
		(typeof(Exception).TypeHandle, ObjectType.Exception),
		(typeof(IAsyncResult).TypeHandle, ObjectType.AsyncResult),
		(typeof(JsonDocument).TypeHandle, ObjectType.JsonDocument),
		(typeof(StringBuilder).TypeHandle, ObjectType.StringBuilder),
		(typeof(Stream).TypeHandle, ObjectType.Stream),
		(typeof(Task).TypeHandle, ObjectType.Task),
		(typeof(Type).TypeHandle, ObjectType.Type),
		(typeof(WeakReference).TypeHandle, ObjectType.WeakReference),
		(typeof(void).TypeHandle, ObjectType.Void),
		(typeof(Memory<>).TypeHandle, ObjectType.Memory),
		(typeof(ReadOnlyMemory<>).TypeHandle, ObjectType.ReadOnlyMemory),
		(typeof(ReadOnlySpan<>).TypeHandle, ObjectType.ReadOnlySpan),
		(typeof(Span<>).TypeHandle, ObjectType.Span),
		(typeof(ValueTask<>).TypeHandle, ObjectType.ValueTask),
		(typeof(WeakReference<>).TypeHandle, ObjectType.WeakReference),
		(typeof(IAsyncEnumerable<>).TypeHandle, ObjectType.AsyncEnumerable),
		(typeof(IAsyncEnumerator<>).TypeHandle, ObjectType.AsyncEnumerator),
		(typeof(IEnumerable).TypeHandle, ObjectType.Enumerable),
		(typeof(IEnumerator).TypeHandle, ObjectType.Enumerator),
		(typeof(IObservable<>).TypeHandle, ObjectType.Observable),
		(typeof(IObserver<>).TypeHandle, ObjectType.Observer),
		(typeof(Lazy<>).TypeHandle, ObjectType.Lazy),
		(typeof(Lazy<,>).TypeHandle, ObjectType.Lazy),
		(typeof(Range).TypeHandle, ObjectType.Range),
		(typeof(Task).TypeHandle, ObjectType.Task),
		(typeof(Task<>).TypeHandle, ObjectType.Task),
		(typeof(Tuple<>).TypeHandle, ObjectType.Tuple),
		(typeof(Tuple<,>).TypeHandle, ObjectType.Tuple),
		(typeof(Tuple<,,>).TypeHandle, ObjectType.Tuple),
		(typeof(Tuple<,,,>).TypeHandle, ObjectType.Tuple),
		(typeof(Tuple<,,,,>).TypeHandle, ObjectType.Tuple),
		(typeof(Tuple<,,,,,>).TypeHandle, ObjectType.Tuple),
		(typeof(Tuple<,,,,,,>).TypeHandle, ObjectType.Tuple),
		(typeof(Tuple<,,,,,,,>).TypeHandle, ObjectType.Tuple),
		(typeof(ValueTask).TypeHandle, ObjectType.ValueTask),
		(typeof(ValueTask<>).TypeHandle, ObjectType.ValueTask),
		(typeof(ValueTuple).TypeHandle, ObjectType.ValueTuple),
		(typeof(ValueTuple<>).TypeHandle, ObjectType.ValueTuple),
		(typeof(ValueTuple<,>).TypeHandle, ObjectType.ValueTuple),
		(typeof(ValueTuple<,,>).TypeHandle, ObjectType.ValueTuple),
		(typeof(ValueTuple<,,,>).TypeHandle, ObjectType.ValueTuple),
		(typeof(ValueTuple<,,,,>).TypeHandle, ObjectType.ValueTuple),
		(typeof(ValueTuple<,,,,,>).TypeHandle, ObjectType.ValueTuple),
		(typeof(ValueTuple<,,,,,,>).TypeHandle, ObjectType.ValueTuple),
		(typeof(ValueTuple<,,,,,,,>).TypeHandle, ObjectType.ValueTuple)
	};

	static TypeStore()
	{
		CollectionTypes = new LazyDictionary<RuntimeTypeHandle, CollectionType>(handle =>
		{
			var type = handle.ToType();

			if (type.IsArray)
				return CollectionType.Array;

			var count = CollectionTypeMap.Count;
			for (var i = 0; i < count; ++i)
			{
				var map = CollectionTypeMap[i];
				if (type.Implements(map.Handle.ToType()))
					return map.CollectionType;
			}

			return CollectionType.None;
		});
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
		ObjectTypes = new LazyDictionary<RuntimeTypeHandle, ObjectType>(handle =>
		{
			var type = handle.ToType();

			if (type.IsPointer)
				return ObjectType.Pointer;

			if (type == typeof(object))
				return ObjectType.Object;

			if (type.IsPrimitive || type.GetDataType() is not ScalarType.None)
				return ObjectType.DataType;

			var count = ObjectTypeMap.Count;
			for (var i = 0; i < count; ++i)
			{
				var map = ObjectTypeMap[i];
				if (type.Implements(map.Handle.ToType()))
					return map.ObjectType;
			}

			return ObjectType.Unknown;
		});
		DataTypes = new Dictionary<RuntimeTypeHandle, ScalarType>(30)
		{
			{ typeof(BigInteger).TypeHandle, ScalarType.BigInteger },
			{ typeof(bool).TypeHandle, ScalarType.Boolean },
			{ typeof(byte).TypeHandle, ScalarType.Byte },
			{ typeof(char).TypeHandle, ScalarType.Char },
			{ typeof(DateOnly).TypeHandle, ScalarType.DateOnly },
			{ typeof(DateTime).TypeHandle, ScalarType.DateTime },
			{ typeof(DateTimeOffset).TypeHandle, ScalarType.DateTimeOffset },
			{ typeof(DBNull).TypeHandle, ScalarType.DBNull },
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
		}.ToImmutableDictionary();
	}

	public static IReadOnlyDictionary<RuntimeTypeHandle, Func<object>> DefaultValueTypeConstructorInvokes { get; }

	public static ConcurrentDictionary<RuntimeFieldHandle, Func<object?, object?>> FieldGetInvokes { get; }

	public static ConcurrentDictionary<RuntimeFieldHandle, Action<object?, object?>> FieldSetInvokes { get; }

	public static IReadOnlyDictionary<(RuntimeTypeHandle, RuntimeMethodHandle), Func<object?[]?, object?>> MethodInvokes { get; }

	internal static IReadOnlyDictionary<RuntimeTypeHandle, CollectionType> CollectionTypes { get; }

	internal static IReadOnlyDictionary<RuntimeTypeHandle, ScalarType> DataTypes { get; }

	internal static IReadOnlyDictionary<RuntimeTypeHandle, ObjectType> ObjectTypes { get; }
}
