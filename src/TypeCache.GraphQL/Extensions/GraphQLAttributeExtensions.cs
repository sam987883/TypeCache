// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using GraphQL.Types;
using TypeCache.Collections.Extensions;
using TypeCache.Extensions;
using TypeCache.GraphQL.Attributes;
using TypeCache.GraphQL.SQL;
using TypeCache.GraphQL.Types;
using TypeCache.Reflection;
using TypeCache.Reflection.Extensions;
using static System.FormattableString;
using static System.Globalization.CultureInfo;
using static TypeCache.Default;

namespace TypeCache.GraphQL.Extensions;

public static class GraphQLAttributeExtensions
{
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static string? GraphQLDeprecationReason(this Member @this)
		=> @this.Attributes.First<GraphQLDeprecationReasonAttribute>()?.DeprecationReason;

	public static string? GraphQLDescription(this Member @this)
		=> @this.Attributes.First<GraphQLDescriptionAttribute>()?.Description switch
		{
			null => null,
			var description when @this is TypeMember type && type.GenericHandle.HasValue => string.Format(InvariantCulture, description, type.GenericTypes.Map(_ => _.GraphQLName()).ToArray()),
			var description => description
		};

	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static string? GraphQLDescription(this MethodParameter @this)
		=> @this.Attributes.First<GraphQLDescriptionAttribute>()?.Description;

	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static bool GraphQLIgnore(this Member @this)
		=> @this.Attributes.Any<GraphQLIgnoreAttribute>();

	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static bool GraphQLIgnore(this MethodParameter @this)
		=> @this.Attributes.Any<GraphQLIgnoreAttribute>();

	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static string? GraphQLKey(this Member @this)
		=> @this.Attributes.First<GraphQLKeyAttribute>()?.Name;

	public static string GraphQLInputName(this TypeMember @this)
		=> @this.Attributes.First<GraphQLInputNameAttribute>()?.Name switch
		{
			null => Invariant($"{@this.GraphQLName()}Input"),
			var name when @this.GenericHandle.HasValue => string.Format(InvariantCulture, name, @this.GenericTypes.Map(_ => _.Name).ToArray()),
			var name => name
		};

	public static string GraphQLName(this Member @this)
		=> @this switch
		{
			MethodMember method => method.GraphQLName(),
			TypeMember type => type.GraphQLName(),
			_ => @this.Attributes.First<GraphQLNameAttribute>()?.Name ?? @this.Name
		};

	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static string GraphQLName(this MethodMember @this)
		=> @this.Attributes.First<GraphQLNameAttribute>()?.Name ?? @this.Name.TrimEnd("Async");

	public static string GraphQLName(this TypeMember @this)
		=> @this.Attributes.First<GraphQLNameAttribute>()?.Name switch
		{
			null when @this.GenericHandle.HasValue => Invariant($"{@this.GenericTypes.First()!.Name}{@this.Name}"),
			null => @this.Name,
			var name when @this.GenericHandle.HasValue => string.Format(InvariantCulture, name, @this.GenericTypes.Map(type => type.Name).ToArray()),
			var name => name
		};

	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static string GraphQLName(this MethodParameter @this)
		=> @this.Attributes.First<GraphQLNameAttribute>()?.Name ?? @this.Name;

	public static Type GraphQLType(this MethodParameter @this)
		=> @this.Attributes.First<GraphQLTypeAttribute>()?.GraphType switch
		{
			null => @this.Type!.GraphQLType(true) switch
			{
				var type when !type.Is(typeof(NonNullGraphType<>)) && @this.Attributes.Any<NotNullAttribute>() => type.ToNonNullGraphType(),
				var type => type
			},
			var type => type
		};

	public static Type GraphQLType(this PropertyMember @this, bool isInputType)
		=> @this.Attributes.First<GraphQLTypeAttribute>()?.GraphType switch
		{
			null => @this.PropertyType.GraphQLType(isInputType) switch
			{
				var type when !type.Is(typeof(NonNullGraphType<>)) && @this.Attributes.Any<NotNullAttribute>() => type.ToNonNullGraphType(),
				var type => type
			},
			var type => type
		};

