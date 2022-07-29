// Copyright (c) 2021 Samuel Abraham

using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using TypeCache.Collections.Extensions;
using TypeCache.Extensions;
using TypeCache.Reflection.Extensions;

namespace TypeCache.Reflection.Converters;

public class PropertyJsonConverter<T> : JsonConverter<T> where T : class, new()
{
	public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		if (reader.TokenType == JsonTokenType.StartObject)
		{
			var output = TypeOf<T>.Create();
			while (reader.Read() && reader.TokenType == JsonTokenType.PropertyName)
			{
				var name = reader.GetString();
				if (!reader.Read() || name.IsBlank())
					continue;

				var property = TypeOf<T>.Properties.If(_ => _.Name.Is(name)).First();
				property?.SetValue(output, reader.TokenType switch
				{
					JsonTokenType.StartObject or JsonTokenType.StartArray => JsonSerializer.Deserialize(ref reader, property.PropertyType, options),
					_ => reader.GetValue()
				});
			}
			return output;
		}

		return null;
	}

	public override void Write(Utf8JsonWriter writer, T? input, JsonSerializerOptions options)
	{
		if (input is null)
		{
			writer.WriteNullValue();
			return;
		}

		writer.WriteStartObject();
		TypeOf<T>.Properties.If(property => property.Getter?.Static is false).Do(property =>
		{
			writer.WritePropertyName(property.Name);
			var value = property.GetValue(input);
			writer.WriteValue(value, options);
		});
		writer.WriteEndObject();
	}
}
