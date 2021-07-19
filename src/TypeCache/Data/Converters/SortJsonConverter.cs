// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using TypeCache.Collections.Extensions;
using TypeCache.Data.Extensions;
using TypeCache.Extensions;

namespace TypeCache.Data.Converters
{
	public class SortJsonConverter : JsonConverter<(string, Sort)[]>
	{
		public override (string, Sort)[] Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			var sorts = new Queue<(string, Sort)>();

			if (reader.TokenType == JsonTokenType.StartArray)
			{
				while (reader.Read() && reader.TokenType == JsonTokenType.StartObject)
				{
					if (reader.Read() && reader.TokenType == JsonTokenType.PropertyName)
					{
						var name = reader.GetString();
						if (reader.Read())
							sorts.Enqueue((reader.GetString()!, name.ToEnum<Sort>()!.Value));
						reader.Read();
					}
				}
			}

			return sorts.ToArray();
		}

		public override void Write(Utf8JsonWriter writer, (string, Sort)[] sorts, JsonSerializerOptions options)
		{
			writer.WriteStartArray();
			sorts.Do(_ =>
			{
				writer.WriteStartObject();
				writer.WriteString(_.Item2.Name(), _.Item1);
				writer.WriteEndObject();
			});
			writer.WriteEndArray();
		}
	}
}
