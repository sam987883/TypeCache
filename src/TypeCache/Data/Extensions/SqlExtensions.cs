// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using TypeCache.Collections.Extensions;
using TypeCache.Extensions;

namespace TypeCache.Data.Extensions
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

		public static string ToSQL([NotNull] this BatchRequest @this)
		{
			var batchDataCsv = @this.Input.Rows.To(row => $"({row.ToCsv(value => value.ToSQL())})").Join(SQL_DELIMETER);
			var sourceColumnCsv = @this.Input.Columns.To(column => column.EscapeIdentifier()).Join(", ");
			var onSql = @this.On.To(column => $"s.{column.EscapeIdentifier()} = t.{column.EscapeIdentifier()}").Join($" {LogicalOperator.And.ToSQL()} ");
			var updateCsv = @this.Update.To(column => $"{column.EscapeIdentifier()} = s.{column.EscapeIdentifier()}").Join(SQL_DELIMETER);
			var insertColumnCsv = @this.Insert.To(column => column.EscapeIdentifier()).Join(SQL_DELIMETER);
			var insertValueCsv = @this.Insert.To(column => $"s.{column.EscapeIdentifier()}").Join(SQL_DELIMETER);

			var sqlBuilder = new StringBuilder();
			if (!@this.Delete && updateCsv.IsBlank())
			{
				sqlBuilder.Append("INSERT INTO ").AppendLine(@this.Table)
					.Append('\t').Append('(').AppendLine()
					.Append('\t').AppendLine(insertColumnCsv)
					.Append('\t').Append(')');

				if (@this.Output.Any())
					sqlBuilder.AppendLine().Append("OUTPUT ").AppendJoin(SQL_DELIMETER, @this.Output.To(_ => $"{_.Value} AS [{_.Key}]"));

				sqlBuilder.AppendLine().Append("VALUES ").Append(batchDataCsv);
			}
			else
			{
				sqlBuilder.Append("MERGE ").Append(@this.Table).AppendLine(" AS t")
					.Append('\t').AppendLine("USING")
					.Append('\t').Append('(').AppendLine()
					.Append('\t').Append("VALUES ").AppendLine(batchDataCsv)
					.Append('\t').Append(')').Append(" AS s ").Append('(').Append(sourceColumnCsv).Append(')').AppendLine()
					.Append('\t').Append("ON ").Append(onSql);

				if (!updateCsv.IsBlank())
				{
					sqlBuilder.AppendLine().AppendLine("WHEN MATCHED THEN")
						.Append('\t').Append("UPDATE SET ").Append(updateCsv);
					if (@this.Delete)
						sqlBuilder.AppendLine().AppendLine("WHEN NOT MATCHED BY SOURCE THEN")
							.Append('\t').Append("DELETE");
				}
				else if (@this.Delete)
					sqlBuilder.AppendLine().AppendLine("WHEN MATCHED THEN")
						.Append('\t').Append("DELETE");

				if (!insertColumnCsv.IsBlank())
					sqlBuilder.AppendLine().AppendLine("WHEN NOT MATCHED BY TARGET THEN")
						.Append('\t').AppendLine("INSERT")
						.Append('\t').Append('(').AppendLine()
						.Append('\t').AppendLine(insertColumnCsv)
						.Append('\t').Append(')').AppendLine()
						.Append('\t').AppendLine("VALUES")
						.Append('\t').Append('(').AppendLine()
						.Append('\t').AppendLine(insertValueCsv)
						.Append('\t').Append(')');

				if (@this.Output.Any())
					sqlBuilder.AppendLine().Append("OUTPUT ").AppendJoin(SQL_DELIMETER, @this.Output.To(_ => $"{_.Value} AS [{_.Key}]"));
			}

			return sqlBuilder.Append(';').AppendLine().ToString();
		}

		public static string ToSQL([NotNull] this DeleteRequest @this)
		{
			var sqlBuilder = new StringBuilder("DELETE FROM ").Append(@this.From);

			if (@this.Output.Any())
				sqlBuilder.AppendLine().Append("OUTPUT ").AppendJoin(SQL_DELIMETER, @this.Output.To(_ => $"{_.Value} AS [{_.Key}]"));

			if (!@this.Where.IsBlank())
				sqlBuilder.AppendLine().Append("WHERE ").Append(@this.Where);

			return sqlBuilder.Append(';').AppendLine().ToString();
		}

		public static string ToSQL([NotNull] this InsertRequest @this)
		{
			var sqlBuilder = new StringBuilder("INSERT INTO ").AppendLine(@this.Into)
				.Append('(').AppendJoin(", ", @this.Insert.To(column => column.EscapeIdentifier())).Append(')');

			if (@this.Output.Any())
				sqlBuilder.AppendLine().Append("OUTPUT ").AppendJoin(SQL_DELIMETER, @this.Output.To(_ => _.ToSQL()));

			return sqlBuilder.AppendLine().Append(((SelectRequest)@this).ToSQL()).ToString();
		}

		public static string ToSQL([NotNull] this UpdateRequest @this)
		{
			var updateCsv = @this.Set.To(_ => $"{_.Key.EscapeIdentifier()} = {_.Value.ToSQL()}").Join(SQL_DELIMETER);

			var sqlBuilder = new StringBuilder("UPDATE ").AppendLine(@this.Table)
				.Append("SET ").Append(updateCsv);

			if (@this.Output.Any())
				sqlBuilder.AppendLine().Append("OUTPUT ").AppendJoin(SQL_DELIMETER, @this.Output.To(_ => $"{_.Value} AS [{_.Key}]"));

			if (!@this.Where.IsBlank())
				sqlBuilder.AppendLine().Append("WHERE ").Append(@this.Where);

			return sqlBuilder.Append(';').AppendLine().ToString();
		}

		public static string ToSQL(this object? @this) => @this switch
		{
			null or DBNull => "NULL",
			bool boolean => boolean ? "1" : "0",
			char text => text.Equals('\'') ? "N''''" : $"N'{text}'",
			string text => $"N'{text.EscapeValue()}'",
			DateTime dateTime => $"'{dateTime:o}'",
			DateTimeOffset dateTimeOffset => $"'{dateTimeOffset:o}'",
			TimeSpan time => $"'{time:c}'",
			Guid guid => $"'{guid:D}'",
			LogicalOperator.And => "AND",
			LogicalOperator.Or => "OR",
			LogicalOperator _ => throw new NotImplementedException($"{nameof(LogicalOperator)}.{@this} is not implemented for SQL."),
			Sort.Ascending => "ASC",
			Sort.Descending => "DESC",
			Sort _ => string.Empty,
			Enum token => token.ToString("D"),
			Index index => $"'{index}'",
			JsonElement json => json.ValueKind switch
			{
				JsonValueKind.String => $"N'{json.GetString()!.EscapeValue()}'",
				JsonValueKind.Number => json.ToString()!,
				JsonValueKind.True => "1",
				JsonValueKind.False => "0",
				JsonValueKind.Null => "NULL",
				_ => $"N'{json.ToString()!.EscapeValue()}'"
			},
			Range range => $"'{range}'",
			Uri uri => $"'{uri.ToString().EscapeValue()}'",
			byte[] binary => Encoding.Default.GetString(binary),
			IEnumerable enumerable => $"({enumerable.As<object>().To(_ => _.ToSQL()).Join(", ")})",
			_ => @this.ToString() ?? "NULL"
		};

		public static string ToSQL([NotNull] this SelectRequest @this)
		{
			var sqlBuilder = new StringBuilder("SELECT ");

			if (@this.Select.Any())
				sqlBuilder.AppendJoin(SQL_DELIMETER, @this.Select.To(_ => $"{_.Value} AS [{_.Key}]")).AppendLine();
			else
				sqlBuilder.AppendLine("*");

			sqlBuilder.Append("FROM ").Append(@this.From);

			if (!@this.Where.IsBlank())
				sqlBuilder.AppendLine().Append("WHERE ").Append(@this.Where);

			if (!@this.Having.IsBlank())
				sqlBuilder.AppendLine().Append("HAVING ").Append(@this.Having);

			if (@this.OrderBy.Any())
				sqlBuilder.AppendLine().Append("ORDER BY ").Append(@this.OrderBy.To(_ => _.ToSQL()).Join(SQL_DELIMETER));

			return sqlBuilder.Append(';').AppendLine().ToString();
		}
	}
}
