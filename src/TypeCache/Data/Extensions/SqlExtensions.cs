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
		private static StringBuilder AppendColumnsSQL(this StringBuilder @this, string[] columns)
			=> @this.Append('(').AppendJoin(", ", columns.To(column => column.EscapeIdentifier())).Append(')');

		private static StringBuilder AppendOutputSQL(this StringBuilder @this, IDictionary<string, string> output)
		{
			@this.AppendLine().Append("OUTPUT ");
			output.Do(pair =>
			{
				var column = pair.Key.EscapeIdentifier();
				if (pair.Value.Is("INSERTED"))
					@this.Append(Invariant($"INSERTED.{column} AS {column}"));
				else if (pair.Value.Is("DELETED"))
					@this.Append(Invariant($"DELETED.{column} AS {column}"));
				else
					@this.Append(Invariant($"{pair.Value} AS {column}"));
			}, () => @this.AppendLine().Append('\t').Append(',').Append(' '));
			return @this;
		}

		private static StringBuilder AppendSetSQL(this StringBuilder @this, IDictionary<string, object?> update)
		{
			@this.Append("SET").Append(' ');
			update.Do(pair => @this.Append(pair.Key.EscapeIdentifier()).Append(' ').Append('=').Append(' ').Append(pair.Value.ToSQL()),
				() => @this.AppendLine().Append('\t').Append(',').Append(' '));
			return @this;
		}

		private static StringBuilder AppendValuesSQL(this StringBuilder @this, object?[][] rows)
		{
			@this.Append("VALUES").Append(' ');
			rows.Do(row => @this.Append('(').Append(row.To(value => value.ToSQL()).Join(", ")).Append(')'),
				() => @this.AppendLine().Append('\t').Append(',').Append(' '));
			return @this;
		}

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
			var sqlBuilder = new StringBuilder();
			if (!@this.Delete && !@this.Update.Any())
			{
				sqlBuilder.Append(Invariant($"INSERT INTO {@this.Table} ")).AppendColumnsSQL(@this.Insert!);
				if (@this.Output.Any())
					sqlBuilder.AppendOutputSQL(@this.Output);
				sqlBuilder.AppendLine().AppendValuesSQL(@this.Input.Rows);
			}
			else
			{
				sqlBuilder.Append(Invariant(@$"MERGE {@this.Table} AS t WITH(UPDLOCK)
USING
(
	")).AppendValuesSQL(@this.Input.Rows).Append(@"
) AS s ").AppendColumnsSQL(@this.Input.Columns).Append(@"
ON ").AppendJoin(Invariant($" {LogicalOperator.And.ToSQL()} "), @this.On.To(column => Invariant($"s.{column.EscapeIdentifier()} = t.{column.EscapeIdentifier()}")));

				if (@this.Update.Any())
					sqlBuilder.Append(@"
WHEN MATCHED THEN
	UPDATE SET ");

				@this.Update.Do(column => sqlBuilder.Append(Invariant($"{column.EscapeIdentifier()} = s.{column.EscapeIdentifier()}")),
					() => sqlBuilder.AppendLine().Append('\t').Append(',').Append(' '));

				if (@this.Delete)
					sqlBuilder.Append(@"
WHEN NOT MATCHED BY SOURCE THEN
	DELETE");

				if (@this.Insert.Any())
					sqlBuilder.Append(Invariant($@"
WHEN NOT MATCHED BY TARGET THEN
	INSERT ")).AppendColumnsSQL(@this.Insert).Append(@"
	VALUES (").AppendJoin(", ", @this.Insert.To(column => Invariant($"s.{column.EscapeIdentifier()}"))).Append(')');

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
			var sqlBuilder = new StringBuilder(Invariant($"INSERT INTO {@this.Into} ")).AppendColumnsSQL(@this.Insert);

			if (@this.Output.Any())
				sqlBuilder.AppendOutputSQL(@this.Output);

			return sqlBuilder.AppendLine().Append(((SelectRequest)@this).ToSQL()).ToString();
		}

		public static string ToSQL([NotNull] this SelectRequest @this)
		{
			var sqlBuilder = new StringBuilder("SELECT ");

			if (@this.Select.Any())
				@this.Select.Do(_ =>
				{
					if (_.Key.Is(_.Value))
						sqlBuilder.Append(Invariant($"[{_.Key}]"));
					else
						sqlBuilder.Append(Invariant($"{_.Value} AS [{_.Key}]"));
				}, () => sqlBuilder.AppendLine().Append('\t').Append(',').Append(' '));
			else
				sqlBuilder.Append('*');

			sqlBuilder.AppendLine().Append(Invariant($"FROM {@this.From} WITH(NOLOCK)"));

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
			var sqlBuilder = new StringBuilder(Invariant($"UPDATE {@this.Table} WITH(UPDLOCK)"))
				.AppendLine().AppendSetSQL(@this.Set);

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
