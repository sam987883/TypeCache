// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Tasks;
using TypeCache.Collections.Extensions;
using TypeCache.Extensions;

namespace TypeCache.Reflection.Extensions
{
	public static class TypeExtensions
	{
		private static IDictionary<RuntimeTypeHandle, CollectionType> _GenericCollectionTypeMap =
			new Dictionary<RuntimeTypeHandle, CollectionType>
			{
				{ typeof(Dictionary<,>).TypeHandle, CollectionType.Dictionary},
				{ typeof(HashSet<>).TypeHandle, CollectionType.HashSet},
				{ typeof(ImmutableArray<>).TypeHandle, CollectionType.ImmutableArray},
				{ typeof(ImmutableDictionary<,>).TypeHandle, CollectionType.ImmutableDictionary},
				{ typeof(ImmutableHashSet<>).TypeHandle, CollectionType.ImmutableHashSet},
				{ typeof(ImmutableList<>).TypeHandle, CollectionType.ImmutableList},
				{ typeof(ImmutableQueue<>).TypeHandle, CollectionType.ImmutableQueue},
				{ typeof(ImmutableSortedDictionary<,>).TypeHandle, CollectionType.ImmutableSortedDictionary},
				{ typeof(ImmutableSortedSet<>).TypeHandle, CollectionType.ImmutableSortedSet},
				{ typeof(ImmutableStack<>).TypeHandle, CollectionType.ImmutableStack},
				{ typeof(LinkedList<>).TypeHandle, CollectionType.LinkedList},
				{ typeof(List<>).TypeHandle, CollectionType.List},
				{ typeof(Queue<>).TypeHandle, CollectionType.Queue},
				{ typeof(SortedDictionary<,>).TypeHandle, CollectionType.SortedDictionary},
				{ typeof(SortedList<,>).TypeHandle, CollectionType.SortedList},
				{ typeof(SortedSet<>).TypeHandle, CollectionType.SortedSet},
				{ typeof(Stack<>).TypeHandle, CollectionType.Stack}
			};

		private static IDictionary<RuntimeTypeHandle, NativeType> _NativeTypeMap =
			new Dictionary<RuntimeTypeHandle, NativeType>
			{
				{ typeof(bool).TypeHandle, NativeType.Boolean},
				{ typeof(sbyte).TypeHandle, NativeType.SByte},
				{ typeof(byte).TypeHandle, NativeType.Byte},
				{ typeof(short).TypeHandle, NativeType.Int16},
				{ typeof(ushort).TypeHandle, NativeType.UInt16},
				{ typeof(int).TypeHandle, NativeType.Int32},
				{ typeof(uint).TypeHandle, NativeType.UInt32},
				{ typeof(long).TypeHandle, NativeType.Int64},
				{ typeof(ulong).TypeHandle, NativeType.UInt64},
				{ typeof(float).TypeHandle, NativeType.Single},
				{ typeof(double).TypeHandle, NativeType.Double},
				{ typeof(decimal).TypeHandle, NativeType.Decimal},
				{ typeof(char).TypeHandle, NativeType.Char},
				{ typeof(DateTime).TypeHandle, NativeType.DateTime},
				{ typeof(DateTimeOffset).TypeHandle, NativeType.DateTimeOffset},
				{ typeof(TimeSpan).TypeHandle, NativeType.TimeSpan},
				{ typeof(Guid).TypeHandle, NativeType.Guid},
				{ typeof(Index).TypeHandle, NativeType.Index},
				{ typeof(Range).TypeHandle, NativeType.Range},
				{ typeof(JsonElement).TypeHandle, NativeType.JsonElement},
				{ typeof(string).TypeHandle, NativeType.String},
				{ typeof(DBNull).TypeHandle, NativeType.DBNull},
				{ typeof(Uri).TypeHandle, NativeType.Uri}
			};

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Any(this Type? @this, params Type[] types)
			=> types.Any(@this.Is!);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string GetName(this Type @this)
			=> @this.GetCustomAttribute<NameAttribute>()?.Name ?? @this.Name;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static object Create(this Type @this, params object[] arguments)
		{
			@this.IsClass.Assert(nameof(@this.IsClass), true);

			var parameter = nameof(arguments).Parameter<object[]>();
			var type = typeof(TypeOf<>).MakeGenericType(@this);
			var invoke = type.CallStatic(nameof(TypeOf<object>.Create), Type.EmptyTypes, parameter).Lambda<Func<object[], object>>().Compile();
			return invoke(arguments);
		}

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

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Implements(this Type @this, Type type)
			=> @this.BaseType.Is(type) || @this.GetInterfaces().Any(_ => _.Is(type));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Is<T>(this Type? @this)
			=> @this.Is(typeof(T));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Is<T>(this RuntimeTypeHandle @this)
			=> @this.Equals(typeof(T).TypeHandle);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Is(this Type? @this, Type type)
			=> @this == type || (@this?.IsGenericTypeDefinition == true && @this == type.ToGenericType());

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsAsync(this Type @this)
			=> @this.IsTask() || @this.IsValueTask() || @this.Is(typeof(IAsyncEnumerable<>));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsNullable(this Type @this)
			=> @this.ToGenericType() == typeof(Nullable<>);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsTask(this Type @this)
			=> @this.Is<Task>() || @this.ToGenericType() == typeof(Task<>);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsValueTask(this Type @this)
			=> @this.Is<ValueTask>() || @this.ToGenericType() == typeof(ValueTask<>);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsVoid(this Type @this)
			=> @this == typeof(void) || @this == typeof(Task) || @this == typeof(ValueTask);

		public static CollectionType ToCollectionType(this Type @this)
			=> @this switch
			{
				_ when @this.IsNullable() || @this.IsTask() || @this.IsValueTask() => @this.GenericTypeArguments[0].ToCollectionType(),
				_ when @this == typeof(string) => CollectionType.None,
				_ when @this.IsArray => CollectionType.Array,
				_ when @this.Implements(typeof(IEnumerable)) => _GenericCollectionTypeMap.GetValue(@this.TypeHandle) ?? CollectionType.Enumerable,
				_ => CollectionType.None
			};

		public static Type? ToGenericType(this Type? @this)
			=> @this switch
			{
				null => null,
				_ when @this.IsGenericTypeDefinition => @this,
				_ when @this.IsGenericType => @this.GetGenericTypeDefinition(),
				_ => null
			};

		public static NativeType ToNativeType(this Type @this)
			=> @this switch
			{
				_ when @this.IsNullable() || @this.IsTask() || @this.IsValueTask() => @this.GenericTypeArguments[0].ToNativeType(),
				_ when @this.IsEnum => NativeType.Enum,
				_ when _NativeTypeMap.TryGetValue(@this.TypeHandle, out var nativeType) => nativeType,
				_ when @this.IsValueType => NativeType.ValueType,
				_ => NativeType.Object
			};

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Type ToType(this RuntimeTypeHandle @this)
			=> Type.GetTypeFromHandle(@this);
	}
}
