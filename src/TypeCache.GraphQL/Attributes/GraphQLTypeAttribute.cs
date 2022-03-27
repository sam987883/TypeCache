// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using GraphQL;
using GraphQL.Types;
using TypeCache.Extensions;
using TypeCache.GraphQL.Extensions;
using TypeCache.GraphQL.Types;
using TypeCache.Reflection.Extensions;

namespace TypeCache.GraphQL.Attributes;

/// <summary>
/// <b>GraphQL</b>
/// <list type="number">
/// <item><term>graphType</term> Overrides the Graph Type of the object property or endpoint parameter.</item>
/// <item><term>scalarType</term> Overrides the Graph Type of the object property or endpoint parameter using a scalar type.</item>
/// <item><term>listType, scalarType</term> Overrides the Graph Type of the object property or endpoint parameter using a list of scalar type.</item>
/// </list>
/// If the parameter a type of <see cref="IResolveFieldContext"/> or <see cref="IResolveFieldContext{TSource}"/>, then it will not show up in the endpoint-<br />
/// Instead it will be injected with the instance of <see cref="IResolveFieldContext"/> or <see cref="IResolveFieldContext{TSource}"/>.
/// </summary>
[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.ReturnValue, AllowMultiple = false)]
public class GraphQLTypeAttribute : Attribute
{
	private static readonly IReadOnlyDictionary<ScalarType, RuntimeTypeHandle> ScalarGraphTypeHandles =
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
		{ ScalarType.NotNullID, typeof(IdGraphType).GraphQLNonNull().TypeHandle },
		{ ScalarType.NotNullHashID, typeof(GraphQLHashIdType).GraphQLNonNull().TypeHandle },
		{ ScalarType.NotNullBoolean, typeof(BooleanGraphType).GraphQLNonNull().TypeHandle },
		{ ScalarType.NotNullSByte, typeof(SByteGraphType).GraphQLNonNull().TypeHandle },
		{ ScalarType.NotNullShort, typeof(ShortGraphType).GraphQLNonNull().TypeHandle },
		{ ScalarType.NotNullInt, typeof(IntGraphType).GraphQLNonNull().TypeHandle },
		{ ScalarType.NotNullLong, typeof(LongGraphType).GraphQLNonNull().TypeHandle },
		{ ScalarType.NotNullByte, typeof(ByteGraphType).GraphQLNonNull().TypeHandle },
		{ ScalarType.NotNullUShort, typeof(UShortGraphType).GraphQLNonNull().TypeHandle },
		{ ScalarType.NotNullUInt, typeof(UIntGraphType).GraphQLNonNull().TypeHandle },
		{ ScalarType.NotNullULong, typeof(ULongGraphType).GraphQLNonNull().TypeHandle },
		{ ScalarType.NotNullBigInteger, typeof(BigIntGraphType).GraphQLNonNull().TypeHandle },
		{ ScalarType.NotNullFloat, typeof(FloatGraphType).GraphQLNonNull().TypeHandle },
		{ ScalarType.NotNullDecimal, typeof(DecimalGraphType).GraphQLNonNull().TypeHandle },
		{ ScalarType.NotNullDate, typeof(DateGraphType).GraphQLNonNull().TypeHandle },
		{ ScalarType.NotNullDateTime, typeof(DateTimeGraphType).GraphQLNonNull().TypeHandle },
		{ ScalarType.NotNullDateTimeOffset, typeof(DateTimeOffsetGraphType).GraphQLNonNull().TypeHandle },
		{ ScalarType.NotNullTimeSpanMilliseconds, typeof(TimeSpanMillisecondsGraphType).GraphQLNonNull().TypeHandle },
		{ ScalarType.NotNullTimeSpanSeconds, typeof(TimeSpanSecondsGraphType).GraphQLNonNull().TypeHandle },
		{ ScalarType.NotNullGuid, typeof(GuidGraphType).GraphQLNonNull().TypeHandle },
		{ ScalarType.NotNullString, typeof(StringGraphType).GraphQLNonNull().TypeHandle },
		{ ScalarType.NotNullUri, typeof(UriGraphType).GraphQLNonNull().TypeHandle },
	}.ToImmutableDictionary();

	private readonly RuntimeTypeHandle _GraphType;

	/// <summary>
	/// Overrides the Graph Type of the object property or endpoint parameter.
	/// </summary>
	public GraphQLTypeAttribute(Type graphType)
	{
		graphType.AssertNotNull();
		graphType.IsGraphType().Assert(true);

		this._GraphType = graphType.TypeHandle;
	}

	/// <summary>
	/// Overrides the Graph Type of the object property or endpoint parameter using a scalar type.
	/// </summary>
	public GraphQLTypeAttribute(ScalarType scalarType)
		=> this._GraphType = ScalarGraphTypeHandles[scalarType];

	/// <summary>
	/// Overrides the Graph Type of the object property or endpoint parameter using a list of scalar type.
	/// </summary>
	public GraphQLTypeAttribute(ListType listType, ScalarType scalarType)
		=> this._GraphType = listType switch
		{
			ListType.List => ScalarGraphTypeHandles[scalarType].ToType().GraphQLList().TypeHandle,
			ListType.NonNullList => ScalarGraphTypeHandles[scalarType].ToType().GraphQLList().GraphQLNonNull().TypeHandle,
			_ => ScalarGraphTypeHandles[scalarType]
		};

	/// <summary>
	/// The Graph Type for override.
	/// </summary>
	public Type GraphType => this._GraphType.ToType();
}
