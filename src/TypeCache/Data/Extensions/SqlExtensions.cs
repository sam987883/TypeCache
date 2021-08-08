// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using TypeCache.Collections.Extensions;
using TypeCache.Data.Requests;
using TypeCache.Data.Schema;
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
			@this.Append("OUTPUT ");
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
			rows.Do(row => @this.Append('(').AppendJoin(", ", row.To(value => value.ToSQL())).Append(')'),
				() => @this.AppendLine().Append("\t, "));
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

		public static string ToSQL([NotNull] this DeleteDataRequest @this)
		{
			var sqlBuilder = new StringBuilder("DELETE x");

			if (@this.Output.Any())
				sqlBuilder.AppendLine().AppendOutputSQL(@this.Output);

			sqlBuilder.AppendLine().Append("FROM ").Append(@this.From).AppendLine(" AS x")
				.AppendLine("INNER JOIN")
				.AppendLine("(")
				.AppendValuesSQL(@this.Input.Rows)
				.AppendLine().Append($") AS i (").AppendJoin(", ", @this.Input.Columns.To(column => column.EscapeIdentifier())).AppendLine(")")
				.Append("ON ");

			@this.Input.Columns.Do(column => sqlBuilder.Append($"i.{column.EscapeIdentifier()} = x.{column.EscapeIdentifier()}"),
				() => sqlBuilder.Append(" AND "));

			return sqlBuilder.Append(';').AppendLine().ToString();
		}

		public static string ToSQL([NotNull] this DeleteRequest @this)
		{
			var sqlBuilder = new StringBuilder("DELETE FROM ").Append(@this.From);

			if (@this.Output.Any())
				sqlBuilder.AppendLine().AppendOutputSQL(@this.Output);

			if (!@this.Where.IsBlank())
				sqlBuilder.AppendLine().Append("WHERE ").Append(@this.Where);

			return sqlBuilder.Append(';').AppendLine().ToString();
		}

		public static string ToSQL([NotNull] this InsertDataRequest @this)
		{
			var sqlBuilder = new StringBuilder(Invariant($"INSERT INTO {@this.Into} ")).AppendColumnsSQL(@this.Input.Columns);

			if (@this.Output.Any())
				sqlBuilder.AppendLine().AppendOutputSQL(@this.Output);

			return sqlBuilder.AppendLine().AppendValuesSQL(@this.Input.Rows).Append(';').AppendLine().ToString();
		}

		public static string ToSQL([NotNull] this InsertRequest @this)
		{
			var sqlBuilder = new StringBuilder(Invariant($"INSERT INTO {@this.Into} ")).AppendColumnsSQL(@this.Insert);

			if (@this.Output.Any())
				sqlBuilder.AppendLine().AppendOutputSQL(@this.Output);

			return sqlBuilder.AppendLine().Append(((SelectRequest)@this).ToSQL()).ToString();
		}

		public static string ToSQL([NotNull] this SelectRequest @this)
		{
			var sqlBuilder = new StringBuilder("SELECT ");

			if (@this.Select.Any())
				@this.Select.Do(_ =>
				{
					if (_.Key.Is(_.Value))
						sqlBuilder.Append(_.Key.EscapeIdentifier());
					else
						sqlBuilder.Append(Invariant($"{_.Value} AS {_.Key.EscapeIdentifier()}"));
				}, () => sqlBuilder.AppendLine().Append("\t, "));
			else
				sqlBuilder.Append('*');

			sqlBuilder.AppendLine().Append(Invariant($"FROM {@this.From} WITH(NOLOCK)"));

			if (!@this.Where.IsBlank())
				sqlBuilder.AppendLine().Append("WHERE ").Append(@this.Where);

			if (!@this.Having.IsBlank())
				sqlBuilder.AppendLine().Append("HAVING ").Append(@this.Having);

			if (@this.OrderBy.Any())
				sqlBuilder.AppendLine().Append("ORDER BY ").AppendJoin(", ", @this.OrderBy.To(_ => Invariant($"{(int.TryParse(_.Item1, out var _) ? _.Item1 : _.Item1.EscapeIdentifier())} {_.Item2.ToSQL()}")));

			return sqlBuilder.Append(';').AppendLine().ToString();
		}

		public static string ToSQL([NotNull] this UpdateRequest @this)
		{
			var sqlBuilder = new StringBuilder(Invariant($"UPDATE {@this.Table} WITH(UPDLOCK)"))
				.AppendLine().AppendSetSQL(@this.Set);

			if (@this.Output.Any())
				sqlBuilder.AppendLine().AppendOutputSQL(@this.Output);

			if (!@this.Where.IsBlank())
				sqlBuilder.AppendLine().Append("WHERE ").Append(@this.Where);

			return sqlBuilder.Append(';').AppendLine().ToString();
		}

		public static string ToSQL([NotNull] this UpdateDataRequest @this, ObjectSchema schema)
		{
			var sqlBuilder = new StringBuilder("UPDATE x WITH(UPDLOCK)");

			var primaryKeys = schema.Columns.If(column => column.PrimaryKey).To(column => column.Name).ToArray();

			sqlBuilder.AppendLine().Append("SET ");
			@this.Input.Columns.Without(primaryKeys)
				.Do(column => sqlBuilder.Append(Invariant($"{column.EscapeIdentifier()} = i.{column.EscapeIdentifier()}")),
					() => sqlBuilder.AppendLine().Append("\t, "));

			if (@this.Output.Any())
				sqlBuilder.AppendLine().AppendOutputSQL(@this.Output);

			sqlBuilder.AppendLine().AppendLine(Invariant($"FROM {@this.Table} AS x"))
				.AppendLine("INNER JOIN")
				.AppendLine("(")
				.AppendValuesSQL(@this.Input.Rows).AppendLine()
				.Append($") AS i (").AppendJoin(", ", @this.Input.Columns.To(column => column.EscapeIdentifier())).AppendLine(")")
				.Append("ON ");

			primaryKeys.Do(primaryKey => sqlBuilder.Append($"i.{primaryKey.EscapeIdentifier()} = x.{primaryKey.EscapeIdentifier()}"),
				() => sqlBuilder.Append(" AND "));

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
