// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using TypeCache.Common;
using TypeCache.Data;

namespace TypeCache.Extensions
{
	public static class SqlExtensions
	{
		private const string SQL_DELIMETER = "\r\n\t, ";

		public static string EscapeIdentifier([NotNull] this string @this)
			=> @this.StartsWith('[') && @this.EndsWith(']')
				? $"[{@this.Substring(1, @this.Length - 2).Replace("]", "]]")}]"
				: $"[{@this.Replace("]", "]]")}]";

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string EscapeLikeValue([NotNull] this string @this)
			=> @this.Replace("'", "''").Replace("[", "[[]").Replace("%", "[%]").Replace("_", "[_]");

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string EscapeValue([NotNull] this string @this)
			=> @this.Replace("'", "''");

		public static string ToSql([NotNull] this BatchRequest @this)
		{
			var batchDataCsv = @this.Input.Rows.Join(SQL_DELIMETER, row => $"({row.ToCsv(value => value.ToSql())})");
			var sourceColumnCsv = @this.Input.Columns.Join(", ", column => column.EscapeIdentifier());
			var onSql = @this.On.Join($" {LogicalOperator.And.ToSql()} ", column => $"s.{column.EscapeIdentifier()} = t.{column.EscapeIdentifier()}");
			var updateCsv = @this.Update.Join(SQL_DELIMETER, column => $"{column.EscapeIdentifier()} = s.{column.EscapeIdentifier()}");
			var insertColumnCsv = @this.Insert.Join(SQL_DELIMETER, column => column.EscapeIdentifier());
			var insertValueCsv = @this.Insert.Join(SQL_DELIMETER, column => $"s.{column.EscapeIdentifier()}");

			var sqlBuilder = new StringBuilder();
			if (!@this.Delete && updateCsv.IsBlank())
			{
				sqlBuilder.Append(@$"INSERT INTO {@this.Table}
(
	{insertColumnCsv}
)");

				if (@this.Output.Any())
					sqlBuilder.AppendLine().Append("OUTPUT ").AppendJoin(SQL_DELIMETER, @this.Output.To(_ => _.ToSql()));

				sqlBuilder.AppendLine().Append($"VALUES {batchDataCsv}");
			}
			else
			{
				sqlBuilder.Append($@"MERGE {@this.Table} AS t
USING
(
	VALUES {batchDataCsv}
) AS s ({sourceColumnCsv})
ON {onSql}");

				if (!updateCsv.IsBlank())
				{
					sqlBuilder.AppendLine().Append(@$"WHEN MATCHED THEN
	UPDATE SET {updateCsv}");
					if (@this.Delete)
						sqlBuilder.AppendLine().Append(@$"WHEN NOT MATCHED BY SOURCE THEN
	DELETE");
				}
				else if (@this.Delete)
					sqlBuilder.AppendLine().Append(@$"WHEN MATCHED THEN
	DELETE");

				if (!insertColumnCsv.IsBlank())
					sqlBuilder.AppendLine().Append(@$"WHEN NOT MATCHED BY TARGET THEN
	INSERT
	(
	{insertColumnCsv}
	)
	VALUES
	(
	{insertValueCsv}
	)");

				if (@this.Output.Any())
					sqlBuilder.AppendLine().Append("OUTPUT ").AppendJoin(SQL_DELIMETER, @this.Output.To(_ => _.ToSql()));
			}

			return sqlBuilder.AppendLine(";").ToString();
		}

		public static string ToSql([NotNull] this DeleteRequest @this)
		{
			var sqlBuilder = new StringBuilder(@$"DELETE FROM {@this.From}");

			if (!@this.Where.IsBlank())
				sqlBuilder.AppendLine().Append($"WHERE {@this.Where}");

			if (@this.Output.Any())
				sqlBuilder.AppendLine().Append("OUTPUT ").AppendJoin(SQL_DELIMETER, @this.Output.To(_ => _.ToSql()));

			return sqlBuilder.AppendLine(";").ToString();
		}

		public static string ToSql([NotNull] this InsertRequest @this)
			=> new StringBuilder("INSERT INTO ").AppendLine(@this.Into)
				.AppendJoin(", ", @this.Insert.To(column => column.EscapeIdentifier())).AppendLine()
				.Append(((SelectRequest)@this).ToSql())
				.ToString();

		public static string ToSql([NotNull] this UpdateRequest @this)
		{
			var updateCsv = @this.Set.Join(SQL_DELIMETER, item => $"{item.Column.EscapeIdentifier()} = {item.Expression}");

			var sqlBuilder = new StringBuilder(@$"UPDATE {@this.Table}
SET {updateCsv}");

			if (!@this.Where.IsBlank())
				sqlBuilder.AppendLine().Append($"WHERE {@this.Where}");

			if (@this.Output.Any())
				sqlBuilder.AppendLine().Append("OUTPUT ").AppendJoin(SQL_DELIMETER, @this.Output.To(_ => _.ToSql()));

			return sqlBuilder.AppendLine(";").ToString();
		}

		public static string ToSql(this object? @this) => @this switch
		{
			null => "NULL",
			DBNull _ => "NULL",
			bool boolean when boolean => "1",
			bool boolean when !boolean => "0",
			char text => text.Equals('\'') ? "N''''" : $"N'{text}'",
			string text => $"N'{text.EscapeValue()}'",
			DateTime dateTime => $"'{dateTime:o}'",
			DateTimeOffset dateTimeOffset => $"'{dateTimeOffset:o}'",
			TimeSpan time => $"'{time:c}'",
			Guid guid => $"'{guid:D}'",
			LogicalOperator token when token == LogicalOperator.And => "AND",
			LogicalOperator token when token == LogicalOperator.Or => "OR",
			LogicalOperator token => throw new NotImplementedException($"{nameof(LogicalOperator)}.{token.Name()} is not implemented for SQL."),
			Sort token when token == Sort.Ascending => "ASC",
			Sort token when token == Sort.Descending => "DESC",
			Sort _ => string.Empty,
			Enum token => token.Number(),
			Index index => $"'{index}'",
			JsonElement json when json.ValueKind == JsonValueKind.String => $"N'{json.GetString().EscapeValue()}'",
			JsonElement json when json.ValueKind == JsonValueKind.Number => json.ToString(),
			JsonElement json when json.ValueKind == JsonValueKind.True => "1",
			JsonElement json when json.ValueKind == JsonValueKind.False => "0",
			JsonElement json when json.ValueKind == JsonValueKind.Null => "NULL",
			JsonElement json => $"N'{json.ToString().EscapeValue()}'",
			Range range => $"'{range}'",
			Uri uri => $"'{uri.ToString().EscapeValue()}'",
			byte[] binary => Encoding.Default.GetString(binary),
			ColumnSort columnSort => $"{columnSort.Expression} {columnSort.Sort.ToSql()}",
			OutputExpression output => !output.As.IsBlank() ? $"{output.Expression} AS {output.As.EscapeIdentifier()}" : output.Expression,
			IEnumerable enumerable => $"({enumerable.As<object>().Join(", ", _ => _.ToSql())})",
			_ => @this.ToString() ?? "NULL"
		};

		public static string ToSql([NotNull] this SelectRequest @this)
		{
			var sqlBuilder = new StringBuilder("SELECT ");

			if (@this.Select.Any())
				sqlBuilder.AppendJoin(SQL_DELIMETER, @this.Select.To(_ => _.ToSql())).AppendLine();
			else
				sqlBuilder.AppendLine("*");

			sqlBuilder.Append($"FROM {@this.From}");

			if (!@this.Where.IsBlank())
				sqlBuilder.AppendLine().Append($"WHERE {@this.Where}");

			if (!@this.Having.IsBlank())
				sqlBuilder.AppendLine().Append($"HAVING {@this.Having}");

			if (@this.OrderBy.Any())
				sqlBuilder.AppendLine().Append($"ORDER BY {@this.OrderBy.Join(SQL_DELIMETER, _ => _.ToSql())}");

			return sqlBuilder.AppendLine(";").ToString();
		}
	}
}
