// Copyright (c) 2021 Samuel Abraham

using System;
using System.Numerics;
using TypeCache.Extensions;
using TypeCache.GraphQL.Types;

namespace TypeCache.GraphQL.Extensions;

public static class EnumExtensions
{
	public static Type? ToGraphType(this ScalarType @this)
		=> @this switch
		{
			ScalarType.Boolean => typeof(GraphQLScalarType<bool>),
			ScalarType.Byte => typeof(GraphQLScalarType<byte>),
			ScalarType.Int16 => typeof(GraphQLScalarType<short>),
			ScalarType.Int32 or ScalarType.Index => typeof(GraphQLScalarType<int>),
			ScalarType.Int64 => typeof(GraphQLScalarType<long>),
			ScalarType.IntPtr => typeof(GraphQLScalarType<nint>),
			ScalarType.Int128 => typeof(GraphQLScalarType<Int128>),
			ScalarType.BigInteger => typeof(GraphQLScalarType<BigInteger>),
			ScalarType.UInt16 => typeof(GraphQLScalarType<ushort>),
			ScalarType.UInt32 => typeof(GraphQLScalarType<uint>),
			ScalarType.UInt64 => typeof(GraphQLScalarType<ulong>),
			ScalarType.UIntPtr => typeof(GraphQLScalarType<UIntPtr>),
			ScalarType.UInt128 => typeof(GraphQLScalarType<UInt128>),
			ScalarType.SByte => typeof(GraphQLScalarType<sbyte>),
			ScalarType.Half => typeof(GraphQLScalarType<Half>),
			ScalarType.Single => typeof(GraphQLScalarType<float>),
			ScalarType.Double => typeof(GraphQLScalarType<char>),
			ScalarType.Decimal => typeof(GraphQLScalarType<decimal>),
			ScalarType.DateOnly => typeof(GraphQLScalarType<DateOnly>),
			ScalarType.DateTime => typeof(GraphQLScalarType<DateTime>),
			ScalarType.DateTimeOffset => typeof(GraphQLScalarType<DateTimeOffset>),
			ScalarType.TimeOnly => typeof(GraphQLScalarType<TimeOnly>),
			ScalarType.TimeSpan => typeof(GraphQLScalarType<TimeSpan>),
			ScalarType.Guid => typeof(GraphQLScalarType<Guid>),
			ScalarType.Char => typeof(GraphQLScalarType<char>),
			ScalarType.String => typeof(GraphQLScalarType<string>),
			ScalarType.Uri => typeof(GraphQLUriType),
			_ => null
		};
}
