// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using GraphQL;
using GraphQL.Types;
using TypeCache.Collections.Extensions;
using TypeCache.Extensions;
using TypeCache.GraphQL.Attributes;
using TypeCache.GraphQL.SQL;
using TypeCache.GraphQL.Types;
using TypeCache.Reflection;
using TypeCache.Reflection.Extensions;
using static TypeCache.Default;

namespace TypeCache.GraphQL.Extensions;

public static class GraphQLAttributeExtensions
{
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static string? GraphDescription(this IMember @this)
		=> @this.Attributes.First<GraphQLDescriptionAttribute>()?.Description;

	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static string? GraphDescription(this MethodParameter @this)
		=> @this.Attributes.First<GraphQLDescriptionAttribute>()?.Description;

	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static bool GraphIgnore(this IMember @this)
		=> @this.Attributes.Any<GraphQLIgnoreAttribute>();

	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static bool GraphIgnore(this MethodParameter @this)
		=> @this.Attributes.Any<GraphQLIgnoreAttribute>() || @this.Type.Handle.Is<IResolveFieldContext>();

	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static string? GraphKey(this IMember @this)
		=> @this.Attributes.First<GraphQLKeyAttribute>()?.Name;

	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static string GraphInputName(this TypeMember @this)
		=> @this.Attributes.First<GraphQLInputNameAttribute>()?.Name ?? $"{@this.Name}Input";

	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static string GraphName(this IMember @this)
		=> @this.Attributes.First<GraphQLNameAttribute>()?.Name ?? @this.Name;

	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static string? GraphName(this IEnumerable<Attribute> @this)
		=> @this.First<GraphQLNameAttribute>()?.Name;

	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static string GraphName(this MethodMember @this)
		=> @this.Attributes.First<GraphQLNameAttribute>()?.Name ?? @this.Name.TrimEnd("Async");

	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static string GraphName(this MethodParameter @this)
		=> @this.Attributes.First<GraphQLNameAttribute>()?.Name ?? @this.Name;

	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static Type? GraphType(this IEnumerable<Attribute> @this)
		=> @this.First<GraphQLTypeAttribute>()?.GraphType;

	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static Type GraphType(this MethodParameter @this)
		=> @this.Attributes.GraphType() ?? @this.Type.GraphType(true, @this.Attributes.Any<NotNullAttribute>());

	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static Type GraphType(this PropertyMember @this, bool isInputType)
		=> @this.Attributes.GraphType() ?? @this.PropertyType.GraphType(isInputType, @this.Attributes.Any<NotNullAttribute>());

	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static Type GraphType(this ReturnParameter @this)
		=> @this.Attributes.GraphType() ?? @this.Type!.GraphType(false, @this.Attributes.Any<NotNullAttribute>());

	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static Type GraphType(this ScalarType @this)
		=> ScalarGraphTypes.TryGetValue(@this, out var handle) ? handle.ToType() : typeof(StringGraphType);

	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static string? ObsoleteMessage(this IMember @this)
		=> @this.Attributes.First<ObsoleteAttribute>()?.Message;

