// Copyright (c) 2021 Samuel Abraham

using System;
using System.Numerics;
using TypeCache.Extensions;
using TypeCache.GraphQL.Types;

namespace TypeCache.GraphQL.Extensions;

public static class EnumExtensions
{
	public static Type? ToGraphType(this ScalarType @this) => @this switch
	{
		ScalarType.Boolean => typeof(GraphQLBooleanType),
		ScalarType.Byte => typeof(GraphQLNumberType<byte>),
		ScalarType.Decimal => typeof(GraphQLNumberType<decimal>),
		ScalarType.DateOnly => typeof(GraphQLStringType<DateOnly>),
		ScalarType.DateTime => typeof(GraphQLStringType<DateTime>),
		ScalarType.DateTimeOffset => typeof(GraphQLStringType<DateTimeOffset>),
		ScalarType.Guid => typeof(GraphQLStringType<Guid>),
		ScalarType.Half => typeof(GraphQLNumberType<Half>),
		ScalarType.Int16 => typeof(GraphQLNumberType<short>),
		ScalarType.Int32 or ScalarType.Index => typeof(GraphQLNumberType<int>),
		ScalarType.Int64 => typeof(GraphQLNumberType<long>),
		ScalarType.IntPtr => typeof(GraphQLNumberType<nint>),
		ScalarType.Int128 => typeof(GraphQLNumberType<Int128>),
		ScalarType.UInt128 => typeof(GraphQLNumberType<UInt128>),
		ScalarType.BigInteger => typeof(GraphQLNumberType<BigInteger>),
		ScalarType.UInt16 => typeof(GraphQLNumberType<ushort>),
		ScalarType.UInt32 => typeof(GraphQLNumberType<uint>),
		ScalarType.UInt64 => typeof(GraphQLNumberType<ulong>),
		ScalarType.UIntPtr => typeof(GraphQLNumberType<UIntPtr>),
		ScalarType.SByte => typeof(GraphQLNumberType<sbyte>),
		ScalarType.Single => typeof(GraphQLNumberType<float>),
		ScalarType.Double => typeof(GraphQLNumberType<char>),
		ScalarType.Char => typeof(GraphQLStringType<char>),
		ScalarType.String => typeof(GraphQLStringType),
		ScalarType.TimeOnly => typeof(GraphQLStringType<TimeOnly>),
		ScalarType.TimeSpan => typeof(GraphQLStringType<TimeSpan>),
		ScalarType.Uri => typeof(GraphQLUriType),
		_ => null
	};
}
