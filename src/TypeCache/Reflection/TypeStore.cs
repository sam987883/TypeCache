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
using TypeCache.Collections;
using TypeCache.Extensions;
using static System.Threading.LazyThreadSafetyMode;

namespace TypeCache.Reflection;

public static class TypeStore
{
	public const char GENERIC_TICKMARK = '`';

	public static IReadOnlyDictionary<RuntimeTypeHandle, IReadOnlySet<Attribute>> Attributes { get; }

	public static IReadOnlyDictionary<RuntimeTypeHandle, ClrType> ClrTypes { get; }

	public static IReadOnlyDictionary<RuntimeTypeHandle, string> CodeNames { get; }

	public static IReadOnlyDictionary<RuntimeTypeHandle, CollectionType> CollectionTypes { get; }

	public static IReadOnlyDictionary<RuntimeTypeHandle, ConstructorSet> Constructors { get; }

	public static IReadOnlyDictionary<RuntimeTypeHandle, IReadOnlyDictionary<string, FieldEntity>> Fields { get; }

	public static IReadOnlyDictionary<RuntimeTypeHandle, IReadOnlySet<RuntimeTypeHandle>> Interfaces { get; }

	public static IReadOnlyDictionary<RuntimeTypeHandle, IReadOnlyDictionary<string, Literal>> Literals { get; }

	public static IReadOnlyDictionary<RuntimeTypeHandle, IReadOnlyDictionary<string, MethodSet<MethodEntity>>> Methods { get; }

	public static IReadOnlyDictionary<RuntimeTypeHandle, ObjectType> ObjectTypes { get; }

	public static IReadOnlyDictionary<RuntimeTypeHandle, IReadOnlyDictionary<string, PropertyEntity>> Properties { get; }

	public static IReadOnlyDictionary<RuntimeTypeHandle, IReadOnlyDictionary<string, PropertyIndexerEntity>> PropertyIndexers { get; }

	public static IReadOnlyDictionary<RuntimeTypeHandle, ScalarType> ScalarTypes { get; }

	public static IReadOnlyDictionary<RuntimeTypeHandle, IReadOnlyDictionary<string, StaticFieldEntity>> StaticFields { get; }

	public static IReadOnlyDictionary<RuntimeTypeHandle, IReadOnlyDictionary<string, MethodSet<StaticMethodEntity>>> StaticMethods { get; }

	public static IReadOnlyDictionary<RuntimeTypeHandle, IReadOnlyDictionary<string, StaticPropertyEntity>> StaticProperties { get; }

	public static IReadOnlyDictionary<RuntimeTypeHandle, IReadOnlyDictionary<string, StaticPropertyIndexerEntity>> StaticPropertyIndexers { get; }

