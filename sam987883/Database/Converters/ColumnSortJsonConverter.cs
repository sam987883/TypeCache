// Copyright (c) 2020 Samuel Abraham

using sam987883.Common.Extensions;
using sam987883.Common.Models;
using sam987883.Database.Models;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace sam987883.Database.Converters
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
                    columnSort.Column = reader.GetString();
                    reader.Read();
                }
            }

            return columnSort;
        }

        public override void Write(Utf8JsonWriter writer, ColumnSort columnSort, JsonSerializerOptions options)
		{
            writer.WriteStartObject();
            writer.WriteString(columnSort.Sort.Name(), columnSort.Column);
            writer.WriteEndObject();
        }
    }
}
