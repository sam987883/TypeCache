// Copyright (c) 2020 Samuel Abraham

using System.Text.Json;
using System.Text.Json.Serialization;
using System;
using sam987883.Database;
using sam987883.Extensions;

namespace sam987883.Web.Middleware
{
	public class RowSetJsonConverter : JsonConverter<RowSet>
	{
        public override RowSet Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var rowSet = JsonSerializer.Deserialize<RowSet>(ref reader, null);
			rowSet.Rows.Do((row, rowIndex) => row.Do((value, i) => row[i] = value is JsonElement json ? json.ValueKind switch
			{
				JsonValueKind.Object => json.ToString(),
				JsonValueKind.Array => json.ToString(),
				JsonValueKind.String => json.GetString(),
				JsonValueKind.Number => json.TryGetInt64(out var number) ? number : json.GetDecimal(),
				JsonValueKind.True => true,
				JsonValueKind.False => false,
				JsonValueKind.Null => null,
				_ => throw new ArgumentException($"Invalid value for [{nameof(value)}] specified in row [{rowIndex}] column [{i}]: {value}.", $"{nameof(RowSet)}.{nameof(rowSet.Rows)}"),
			} : value));
			return rowSet;
		}

		public override void Write(Utf8JsonWriter writer, RowSet value, JsonSerializerOptions options) =>
            JsonSerializer.Serialize(writer, value, null);
	}
}
