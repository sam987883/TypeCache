// Copyright (c) 2021 Samuel Abraham

using System.Text.Json;
using System.Text.Json.Serialization;

namespace TypeCache.Converters;

public sealed class UInt128JsonConverter : JsonConverter<UInt128>
{
	public override UInt128 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		=> (UInt128)reader.GetDecimal();

	public override void Write(Utf8JsonWriter writer, UInt128 value, JsonSerializerOptions options)
		=> writer.WriteNumberValue((decimal)value);
}
