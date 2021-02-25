// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Numerics;
using System.Text.Json;
using System.Threading.Tasks;
using TypeCache.Collections;
using TypeCache.Collections.Extensions;
using TypeCache.Reflection.Extensions;

namespace TypeCache.Reflection
{
	public static class MemberCache
	{
		static MemberCache()
		{
			Constructors = new LazyDictionary<RuntimeTypeHandle, IImmutableList<ConstructorMember>>(MemberFactory.CreateConstructorMembers);
			Fields = new LazyDictionary<RuntimeTypeHandle, IImmutableDictionary<string, FieldMember>>(MemberFactory.CreateFieldMembers);
			Indexers = new LazyDictionary<RuntimeTypeHandle, IImmutableList<IndexerMember>>(MemberFactory.CreateIndexerMembers);
			Methods = new LazyDictionary<RuntimeTypeHandle, IImmutableDictionary<string, IImmutableList<MethodMember>>>(MemberFactory.CreateMethodMembers);
			Properties = new LazyDictionary<RuntimeTypeHandle, IImmutableDictionary<string, PropertyMember>>(MemberFactory.CreatePropertyMembers);
			StaticFields = new LazyDictionary<RuntimeTypeHandle, IImmutableDictionary<string, StaticFieldMember>>(MemberFactory.CreateStaticFieldMembers);
			StaticMethods = new LazyDictionary<RuntimeTypeHandle, IImmutableDictionary<string, IImmutableList<StaticMethodMember>>>(MemberFactory.CreateStaticMethodMembers);
			StaticProperties = new LazyDictionary<RuntimeTypeHandle, IImmutableDictionary<string, StaticPropertyMember>>(MemberFactory.CreateStaticPropertyMembers);
			Types = new LazyDictionary<RuntimeTypeHandle, TypeMember>(typeHandle => MemberFactory.CreateTypeMember(typeHandle.ToType()));
			SystemTypeMap = new Dictionary<RuntimeTypeHandle, SystemType>
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
				{ typeof(Func<,,,,,,,,,,,,,,,,>).TypeHandle, SystemType.Func}
			}.ToImmutable();
		}

		public static IReadOnlyDictionary<RuntimeTypeHandle, IImmutableList<ConstructorMember>> Constructors { get; }

		public static IReadOnlyDictionary<RuntimeTypeHandle, IImmutableDictionary<string, FieldMember>> Fields { get; }

		public static IReadOnlyDictionary<RuntimeTypeHandle, IImmutableList<IndexerMember>> Indexers { get; }

		public static IReadOnlyDictionary<RuntimeTypeHandle, IImmutableDictionary<string, IImmutableList<MethodMember>>> Methods { get; }

		public static IReadOnlyDictionary<RuntimeTypeHandle, IImmutableDictionary<string, PropertyMember>> Properties { get; }

		public static IReadOnlyDictionary<RuntimeTypeHandle, IImmutableDictionary<string, StaticFieldMember>> StaticFields { get; }

		public static IReadOnlyDictionary<RuntimeTypeHandle, IImmutableDictionary<string, IImmutableList<StaticMethodMember>>> StaticMethods { get; }

		public static IReadOnlyDictionary<RuntimeTypeHandle, IImmutableDictionary<string, StaticPropertyMember>> StaticProperties { get; }

		public static IReadOnlyDictionary<RuntimeTypeHandle, TypeMember> Types { get; }

		public static IImmutableDictionary<RuntimeTypeHandle, SystemType> SystemTypeMap { get; }
	}
}
