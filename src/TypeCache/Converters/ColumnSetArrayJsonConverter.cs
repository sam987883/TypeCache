// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using TypeCache.Collections.Extensions;
using TypeCache.Data;
using TypeCache.Extensions;

namespace TypeCache.Converters
{
	public class ColumnSetArrayJsonConverter : JsonConverter<ColumnSet[]>
	{
		public override ColumnSet[] Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			var columnSets = new List<ColumnSet>();

			if (reader.TokenType == JsonTokenType.StartObject)
			{
				while (reader.Read() && reader.TokenType == JsonTokenType.PropertyName)
				{
					var name = reader.GetString()!;
					if (reader.Read())
						columnSets.Add(new ColumnSet(name, reader.GetValue()));
				}
			}

			return columnSets.ToArray();
		}

		public override void Write(Utf8JsonWriter writer, ColumnSet[] columnSets, JsonSerializerOptions options)
		{
			if (columnSets.Length > 0)
			{
				writer.WriteStartObject();
				columnSets.Do(columnSet => writer.WriteString(columnSet.Column, columnSet.Expression?.ToString()));
				writer.WriteEndObject();
			}
			else
				writer.WriteNullValue();
		}
	}
}
