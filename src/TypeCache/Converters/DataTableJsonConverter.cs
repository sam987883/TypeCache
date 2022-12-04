// Copyright (c) 2021 Samuel Abraham

using System.Data;
using System.Text.Json;
using System.Text.Json.Serialization;
using TypeCache.Extensions;

namespace TypeCache.Converters;

public sealed class DataTableJsonConverter : JsonConverter<DataTable>
{
	public override DataTable? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		if (reader.TokenType is JsonTokenType.Null)
			return null;

		if (reader.TokenType is JsonTokenType.StartArray)
		{
			var table = new DataTable();
			if (reader.Read() && reader.TokenType is JsonTokenType.StartObject)
			{
				var dictionary = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
				while (reader.Read() && reader.TokenType is not JsonTokenType.EndObject)
				{
					if (reader.TokenType is JsonTokenType.PropertyName)
					{
						var column = reader.GetString()!;
						if (reader.Read())
							dictionary.Add(column, reader.GetValue());
					}
				}

				table.Columns.AddRange(dictionary.Select(pair => pair.Value is not null ? new DataColumn(pair.Key, pair.Value.GetType()) : new DataColumn(pair.Key)).ToArray());
				var row = table.NewRow();
				foreach (var pair in dictionary)
					row[pair.Key] = pair.Value ?? DBNull.Value;

				table.Rows.Add(row);

				while (reader.TokenType is JsonTokenType.StartObject)
				{
					row = table.NewRow();
					while (reader.Read() && reader.TokenType is not JsonTokenType.EndObject)
					{
						if (reader.TokenType is JsonTokenType.PropertyName)
						{
							var column = reader.GetString()!;
							if (reader.Read())
								row[column] = reader.GetValue() ?? DBNull.Value;
						}
					}
					table.Rows.Add(row);
				}
			}
			return table;
		}

		throw new ArgumentOutOfRangeException($"{nameof(reader)}.{nameof(reader.TokenType)}", $"Cannot deserialize {nameof(DataTable)} from starting token of: {reader.TokenType}.");
	}

	public override void Write(Utf8JsonWriter writer, DataTable table, JsonSerializerOptions options)
	{
		if (table is null)
			writer.WriteNullValue();
		else
		{
			var columns = table.Columns.OfType<DataColumn>().ToArray();
			writer.WriteStartArray();
			foreach (var row in table.Rows.OfType<DataRow>())
			{
				writer.WriteStartObject();
				columns.ForEach(column =>
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
			writer.WriteEndArray();
		}
	}
}
