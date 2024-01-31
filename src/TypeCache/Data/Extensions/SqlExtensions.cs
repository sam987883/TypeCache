// Copyright (c) 2021 Samuel Abraham

using System.Collections;
using System.Data;
using System.Text.Json;
using System.Text.Json.Nodes;
using TypeCache.Extensions;

namespace TypeCache.Data.Extensions;

public static class SqlExtensions
{
	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.Replace("'", "''");</c>
	/// </summary>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static string EscapeValue([NotNull] this string @this)
		=> @this.Replace("'", "''");

	/// <exception cref="UnreachableException"></exception>
	public static string ToSQL<T>(this T? @this) => @this switch
	{
		null or DBNull => "NULL",
		true => "1",
		false => "0",
		SqlCommand command => command.SQL,
		'\'' => "N''''",
		char text => Invariant($"N'{text}'"),
		string text => Invariant($"N'{text.EscapeValue()}'"),
		DateOnly dateOnly => Invariant($"'{dateOnly:o}'"),
		DateTime dateTime => Invariant($"'{dateTime:o}'"),
		DateTimeOffset dateTimeOffset => Invariant($"'{dateTimeOffset:o}'"),
		TimeOnly timeOnly => Invariant($"'{timeOnly:o}'"),
		TimeSpan timeSpan => Invariant($"'{timeSpan:c}'"),
		Guid guid => Invariant($"'{guid:D}'"),
		LogicalOperator.And => "AND",
		LogicalOperator.Or => "OR",
		LogicalOperator value => throw new UnreachableException(Invariant($"{nameof(LogicalOperator)}.{value:F} is not implemented for SQL.")),
		Sort.Ascending => "ASC",
		Sort.Descending => "DESC",
		Sort value => throw new UnreachableException(Invariant($"{nameof(Sort)}.{value:F} is not implemented for SQL.")),
		Enum token => token.ToString("D"),
		Range range => Invariant($"'{range}'"),
		Uri uri => Invariant($"'{uri.ToString().EscapeValue()}'"),
		byte[] binary => Invariant($"0x{binary.ToHexString()}"),
		JsonElement json => json.ValueKind switch
		{
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
		JsonNode jsonNode => Invariant($"N'{jsonNode.ToString().Replace("\"", string.Empty).EscapeValue()}'"),
		DataRow row => row.ItemArray.ToSQL(),
		DataTable table => string.Join("\r\t, ", table.Rows.OfType<DataRow>().Select(row => row.ToSQL())),
		IEnumerable enumerable => Invariant($"({enumerable.Cast<object>().Select(_ => _.ToSQL()).ToCSV()})"),
		_ => @this.ToString() ?? "NULL"
	};
}
