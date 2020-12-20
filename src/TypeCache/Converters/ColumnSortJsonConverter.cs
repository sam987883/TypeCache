// Copyright (c) 2021 Samuel Abraham

using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using TypeCache.Common;
using TypeCache.Data;
using TypeCache.Extensions;

namespace TypeCache.Converters
{
	public class ColumnSortJsonConverter : JsonConverter<ColumnSort>
	{
        public override ColumnSort Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var columnSort = new ColumnSort();

            if (reader.TokenType == JsonTokenType.StartObject)
            {
                if (reader.Read() && reader.TokenType == JsonTokenType.PropertyName)
                {
                    columnSort.Sort = Enum.Parse<Sort>(reader.GetString());
                    reader.Read();
                    columnSort.Expression = reader.GetString();
                    reader.Read();
                }
            }

            return columnSort;
        }

        public override void Write(Utf8JsonWriter writer, ColumnSort columnSort, JsonSerializerOptions options)
		{
            writer.WriteStartObject();
            writer.WriteString(columnSort.Sort.Name(), columnSort.Expression);
            writer.WriteEndObject();
        }
    }
}
