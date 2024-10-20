// Copyright (c) 2021 Samuel Abraham

using System.Numerics;
using TypeCache.Extensions;
using TypeCache.GraphQL.Types;

namespace TypeCache.GraphQL.Extensions;

public static class EnumExtensions
{
	public static Type? ToGraphType(this ScalarType @this)
		=> @this switch
		{
			ScalarType.Boolean => typeof(GraphQLBooleanType),
			ScalarType.SByte => typeof(GraphQLNumberType<sbyte>),
			ScalarType.Int16 => typeof(GraphQLNumberType<short>),
			ScalarType.Int32 or ScalarType.Index => typeof(GraphQLNumberType<int>),
			ScalarType.Int64 => typeof(GraphQLNumberType<long>),
			ScalarType.Int128 => typeof(GraphQLNumberType<Int128>),
			ScalarType.BigInteger => typeof(GraphQLNumberType<BigInteger>),
			ScalarType.Byte => typeof(GraphQLNumberType<byte>),
			ScalarType.UInt16 => typeof(GraphQLNumberType<ushort>),
			ScalarType.UInt32 => typeof(GraphQLNumberType<uint>),
			ScalarType.UInt64 => typeof(GraphQLNumberType<ulong>),
			ScalarType.UInt128 => typeof(GraphQLNumberType<UInt128>),
			ScalarType.IntPtr => typeof(GraphQLNumberType<nint>),
			ScalarType.UIntPtr => typeof(GraphQLNumberType<UIntPtr>),
			ScalarType.Half => typeof(GraphQLNumberType),
			ScalarType.Single => typeof(GraphQLNumberType),
			ScalarType.Double => typeof(GraphQLNumberType),
			ScalarType.Decimal => typeof(GraphQLNumberType),
			ScalarType.DateOnly => typeof(GraphQLStringType<DateOnly>),
			ScalarType.DateTime => typeof(GraphQLStringType<DateTime>),
			ScalarType.DateTimeOffset => typeof(GraphQLStringType<DateTimeOffset>),
			ScalarType.TimeOnly => typeof(GraphQLStringType<TimeOnly>),
			ScalarType.TimeSpan => typeof(GraphQLStringType<TimeSpan>),
			ScalarType.Guid => typeof(GraphQLStringType<Guid>),
			ScalarType.Char => typeof(GraphQLStringType<char>),
			ScalarType.String => typeof(GraphQLStringType),
			ScalarType.Uri => typeof(GraphQLUriType),
			_ => null
		};
}
