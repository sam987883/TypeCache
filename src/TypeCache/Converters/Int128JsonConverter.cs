// Copyright (c) 2021 Samuel Abraham

using System.Text.Json;
using System.Text.Json.Serialization;

namespace TypeCache.Converters;

public sealed class Int128JsonConverter : JsonConverter<Int128>
{
	public override Int128 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		=> (Int128)reader.GetDecimal();

	public override void Write(Utf8JsonWriter writer, Int128 value, JsonSerializerOptions options)
		=> writer.WriteNumberValue((decimal)value);
}
