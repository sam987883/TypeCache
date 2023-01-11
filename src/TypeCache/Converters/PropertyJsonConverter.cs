// Copyright (c) 2021 Samuel Abraham

using System.Text.Json;
using System.Text.Json.Serialization;
using TypeCache.Extensions;

namespace TypeCache.Reflection.Converters;

public sealed class PropertyJsonConverter<T> : JsonConverter<T> where T : class, new()
{
	public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		if (reader.TokenType is JsonTokenType.StartObject)
		{
			var output = TypeOf<T>.Create()!;
			while (reader.Read() && reader.TokenType is JsonTokenType.PropertyName)
			{
				var name = reader.GetString();
				if (!reader.Read() || name.IsBlank())
					continue;

				typeof(T).SetPropertyValue(name, output, reader.TokenType switch
				{
					JsonTokenType.StartObject or JsonTokenType.StartArray => JsonSerializer.Deserialize(ref reader, typeof(T).GetProperty(name)!.PropertyType, options),
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
		foreach (var property in TypeOf<T>.Properties.Where(property => property.GetMethod?.IsStatic is false))
		{
			writer.WritePropertyName(property.Name());
			var value = property.GetPropertyValue(input);
			writer.WriteValue(value, options);
		}
		writer.WriteEndObject();
	}
}
