// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Nodes;
using TypeCache.Extensions;
using static System.FormattableString;
using static TypeCache.Default;

namespace TypeCache.Data.Extensions;

public static class SqlExtensions
{
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static string EscapeValue([NotNull] this string @this)
		=> @this.Replace("'", "''");

	public static string ToSQL(this object? @this) => @this switch
	{
		null or DBNull => "NULL",
		true => "1",
		false => "0",
		SqlCommand command => command.SQL,
		'\'' => "N''''",
		char text => Invariant($"N'{text}'"),
		string text => Invariant($"N'{text.EscapeValue()}'"),
		DateTime dateTime => Invariant($"'{dateTime:o}'"),
		DateTimeOffset dateTimeOffset => Invariant($"'{dateTimeOffset:o}'"),
		TimeSpan time => Invariant($"'{time:c}'"),
		Guid guid => Invariant($"'{guid:D}'"),
		LogicalOperator.And => "AND",
		LogicalOperator.Or => "OR",
		LogicalOperator _ => throw new UnreachableException(Invariant($"{nameof(LogicalOperator)}.{@this} is not implemented for SQL.")),
		Sort.Ascending => "ASC",
		Sort.Descending => "DESC",
		Sort _ => string.Empty,
		Enum token => token.ToString("D"),
		Index index => index.Value.ToString(),
		Range range => Invariant($"'{range}'"),
		Uri uri => Invariant($"'{uri.ToString().EscapeValue()}'"),
		byte[] binary => Invariant($"0x{binary.ToHex()}"),
		JsonElement json => json.ValueKind switch
		{
			JsonValueKind.String => Invariant($"N'{json.GetString()!.EscapeValue()}'"),
			JsonValueKind.Number => json.ToString()!,
			JsonValueKind.True => "1",
			JsonValueKind.False => "0",
			JsonValueKind.Null => "NULL",
			_ => Invariant($"N'{json.ToString()!.EscapeValue()}'")
		},
		JsonArray jsonArray when jsonArray[0] is JsonObject jsonObject => string.Join("\r\t, ", jsonArray.Select(item => jsonObject.Select(pair => item!.AsObject()[pair.Key]!.AsValue().GetValue<object>()).ToSQL())),
		JsonArray jsonArray when jsonArray[0] is JsonValue => jsonArray.Select(item => new[] { item!.AsValue().GetValue<object>() }.ToSQL()).ToCSV(),
		JsonObject jsonObject => jsonObject.ToJsonString().EscapeValue(),
		JsonValue jsonValue => jsonValue.GetValue<object>().ToSQL(),
		JsonNode jsonNode => jsonNode.ToString().Replace("\"", string.Empty).EscapeValue(),
		DataRow row => row.ItemArray.ToSQL(),
		DataTable table => string.Join("\r\t, ", table.Rows.OfType<DataRow>().Select(row => row.ToSQL())),
		IEnumerable enumerable => Invariant($"({enumerable.Cast<object>().Select(_ => _.ToSQL()).ToCSV()})"),
		_ => @this.ToString() ?? "NULL"
	};
}
