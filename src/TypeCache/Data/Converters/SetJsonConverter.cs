// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using TypeCache.Collections.Extensions;
using TypeCache.Extensions;

namespace TypeCache.Data.Converters
{
	public class SetJsonConverter : JsonConverter<IDictionary<string, object?>>
	{
		public override IDictionary<string, object?> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			var update = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);

			if (reader.TokenType == JsonTokenType.StartObject)
			{
				while (reader.Read() && reader.TokenType == JsonTokenType.PropertyName)
				{
					var name = reader.GetString()!;
					if (reader.Read())
						update.Add(name, reader.GetValue());
				}
			}

			return update;
		}

		public override void Write(Utf8JsonWriter writer, IDictionary<string, object?> update, JsonSerializerOptions options)
		{
			if (update.Any())
			{
				writer.WriteStartObject();
				update.Do(_ => writer.WriteString(_.Key, _.Value?.ToString()));
				writer.WriteEndObject();
			}
			else
				writer.WriteNullValue();
		}
	}
}