	internal static Type GraphType(this TypeMember @this, bool isInputType, bool isNotNull)
		=> (@this.Kind, @this.SystemType) switch
		{
			(_, SystemType.Object) => throw new ArgumentOutOfRangeException($"{nameof(TypeMember)}.{nameof(@this.SystemType)}", $"No custom graph type was found that supports: {@this.SystemType.Name()}"),
			(Kind.Delegate, _) or (Kind.Pointer, _) => throw new ArgumentOutOfRangeException($"{nameof(TypeMember)}.{nameof(@this.Kind)}", $"No custom graph type was found that supports: {@this.Kind.Name()}"),
			(Kind.Enum, _) => typeof(NonNullGraphType<>).MakeGenericType(typeof(GraphQLEnumType<>).MakeGenericType(@this)),
			(_, SystemType.Nullable) when @this.GenericTypes.First()!.Kind == Kind.Enum => typeof(GraphQLEnumType<>).MakeGenericType(@this),
			(_, SystemType.Nullable) => SystemGraphTypes[@this.GenericTypes.First()!.SystemType].ToType(),
			(_, SystemType.ValueTask) or (_, SystemType.Task) => @this.GenericTypes.First()!.GraphType(isInputType, isNotNull),
			_ when SystemGraphTypes.TryGetValue(@this.SystemType, out var handle) => typeof(NonNullGraphType<>).MakeGenericType(handle.ToType()),
			_ when @this.SystemType.IsCollection() && isNotNull => typeof(NonNullGraphType<>).MakeGenericType(typeof(ListGraphType<>).MakeGenericType((Type)@this.CollectionType()!.GraphType(isInputType, false))),
			_ when @this.SystemType.IsCollection() => typeof(ListGraphType<>).MakeGenericType((Type)@this.CollectionType()!.GraphType(isInputType, false)),
			_ when @this.Is(typeof(OrderBy<>)) && isNotNull => typeof(NonNullGraphType<>).MakeGenericType(typeof(GraphQLOrderByType<>).MakeGenericType(@this.GenericTypes.First()!)),
			_ when @this.Is(typeof(OrderBy<>)) => typeof(GraphQLOrderByType<>).MakeGenericType(@this.GenericTypes.First()!),
			(Kind.Interface, _) when isNotNull => typeof(NonNullGraphType<>).MakeGenericType(typeof(GraphQLInterfaceType<>).MakeGenericType(@this)),
			(Kind.Interface, _) => typeof(GraphQLInterfaceType<>).MakeGenericType(@this),
			_ when isInputType && isNotNull => typeof(NonNullGraphType<>).MakeGenericType(typeof(GraphQLInputType<>).MakeGenericType(@this)),
			_ when isInputType => typeof(GraphQLInputType<>).MakeGenericType(@this),
			_ when isNotNull => typeof(NonNullGraphType<>).MakeGenericType(typeof(GraphQLObjectType<>).MakeGenericType(@this)),
			_ => typeof(GraphQLObjectType<>).MakeGenericType(@this)
		};

	private static readonly IImmutableDictionary<ScalarType, RuntimeTypeHandle> ScalarGraphTypes =
		new Dictionary<ScalarType, RuntimeTypeHandle>(EnumOf<ScalarType>.Tokens.Count, EnumOf<ScalarType>.Comparer)
	{
		{ ScalarType.ID, typeof(IdGraphType).TypeHandle },
		{ ScalarType.HashID, typeof(GraphQLHashIdType).TypeHandle },
		{ ScalarType.Boolean, typeof(BooleanGraphType).TypeHandle },
		{ ScalarType.SByte, typeof(SByteGraphType).TypeHandle },
		{ ScalarType.Short, typeof(ShortGraphType).TypeHandle },
		{ ScalarType.Int, typeof(IntGraphType).TypeHandle },
		{ ScalarType.Long, typeof(LongGraphType).TypeHandle },
		{ ScalarType.Byte, typeof(ByteGraphType).TypeHandle },
		{ ScalarType.UShort, typeof(UShortGraphType).TypeHandle },
		{ ScalarType.UInt, typeof(UIntGraphType).TypeHandle },
		{ ScalarType.ULong, typeof(ULongGraphType).TypeHandle },
		{ ScalarType.BigInteger, typeof(BigIntGraphType).TypeHandle },
		{ ScalarType.Float, typeof(FloatGraphType).TypeHandle },
		{ ScalarType.Decimal, typeof(DecimalGraphType).TypeHandle },
		{ ScalarType.Date, typeof(DateGraphType).TypeHandle },
		{ ScalarType.DateTime, typeof(DateTimeGraphType).TypeHandle },
		{ ScalarType.DateTimeOffset, typeof(DateTimeOffsetGraphType).TypeHandle },
		{ ScalarType.TimeSpanMilliseconds, typeof(TimeSpanMillisecondsGraphType).TypeHandle },
		{ ScalarType.TimeSpanSeconds, typeof(TimeSpanSecondsGraphType).TypeHandle },
		{ ScalarType.Guid, typeof(GuidGraphType).TypeHandle },
		{ ScalarType.String, typeof(StringGraphType).TypeHandle },
		{ ScalarType.Uri, typeof(UriGraphType).TypeHandle },
		{ ScalarType.NotNullID, typeof(NonNullGraphType<IdGraphType>).TypeHandle },
		{ ScalarType.NotNullHashID, typeof(NonNullGraphType<GraphQLHashIdType>).TypeHandle },
		{ ScalarType.NotNullBoolean, typeof(NonNullGraphType<BooleanGraphType>).TypeHandle },
		{ ScalarType.NotNullSByte, typeof(NonNullGraphType<SByteGraphType>).TypeHandle },
		{ ScalarType.NotNullShort, typeof(NonNullGraphType<ShortGraphType>).TypeHandle },
		{ ScalarType.NotNullInt, typeof(NonNullGraphType<IntGraphType>).TypeHandle },
		{ ScalarType.NotNullLong, typeof(NonNullGraphType<LongGraphType>).TypeHandle },
		{ ScalarType.NotNullByte, typeof(NonNullGraphType<ByteGraphType>).TypeHandle },
		{ ScalarType.NotNullUShort, typeof(NonNullGraphType<UShortGraphType>).TypeHandle },
		{ ScalarType.NotNullUInt, typeof(NonNullGraphType<UIntGraphType>).TypeHandle },
		{ ScalarType.NotNullULong, typeof(NonNullGraphType<ULongGraphType>).TypeHandle },
		{ ScalarType.NotNullBigInteger, typeof(NonNullGraphType<BigIntGraphType>).TypeHandle },
		{ ScalarType.NotNullFloat, typeof(NonNullGraphType<FloatGraphType>).TypeHandle },
		{ ScalarType.NotNullDecimal, typeof(NonNullGraphType<DecimalGraphType>).TypeHandle },
		{ ScalarType.NotNullDate, typeof(NonNullGraphType<DateGraphType>).TypeHandle },
		{ ScalarType.NotNullDateTime, typeof(NonNullGraphType<DateTimeGraphType>).TypeHandle },
		{ ScalarType.NotNullDateTimeOffset, typeof(NonNullGraphType<DateTimeOffsetGraphType>).TypeHandle },
		{ ScalarType.NotNullTimeSpanMilliseconds, typeof(NonNullGraphType<TimeSpanMillisecondsGraphType>).TypeHandle },
		{ ScalarType.NotNullTimeSpanSeconds, typeof(NonNullGraphType<TimeSpanSecondsGraphType>).TypeHandle },
		{ ScalarType.NotNullGuid, typeof(NonNullGraphType<GuidGraphType>).TypeHandle },
		{ ScalarType.NotNullString, typeof(NonNullGraphType<StringGraphType>).TypeHandle },
		{ ScalarType.NotNullUri, typeof(NonNullGraphType<UriGraphType>).TypeHandle },
	}.ToImmutableDictionary();

