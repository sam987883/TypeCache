// Copyright (c) 2021 Samuel Abraham

using System.Collections;
using System.Data;
using System.Text.Json;
using System.Text.Json.Nodes;
using TypeCache.Extensions;
using static TypeCache.Data.DataSourceType;

namespace TypeCache.Data.Extensions;

public static class SqlExtensions
{
	public static string EscapeIdentifier([NotNull] this string @this, DataSourceType type)
		=> type switch
		{
			SqlServer => Invariant($"[{@this.Replace("]", "]]")}]"),
			MySql => Invariant($"`{@this.Replace("`", "``")}`"),
			_ => Invariant($"\"{@this.Replace("\"", "\"\"")}\""),
		};

	/// <summary>
	/// <c>=&gt; <see langword="this"/>.EscapeValue(@<paramref name="this"/>).Replace("[", "[[]").Replace("%", "[%]").Replace("_", "[_]");</c>
	/// </summary>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static string EscapeLikeValue([NotNull] this string @this)
		=> @this.Replace("'", "''").Replace("[", "[[]").Replace("%", "[%]").Replace("_", "[_]");

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.Replace("'", "''");</c>
	/// </summary>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static string EscapeValue([NotNull] this string @this)
		=> @this.Replace("'", "''");

	/// <exception cref="UnreachableException"></exception>
	public static string ToSQL<T>(this T? @this)
		=> @this switch
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
			LogicalOperator value => throw new UnreachableException(Invariant($"{nameof(LogicalOperator)}.{value.Name} is not implemented for SQL.")),
			Sort.Ascending => "ASC",
			Sort.Descending => "DESC",
			Sort value => throw new UnreachableException(Invariant($"{nameof(Sort)}.{value.Name} is not implemented for SQL.")),
			Enum token => token.Name,
			Range range => Invariant($"'{range}'"),
			Uri uri => Invariant($"'{uri.ToString().EscapeValue()}'"),
			byte[] binary => Invariant($"0x{binary.ToHexString()}"),
			JsonArray jsonArray => string.Join(", ", jsonArray.Select(_ => _.ToSQL())),
			JsonObject jsonObject => jsonObject.ToJsonString().EscapeValue(),
			JsonValue jsonValue => jsonValue.GetValueKind() switch
			{
				JsonValueKind.Null => "NULL",
				JsonValueKind.False => "0",
				JsonValueKind.True => "1",
				JsonValueKind.Number => jsonValue.ToString(),
				_ => Invariant($"N'{jsonValue.ToString()!.EscapeValue()}'")
			},
			JsonElement jsonElement => jsonElement.ValueKind switch
			{
				JsonValueKind.Null => "NULL",
				JsonValueKind.False => "0",
				JsonValueKind.True => "1",
				JsonValueKind.Number => jsonElement.ToString()!,
				_ => Invariant($"N'{jsonElement.ToString()!.EscapeValue()}'")
			},
			DataRow row => row.ItemArray.ToSQL(),
			DataTable table => string.Join("\r\t, ", table.Rows.OfType<DataRow>().Select(row => row.ToSQL())),
			object[] array => Invariant($"({array.Select(_ => _.ToSQL()).ToCSV()})"),
			IEnumerable enumerable => Invariant($"({enumerable.Cast<object>().Select(_ => _.ToSQL()).ToCSV()})"),
			_ => @this.ToString() ?? "NULL"
		};

	public static string UnEscapeIdentifier([NotNull] this string @this, DataSourceType type)
		=> type switch
		{
			SqlServer => @this.TrimStart('[').TrimEnd(']').Replace("]]", "]"),
			MySql => @this.Trim('`').Replace("``", "`"),
			_ => @this.Trim('"').Replace("\"\"", "\"")
		};
}
