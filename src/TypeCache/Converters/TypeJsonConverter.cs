// Copyright (c) 2021 Samuel Abraham

using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TypeCache.Converters;

public class TypeJsonConverter : JsonConverter<Type>
{
	public override Type Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		=> throw new NotImplementedException();

	public override void Write(Utf8JsonWriter writer, Type type, JsonSerializerOptions options)
	{
		if (type is not null)
		{
			writer.WriteStartObject();
			writer.WriteString(nameof(type.Name), type.Name);
			writer.WriteEndObject();
		}
		else
			writer.WriteNullValue();
	}
}
