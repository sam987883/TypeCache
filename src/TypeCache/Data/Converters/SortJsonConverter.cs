// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using TypeCache.Collections.Extensions;
using TypeCache.Extensions;

namespace TypeCache.Data.Converters
{
	public class SortJsonConverter : JsonConverter<IDictionary<string, Sort>>
	{
		public override IDictionary<string, Sort> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			var sorts = new Dictionary<string, Sort>(StringComparer.OrdinalIgnoreCase);

			if (reader.TokenType == JsonTokenType.StartArray)
			{
				while (reader.Read() && reader.TokenType == JsonTokenType.StartObject)
				{
					if (reader.Read() && reader.TokenType == JsonTokenType.PropertyName)
					{
						var name = reader.GetString();
						if (reader.Read())
							sorts[reader.GetString()!] = name.ToEnum<Sort>()!.Value;
						reader.Read();
					}
				}
			}

			return sorts;
		}

		public override void Write(Utf8JsonWriter writer, IDictionary<string, Sort> sorts, JsonSerializerOptions options)
		{
			writer.WriteStartArray();
			sorts.Do(_ =>
			{
				writer.WriteStartObject();
				writer.WriteString(_.Value.Name(), _.Key);
				writer.WriteEndObject();
			});
			writer.WriteEndArray();
		}
	}
}
