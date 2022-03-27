// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
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
	public static string? GraphQLDescription(this Member @this)
		=> @this switch
		{
			TypeMember type => type.GraphQLDescription(),
			_ => @this.Attributes.First<GraphQLDescriptionAttribute>()?.Description
		};

	public static string? GraphQLDescription(this TypeMember @this)
		=> @this.Attributes.First<GraphQLDescriptionAttribute>()?.Description switch
		{
			null => null,
			var description when @this.GenericHandle.HasValue => string.Format(InvariantCulture, description, @this.GenericTypes.Map(type => type.GraphQLName()).ToArray()),
			var description => description
		};

	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static string? GraphQLDescription(this MethodParameter @this)
		=> @this.Attributes.First<GraphQLDescriptionAttribute>()?.Description;

	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static bool GraphQLIgnore(this Member @this)
		=> @this.Attributes.Any<GraphQLIgnoreAttribute>();

	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static bool GraphQLIgnore(this MethodParameter @this)
		=> @this.Attributes.Any<GraphQLIgnoreAttribute>();

	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static string? GraphQLKey(this Member @this)
		=> @this.Attributes.First<GraphQLKeyAttribute>()?.Name;

	public static string GraphQLInputName(this TypeMember @this)
		=> @this.Attributes.First<GraphQLNameAttribute>()?.Name
			?? (@this.GenericHandle.HasValue
				? string.Format(InvariantCulture, @this.Name, @this.GenericTypes.Map(_ => _.Name).ToArray())
				: Invariant($"{@this.Name}Input"));

	public static string GraphQLName(this Member @this)
		=> @this switch
		{
			MethodMember method => method.GraphQLName(),
			TypeMember type => type.GraphQLName(),
			_ => @this.Attributes.First<GraphQLNameAttribute>()?.Name ?? @this.Name
		};

	[MethodImpl(METHOD_IMPL_OPTIONS)]
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

	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static string GraphQLName(this MethodParameter @this)
		=> @this.Attributes.First<GraphQLNameAttribute>()?.Name ?? @this.Name;

	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static Type? GraphQLType(this IEnumerable<Attribute> @this)
		=> @this.First<GraphQLTypeAttribute>()?.GraphType;

	public static Type GraphQLType(this MethodParameter @this)
		=> @this.Attributes.GraphQLType()
			?? (@this.Attributes.Any<NotNullAttribute>()
				? @this.Type!.GraphQLType(true).GraphQLNonNull()
				: @this.Type!.GraphQLType(true));

	public static Type GraphQLType(this PropertyMember @this, bool isInputType)
		=> @this.Attributes.GraphQLType()
			?? (@this.Attributes.Any<NotNullAttribute>()
				? @this.PropertyType.GraphQLType(isInputType).GraphQLNonNull()
				: @this.PropertyType.GraphQLType(isInputType));

	public static Type GraphQLType(this ReturnParameter @this)
		=> @this.Attributes.GraphQLType()
			?? (@this.Attributes.Any<NotNullAttribute>()
				? @this.Type!.GraphQLType(false).GraphQLNonNull()
				: @this.Type!.GraphQLType(false));

	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static string? ObsoleteMessage(this Member @this)
		=> @this.Attributes.First<ObsoleteAttribute>()?.Message;

	[MethodImpl(METHOD_IMPL_OPTIONS)]
	internal static Type GraphQLList(this Type @this)
		=> typeof(ListGraphType<>).MakeGenericType(@this);

	[MethodImpl(METHOD_IMPL_OPTIONS)]
	internal static Type GraphQLNonNull(this Type @this)
		=> typeof(NonNullGraphType<>).MakeGenericType(@this);

	internal static Type GraphQLType(this TypeMember @this, bool isInputType)
	{
		var objectGraphQLType = isInputType ? typeof(GraphQLInputType<>) : typeof(GraphQLObjectType<>);
		return getGraphQLType(@this);

		Type getGraphQLType(TypeMember typeMember)
			=> typeMember switch
			{
				{ Kind: Kind.Delegate or Kind.Pointer } => throw new ArgumentOutOfRangeException($"{nameof(TypeMember)}.{nameof(@this.Kind)}", $"No custom graph type was found that supports: {@this.Kind.Name()}"),
				{ SystemType: SystemType.Object } => throw new ArgumentOutOfRangeException($"{nameof(TypeMember)}.{nameof(@this.SystemType)}", $"No custom graph type was found that supports: {@this.SystemType.Name()}"),
				{ SystemType: SystemType.Nullable } => typeMember.GenericTypes.First()! switch
				{
					{ Kind: Kind.Enum } => typeof(GraphQLEnumType<>).MakeGenericType(typeMember),
					var genericTypeMember => SystemGraphTypes[genericTypeMember.SystemType].ToType()
				},
				_ when typeMember.SystemType.IsCollection() => typeof(ListGraphType<>).MakeGenericType(getGraphQLType(typeMember.CollectionType()!)),
				_ when typeMember.Is(typeof(OrderBy<>)) => typeof(GraphQLOrderByType<>).MakeGenericType(typeMember.GenericTypes.First()!),
				{ SystemType: SystemType.ValueTask or SystemType.Task } => getGraphQLType(typeMember.GenericTypes.First()!),
				{ Kind: Kind.Enum } => typeof(GraphQLEnumType<>).MakeGenericType(typeMember).GraphQLNonNull(),
				{ Kind: Kind.Class } when SystemGraphTypes.TryGetValue(typeMember.SystemType, out var handle) => handle.ToType(),
				{ Kind: Kind.Struct } when SystemGraphTypes.TryGetValue(typeMember.SystemType, out var handle) => handle.ToType().GraphQLNonNull(),
				{ Kind: Kind.Interface } => typeof(GraphQLInterfaceType<>).MakeGenericType(typeMember),
				_ => objectGraphQLType.MakeGenericType(typeMember)
			};
	}

	private static readonly IReadOnlyDictionary<SystemType, RuntimeTypeHandle> SystemGraphTypes =
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
