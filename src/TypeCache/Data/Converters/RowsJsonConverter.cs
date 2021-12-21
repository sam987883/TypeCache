// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using TypeCache.Collections.Extensions;
using TypeCache.Converters;
using TypeCache.Extensions;

namespace TypeCache.Data.Converters;

public class RowsJsonConverter : JsonConverter<object?[][]>
{
	public override object?[][] Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		var rows = new Queue<object[]>();

		if (reader.TokenType == JsonTokenType.StartArray && reader.Read())
		{
			var valueOptions = new JsonSerializerOptions(options);
			valueOptions.Converters.Add(new ObjectJsonConverter());
			rows.Enqueue(JsonSerializer.Deserialize<object[]>(ref reader, valueOptions)!);
			while (reader.Read() && reader.TokenType == JsonTokenType.StartArray)
				rows.Enqueue(JsonSerializer.Deserialize<object[]>(ref reader, valueOptions)!);
		}

		return rows.ToArray();
	}

	public override void Write(Utf8JsonWriter writer, object?[][] rows, JsonSerializerOptions options)
	{
		if (rows.Any())
		{
			writer.WriteStartArray();
			rows.Do(values =>
			{
				writer.WriteStartArray();
				values.Do(value => writer.WriteValue(value, options));
				writer.WriteEndArray();
			});
			writer.WriteEndArray();
		}
		else
			writer.WriteNullValue();
	}
}
