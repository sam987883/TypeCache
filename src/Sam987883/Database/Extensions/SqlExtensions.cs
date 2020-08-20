// Copyright (c) 2020 Samuel Abraham

using Sam987883.Common.Extensions;
using Sam987883.Common.Models;
using Sam987883.Database.Models;
using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.Json;

namespace Sam987883.Database.Extensions
{
	public static class SqlExtensions
	{
		private const string SQL_DELIMETER = "\r\n\t, ";

		public static string EscapeIdentifier([NotNull] this string @this) =>
			$"[{@this.Replace("]", "]]")}]";

		public static string EscapeLikeValue([NotNull] this string @this) =>
			@this.Replace("'", "''").Replace("[", "[[]").Replace("%", "[%]").Replace("_", "[_]");

		public static string EscapeValue([NotNull] this string @this) =>
			@this.Replace("'", "''");

		public static string ToSQL([NotNull] this BatchRequest @this, ObjectSchema schema)
		{
			var batchDataCsv = @this.Input.Rows.Join(SQL_DELIMETER, row => $"({row.ToCsv(value => value.ToSQL(null))})");
			var sourceColumnCsv = @this.Input.Columns.Join(", ", column => column.EscapeIdentifier());
			var onSql = @this.On.Join($" {LogicalOperator.And.ToSQL()} ", column => $"s.{column.EscapeIdentifier()} = t.{column.EscapeIdentifier()}");
			var updateCsv = @this.Update.Join(SQL_DELIMETER, column => $"{column.EscapeIdentifier()} = s.{column.EscapeIdentifier()}");
			var insertColumnCsv = @this.Insert.Join(SQL_DELIMETER, column => column.EscapeIdentifier());
			var insertValueCsv = @this.Insert.Join(SQL_DELIMETER, column => $"s.{column.EscapeIdentifier()}");

			var sqlBuilder = new StringBuilder();
			if (!@this.Delete && updateCsv.IsBlank())
			{
				sqlBuilder.Append(@$"INSERT INTO {schema.Name}
(
	{insertColumnCsv}
)");

				var outputColumnCsv = @this.OutputInserted.Join(SQL_DELIMETER, column => $"INSERTED.{column.EscapeIdentifier()}");
				if (!outputColumnCsv.IsBlank())
					sqlBuilder.AppendLine().Append($"OUTPUT {outputColumnCsv}");

				sqlBuilder.AppendLine().Append($"VALUES {batchDataCsv}");
			}
			else
			{
				sqlBuilder.Append($@"MERGE {schema.Name} AS t
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

				if (@this.OutputDeleted.Any() || @this.OutputInserted.Any())
				{
					sqlBuilder.AppendLine().Append("OUTPUT ");
					if (@this.OutputDeleted.Any())
					{
						var outputColumnCsv = @this.OutputDeleted.Join(SQL_DELIMETER, column => $"DELETED.{column.EscapeIdentifier()}");
						sqlBuilder.Append(outputColumnCsv);
					}

					if (@this.OutputInserted.Any())
					{
						var outputColumnCsv = @this.OutputInserted.Join(SQL_DELIMETER, column => $"INSERTED.{column.EscapeIdentifier()}");
						sqlBuilder.Append(outputColumnCsv);
					}
				}
			}

			return sqlBuilder.AppendLine(";").ToString();
		}

		public static string ToSQL([NotNull] this DeleteRequest @this, ObjectSchema schema)
		{
			var sqlBuilder = new StringBuilder(@$"DELETE FROM {schema.Name}");

			var whereSql = @this.Where.ToSQL(schema);
			if (!whereSql.IsBlank())
				sqlBuilder.AppendLine().Append($"WHERE {whereSql}");

			var outputColumnCsv = @this.Output.Join(SQL_DELIMETER, column => $"DELETED.{column.EscapeIdentifier()}");
			if (!outputColumnCsv.IsBlank())
				sqlBuilder.AppendLine().Append($"OUTPUT {outputColumnCsv}");

			return sqlBuilder.AppendLine(";").ToString();
		}

		public static string ToSQL([NotNull] this UpdateRequest @this, ObjectSchema schema)
		{
			var updateCsv = @this.Set.Join(SQL_DELIMETER, item => $"{item.Column.EscapeIdentifier()} = {item.Value.ToSQL(schema)}");

			var sqlBuilder = new StringBuilder(@$"UPDATE {schema.Name}
SET {updateCsv}");

			var whereSql = @this.Where.ToSQL(schema);
			if (!whereSql.IsBlank())
				sqlBuilder.AppendLine().Append($"WHERE {whereSql}");

			if (@this.OutputDeleted.Any() || @this.OutputInserted.Any())
			{
				sqlBuilder.AppendLine().Append("OUTPUT ");
				if (@this.OutputDeleted.Any())
				{
					var outputColumnCsv = @this.OutputDeleted.Join(SQL_DELIMETER, column => $"DELETED.{column.EscapeIdentifier()}");
					sqlBuilder.Append(outputColumnCsv);
				}

				if (@this.OutputInserted.Any())
				{
					var outputColumnCsv = @this.OutputInserted.Join(SQL_DELIMETER, column => $"INSERTED.{column.EscapeIdentifier()}");
					sqlBuilder.AppendLine().Append(outputColumnCsv);
				}
			}

			return sqlBuilder.AppendLine(";").ToString();
		}

		public static string ToSQL([NotNull] this ExpressionSet @this, ObjectSchema schema)
		{
			var logicalOperator = $" {@this.Operator.ToSQL()} ";
			var sql = string.Empty;

			if (@this.Expressions.Any())
			{
				sql = $"({@this.Expressions.Join(logicalOperator, expression => expression.ToSQL(schema))})";

				if (@this.ExpressionSets.Any())
				{
					sql = new StringBuilder(sql)
						.Append(logicalOperator)
						.Append(@this.ExpressionSets.Join(logicalOperator, expressionSet => expressionSet.ToSQL(schema)))
						.ToString();
				}
			}
			else if (@this.ExpressionSets.Any())
				sql = @this.ExpressionSets.Join(logicalOperator, expressionSet => expressionSet.ToSQL(schema));

			return sql;
		}

		public static string ToSQL([NotNull] this Expression @this, ObjectSchema schema)
		{
			var field = @this.Field.EscapeIdentifier();
			var value = @this.Value.ToSQL(schema);
			var @operator = @this.Operator.ToSQL();

			if (@this.Value is string pattern)
			{
				switch (@this.Operator)
				{
					case ComparisonOperator.Like:
					case ComparisonOperator.NotLike:
						return schema.HasColumn(pattern) || schema.HasParameter(pattern)
							? $"{field} {@operator} (N'%' + {value} + N'%')"
							: $"{field} {@operator} N'%{pattern.EscapeLikeValue()}%'";
					case ComparisonOperator.StartWith:
					case ComparisonOperator.NotStartWith:
						return schema.HasColumn(pattern) || schema.HasParameter(pattern)
							? $"{field} {@operator} ({value} + N'%')"
							: $"{field} {@operator} N'{pattern.EscapeLikeValue()}%'";
					case ComparisonOperator.EndWith:
					case ComparisonOperator.NotEndWith:
						return schema.HasColumn(pattern) || schema.HasParameter(pattern)
							? $"{field} {@operator} (N'%' + {value})"
							: $"{field} {@operator} N'%{pattern.EscapeLikeValue()}'";
				}
			}

			if (@this.Value == null)
			{
				switch (@this.Operator)
				{
					case ComparisonOperator.Equal: return $"{field} IS {value}";
					case ComparisonOperator.NotEqual: return $"{field} IS NOT {value}";
				}
			}

			if (@this.Value is IEnumerable && !(@this.Value is string))
			{
				switch (@this.Operator)
				{
					case ComparisonOperator.Equal: return $"{field} IN {value}";
					case ComparisonOperator.NotEqual: return $"{field} NOT IN {value}";
				}
			}

			return $"{field} {@operator} {value}";
		}

		public static string ToSQL(this ComparisonOperator @this) => @this switch
		{
			ComparisonOperator.Equal => "=",
			ComparisonOperator.NotEqual => "<>",
			ComparisonOperator.LessThan => "<",
			ComparisonOperator.LessThanOrEqual => "<=",
			ComparisonOperator.MoreThan => ">",
			ComparisonOperator.MoreThanOrEqual => ">=",
			ComparisonOperator.Like => "LIKE",
			ComparisonOperator.StartWith => "LIKE",
			ComparisonOperator.EndWith => "LIKE",
			ComparisonOperator.NotLike => "NOT LIKE",
			ComparisonOperator.NotStartWith => "NOT LIKE",
			ComparisonOperator.NotEndWith => "NOT LIKE",
			_ => throw new NotImplementedException($"{nameof(ComparisonOperator)}.{@this.Name()} is not implemented for SQL.")
		};

		public static string ToSQL(this LogicalOperator @this) => @this switch
		{
			LogicalOperator.And => "AND",
			LogicalOperator.Or => "OR",
			_ => throw new NotImplementedException($"{nameof(LogicalOperator)}.{@this.Name()} is not implemented for SQL.")
		};

		public static string ToSQL(this object? @this, ObjectSchema? schema) => @this switch
		{
			null => "NULL",
			DBNull _ => "NULL",
			bool boolean when boolean => "1",
			bool boolean when !boolean => "0",
			char text => text.Equals('\'') ? "N''''" : $"N'{text}'",
			string text when schema?.HasColumn(text) == true => text.EscapeIdentifier(),
			string text when schema?.HasParameter(text) == true => '@' + text,
			string text => $"N'{text.EscapeValue()}'",
			DateTime dateTime => $"'{dateTime:o}'",
			DateTimeOffset dateTimeOffset => $"'{dateTimeOffset:o}'",
			TimeSpan time => $"'{time:c}'",
			Guid guid => $"'{guid:D}'",
			Enum token => token.Number(),
			Index index => $"'{index}'",
			JsonElement json when json.ValueKind == JsonValueKind.String => json.GetString().ToSQL(schema),
			JsonElement json when json.ValueKind == JsonValueKind.Number => json.ToString(),
			JsonElement json when json.ValueKind == JsonValueKind.True => "1",
			JsonElement json when json.ValueKind == JsonValueKind.False => "0",
			JsonElement json when json.ValueKind == JsonValueKind.Null => "NULL",
			JsonElement json => $"N'{json.ToString().EscapeValue()}'",
			Range range => $"'{range}'",
			Uri uri => $"'{uri.ToString().EscapeValue()}'",
			byte[] binary => Encoding.Default.GetString(binary),
			IEnumerable enumerable => $"({enumerable.As<object>().Join(", ", _ => _.ToSQL(schema))})",
			_ => @this.ToString() ?? "NULL"
		};

		public static string? ToSQL(this AggregateFunction @this) => @this switch
		{
			AggregateFunction.Average => "AVERAGE",
			AggregateFunction.CheckSum => "CHECKSUM_AGG",
			AggregateFunction.Count => "COUNT_BIG",
			AggregateFunction.Maximum => "MAX",
			AggregateFunction.Minimum => "MIN",
			AggregateFunction.Rank => "DENSE_RANK",
			AggregateFunction.RowNumber => "ROW_NUMBER",
			AggregateFunction.StandardDeviation => "STDEV",
			AggregateFunction.PopulationStandardDeviation => "STDEVP",
			AggregateFunction.Summation => "SUM",
			AggregateFunction.Variance => "VAR",
			AggregateFunction.PopulationVariance => "VARP",
			_ => null
		};

		public static string ToSQL([NotNull] this ColumnSort @this) =>
			$"{@this.Column.EscapeIdentifier()} {@this.Sort.ToSQL()}";

		public static string ToSQL([NotNull] this ColumnOutput @this, ObjectSchema schema)
		{
			var sql = @this.Column?.EscapeIdentifier() ?? string.Empty;
			var aggregate = @this.Aggregate.ToSQL();

			if (!sql.IsBlank())
			{
				if (!@this.NullIf.IsBlank())
					sql = $"NULLIF({sql}, {@this.NullIf.ToSQL(schema)})";

				if (@this.Coalesce?.Length == 1)
					sql = $"ISNULL({sql}, {@this.Coalesce[0].ToSQL(schema)})";
				else if (@this.Coalesce.Any())
					sql = $"COALESCE({sql}, {@this.Coalesce.Join(", ", _ => _.ToSQL(schema))})";

				if (!aggregate.IsBlank())
				{
					sql = $"{aggregate}({sql})";

					if (!@this.PartitionBy.IsBlank() && @this.OrderBy.HasValue)
						sql = $"{sql} OVER (PARTITION BY {@this.PartitionBy.EscapeIdentifier()} ORDER BY {@this.OrderBy.Value.ToSQL()})";
					else if (!@this.PartitionBy.IsBlank())
						sql = $"{sql} OVER (PARTITION BY {@this.PartitionBy.EscapeIdentifier()})";
					else if (@this.OrderBy.HasValue)
						sql = $"{sql} OVER (ORDER BY {@this.OrderBy.Value.ToSQL()})";
				}

				if (!@this.Format.IsBlank())
					sql = $"FORMAT({sql}, {@this.Format.ToSQL(schema)})";
			}
			else if (!aggregate.IsBlank())
			{
				sql = @this.Aggregate switch
				{
					AggregateFunction.Count => $"{aggregate}(*)",
					AggregateFunction.RowNumber => $"{aggregate}()",
					_ => throw new ArgumentException($"Column is required for aggregate {aggregate}()."),
				};

				if (!@this.PartitionBy.IsBlank() && @this.OrderBy.HasValue)
					sql = $"{sql} OVER (PARTITION BY {@this.PartitionBy.EscapeIdentifier()} ORDER BY {@this.OrderBy.Value.ToSQL()})";
				else if (!@this.PartitionBy.IsBlank())
					sql = $"{sql} OVER (PARTITION BY {@this.PartitionBy.EscapeIdentifier()})";
				else if (@this.OrderBy.HasValue)
					sql = $"{sql} OVER (ORDER BY {@this.OrderBy.Value.ToSQL()})";
			}

			return !@this.Alias.IsBlank() ? $"{sql} AS {@this.Alias.EscapeIdentifier()}" : sql;
		}

		public static string ToSQL([NotNull] this SelectRequest @this, ObjectSchema schema)
		{
			var sqlBuilder = new StringBuilder("SELECT ");

			if (@this.Output.Any())
				sqlBuilder.AppendLine(@this.Output.Join(SQL_DELIMETER, _ => _.ToSQL(schema)));
			else
				sqlBuilder.AppendLine("*");

			sqlBuilder.Append($"FROM {@this.From}");

			if (@this.Where != null)
				sqlBuilder.AppendLine().Append($"WHERE {@this.Where.ToSQL(schema)}");

			if (@this.Having != null)
				sqlBuilder.AppendLine().Append($"HAVING {@this.Having.ToSQL(schema)}");

			if (@this.OrderBy.Any())
				sqlBuilder.AppendLine().Append($"ORDER BY {@this.OrderBy.Join(SQL_DELIMETER, _ => _.ToSQL())}");

			return sqlBuilder.AppendLine(";").ToString();
		}

		public static string ToSQL(this Sort @this) => @this switch
		{
			Sort.Ascending => "ASC",
			Sort.Descending => "DESC",
			_ => string.Empty
		};
	}
}