	static TypeStore()
	{
		const BindingFlags INSTANCE_BINDING = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
		const BindingFlags STATIC_BINDING = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy;

		Attributes = LazyDictionary.Create<RuntimeTypeHandle, IReadOnlySet<Attribute>>(handle =>
			handle.ToType().GetCustomAttributes().ToFrozenSet());

		ClrTypes = LazyDictionary.Create<RuntimeTypeHandle, ClrType>(handle => GetClrType(handle.ToType()));

		CodeNames = LazyDictionary.Create<RuntimeTypeHandle, string>(handle => GetCodeName(handle.ToType()));

		CollectionTypes = LazyDictionary.Create<RuntimeTypeHandle, CollectionType>(handle => GetCollectionType(handle.ToType()));

		Constructors = LazyDictionary.Create<RuntimeTypeHandle, ConstructorSet>(handle => new(handle.ToType()));

		Fields = LazyDictionary.Create<RuntimeTypeHandle, IReadOnlyDictionary<string, FieldEntity>>(handle =>
		{
			var fieldInfos = handle.ToType().GetFields(INSTANCE_BINDING).Where(_ => !_.IsLiteral);
			if (!fieldInfos.Any())
				return FrozenDictionary<string, FieldEntity>.Empty;

			var names = fieldInfos.Select(_ => _.Name).ToHashSet(StringComparer.Ordinal);
			var nameComparer = names.IsCaseSensitive() ? StringComparer.Ordinal : StringComparer.OrdinalIgnoreCase;
			return fieldInfos.ToFrozenDictionary(fieldInfo => fieldInfo.Name, fieldInfo => new FieldEntity(fieldInfo), nameComparer);
		});

		Interfaces = LazyDictionary.Create<RuntimeTypeHandle, IReadOnlySet<RuntimeTypeHandle>>(handle =>
			handle.ToType().GetInterfaces().Select(_ => _.TypeHandle).ToFrozenSet());

		Literals = LazyDictionary.Create<RuntimeTypeHandle, IReadOnlyDictionary<string, Literal>>(handle =>
		{
			var fieldInfos = handle.ToType().GetFields(STATIC_BINDING).Where(_ => _.IsLiteral);
			if (!fieldInfos.Any())
				return FrozenDictionary<string, Literal>.Empty;

			var names = fieldInfos.Select(_ => _.Name).ToHashSet(StringComparer.Ordinal);
			var nameComparer = names.IsCaseSensitive() ? StringComparer.Ordinal : StringComparer.OrdinalIgnoreCase;
			return fieldInfos.ToFrozenDictionary(fieldInfo => fieldInfo.Name, fieldInfo => new Literal(fieldInfo), nameComparer);
		});

		Methods = LazyDictionary.Create<RuntimeTypeHandle, IReadOnlyDictionary<string, MethodSet<MethodEntity>>>(handle =>
		{
			var methodInfos = handle.ToType().GetMethods(INSTANCE_BINDING);
			if (methodInfos.Length is 0)
				return FrozenDictionary<string, MethodSet<MethodEntity>>.Empty;

			var names = methodInfos.Select(_ => _.Name).ToHashSet(StringComparer.Ordinal);
			var comparer = names.IsCaseSensitive() ? StringComparer.Ordinal : StringComparer.OrdinalIgnoreCase;
			return methodInfos
				.GroupBy(_ => _.Name, comparer)
				.ToFrozenDictionary(_ => _.Key, createMethodSet, comparer);
		});

		ObjectTypes = LazyDictionary.Create<RuntimeTypeHandle, ObjectType>(handle => GetObjectType(handle.ToType()));

		Properties = LazyDictionary.Create<RuntimeTypeHandle, IReadOnlyDictionary<string, PropertyEntity>>(handle =>
		{
			var propertyInfos = handle.ToType().GetProperties(INSTANCE_BINDING)
				.Where(_ => _.GetIndexParameters().Length is 0);
			if (!propertyInfos.Any())
				return FrozenDictionary<string, PropertyEntity>.Empty;

			var nameComparer = propertyInfos.Select(_ => _.Name).IsCaseSensitive() ? StringComparer.Ordinal : StringComparer.OrdinalIgnoreCase;
			return propertyInfos.ToFrozenDictionary(propertyInfo => propertyInfo.Name, propertyInfo => new PropertyEntity(propertyInfo), nameComparer);
		});

		PropertyIndexers = LazyDictionary.Create<RuntimeTypeHandle, IReadOnlyDictionary<string, PropertyIndexerEntity>>(handle =>
		{
			var propertyInfos = handle.ToType().GetProperties(INSTANCE_BINDING)
				.Where(_ => _.GetIndexParameters().Length > 0);
			if (!propertyInfos.Any())
				return FrozenDictionary<string, PropertyIndexerEntity>.Empty;

			var nameComparer = propertyInfos.Select(_ => _.Name).IsCaseSensitive() ? StringComparer.Ordinal : StringComparer.OrdinalIgnoreCase;
			return propertyInfos.ToFrozenDictionary(propertyInfo => propertyInfo.Name, propertyInfo => new PropertyIndexerEntity(propertyInfo), nameComparer);
		});

		ScalarTypes = new (Type Type, ScalarType ScalarType)[]
		{
			(typeof(BigInteger), ScalarType.BigInteger),
			(typeof(bool), ScalarType.Boolean),
			(typeof(byte), ScalarType.Byte),
			(typeof(char), ScalarType.Char),
			(typeof(DateOnly), ScalarType.DateOnly),
			(typeof(DateTime), ScalarType.DateTime),
			(typeof(DateTimeOffset), ScalarType.DateTimeOffset),
			(typeof(decimal), ScalarType.Decimal),
			(typeof(double), ScalarType.Double),
			(typeof(Enum), ScalarType.Enum),
			(typeof(Guid), ScalarType.Guid),
			(typeof(Half), ScalarType.Half),
			(typeof(Index), ScalarType.Index),
			(typeof(Int128), ScalarType.Int128),
			(typeof(short), ScalarType.Int16),
			(typeof(int), ScalarType.Int32),
			(typeof(long), ScalarType.Int64),
			(typeof(IntPtr), ScalarType.IntPtr),
			(typeof(sbyte), ScalarType.SByte),
			(typeof(float), ScalarType.Single),
			(typeof(string), ScalarType.String),
			(typeof(TimeOnly), ScalarType.TimeOnly),
			(typeof(TimeSpan), ScalarType.TimeSpan),
			(typeof(UInt128), ScalarType.UInt128),
			(typeof(ushort), ScalarType.UInt16),
			(typeof(uint), ScalarType.UInt32),
			(typeof(ulong), ScalarType.UInt64),
			(typeof(UIntPtr), ScalarType.UIntPtr),
			(typeof(Uri), ScalarType.Uri)
		}.ToFrozenDictionary(_ => _.Type.TypeHandle, _ => _.ScalarType);

		StaticFields = LazyDictionary.Create<RuntimeTypeHandle, IReadOnlyDictionary<string, StaticFieldEntity>>(handle =>
		{
			var fieldInfos = handle.ToType().GetFields(STATIC_BINDING).Where(_ => !_.IsLiteral);
			if (!fieldInfos.Any())
				return FrozenDictionary<string, StaticFieldEntity>.Empty;

			var names = fieldInfos.Select(_ => _.Name).ToHashSet(StringComparer.Ordinal);
			var nameComparer = names.IsCaseSensitive() ? StringComparer.Ordinal : StringComparer.OrdinalIgnoreCase;
			return fieldInfos.ToFrozenDictionary(fieldInfo => fieldInfo.Name, fieldInfo => new StaticFieldEntity(fieldInfo), nameComparer);
		});

		StaticMethods = LazyDictionary.Create<RuntimeTypeHandle, IReadOnlyDictionary<string, MethodSet<StaticMethodEntity>>>(handle =>
		{
			var methodInfos = handle.ToType().GetMethods(STATIC_BINDING);
			if (methodInfos.Length is 0)
				return FrozenDictionary<string, MethodSet<StaticMethodEntity>>.Empty;

			var names = methodInfos.Select(_ => _.Name).ToHashSet(StringComparer.Ordinal);
			var nameComparer = names.IsCaseSensitive() ? StringComparer.Ordinal : StringComparer.OrdinalIgnoreCase;
			return methodInfos
				.GroupBy(_ => _.Name, nameComparer)
				.ToFrozenDictionary(_ => _.Key, createStaticMethodSet, nameComparer);
		});

		StaticProperties = LazyDictionary.Create<RuntimeTypeHandle, IReadOnlyDictionary<string, StaticPropertyEntity>>(handle =>
		{
			var propertyInfos = handle.ToType().GetProperties(STATIC_BINDING)
				.Where(_ => _.GetIndexParameters().Length is 0);
			if (!propertyInfos.Any())
				return FrozenDictionary<string, StaticPropertyEntity>.Empty;

			var nameComparer = propertyInfos.Select(_ => _.Name).IsCaseSensitive() ? StringComparer.Ordinal : StringComparer.OrdinalIgnoreCase;
			return propertyInfos.ToFrozenDictionary(propetyInfo => propetyInfo.Name, propetyInfo => new StaticPropertyEntity(propetyInfo), nameComparer);
		});

		StaticPropertyIndexers = LazyDictionary.Create<RuntimeTypeHandle, IReadOnlyDictionary<string, StaticPropertyIndexerEntity>>(handle =>
		{
			var propertyInfos = handle.ToType().GetProperties(STATIC_BINDING)
				.Where(_ => _.GetIndexParameters().Length > 0);
			if (!propertyInfos.Any())
				return FrozenDictionary<string, StaticPropertyIndexerEntity>.Empty;

			var nameComparer = propertyInfos.Select(_ => _.Name).IsCaseSensitive() ? StringComparer.Ordinal : StringComparer.OrdinalIgnoreCase;
			return propertyInfos.ToFrozenDictionary(propetyInfo => propetyInfo.Name, propetyInfo => new StaticPropertyIndexerEntity(propetyInfo), nameComparer);
		});

		static MethodSet<MethodEntity> createMethodSet(IGrouping<string, MethodInfo> methodInfos)
		{
			var methods = methodInfos
				.Where(methodInfo => !methodInfo.IsGenericMethodDefinition)
				.Select(methodInfo => new MethodEntity(methodInfo));
			var genericMethods = methodInfos
				.Where(methodInfo => methodInfo.IsGenericMethodDefinition)
				.Select(methodInfo => new GenericMethodDefinition(methodInfo));

			return new(methodInfos.Key, methods, genericMethods);
		}

		static MethodSet<StaticMethodEntity> createStaticMethodSet(IGrouping<string, MethodInfo> methodInfos)
		{
			var methods = methodInfos
				.Where(methodInfo => !methodInfo.IsGenericMethodDefinition)
				.Select(methodInfo => new StaticMethodEntity(methodInfo));
			var genericMethods = methodInfos
				.Where(methodInfo => methodInfo.IsGenericMethodDefinition)
				.Select(methodInfo => new GenericMethodDefinition(methodInfo));

			return new(methodInfos.Key, methods, genericMethods);
		}
	}

