// Copyright (c) 2021 Samuel Abraham

using System.Buffers;
using System.Globalization;
using System.Numerics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TypeCache.Converters;

public sealed class BigIntegerJsonConverter : JsonConverter<BigInteger>
{
	public override BigInteger Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		=> new(reader.HasValueSequence ? reader.ValueSequence.ToArray() : reader.ValueSpan);

	public override void Write(Utf8JsonWriter writer, BigInteger value, JsonSerializerOptions options)
		=> writer.WriteRawValue(value.ToString(NumberFormatInfo.InvariantInfo), false);
}
