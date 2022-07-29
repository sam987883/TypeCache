// Copyright (c) 2021 Samuel Abraham

using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using TypeCache.Collections.Extensions;
using TypeCache.Extensions;
using TypeCache.Reflection.Extensions;

namespace TypeCache.Converters;

public class FieldJsonConverter<T> : JsonConverter<T?>
	where T : class, new()
{
	public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		if (reader.TokenType == JsonTokenType.StartObject)
		{
			var output = TypeOf<T>.Create();
			while (reader.Read() && reader.TokenType == JsonTokenType.PropertyName)
			{
				var name = reader.GetString()!;
				if (reader.Read())
				{
					var field = TypeOf<T>.Fields.If(_ => _.Name.Is(name)).First();
					if (field is not null && !field.Static && field.Setter is not null)
						field.SetValue(output, reader.TokenType switch
						{
							JsonTokenType.StartObject or JsonTokenType.StartArray => JsonSerializer.Deserialize(ref reader, field.FieldType, options),
							_ => reader.GetValue()
						});
				}
			}

			return output;
		}

		return null;
	}

	public override void Write(Utf8JsonWriter writer, T? input, JsonSerializerOptions options)
	{
		if (input is not null)
		{
			writer.WriteStartObject();
			TypeOf<T>.Fields.If(field => !field.Static && field!.Getter is not null).Do(field =>
			{
				writer.WritePropertyName(field!.Name);
				var value = field.GetValue(input);
				writer.WriteValue(value, options);
			});
			writer.WriteEndObject();
		}
		else
			writer.WriteNullValue();
	}
}