	private static ClrType GetClrType(Type type)
		=> type switch
		{
			{ IsEnum: true } => ClrType.Enum,
			{ IsValueType: true } => ClrType.Struct,
			{ IsInterface: true } => ClrType.Interface,
			_ when type.IsAssignableTo<Delegate>() => ClrType.Delegate,
			_ => ClrType.Class,
		};

	private static string GetCodeName(Type type)
		=> type switch
		{
			_ when type == typeof(string) => type.Name,
			{ IsPointer: true } => Invariant($"{GetCodeName(type.GetElementType()!)}*"),
			{ IsArray: true } => Invariant($"{GetCodeName(type.GetElementType()!)}[{string.Concat(','.Repeat(type.GetArrayRank() - 1))}]"),
			{ IsByRef: true } => Invariant($"{GetCodeName(type.GetElementType()!)}&"),
			{ IsGenericTypeDefinition: true } => Invariant($"{type.Name[0..type.Name.IndexOf(GENERIC_TICKMARK)]}<{string.Concat(','.Repeat(type.GetGenericArguments().Length - 1))}>"),
			{ IsConstructedGenericType: true } => Invariant($"{type.Name[0..type.Name.IndexOf(GENERIC_TICKMARK)]}<{type.GetGenericArguments().Select(GetCodeName).ToCSV()}>"),
			_ => type.Name
		};

