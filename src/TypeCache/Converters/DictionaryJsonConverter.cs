// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using TypeCache.Collections.Extensions;
using TypeCache.Extensions;

namespace TypeCache.Converters
{
	public class DictionaryJsonConverter : JsonConverter<IDictionary<string, object?>>
	{
		public override IDictionary<string, object?> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			var dictionary = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);

			if (reader.TokenType == JsonTokenType.StartObject)
			{
				while (reader.Read() && reader.TokenType == JsonTokenType.PropertyName)
				{
					var name = reader.GetString()!;
					if (reader.Read())
						dictionary.Add(name, reader.GetValue());
				}
			}

			return dictionary;
		}

		public override void Write(Utf8JsonWriter writer, IDictionary<string, object?> dictionary, JsonSerializerOptions options)
		{
			if (dictionary.Any())
			{
				writer.WriteStartObject();
				dictionary.Do(_ =>
				{
					writer.WritePropertyName(_.Key);
					writer.WriteValue(_.Value, options);
				});
				writer.WriteEndObject();
			}
			else
				writer.WriteNullValue();
		}
	}
}
