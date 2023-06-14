// Copyright (c) 2021 Samuel Abraham

using System;
using GraphQL.Types;
using TypeCache.Extensions;

namespace TypeCache.GraphQL.Extensions;

public static class EnumExtensions
{
	public static Type? ToGraphType(this SystemType @this)
		=> @this switch
		{
			SystemType.String or SystemType.Char or SystemType.Range => typeof(StringGraphType),
			SystemType.Uri => typeof(UriGraphType),
			SystemType.Boolean => typeof(BooleanGraphType),
			SystemType.SByte => typeof(SByteGraphType),
			SystemType.Int16 => typeof(ShortGraphType),
			SystemType.Int32 or SystemType.Index => typeof(IntGraphType),
			SystemType.Int64 or SystemType.IntPtr => typeof(LongGraphType),
			SystemType.Int128 or SystemType.UInt128 or SystemType.BigInteger => typeof(BigIntGraphType),
			SystemType.Byte => typeof(ByteGraphType),
			SystemType.UInt16 => typeof(UShortGraphType),
			SystemType.UInt32 => typeof(UIntGraphType),
			SystemType.UInt64 or SystemType.UIntPtr => typeof(ULongGraphType),
			SystemType.Half => typeof(HalfGraphType),
			SystemType.Single or SystemType.Double => typeof(FloatGraphType),
			SystemType.Decimal => typeof(DecimalGraphType),
			SystemType.DateOnly => typeof(DateOnlyGraphType),
			SystemType.DateTime => typeof(DateTimeGraphType),
			SystemType.DateTimeOffset => typeof(DateTimeOffsetGraphType),
			SystemType.TimeOnly => typeof(TimeOnlyGraphType),
			SystemType.TimeSpan => typeof(TimeSpanSecondsGraphType),
			SystemType.Guid => typeof(GuidGraphType),
			_ => null
		};
}