	public static Type GraphQLType(this ReturnParameter @this)
		=> @this.Attributes.First<GraphQLTypeAttribute>()?.GraphType switch
		{
			null => @this.Type!.GraphQLType(false) switch
			{
				var type when !type.Is(typeof(NonNullGraphType<>)) && @this.Attributes.Any<NotNullAttribute>() => type.ToNonNullGraphType(),
				var type => type
			},
			var type => type
		};

	internal static Type GraphQLType(this TypeMember @this, bool isInputType)
		=> @this switch
		{
			{ Kind: Kind.Delegate or Kind.Pointer } => throw new ArgumentOutOfRangeException($"{nameof(TypeMember)}.{nameof(@this.Kind)}", $"No custom graph type was found that supports: {@this.Kind.Name()}"),
			{ SystemType: SystemType.Object } => throw new ArgumentOutOfRangeException($"{nameof(TypeMember)}.{nameof(@this.SystemType)}", $"No custom graph type was found that supports: {@this.SystemType.Name()}"),
			{ SystemType: SystemType.Nullable } => @this.GenericTypes.First()! switch
			{
				{ Kind: Kind.Enum } => typeof(GraphQLEnumType<>).MakeGenericType(@this),
				var genericTypeMember => SystemGraphTypes[genericTypeMember.SystemType].ToType()
			},
			_ when @this.SystemType.IsCollection() => @this.CollectionType()!.GraphQLType(isInputType).ToListGraphType(),
			_ when @this.Is(typeof(OrderBy<>)) => typeof(GraphQLOrderByType<>).MakeGenericType(@this.GenericTypes.First()!),
			{ SystemType: SystemType.ValueTask or SystemType.Task } => @this.GenericTypes.First()!.GraphQLType(isInputType),
			{ Kind: Kind.Enum } => typeof(GraphQLEnumType<>).MakeGenericType(@this).ToNonNullGraphType(),
			{ Kind: Kind.Class } when SystemGraphTypes.TryGetValue(@this.SystemType, out var handle) => handle.ToType(),
			{ Kind: Kind.Struct } when SystemGraphTypes.TryGetValue(@this.SystemType, out var handle) => handle.ToType().ToNonNullGraphType(),
			{ Kind: Kind.Interface } => typeof(GraphQLInterfaceType<>).MakeGenericType(@this),
			_ when isInputType => ((Type)@this).ToGraphQLInputType(),
			_ => ((Type)@this).ToGraphQLObjectType()
		};

	private static readonly IReadOnlyDictionary<SystemType, RuntimeTypeHandle> SystemGraphTypes = new Dictionary<SystemType, RuntimeTypeHandle>(22, EnumOf<SystemType>.Comparer)
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
		{ SystemType.BigInteger, typeof(BigIntGraphType).TypeHandle },
		{ SystemType.Single, typeof(FloatGraphType).TypeHandle },
		{ SystemType.Double, typeof(FloatGraphType).TypeHandle },
		{ SystemType.Decimal, typeof(DecimalGraphType).TypeHandle },
		{ SystemType.DateOnly, typeof(DateOnlyGraphType).TypeHandle },
		{ SystemType.DateTime, typeof(DateTimeGraphType).TypeHandle },
		{ SystemType.DateTimeOffset, typeof(DateTimeOffsetGraphType).TypeHandle },
		{ SystemType.TimeOnly, typeof(TimeOnlyGraphType).TypeHandle },
		{ SystemType.TimeSpan, typeof(TimeSpanSecondsGraphType).TypeHandle },
		{ SystemType.Guid, typeof(GuidGraphType).TypeHandle },
		{ SystemType.Range, typeof(StringGraphType).TypeHandle },
	}.ToImmutableDictionary();
}
