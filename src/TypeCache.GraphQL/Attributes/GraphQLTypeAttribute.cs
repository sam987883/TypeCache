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
		{ ScalarType.NotNullID, typeof(IdGraphType).ToNonNullGraphType().TypeHandle },
		{ ScalarType.NotNullHashID, typeof(GraphQLHashIdType).ToNonNullGraphType().TypeHandle },
		{ ScalarType.NotNullBoolean, typeof(BooleanGraphType).ToNonNullGraphType().TypeHandle },
		{ ScalarType.NotNullSByte, typeof(SByteGraphType).ToNonNullGraphType().TypeHandle },
		{ ScalarType.NotNullShort, typeof(ShortGraphType).ToNonNullGraphType().TypeHandle },
		{ ScalarType.NotNullInt, typeof(IntGraphType).ToNonNullGraphType().TypeHandle },
		{ ScalarType.NotNullLong, typeof(LongGraphType).ToNonNullGraphType().TypeHandle },
		{ ScalarType.NotNullByte, typeof(ByteGraphType).ToNonNullGraphType().TypeHandle },
		{ ScalarType.NotNullUShort, typeof(UShortGraphType).ToNonNullGraphType().TypeHandle },
		{ ScalarType.NotNullUInt, typeof(UIntGraphType).ToNonNullGraphType().TypeHandle },
		{ ScalarType.NotNullULong, typeof(ULongGraphType).ToNonNullGraphType().TypeHandle },
		{ ScalarType.NotNullBigInteger, typeof(BigIntGraphType).ToNonNullGraphType().TypeHandle },
		{ ScalarType.NotNullFloat, typeof(FloatGraphType).ToNonNullGraphType().TypeHandle },
		{ ScalarType.NotNullDecimal, typeof(DecimalGraphType).ToNonNullGraphType().TypeHandle },
		{ ScalarType.NotNullDate, typeof(DateGraphType).ToNonNullGraphType().TypeHandle },
		{ ScalarType.NotNullDateTime, typeof(DateTimeGraphType).ToNonNullGraphType().TypeHandle },
		{ ScalarType.NotNullDateTimeOffset, typeof(DateTimeOffsetGraphType).ToNonNullGraphType().TypeHandle },
		{ ScalarType.NotNullTimeSpanMilliseconds, typeof(TimeSpanMillisecondsGraphType).ToNonNullGraphType().TypeHandle },
		{ ScalarType.NotNullTimeSpanSeconds, typeof(TimeSpanSecondsGraphType).ToNonNullGraphType().TypeHandle },
		{ ScalarType.NotNullGuid, typeof(GuidGraphType).ToNonNullGraphType().TypeHandle },
		{ ScalarType.NotNullString, typeof(StringGraphType).ToNonNullGraphType().TypeHandle },
		{ ScalarType.NotNullUri, typeof(UriGraphType).ToNonNullGraphType().TypeHandle },
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
			ListType.List => ScalarGraphTypeHandles[scalarType].ToType().ToListGraphType().TypeHandle,
			ListType.NonNullList => ScalarGraphTypeHandles[scalarType].ToType().ToListGraphType().ToNonNullGraphType().TypeHandle,
			_ => ScalarGraphTypeHandles[scalarType]
		};

	/// <summary>
	/// The Graph Type for override.
	/// </summary>
	public Type GraphType => this._GraphType.ToType();
}
