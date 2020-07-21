// Copyright (c) 2020 Samuel Abraham

using sam987883.Common;
using sam987883.Database.Requests;
using sam987883.Extensions;
using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace sam987883.Database.Extensions
{
	public static class SqlExtensions
	{
		private const string SQL_DELIMETER = "\r\n\t, ";

		public static string EscapeIdentifier(this string @this) =>
			@this.Replace("]", "]]");

		public static string EscapeValue(this string @this) =>
			@this.Replace("'", "''");

		public static string ToSql(this BatchRequest @this)
		{
			var batchDataCsv = @this.Input.Rows.Join(SQL_DELIMETER, row => $"({row.ToCsv(value => value.ToSql())})");
			var sourceColumnCsv = @this.Input.Columns.Join(SQL_DELIMETER, column => $"[{column.EscapeIdentifier()}]");
			var onSql = @this.On.Join(" AND ", column => $"s.[{column.EscapeIdentifier()}] = t.[{column.EscapeIdentifier()}]");
			var updateCsv = @this.Update.ToCsv(column => $"[{column.EscapeIdentifier()}] = s.[{column.EscapeIdentifier()}]");
			var insertColumnCsv = @this.Insert.ToCsv(column => $"[{column.EscapeIdentifier()}]");
			var insertValueCsv = @this.Insert.ToCsv(column => $"s.[{column.EscapeIdentifier()}]");

			var sqlBuilder = new StringBuilder();
			if (!@this.Delete && updateCsv.IsBlank())
			{
				sqlBuilder.Append(@$"INSERT INTO {@this.Table.EscapeIdentifier()}
(
	{insertColumnCsv}
)");

				var outputColumnCsv = @this.OutputInserted.Join(SQL_DELIMETER, column => $"INSERTED.[{column.EscapeIdentifier()}]");
				if (!outputColumnCsv.IsBlank())
					sqlBuilder.AppendLine().Append($"OUTPUT {outputColumnCsv}");

				sqlBuilder.AppendLine().Append($"VALUES {batchDataCsv}");
			}
			else
			{
				sqlBuilder.Append($@"MERGE {@this.Table.EscapeIdentifier()} AS t
USING
(
	VALUES {batchDataCsv}
) AS s ({sourceColumnCsv})
ON {onSql}");
				//WHEN MATCHED THEN
				//	UPDATE SET {updateCsv}
				//WHEN NOT MATCHED BY TARGET THEN
				//	INSERT ({insertColumnCsv})
				//	VALUES ({insertValueCsv})");

				if (!updateCsv.IsBlank())
					sqlBuilder.AppendLine().Append(@$"WHEN MATCHED THEN
	UPDATE SET {updateCsv}");

				if (@this.Delete)
					sqlBuilder.AppendLine().Append(@$"WHEN MATCHED THEN DELETE");

				if (@this.OutputDeleted.Any() || (@this.OutputInserted.Any() && updateCsv.IsBlank()))
				{
					sqlBuilder.AppendLine().Append("OUTPUT ");
					if (@this.OutputDeleted.Any())
					{
						var outputColumnCsv = @this.OutputDeleted.Join(SQL_DELIMETER, column => $"DELETED.[{column.EscapeIdentifier()}]");
						sqlBuilder.Append(outputColumnCsv);
					}

					if (@this.OutputInserted.Any())
					{
						var outputColumnCsv = @this.OutputInserted.Join(SQL_DELIMETER, column => $"INSERTED.[{column.EscapeIdentifier()}]");
						sqlBuilder.AppendLine().Append($"OUTPUT {outputColumnCsv}");
					}
				}
			}

			return sqlBuilder.AppendLine(";").ToString();
		}

		public static string ToSql(this DeleteRequest @this)
		{
			var sqlBuilder = new StringBuilder(@$"DELETE FROM {@this.From.EscapeIdentifier()}");

			var whereSql = @this.Where.ToSql();
			if (!whereSql.IsBlank())
				sqlBuilder.AppendLine().Append($"WHERE {whereSql}");

			var outputColumnCsv = @this.Output.Join(SQL_DELIMETER, column => $"DELETED.[{column.EscapeIdentifier()}]");
			if (!outputColumnCsv.IsBlank())
				sqlBuilder.AppendLine().Append($"OUTPUT {outputColumnCsv}");

			return sqlBuilder.AppendLine(";").ToString();
		}

		public static string ToSql(this UpdateRequest @this)
		{
			var updateCsv = @this.Set.Join(SQL_DELIMETER, item => $"[{item.Column.EscapeIdentifier()}] = {item.Value.ToSql()}");

			var sqlBuilder = new StringBuilder(@$"UPDATE {@this.Table.EscapeIdentifier()}
SET {updateCsv}");

			var whereSql = @this.Where.ToSql();
			if (!whereSql.IsBlank())
				sqlBuilder.AppendLine().Append($"WHERE {whereSql}");

			if (@this.OutputDeleted.Any() || @this.OutputInserted.Any())
			{
				sqlBuilder.AppendLine().Append("OUTPUT ");
				if (@this.OutputDeleted.Any())
				{
					var outputColumnCsv = @this.OutputDeleted.Join(SQL_DELIMETER, column => $"DELETED.[{column.EscapeIdentifier()}]");
					sqlBuilder.Append(outputColumnCsv);
				}

				if (@this.OutputInserted.Any())
				{
					var outputColumnCsv = @this.OutputInserted.Join(SQL_DELIMETER, column => $"INSERTED.[{column.EscapeIdentifier()}]");
					sqlBuilder.AppendLine().Append(outputColumnCsv);
				}
			}

			return sqlBuilder.AppendLine(";").ToString();
		}

		public static string ToSql(this ExpressionSet @this)
		{
			if (@this.Operator != LogicalOperator.And && @this.Operator != LogicalOperator.Or)
				throw new NotImplementedException($"{nameof(LogicalOperator)}.{@this.Operator.Name().ToUpper()} is not implemented for SQL.");

			var logicalOperator = $" {@this.Operator.Name().ToUpper()} ";
			var sql = string.Empty;

			if (@this.Expressions.Any())
			{
				sql = $"({@this.Expressions.Join(logicalOperator, expression => expression.ToSql())})";

				if (@this.ExpressionSets.Any())
				{
					sql = new StringBuilder(sql)
						.Append(logicalOperator)
						.Append(@this.ExpressionSets.Join(logicalOperator, expressionSet => expressionSet.ToSql()))
						.ToString();
				}
			}
			else if (@this.ExpressionSets.Any())
				sql = @this.ExpressionSets.Join(logicalOperator, expressionSet => expressionSet.ToSql());

			return sql;
		}

		public static string ToSql([NotNull] this Expression @this)
		{
			var field = @this.Field.EscapeIdentifier();
			var value = @this.Value.ToSql();
			var @operator = @this.Operator.ToSql();

			if (@this.Value is string pattern)
			{
				switch (@this.Operator)
				{
					case ComparisonOperator.Like:
					case ComparisonOperator.NotLike:
						return $"{field} {@operator} N'%{pattern.EscapeValue()}%'";
					case ComparisonOperator.StartWith:
					case ComparisonOperator.NotStartWith:
						return $"{field} {@operator} N'{pattern.EscapeValue()}%'";
					case ComparisonOperator.EndWith:
					case ComparisonOperator.NotEndWith:
						return $"{field} {@operator} N'%{pattern.EscapeValue()}'";
				}
			}

			if (@this.Value == null)
			{
				switch (@this.Operator)
				{
					case ComparisonOperator.Equal: return $"{field} IS NULL";
					case ComparisonOperator.NotEqual: return $"{field} IS NOT NULL";
				}
			}

			if (@this.Value is IEnumerable)
			{
				switch (@this.Operator)
				{
					case ComparisonOperator.Equal: return $"{field} IN ({value})";
					case ComparisonOperator.NotEqual: return $"{field} NOT IN ({value})";
				}
			}

			return $"{field} {@operator} {value}";
		}

		public static string ToSql(this ComparisonOperator @this) => @this switch
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
			_ => throw new NotImplementedException($"{nameof(ComparisonOperator)}.{@this.Name().ToUpper()} is not implemented for SQL.")
		};

		public static string ToSql(this object? @this) => @this switch
		{
			null => "NULL",
			bool boolean => boolean ? "1" : "0",
			char text => text.Equals('\'') ? "N''''" : $"N'{text}'",
			string text => $"N'{text.EscapeValue()}'",
			DateTime dateTime => $"'{dateTime:o}'",
			DateTimeOffset dateTimeOffset => $"'{dateTimeOffset:o}'",
			TimeSpan time => $"'{time:c}'",
			Guid guid => $"'{guid:D}'",
			Enum token => token.Number(),
			IEnumerable enumerable => enumerable.As<object>().To(_ => _.ToSql()).ToCsv(),
			_ => @this.ToString() ?? string.Empty
		};

		public static string ToSql(this SelectRequest @this)
		{
			var sqlBuilder = new StringBuilder("SELECT ");

			if (@this.Output.Any())
				sqlBuilder.AppendLine(@this.Output.Join("\r\n, ", _ => !_.Alias.IsBlank() ? $"[{_.Column.EscapeIdentifier()}] AS [{_.Alias.EscapeIdentifier()}]" : $"[{_.Column.EscapeIdentifier()}]"));
			else
				sqlBuilder.AppendLine("*");

			sqlBuilder.Append($"FROM {@this.From}");

			if (@this.Where != null)
				sqlBuilder.AppendLine().Append($"WHERE {@this.Where.ToSql()}");

			if (@this.Having != null)
				sqlBuilder.AppendLine().Append($"HAVING {@this.Having.ToSql()}");

			if (!@this.OrderBy.Any())
				sqlBuilder.AppendLine().Append($"ORDER BY {@this.OrderBy.Join("\r\n\t, ", orderBy => $"[{orderBy.Column.EscapeIdentifier()}] {orderBy.Sort.ToSql()}")}");

			return sqlBuilder.AppendLine(";").ToString();
		}

		public static string ToSql(this Sort @this) => @this switch
		{
			Sort.Ascending => "ASC",
			Sort.Descending => "DESC",
			_ => string.Empty
		};
	}
}
