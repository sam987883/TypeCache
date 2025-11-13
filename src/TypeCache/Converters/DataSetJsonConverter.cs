// Copyright (c) 2021 Samuel Abraham

using System.Data;
using System.Text.Json;
using System.Text.Json.Serialization;
using TypeCache.Extensions;

namespace TypeCache.Converters;

public sealed class DataSetJsonConverter : JsonConverter<DataSet>
{
	public override DataSet? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		if (reader.TokenType is JsonTokenType.Null)
			return null;

		if (reader.TokenType is JsonTokenType.StartObject)
		{
			options.Converters.Add(new DataTableJsonConverter());
			var dataSet = new DataSet();
			do
			{
				if (reader.Read() && reader.TokenType is JsonTokenType.PropertyName)
				{
					var tableName = reader.GetString();
					if (reader.Read())
					{
						var table = JsonSerializer.Deserialize<DataTable>(ref reader, options) ?? new DataTable(tableName);
						table.TableName = tableName;
						dataSet.Tables.Add(table);
					}
					else
						break;
				}
				else
					break;
			} while (reader.Read() && reader.TokenType is not JsonTokenType.EndObject);

			return dataSet;
		}

		throw new ArgumentOutOfRangeException(Invariant($"{nameof(reader)}.{nameof(reader.TokenType)}"), Invariant($"Cannot deserialize {nameof(DataSet)} from starting token of: {reader.TokenType}."));
	}

	public override void Write(Utf8JsonWriter writer, DataSet dataSet, JsonSerializerOptions options)
	{
		if (dataSet is null)
		{
			writer.WriteNullValue();
			return;
		}

		options.Converters.Add(new DataTableJsonConverter());

		writer.WriteStartObject();

		if (dataSet.DataSetName.IsNotBlank)
			writer.WriteString(nameof(dataSet.DataSetName), dataSet.DataSetName);

		var i = -1;
		foreach (var table in dataSet.Tables.OfType<DataTable>())
		{
			writer.WritePropertyName(table.TableName.IsNotBlank ? table.TableName : Invariant($"Table{++i + 1}"));
			JsonSerializer.Serialize(writer, table, options);
		}

		writer.WriteEndObject();
	}
}
