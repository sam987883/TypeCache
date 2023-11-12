// Copyright (c) 2021 Samuel Abraham

using System;
using System.Globalization;
using GraphQLParser.AST;
using TypeCache.Extensions;
using TypeCache.Utilities;

namespace TypeCache.GraphQL.Types;

public sealed class GraphQLNumberType<T> : GraphQLScalarType<GraphQLIntValue>
	where T : ISpanParsable<T>
{
	public GraphQLNumberType() : base(
		typeof(T).Name,
		value => T.TryParse(value.Value.Span, CultureInfo.InvariantCulture, out _),
		value => T.Parse(value.Value.Span, CultureInfo.InvariantCulture),
		value => typeof(T).GetScalarType() switch
		{
			ScalarType.SByte => ValueConverter.ConvertToSByte(value),
			ScalarType.Int16 => ValueConverter.ConvertToInt16(value),
			ScalarType.Int32 => ValueConverter.ConvertToInt32(value),
			ScalarType.Int64 => ValueConverter.ConvertToInt64(value),
			ScalarType.Int128 => ValueConverter.ConvertToInt128(value),
			ScalarType.BigInteger => ValueConverter.ConvertToBigInteger(value),
			ScalarType.IntPtr => ValueConverter.ConvertToIntPtr(value),
			ScalarType.Byte => ValueConverter.ConvertToByte(value),
			ScalarType.UInt16 => ValueConverter.ConvertToUInt16(value),
			ScalarType.UInt32 => ValueConverter.ConvertToUInt32(value),
			ScalarType.UInt64 => ValueConverter.ConvertToUInt64(value),
			ScalarType.UInt128 => ValueConverter.ConvertToUInt128(value),
			ScalarType.UIntPtr => ValueConverter.ConvertToUIntPtr(value),
			ScalarType.Half => ValueConverter.ConvertToHalf(value),
			ScalarType.Single => ValueConverter.ConvertToSingle(value),
			ScalarType.Double => ValueConverter.ConvertToDouble(value),
			ScalarType.Decimal => ValueConverter.ConvertToDecimal(value),
			_ => null
		})
	{
	}
}
