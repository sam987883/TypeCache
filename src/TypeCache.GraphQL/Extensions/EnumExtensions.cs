// Copyright (c) 2021 Samuel Abraham

using global::GraphQL.Types;
using TypeCache.Reflection;

namespace TypeCache.GraphQL.Extensions;

public static class EnumExtensions
{
	extension(ScalarType @this)
	{
		public Type? ToGraphType()
			=> @this switch
			{
				ScalarType.Boolean => typeof(BooleanGraphType),
				ScalarType.SByte => typeof(SByteGraphType),
				ScalarType.Int16 => typeof(ShortGraphType),
				ScalarType.Int32 or ScalarType.Index => typeof(IntGraphType),
				ScalarType.Int64 or ScalarType.IntPtr => typeof(LongGraphType),
				ScalarType.BigInteger or ScalarType.Int128 => typeof(BigIntGraphType),
				ScalarType.Byte => typeof(ByteGraphType),
				ScalarType.UInt16 => typeof(UShortGraphType),
				ScalarType.UInt32 => typeof(UIntGraphType),
				ScalarType.UInt64 or ScalarType.UInt128 or ScalarType.UIntPtr => typeof(ULongGraphType),
				ScalarType.Half => typeof(HalfGraphType),
				ScalarType.Single or ScalarType.Double => typeof(FloatGraphType),
				ScalarType.Decimal => typeof(DecimalGraphType),
				ScalarType.DateOnly => typeof(DateOnlyGraphType),
				ScalarType.DateTime => typeof(DateTimeGraphType),
				ScalarType.DateTimeOffset => typeof(DateTimeOffsetGraphType),
				ScalarType.TimeOnly => typeof(TimeOnlyGraphType),
				ScalarType.TimeSpan => typeof(TimeSpanSecondsGraphType),
				ScalarType.Guid => typeof(GuidGraphType),
				ScalarType.String or ScalarType.Char => typeof(StringGraphType),
				ScalarType.Uri => typeof(UriGraphType),
				_ => null
			};
	}
}
