// Copyright (c) 2021 Samuel Abraham

using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Primitives;
using TypeCache.Extensions;

namespace TypeCache.Converters;

public sealed class StringValuesJsonConverter : JsonConverter<StringValues>
{
	public override StringValues Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		if (reader.TokenType is JsonTokenType.StartArray)
		{
			var list = new List<string?>(0);
			while (reader.Read() && reader.TokenType is not JsonTokenType.EndArray)
				list.Add(reader.GetString());

			return new(list.ToArray());
		}

		return new(reader.GetString());
	}

	public override void Write(Utf8JsonWriter writer, StringValues value, JsonSerializerOptions options)
	{
		if (value.Count > 1)
		{
			writer.WriteStartArray();
			value.ToArray().ForEach(writer.WriteStringValue);
			writer.WriteEndArray();
			return;
		}

		var text = (string?)value;
		if (text is not null)
			writer.WriteStringValue(text);
		else
			writer.WriteNullValue();
	}
}