	private static CollectionType GetCollectionType(Type type)
		=> type switch
		{
			{ IsArray: true } => CollectionType.Array,
			_ when type.Implements(typeof(ArrayList)) => CollectionType.ArrayList,
			_ when type.Implements(typeof(BitArray)) => CollectionType.BitArray,
			_ when type.Implements(typeof(BlockingCollection<>)) => CollectionType.BlockingCollection,
			_ when type.Implements(typeof(ConcurrentBag<>)) => CollectionType.ConcurrentBag,
			_ when type.Implements(typeof(ConcurrentDictionary<,>)) => CollectionType.ConcurrentDictionary,
			_ when type.Implements(typeof(ConcurrentQueue<>)) => CollectionType.ConcurrentQueue,
			_ when type.Implements(typeof(ConcurrentStack<>)) => CollectionType.ConcurrentStack,
			_ when type.Implements(typeof(FrozenDictionary<,>)) => CollectionType.FrozenDictionary,
			_ when type.Implements(typeof(FrozenSet<>)) => CollectionType.FrozenSet,
			_ when type.Implements(typeof(Hashtable)) => CollectionType.Hashtable,
			_ when type.Implements(typeof(HybridDictionary)) => CollectionType.HybridDictionary,
			_ when type.Implements(typeof(ImmutableArray<>)) => CollectionType.ImmutableArray,
			_ when type.Implements(typeof(ImmutableSortedDictionary<,>)) => CollectionType.ImmutableSortedDictionary,
			_ when type.Implements(typeof(IImmutableDictionary<,>)) => CollectionType.ImmutableDictionary,
			_ when type.Implements(typeof(ImmutableSortedSet<>)) => CollectionType.ImmutableSortedSet,
			_ when type.Implements(typeof(IImmutableSet<>)) => CollectionType.ImmutableSet,
			_ when type.Implements(typeof(IImmutableList<>)) => CollectionType.ImmutableList,
			_ when type.Implements(typeof(IImmutableQueue<>)) => CollectionType.ImmutableQueue,
			_ when type.Implements(typeof(IImmutableStack<>)) => CollectionType.ImmutableStack,
			_ when type.Implements(typeof(KeyedCollection<,>)) => CollectionType.KeyedCollection,
			_ when type.Implements(typeof(LinkedList<>)) => CollectionType.LinkedList,
			_ when type.Implements(typeof(ListDictionary)) => CollectionType.ListDictionary,
			_ when type.Implements(typeof(NameObjectCollectionBase)) => CollectionType.NameObjectCollection,
			_ when type.Implements(typeof(ObservableCollection<>)) => CollectionType.ObservableCollection,
			_ when type.Implements(typeof(IOrderedDictionary)) => CollectionType.OrderedDictionary,
			_ when type.Implements(typeof(PriorityQueue<,>)) => CollectionType.PriorityQueue,
			_ when type.Implements(typeof(Queue<>)) => CollectionType.Queue,
			_ when type.Implements(typeof(Queue)) => CollectionType.Queue,
			_ when type.Implements(typeof(ReadOnlyObservableCollection<>)) => CollectionType.ReadOnlyObservableCollection,
			_ when type.Implements(typeof(SortedDictionary<,>)) => CollectionType.SortedDictionary,
			_ when type.Implements(typeof(SortedList<,>)) => CollectionType.SortedList,
			_ when type.Implements(typeof(SortedList)) => CollectionType.SortedList,
			_ when type.Implements(typeof(SortedSet<>)) => CollectionType.SortedSet,
			_ when type.Implements(typeof(Stack<>)) => CollectionType.Stack,
			_ when type.Implements(typeof(Stack)) => CollectionType.Stack,
			_ when type.Implements(typeof(StringCollection)) => CollectionType.StringCollection,
			_ when type.Implements(typeof(CollectionBase)) => CollectionType.List,
			_ when type.Implements(typeof(System.Collections.ObjectModel.ReadOnlyCollection<>)) => CollectionType.ReadOnlyList,
			_ when type.Implements(typeof(IReadOnlySet<>)) => CollectionType.ReadOnlySet,
			_ when type.Implements(typeof(ISet<>)) => CollectionType.Set,
			_ when type.Implements(typeof(IDictionary<,>)) => CollectionType.Dictionary,
			_ when type.Implements(typeof(IReadOnlyDictionary<,>)) => CollectionType.ReadOnlyDictionary,
			_ when type.Implements(typeof(IList)) => CollectionType.List,
			_ when type.Implements(typeof(IList<>)) => CollectionType.List,
			_ when type.Implements(typeof(IReadOnlyList<>)) => CollectionType.ReadOnlyList,
			_ when type.Implements(typeof(IReadOnlyCollection<>)) => CollectionType.ReadOnlyCollection,
			_ when type.Implements(typeof(ICollection)) => CollectionType.ReadOnlyCollection,
			_ when type.Implements(typeof(ICollection<>)) => CollectionType.Collection,
			_ => CollectionType.None
		};

