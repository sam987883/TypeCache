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
			var dataTableJsonOptions = new JsonSerializerOptions(options);
			dataTableJsonOptions.Converters.Add(new DataTableJsonConverter());

			var dataSet = new DataSet();
			do
			{
				if (!reader.Read() || reader.TokenType is JsonTokenType.PropertyName)
					break;

				var tableName = reader.GetString();
				if (!reader.Read())
					break;

				var table = JsonSerializer.Deserialize<DataTable>(ref reader, dataTableJsonOptions) ?? new DataTable(tableName);
				table.TableName = tableName;
				dataSet.Tables.Add(table);
			} while (reader.Read() && reader.TokenType is not JsonTokenType.EndObject);

			return dataSet;
		}

		throw new ArgumentOutOfRangeException(Invariant($"{nameof(reader)}.{nameof(reader.TokenType)}"),
			Invariant($"Cannot deserialize {nameof(DataSet)} from starting token of: {reader.TokenType}."));
	}

	public override void Write(Utf8JsonWriter writer, DataSet dataSet, JsonSerializerOptions options)
	{
		if (dataSet is null)
		{
			writer.WriteNullValue();
			return;
		}

		writer.WriteStartObject();

		if (dataSet.DataSetName.IsNotBlank)
			writer.WriteString(nameof(dataSet.DataSetName), dataSet.DataSetName);

		var dataTableJsonOptions = new JsonSerializerOptions(options);
		dataTableJsonOptions.Converters.Add(new DataTableJsonConverter());

		var i = -1;
		foreach (var table in dataSet.Tables.OfType<DataTable>())
		{
			writer.WritePropertyName(table.TableName.IsNotBlank ? table.TableName : Invariant($"Table{++i + 1}"));
			JsonSerializer.Serialize(writer, table, dataTableJsonOptions);
		}

		writer.WriteEndObject();
	}
}