	private static readonly IImmutableDictionary<SystemType, RuntimeTypeHandle> SystemGraphTypes =
		new Dictionary<SystemType, RuntimeTypeHandle>(22, EnumOf<SystemType>.Comparer)
	{
		{ SystemType.String, typeof(StringGraphType).TypeHandle },
		{ SystemType.Uri, typeof(UriGraphType).TypeHandle },
		{ SystemType.Boolean, typeof(BooleanGraphType).TypeHandle },
		{ SystemType.SByte, typeof(SByteGraphType).TypeHandle },
		{ SystemType.Int16, typeof(ShortGraphType).TypeHandle },
		{ SystemType.Int32, typeof(IntGraphType).TypeHandle },
		{ SystemType.Index, typeof(IntGraphType).TypeHandle },
		{ SystemType.Int64, typeof(LongGraphType).TypeHandle },
		{ SystemType.IntPtr, typeof(LongGraphType).TypeHandle },
		{ SystemType.Byte, typeof(ByteGraphType).TypeHandle },
		{ SystemType.UInt16, typeof(UShortGraphType).TypeHandle },
		{ SystemType.UInt32, typeof(UIntGraphType).TypeHandle },
		{ SystemType.UInt64, typeof(ULongGraphType).TypeHandle },
		{ SystemType.UIntPtr, typeof(ULongGraphType).TypeHandle },
		{ SystemType.Single, typeof(FloatGraphType).TypeHandle },
		{ SystemType.Double, typeof(FloatGraphType).TypeHandle },
		{ SystemType.Decimal, typeof(DecimalGraphType).TypeHandle },
		{ SystemType.DateTime, typeof(DateTimeGraphType).TypeHandle },
		{ SystemType.DateTimeOffset, typeof(DateTimeOffsetGraphType).TypeHandle },
		{ SystemType.TimeSpan, typeof(TimeSpanSecondsGraphType).TypeHandle },
		{ SystemType.Guid, typeof(GuidGraphType).TypeHandle },
		{ SystemType.Range, typeof(StringGraphType).TypeHandle },
	}.ToImmutableDictionary();
}
