// Copyright (c) 2021 Samuel Abraham

using System.Text.Json;
using System.Text.Json.Serialization;
using TypeCache.Extensions;

namespace TypeCache.Converters;

public sealed class FieldJsonConverter<T> : JsonConverter<T?>
	where T : class, new()
{
	public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		if (reader.TokenType is JsonTokenType.StartObject)
		{
			var output = (T)typeof(T).Create()!;
			while (reader.Read() && reader.TokenType is JsonTokenType.PropertyName)
			{
				var name = reader.GetString()!;
				if (reader.Read())
				{
					var fieldInfo = typeof(T).GetPublicFields().FirstOrDefault(_ => _.Name().EqualsIgnoreCase(name));
					if (fieldInfo is not null && !fieldInfo.IsInitOnly)
						fieldInfo.SetFieldValue(output!, reader.TokenType switch
						{
							JsonTokenType.StartObject or JsonTokenType.StartArray => JsonSerializer.Deserialize(ref reader, fieldInfo.FieldType, options),
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
			foreach (var field in typeof(T).GetPublicFields())
			{
				writer.WritePropertyName(field!.Name());
				var value = field.GetFieldValue(input);
				writer.WriteValue(value, options);
			}
			writer.WriteEndObject();
		}
		else
			writer.WriteNullValue();
	}
}
