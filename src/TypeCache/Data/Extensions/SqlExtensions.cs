// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using TypeCache.Collections.Extensions;
using TypeCache.Data.Requests;
using TypeCache.Extensions;
using static System.FormattableString;
using static TypeCache.Default;

namespace TypeCache.Data.Extensions;

public static class SqlExtensions
{
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static string EscapeIdentifier([NotNull] this string @this)
		=> Invariant($"[{@this.EscapeValue().Replace("]", "]]")}]");

	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static string EscapeLikeValue([NotNull] this string @this)
		=> @this.EscapeValue().Replace("[", "[[]").Replace("%", "[%]").Replace("_", "[_]");

	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static string EscapeValue([NotNull] this string @this)
		=> @this.Replace("'", "''");

	public static string ToSQL([NotNull] this CountRequest @this)
		=> new StringBuilder()
			.AppendSQL("SELECT", "COUNT_BIG(1)")
			.AppendSQL("FROM", @this.From, @this.TableHints ?? "WITH(NOLOCK)")
			.AppendSQL("WHERE", @this.Where)
			.AppendStatementEndSQL()
			.ToString();

	public static string ToSQL([NotNull] this DeleteDataRequest @this)
		=> new StringBuilder()
			.AppendSQL("DELETE", "FROM", "x")
			.AppendSQL("OUTPUT", @this.Output)
			.AppendSQL("FROM", @this.From, "x")
			.AppendLine("INNER JOIN")
			.Append('(').AppendLine()
			.AppendValuesSQL(@this.Input.Rows)
			.Append(')').Append(" AS i ").AppendColumnsSQL(@this.Input.Columns)
			.AppendSQL("ON", @this.Input.Columns.Each(column => Invariant($"i.{column.EscapeIdentifier()} = x.{column.EscapeIdentifier()}")).Join(" AND "))
			.AppendStatementEndSQL()
			.ToString();

	public static string ToSQL([NotNull] this DeleteRequest @this)
		=> new StringBuilder()
			.AppendSQL("DELETE", @this.From)
			.AppendSQL("OUTPUT", @this.Output)
			.AppendSQL("WHERE", @this.Where)
			.AppendStatementEndSQL()
			.ToString();

	public static string ToSQL([NotNull] this InsertDataRequest @this)
		=> new StringBuilder()
			.AppendInsertSQL(@this.Into, @this.Input.Columns)
			.AppendSQL("OUTPUT", @this.Output)
			.AppendValuesSQL(@this.Input.Rows)
			.AppendStatementEndSQL()
			.ToString();

	public static string ToSQL([NotNull] this InsertRequest @this)
		=> new StringBuilder()
			.AppendInsertSQL(@this.Into, @this.Insert)
			.AppendSQL("OUTPUT", @this.Output)
			.AppendSQL("SELECT", @this.Select.Any() ? @this.Select : new[] { "*" })
			.AppendSQL("FROM", @this.From, @this.TableHints ?? "WITH(NOLOCK)")
			.AppendSQL("WHERE", @this.Where)
			.AppendSQL("GROUP BY", @this.GroupBy)
			.AppendSQL("HAVING", @this.Having)
			.AppendSQL("ORDER BY", @this.OrderBy)
			.AppendStatementEndSQL()
			.ToString();

	public static string ToSQL([NotNull] this SelectRequest @this)
	{
		var sqlBuilder = new StringBuilder()
			.AppendSQL("SELECT", @this.Select.Any() ? @this.Select : new[] { "*" })
			.AppendSQL("FROM", @this.From, @this.TableHints ?? "WITH(NOLOCK)")
			.AppendSQL("WHERE", @this.Where)
			.AppendSQL("GROUP BY", @this.GroupBy)
			.AppendSQL("HAVING", @this.Having)
			.AppendSQL("ORDER BY", @this.OrderBy)
			.AppendPagerSQL(@this.Pager)
			.AppendStatementEndSQL();

		if (@this.Pager.HasValue)
		{
			sqlBuilder.AppendLine()
				.AppendSQL("SELECT", "@Count = COUNT_BIG(1)")
				.AppendSQL("FROM", @this.From, "WITH(NOLOCK)")
				.AppendSQL("WHERE", @this.Where);
		}

		return sqlBuilder.AppendStatementEndSQL().ToString();
	}

	public static string ToSQL([NotNull] this UpdateRequest @this)
		=> new StringBuilder()
			.AppendSQL("UPDATE", @this.Table, @this.TableHints ?? "WITH(UPDLOCK)")
			.AppendSQL("SET", @this.Set)
			.AppendSQL("OUTPUT", @this.Output)
			.AppendSQL("WHERE", @this.Where)
			.AppendStatementEndSQL()
			.ToString();

	public static string ToSQL([NotNull] this UpdateDataRequest @this)
		=> new StringBuilder()
			.AppendSQL("UPDATE", "x", @this.TableHints ?? "WITH(UPDLOCK)")
			.AppendSQL("SET", @this.Input.Columns.Without(@this.On).Each(column => Invariant($"{column.EscapeIdentifier()} = i.{column.EscapeIdentifier()}")))
			.AppendSQL("OUTPUT", @this.Output)
			.AppendSQL("FROM", @this.Table, "x")
			.AppendLine("INNER JOIN")
			.Append('(').AppendLine()
			.AppendValuesSQL(@this.Input.Rows)
			.Append(") AS i (").AppendJoin(", ", @this.Input.Columns.To(column => column.EscapeIdentifier())).Append(')').AppendLine()
			.AppendSQL("ON", @this.On.Each(column => Invariant($"i.{column.EscapeIdentifier()} = x.{column.EscapeIdentifier()}")).Join(" AND "))
			.AppendStatementEndSQL()
			.ToString();

	public static string ToSQL(this object? @this) => @this switch
	{
		null or DBNull => "NULL",
		bool boolean => boolean ? "1" : "0",
		char text => text.Equals('\'') ? "N''''" : Invariant($"N'{text}'"),
		string text => Invariant($"N'{text.EscapeValue()}'"),
		DateTime dateTime => Invariant($"'{dateTime:o}'"),
		DateTimeOffset dateTimeOffset => Invariant($"'{dateTimeOffset:o}'"),
		TimeSpan time => Invariant($"'{time:c}'"),
		Guid guid => Invariant($"'{guid:D}'"),
		LogicalOperator.And => "AND",
		LogicalOperator.Or => "OR",
		LogicalOperator _ => throw new NotImplementedException(Invariant($"{nameof(LogicalOperator)}.{@this} is not implemented for SQL.")),
		Sort.Ascending => "ASC",
		Sort.Descending => "DESC",
		Sort _ => string.Empty,
		Enum token => token.ToString("D"),
		Index index => Invariant($"'{index}'"),
		JsonElement json => json.ValueKind switch
		{
			JsonValueKind.String => Invariant($"N'{json.GetString()!.EscapeValue()}'"),
			JsonValueKind.Number => json.ToString()!,
			JsonValueKind.True => "1",
			JsonValueKind.False => "0",
			JsonValueKind.Null => "NULL",
			_ => Invariant($"N'{json.ToString()!.EscapeValue()}'")
		},
		Range range => Invariant($"'{range}'"),
		Uri uri => Invariant($"'{uri.ToString().EscapeValue()}'"),
		byte[] binary => Invariant($"0x{binary.ToHex()}"),
		IEnumerable enumerable => Invariant($"({enumerable.As<object>().To(_ => _.ToSQL()).Join(", ")})"),
		_ => @this.ToString() ?? "NULL"
	};
}
