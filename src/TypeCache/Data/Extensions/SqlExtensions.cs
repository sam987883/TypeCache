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
				sqlBuilder.Append("INSERT INTO ").AppendLine(@this.Table)
					.Append('\t').Append('(').AppendLine()
					.Append('\t').AppendLine(insertColumnCsv)
					.Append('\t').Append(')');

				if (@this.Output.Any())
					sqlBuilder.AppendLine().Append("OUTPUT ").AppendJoin(SQL_DELIMETER, @this.Output.To(_ => _.ToSql()));

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
					sqlBuilder.AppendLine().Append("OUTPUT ").AppendJoin(SQL_DELIMETER, @this.Output.To(_ => _.ToSql()));
			}

			return sqlBuilder.Append(';').AppendLine().ToString();
		}

		public static string ToSql([NotNull] this DeleteRequest @this)
		{
			var sqlBuilder = new StringBuilder("DELETE FROM ").Append(@this.From);

			if (@this.Output.Any())
				sqlBuilder.AppendLine().Append("OUTPUT ").AppendJoin(SQL_DELIMETER, @this.Output.To(_ => _.ToSql()));

			if (!@this.Where.IsBlank())
				sqlBuilder.AppendLine().Append("WHERE ").Append(@this.Where);

			return sqlBuilder.Append(';').AppendLine().ToString();
		}

		public static string ToSql([NotNull] this InsertRequest @this)
		{
			var sqlBuilder = new StringBuilder("INSERT INTO ").AppendLine(@this.Into)
				.Append('(').AppendJoin(", ", @this.Insert.To(column => column.EscapeIdentifier())).Append(')');

			if (@this.Output.Any())
				sqlBuilder.AppendLine().Append("OUTPUT ").AppendJoin(SQL_DELIMETER, @this.Output.To(_ => _.ToSql()));

			return sqlBuilder.AppendLine().Append(((SelectRequest)@this).ToSql()).ToString();
		}

		public static string ToSql([NotNull] this UpdateRequest @this)
		{
			var updateCsv = @this.Set.Join(SQL_DELIMETER, item => $"{item.Column.EscapeIdentifier()} = {item.Expression}");

			var sqlBuilder = new StringBuilder("UPDATE ").AppendLine(@this.Table)
				.Append("SET ").Append(updateCsv);

			if (@this.Output.Any())
				sqlBuilder.AppendLine().Append("OUTPUT ").AppendJoin(SQL_DELIMETER, @this.Output.To(_ => _.ToSql()));

			if (!@this.Where.IsBlank())
				sqlBuilder.AppendLine().Append("WHERE ").Append(@this.Where);

			return sqlBuilder.Append(';').AppendLine().ToString();
		}

		public static string ToSql(this object? @this) => @this switch
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
			Enum token => token.Number(),
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

			sqlBuilder.Append("FROM ").Append(@this.From);

			if (!@this.Where.IsBlank())
				sqlBuilder.AppendLine().Append("WHERE ").Append(@this.Where);

			if (!@this.Having.IsBlank())
				sqlBuilder.AppendLine().Append("HAVING ").Append(@this.Having);

			if (@this.OrderBy.Any())
				sqlBuilder.AppendLine().Append("ORDER BY ").Append(@this.OrderBy.Join(SQL_DELIMETER, _ => _.ToSql()));

			return sqlBuilder.Append(';').AppendLine().ToString();
		}
	}
}
