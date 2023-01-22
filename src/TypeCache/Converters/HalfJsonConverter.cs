// Copyright (c) 2021 Samuel Abraham

using System.Text.Json;
using System.Text.Json.Serialization;

namespace TypeCache.Converters;

public sealed class HalfJsonConverter : JsonConverter<Half>
{
	public override Half Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		=> (Half)reader.GetDouble();

	public override void Write(Utf8JsonWriter writer, Half value, JsonSerializerOptions options)
		=> writer.WriteNumberValue((double)value);
}
