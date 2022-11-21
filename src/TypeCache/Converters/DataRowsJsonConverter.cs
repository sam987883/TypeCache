// Copyright (c) 2021 Samuel Abraham

using System;
using System.Data;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using TypeCache.Extensions;

namespace TypeCache.Converters;

public sealed class DataRowsJsonConverter : JsonConverter<DataRow[]>
{
	public override DataRow[]? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		=> throw new NotImplementedException("Cannot deserialize DataRow[].");

	public override void Write(Utf8JsonWriter writer, DataRow[] rows, JsonSerializerOptions options)
	{
		if (rows is null)
		{
			writer.WriteNullValue();
			return;
		}

		var columns = rows.FirstOrDefault()?.Table.Columns.OfType<DataColumn>().ToArray();
		writer.WriteStartArray();
		rows.ForEach(row =>
		{
			writer.WriteStartObject();
			columns?.ForEach(column =>
			{
				var value = row[column];
				if (value is DBNull || value is null)
					writer.WriteNull(column.ColumnName);
				else
				{
					writer.WritePropertyName(column.ColumnName);
					writer.WriteValue(row[column], options);
				}
			});
			writer.WriteEndObject();
		});
		writer.WriteEndArray();
	}
}
