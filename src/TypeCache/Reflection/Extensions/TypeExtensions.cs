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

namespace TypeCache.Reflection.Extensions
{
	public static class TypeExtensions
	{
		private static IDictionary<RuntimeTypeHandle, CollectionType> CollectionGenericTypeMap =
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

		private static IDictionary<RuntimeTypeHandle, NativeType> NativeTypeMap =
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
				{ typeof(IntPtr).TypeHandle, NativeType.IntPtr},
				{ typeof(UIntPtr).TypeHandle, NativeType.UIntPtr},
				{ typeof(BigInteger).TypeHandle, NativeType.BigInteger},
				{ typeof(float).TypeHandle, NativeType.Single},
				{ typeof(double).TypeHandle, NativeType.Double},
				{ typeof(Half).TypeHandle, NativeType.Half},
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
				{ typeof(Uri).TypeHandle, NativeType.Uri},
				{ typeof(void).TypeHandle, NativeType.Void}
			};

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Any(this Type? @this, params Type[] types)
			=> types.Any(@this.Is!);

		public static object Create(this Type @this, params object[] parameters)
			=> @this.GetConstructorCache().FirstValue(constructor => constructor!.IsCallableWith(parameters))?.Invoke(parameters)
				?? throw new ArgumentException($"Create instance of class {@this.Name} failed with {parameters?.Length ?? 0} parameters.");

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string GetName(this Type @this)
			=> @this.GetCustomAttribute<NameAttribute>()?.Name ?? @this.Name;

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

		public static bool Implements(this Type @this, Type type)
			=> type.IsInterface ? @this.GetInterfaces().Any(_ => _.Is(type)) : @this.BaseType.Is(type);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Is<T>(this Type? @this)
			=> @this == typeof(T);

		public static bool Is(this Type? @this, Type type)
			=> type.IsGenericTypeDefinition ? @this.ToGenericType() == type : @this == type;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsAsync(this Type @this)
			=> @this.IsTask() || @this.IsValueTask() || @this.Is(typeof(IAsyncEnumerable<>));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsLazy(this Type @this)
			=> @this.ToGenericType() == typeof(Lazy<>);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsNullable(this Type @this)
			=> @this.ToGenericType() == typeof(Nullable<>);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsTask(this Type @this)
			=> @this.Is<Task>() || @this.ToGenericType() == typeof(Task<>);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsValueTask(this Type @this)
			=> @this.Is<ValueTask>() || @this.ToGenericType() == typeof(ValueTask<>);

		public static bool IsVoid(this Type @this)
			=> @this == typeof(void) || @this == typeof(Task) || @this == typeof(ValueTask);

		public static CollectionType ToCollectionType(this Type @this)
			=> @this switch
			{
				_ when @this.IsNullable() || @this.IsTask() || @this.IsValueTask() => @this.GenericTypeArguments[0].ToCollectionType(),
				_ when @this == typeof(string) => CollectionType.None,
				_ when @this.IsArray => CollectionType.Array,
				_ when @this.Implements(typeof(IEnumerable)) => CollectionGenericTypeMap.GetValue(@this.TypeHandle) ?? CollectionType.Enumerable,
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

		public static Kind ToKind(this Type @this)
			=> @this switch
			{
				_ when typeof(Delegate).IsAssignableFrom(@this.BaseType) => Kind.Delegate,
				_ when @this.IsEnum => Kind.Enum,
				_ when @this.IsInterface => Kind.Interface,
				_ when @this.IsValueType => Kind.Struct,
				_ => Kind.Class,
			};

		public static NativeType ToNativeType(this Type @this)
			=> @this switch
			{
				_ when NativeTypeMap.TryGetValue(@this.TypeHandle, out var nativeType) => nativeType,
				_ when @this.ToKind() == Kind.Enum => @this.GetEnumUnderlyingType().ToNativeType(),
				_ when @this.IsLazy() => @this.GenericTypeArguments[0].ToNativeType(),
				_ when @this.IsNullable() => @this.GenericTypeArguments[0].ToNativeType(),
				_ when @this.IsTask() => @this.GenericTypeArguments[0].ToNativeType(),
				_ when @this.IsValueTask() => @this.GenericTypeArguments[0].ToNativeType(),
				_ when @this.IsArray && @this.IsGenericType => @this.GenericTypeArguments[0].ToNativeType(),
				_ => NativeType.None
			};
	}
}