	private static ObjectType GetObjectType(Type type)
		=> type switch
		{
			{ IsConstructedGenericType: true } => GetObjectType(type.GetGenericTypeDefinition()),
			{ IsPointer: true } => ObjectType.Pointer,
			{ IsPrimitive: true } => ObjectType.ScalarType,
			_ when type.ScalarType is not ScalarType.None => ObjectType.ScalarType,
			_ when type.IsAssignableTo<Delegate>() => type switch
			{
				_ when type == typeof(Action) => ObjectType.Action,
				_ when type == typeof(Action<>) => ObjectType.Action,
				_ when type == typeof(Action<,>) => ObjectType.Action,
				_ when type == typeof(Action<,,>) => ObjectType.Action,
				_ when type == typeof(Action<,,,>) => ObjectType.Action,
				_ when type == typeof(Action<,,,,>) => ObjectType.Action,
				_ when type == typeof(Action<,,,,,>) => ObjectType.Action,
				_ when type == typeof(Action<,,,,,,>) => ObjectType.Action,
				_ when type == typeof(Action<,,,,,,,>) => ObjectType.Action,
				_ when type == typeof(Action<,,,,,,,,>) => ObjectType.Action,
				_ when type == typeof(Action<,,,,,,,,,>) => ObjectType.Action,
				_ when type == typeof(Action<,,,,,,,,,,>) => ObjectType.Action,
				_ when type == typeof(Action<,,,,,,,,,,,>) => ObjectType.Action,
				_ when type == typeof(Action<,,,,,,,,,,,,>) => ObjectType.Action,
				_ when type == typeof(Action<,,,,,,,,,,,,,>) => ObjectType.Action,
				_ when type == typeof(Action<,,,,,,,,,,,,,,>) => ObjectType.Action,
				_ when type == typeof(Action<,,,,,,,,,,,,,,,>) => ObjectType.Action,
				_ when type == typeof(Func<>) => ObjectType.Func,
				_ when type == typeof(Func<,>) => ObjectType.Func,
				_ when type == typeof(Func<,,>) => ObjectType.Func,
				_ when type == typeof(Func<,,,>) => ObjectType.Func,
				_ when type == typeof(Func<,,,,>) => ObjectType.Func,
				_ when type == typeof(Func<,,,,,>) => ObjectType.Func,
				_ when type == typeof(Func<,,,,,,>) => ObjectType.Func,
				_ when type == typeof(Func<,,,,,,,>) => ObjectType.Func,
				_ when type == typeof(Func<,,,,,,,,>) => ObjectType.Func,
				_ when type == typeof(Func<,,,,,,,,,>) => ObjectType.Func,
				_ when type == typeof(Func<,,,,,,,,,,>) => ObjectType.Func,
				_ when type == typeof(Func<,,,,,,,,,,,>) => ObjectType.Func,
				_ when type == typeof(Func<,,,,,,,,,,,,>) => ObjectType.Func,
				_ when type == typeof(Func<,,,,,,,,,,,,,>) => ObjectType.Func,
				_ when type == typeof(Func<,,,,,,,,,,,,,,>) => ObjectType.Func,
				_ when type == typeof(Func<,,,,,,,,,,,,,,,>) => ObjectType.Func,
				_ when type == typeof(Func<,,,,,,,,,,,,,,,,>) => ObjectType.Func,
				_ => ObjectType.Delegate
			},
			_ when type.Implements(typeof(IAsyncEnumerable<>)) => ObjectType.AsyncEnumerable,
			_ when type.Implements(typeof(IAsyncEnumerator<>)) => ObjectType.AsyncEnumerator,
			_ when type.Implements(typeof(Attribute)) => ObjectType.Attribute,
			_ when type.Implements(typeof(DataColumn)) => ObjectType.DataColumn,
			_ when type.Implements(typeof(DataRow)) => ObjectType.DataRow,
			_ when type.Implements(typeof(DataRowView)) => ObjectType.DataRowView,
			_ when type.Implements(typeof(DataSet)) => ObjectType.DataSet,
			_ when type.Implements(typeof(DataTable)) => ObjectType.DataTable,
			_ when type.Implements(typeof(IEnumerator)) => ObjectType.Enumerator,
			_ when type.Implements(typeof(Exception)) => ObjectType.Exception,
			_ when type == typeof(JsonArray) => ObjectType.JsonArray,
			_ when type == typeof(JsonDocument) => ObjectType.JsonDocument,
			_ when type == typeof(JsonElement) => ObjectType.JsonElement,
			_ when type == typeof(JsonObject) => ObjectType.JsonObject,
			_ when type.Implements(typeof(JsonValue)) => ObjectType.JsonValue,
			_ when type.Implements(typeof(Lazy<>)) => ObjectType.Lazy,
			_ when type.Implements(typeof(Lazy<,>)) => ObjectType.Lazy,
			_ when type == typeof(Memory<>) => ObjectType.Memory,
			_ when type == typeof(Nullable<>) => ObjectType.Nullable,
			_ when type == typeof(object) => ObjectType.Object,
			_ when type.Implements(typeof(IObservable<>)) => ObjectType.Observable,
			_ when type.Implements(typeof(IObserver<>)) => ObjectType.Observer,
			_ when type == typeof(Range) => ObjectType.Range,
			_ when type == typeof(ReadOnlyMemory<>) => ObjectType.ReadOnlyMemory,
			_ when type == typeof(ReadOnlySpan<>) => ObjectType.ReadOnlySpan,
			_ when type == typeof(Span<>) => ObjectType.Span,
			_ when type.Implements(typeof(Stream)) => ObjectType.Stream,
			_ when type.Implements(typeof(StringBuilder)) => ObjectType.StringBuilder,
			_ when type.Implements(typeof(Task)) => ObjectType.Task,
			_ when type.Implements(typeof(IAsyncResult)) => ObjectType.AsyncResult,
			_ when type.Implements(typeof(ITuple)) => type.IsValueType ? ObjectType.ValueTuple : ObjectType.Tuple,
			_ when type.Implements(typeof(Type)) => ObjectType.Type,
			_ when type == typeof(ValueTask) => ObjectType.ValueTask,
			_ when type == typeof(ValueTask<>) => ObjectType.ValueTask,
			_ when type == typeof(void) => ObjectType.Void,
			_ when type.Implements(typeof(WeakReference)) => ObjectType.WeakReference,
			_ when type.Implements(typeof(WeakReference<>)) => ObjectType.WeakReference,
			_ when type.Implements(typeof(IEnumerable)) => ObjectType.Enumerable,
			_ => ObjectType.Unknown
		};
}
