// Copyright (c) 2021 Samuel Abraham

using System.Text.Json;
using System.Text.Json.Serialization;
using TypeCache.Extensions;
using TypeCache.Reflection;

namespace TypeCache.Converters;

public sealed class FieldJsonConverter<T> : JsonConverter<T?>
	where T : class, new()
{
	public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		if (reader.TokenType is JsonTokenType.StartObject)
		{
			var output = Type<T>.Create()!;
			while (reader.Read() && reader.TokenType is JsonTokenType.PropertyName)
			{
				var name = reader.GetString()!;
				if (!reader.Read() || name.IsBlank())
					continue;

				var field = Type<T>.Fields[name];
				if (field?.SetValue is null)
					continue;

				var value = reader.TokenType switch
				{
					JsonTokenType.StartObject or JsonTokenType.StartArray => JsonSerializer.Deserialize(ref reader, field.FieldType, options),
					_ => reader.GetValue()
				};
				field.SetValue(output!, value);
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
			foreach (var field in Type<T>.Fields.Values)
			{
				writer.WritePropertyName(field.Name);
				var value = field.GetValue(input);
				writer.WriteValue(value, options);
			}
			writer.WriteEndObject();
		}
		else
			writer.WriteNullValue();
	}
}
