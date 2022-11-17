// Copyright (c) 2021 Samuel Abraham

using System;
using System.Data;
using System.Text.Json;
using System.Text.Json.Serialization;
using TypeCache.Collections.Extensions;
using TypeCache.Extensions;

namespace TypeCache.Converters;

public sealed class DataViewJsonConverter : JsonConverter<DataView>
{
	public override DataView? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		=> throw new NotImplementedException($"Cannot deserialize {nameof(DataView)} at all.");

	public override void Write(Utf8JsonWriter writer, DataView view, JsonSerializerOptions options)
	{
		if (view?.Table is null)
		{
			writer.WriteNullValue();
			return;
		}

		var columns = view.Table.Columns.If<DataColumn>().Map(column => column.ColumnName);
		writer.WriteStartArray();
		view.If<DataRowView>().Do(row =>
		{
			writer.WriteStartObject();
			columns.Do(column =>
			{
				var value = row[column];
				if (value is DBNull || value is null)
					writer.WriteNull(column);
				else
				{
					writer.WritePropertyName(column);
					writer.WriteValue(row[column], options);
				}
			});
			writer.WriteEndObject();
		});
		writer.WriteEndArray();
	}
}
