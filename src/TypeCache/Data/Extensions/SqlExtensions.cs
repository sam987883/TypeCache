// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using TypeCache.Collections.Extensions;
using TypeCache.Extensions;
using static System.FormattableString;

namespace TypeCache.Data.Extensions
{
	public static class SqlExtensions
	{
		private const string SQL_DELIMETER = "\r\n\t, ";

		private static StringBuilder AppendColumnsSQL(this StringBuilder @this, string[] columns)
			=> @this.Append('(').AppendJoin(", ", columns.To(column => column.EscapeIdentifier())).Append(')');

		private static StringBuilder AppendOutputSQL(this StringBuilder @this, IDictionary<string, string> output)
			=> @this.AppendLine().Append("OUTPUT ").AppendJoin(SQL_DELIMETER, output.To(pair =>
			{
				var column = pair.Key.EscapeIdentifier();
				if (pair.Value.Is("INSERTED"))
					return Invariant($"INSERTED.{column} AS {column}");
				else if (pair.Value.Is("DELETED"))
					return Invariant($"DELETED.{column} AS {column}");
				return Invariant($"{pair.Value} AS {column}");
			}));

		public static string EscapeIdentifier([NotNull] this string @this)
			=> @this.StartsWith('[') && @this.EndsWith(']')
				? Invariant($"[{@this.Substring(1, @this.Length - 2).Replace("]", "]]")}]")
				: Invariant($"[{@this.Replace("]", "]]")}]");

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string EscapeLikeValue([NotNull] this string @this)
			=> @this.Replace("'", "''").Replace("[", "[[]").Replace("%", "[%]").Replace("_", "[_]");

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string EscapeValue([NotNull] this string @this)
			=> @this.Replace("'", "''");

		public static string ToSQL([NotNull] this BatchRequest @this)
		{
			var batchDataCsv = @this.Input.Rows.To(row => $"({row.To(value => value.ToSQL()).Join(", ")})").Join(SQL_DELIMETER);

			var sqlBuilder = new StringBuilder();
			if (!@this.Delete && !@this.Update.Any())
			{
				sqlBuilder.Append(Invariant($"INSERT INTO {@this.Table} ")).AppendColumnsSQL(@this.Insert!);
				if (@this.Output.Any())
					sqlBuilder.AppendOutputSQL(@this.Output);

				sqlBuilder.Append(Invariant(@$"
VALUES {batchDataCsv}"));
			}
			else
			{
				sqlBuilder.Append(Invariant(@$"MERGE {@this.Table} AS t WITH(UPDLOCK)
USING
(
	VALUES {batchDataCsv}
) AS s ")).AppendColumnsSQL(@this.Input.Columns).Append(@"
ON ").AppendJoin(Invariant($" {LogicalOperator.And.ToSQL()} "), @this.On.To(column => Invariant($"s.{column.EscapeIdentifier()} = t.{column.EscapeIdentifier()}")));

				if (@this.Update.Any())
					sqlBuilder.Append(@"
WHEN MATCHED THEN
	UPDATE SET ").AppendJoin(SQL_DELIMETER, @this.Update.To(column => Invariant($"{column.EscapeIdentifier()} = s.{column.EscapeIdentifier()}")));

				if (@this.Delete)
					sqlBuilder.Append(@"
WHEN NOT MATCHED BY SOURCE THEN
	DELETE");

				if (@this.Insert.Any())
					sqlBuilder.Append(Invariant($@"
WHEN NOT MATCHED BY TARGET THEN
	INSERT ")).AppendColumnsSQL(@this.Insert).Append(@"
	VALUES
	(
	").AppendJoin(SQL_DELIMETER, @this.Insert.To(column => Invariant($"s.{column.EscapeIdentifier()}"))).Append(@"
	)");

				if (@this.Output.Any())
					sqlBuilder.AppendOutputSQL(@this.Output);
			}

			return sqlBuilder.Append(';').AppendLine().ToString();
		}

		public static string ToSQL([NotNull] this DeleteRequest @this)
		{
			var sqlBuilder = new StringBuilder("DELETE FROM ").Append(@this.From);

			if (@this.Output.Any())
				sqlBuilder.AppendOutputSQL(@this.Output);

			if (!@this.Where.IsBlank())
				sqlBuilder.AppendLine().Append("WHERE ").Append(@this.Where);

			return sqlBuilder.Append(';').AppendLine().ToString();
		}

		public static string ToSQL([NotNull] this InsertRequest @this)
		{
			var sqlBuilder = new StringBuilder("INSERT INTO ").Append(@this.Into).Append(' ').AppendColumnsSQL(@this.Insert);

			if (@this.Output.Any())
				sqlBuilder.AppendOutputSQL(@this.Output);

			return sqlBuilder.AppendLine().Append(((SelectRequest)@this).ToSQL()).ToString();
		}

		public static string ToSQL([NotNull] this SelectRequest @this)
		{
			var sqlBuilder = new StringBuilder("SELECT ");

			if (@this.Select.Any())
				sqlBuilder.AppendJoin(SQL_DELIMETER, @this.Select.To(_ => _.Key.Is(_.Value) ? Invariant($"[{_.Key}]") : Invariant($"{_.Value} AS [{_.Key}]"))).AppendLine();
			else
				sqlBuilder.AppendLine("*");

			sqlBuilder.Append("FROM ").Append(@this.From).Append(" WITH(NOLOCK)");

			if (!@this.Where.IsBlank())
				sqlBuilder.AppendLine().Append("WHERE ").Append(@this.Where);

			if (!@this.Having.IsBlank())
				sqlBuilder.AppendLine().Append("HAVING ").Append(@this.Having);

			if (@this.OrderBy.Any())
				sqlBuilder.AppendLine().Append("ORDER BY ").AppendJoin(", ", @this.OrderBy.To(_ => Invariant($"[{_.Key}] {_.Value.ToSQL()}")));

			return sqlBuilder.Append(';').AppendLine().ToString();
		}

		public static string ToSQL([NotNull] this UpdateRequest @this)
		{
			var updateCsv = @this.Set.To(_ => Invariant($"{_.Key.EscapeIdentifier()} = {_.Value.ToSQL()}")).Join(SQL_DELIMETER);

			var sqlBuilder = new StringBuilder("UPDATE ").Append(@this.Table).AppendLine(" WITH(UPDLOCK)")
				.Append("SET ").Append(updateCsv);

			if (@this.Output.Any())
				sqlBuilder.AppendOutputSQL(@this.Output);

			if (!@this.Where.IsBlank())
				sqlBuilder.AppendLine().Append("WHERE ").Append(@this.Where);

			return sqlBuilder.Append(';').AppendLine().ToString();
		}

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
			byte[] binary => Encoding.Default.GetString(binary),
			IEnumerable enumerable => Invariant($"({enumerable.As<object>().To(_ => _.ToSQL()).Join(", ")})"),
			_ => @this.ToString() ?? "NULL"
		};
	}
}
