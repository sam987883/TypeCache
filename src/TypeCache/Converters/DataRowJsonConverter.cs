// Copyright (c) 2021 Samuel Abraham

using System.Data;
using System.Text.Json;
using System.Text.Json.Serialization;
using TypeCache.Extensions;

namespace TypeCache.Converters;

public sealed class DataRowJsonConverter : JsonConverter<DataRow>
{
	/// <exception cref="NotImplementedException"></exception>
	public override DataRow? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		=> throw new NotImplementedException("Cannot deserialize DataRow.");

	public override void Write(Utf8JsonWriter writer, DataRow row, JsonSerializerOptions options)
	{
		if (row is null)
		{
			writer.WriteNullValue();
			return;
		}

		writer.WriteStartObject();
		row.Table.Columns.OfType<DataColumn>().ForEach(column =>
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
	}
}
