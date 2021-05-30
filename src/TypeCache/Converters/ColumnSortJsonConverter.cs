// Copyright (c) 2021 Samuel Abraham

using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using TypeCache.Data;
using TypeCache.Extensions;

namespace TypeCache.Converters
{
	public class ColumnSortJsonConverter : JsonConverter<ColumnSort>
	{
		public override ColumnSort Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			var sort = Sort.Ascending;
			string? expression = null;

			if (reader.TokenType == JsonTokenType.StartObject && reader.Read() && reader.TokenType == JsonTokenType.PropertyName)
			{
				sort = reader.GetString().ToEnum<Sort>()!.Value;
				reader.Read();
				expression = reader.GetString()!;
				reader.Read();
			}

			return new ColumnSort(expression!, sort);
		}

		public override void Write(Utf8JsonWriter writer, ColumnSort columnSort, JsonSerializerOptions options)
		{
			writer.WriteStartObject();
			writer.WriteString(columnSort.Sort.Name(), columnSort.Expression);
			writer.WriteEndObject();
		}
	}
}
