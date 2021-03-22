// Copyright (c) 2021 Samuel Abraham

using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using TypeCache.Data;

namespace TypeCache.Converters
{
	public class ColumnSetJsonConverter : JsonConverter<ColumnSet>
	{
		public override ColumnSet Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			string? column = null;
			string? expression = null;

			if (reader.TokenType == JsonTokenType.StartObject && reader.Read() && reader.TokenType == JsonTokenType.PropertyName)
			{
				column = reader.GetString();
				reader.Read();
				expression = reader.GetString();
				reader.Read();
			}

			return new ColumnSet(column!, expression!);
		}

		public override void Write(Utf8JsonWriter writer, ColumnSet columnSet, JsonSerializerOptions options)
		{
			writer.WriteStartObject();
			writer.WriteString(columnSet.Column, columnSet.Expression!.ToString());
			writer.WriteEndObject();
		}
	}
}
