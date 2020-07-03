// Copyright (c) 2020 Samuel Abraham

using sam987883.Common;
using sam987883.Database.Commands;
using sam987883.Extensions;
using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace sam987883.Database.Extensions
{
	public static class SqlExtensions
	{
		private const string SQL_DELIMETER = "\r\n\t, ";

		private readonly static JsonSerializerOptions JsonOptions;

		static SqlExtensions()
		{
			JsonOptions = new JsonSerializerOptions
			{
				AllowTrailingCommas = false,
				IgnoreNullValues = false,
				PropertyNameCaseInsensitive = true,
				WriteIndented = false,
				MaxDepth = 100
			};
			JsonOptions.Converters.Add(new JsonStringEnumConverter());
		}

		public static string EscapeIdentifier(this string @this) =>
			@this.Replace("]", "]]");

		public static string EscapeValue(this string @this) =>
			@this.Replace("'", "''");

		public static string ToSql(this BatchDelete @this)
		{
			var batchDataCsv = @this.Input.Rows.Join(SQL_DELIMETER, row => $"({row.ToCsv(value => value.ToSql())})");
			var sourceColumnCsv = @this.Input.Columns.Join(SQL_DELIMETER, column => $"[{column.EscapeIdentifier()}]");
			var onSql = @this.OnColumns.Join(" AND ", column => $"s.[{column.EscapeIdentifier()}] = t.[{column.EscapeIdentifier()}]");

			var sqlBuilder = new StringBuilder(@$"MERGE [{@this.Table.EscapeIdentifier()}] AS t
USING
(
	VALUES {batchDataCsv}
) AS s ({sourceColumnCsv})
ON {onSql}
WHEN MATCHED THEN DELETE");

			var outputColumnCsv = @this.Output.Columns.Join(SQL_DELIMETER, column => $"DELETED.[{column.EscapeIdentifier()}]");
			if (!outputColumnCsv.IsBlank())
				sqlBuilder.AppendLine().Append($"OUTPUT {outputColumnCsv}");

			return sqlBuilder.AppendLine(";").ToString();
		}

		public static string ToSql<T>(this BatchDelete<T> @this) where T : class, new()
		{
			var inputProperties = @this.PropertyCache.Properties
				.GetValues(@this.Input.Columns)
				.If(property => property.GetMethod?.Public == true)
				.ToArray();
			var batch = new BatchDelete
			{
				Input = new RowSet
				{
					Columns = @this.Input.Columns,
					Rows = @this.Input.Rows
						.To(row => inputProperties
							.To(property => property[row])
							.ToArray(inputProperties.Length))
						.ToArray(@this.Input.Rows.Length)
				},
				OnColumns = @this.OnColumns,
				Table = @this.Table
			};
			batch.Output.Columns = @this.Output.Columns;
			return batch.ToSql();
		}

		public static string ToSql(this BatchInsert @this)
		{
			var insertColumnCsv = @this.InsertColumns.Join(SQL_DELIMETER, column => $"[{column.EscapeIdentifier()}]");
			var columnIndexes = @this.Input.Columns.ToIndex(column => @this.InsertColumns.Has(column, StringComparer.OrdinalIgnoreCase)).ToArray();
			var batchDataCsv = @this.Input.Rows.Join(SQL_DELIMETER, row => $"({columnIndexes.ToCsv(i => row[i].ToSql())}");

			var sqlBuilder = new StringBuilder(@$"INSERT INTO {@this.Table.EscapeIdentifier()}
(
	{insertColumnCsv}
)");

			var outputColumnCsv = @this.Output.Columns.Join(SQL_DELIMETER, column => $"INSERTED.[{column.EscapeIdentifier()}]");
			if (!outputColumnCsv.IsBlank())
				sqlBuilder.AppendLine().Append($"OUTPUT {outputColumnCsv}");

			return sqlBuilder.AppendLine().AppendLine($"VALUES {batchDataCsv};").ToString();
		}

		public static string ToSql<T>(this BatchInsert<T> @this) where T : class, new()
		{
			var inputProperties = @this.PropertyCache.Properties
				.GetValues(@this.Input.Columns)
				.If(propertyMember => propertyMember.GetMethod?.Public == true)
				.ToArray();
			var batch = new BatchInsert
			{
				Input = new RowSet
				{
					Columns = @this.Input.Columns,
					Rows = @this.Input.Rows
						.To(row => inputProperties
							.To(property => property[row])
							.ToArray(inputProperties.Length))
						.ToArray(@this.Input.Rows.Length)
				},
				Table = @this.Table
			};
			batch.Output.Columns = @this.Output.Columns;
			return batch.ToSql();
		}

		public static string ToSql(this BatchUpdate @this)
		{
			var batchDataCsv = @this.Input.Rows.Join(SQL_DELIMETER, row => $"({row.ToCsv(value => value.ToSql())})");
			var sourceColumnCsv = @this.Input.Columns.Join(SQL_DELIMETER, column => $"[{column.EscapeIdentifier()}]");
			var onSql = @this.OnColumns.Join(" AND ", column => $"s.[{column.EscapeIdentifier()}] = t.[{column.EscapeIdentifier()}]");
			var updateCsv = @this.UpdateColumns.ToCsv(column => $"[{column.EscapeIdentifier()}] = s.[{column.EscapeIdentifier()}]");

			var sqlBuilder = new StringBuilder($@"MERGE {@this.Table.EscapeIdentifier()} AS t
USING
(
	VALUES {batchDataCsv}
) AS s ({sourceColumnCsv})
ON {onSql}
WHEN MATCHED THEN
	UPDATE SET {updateCsv}");

			var outputSql = @this.Output.ToSql();
			if (!outputSql.IsBlank())
				sqlBuilder.AppendLine().Append(outputSql);

			return sqlBuilder.AppendLine(";").ToString();
		}

		public static string ToSql<T>(this BatchUpdate<T> @this) where T : class, new()
		{
			var inputProperties = @this.PropertyCache.Properties
				.GetValues(@this.Input.Columns)
				.If(property => property.GetMethod?.Public == true)
				.ToArray();
			var batch = new BatchUpdate
			{
				Input = new RowSet
				{
					Columns = @this.Input.Columns,
					Rows = @this.Input.Rows.To(row => inputProperties.To(property => property[row]).ToArray(inputProperties.Length)).ToArray(@this.Input.Rows.Length)
				},
				UpdateColumns = @this.UpdateColumns,
				OnColumns = @this.OnColumns,
				Table = @this.Table
			};
			batch.Output.Deleted.Columns = @this.Output.Deleted.Columns;
			batch.Output.Inserted.Columns = @this.Output.Inserted.Columns;
			return batch.ToSql();
		}

		public static string ToSql(this BatchUpsert @this)
		{
			var batchDataCsv = @this.Input.Rows.Join(SQL_DELIMETER, row => $"({row.ToCsv(value => value.ToSql())})");
			var sourceColumnCsv = @this.Input.Columns.Join(SQL_DELIMETER, column => $"[{column.EscapeIdentifier()}]");
			var onSql = @this.OnColumns.Join(" AND ", column => $"s.[{column.EscapeIdentifier()}] = t.[{column.EscapeIdentifier()}]");
			var updateCsv = @this.UpdateColumns.ToCsv(column => $"[{column.EscapeIdentifier()}] = s.[{column.EscapeIdentifier()}]");
			var insertColumnCsv = @this.InsertColumns.ToCsv(column => $"[{column.EscapeIdentifier()}]");
			var insertValueCsv = @this.InsertColumns.ToCsv(column => $"s.[{column.EscapeIdentifier()}]");

			var sqlBuilder = new StringBuilder($@"MERGE {@this.Table.EscapeIdentifier()} AS t
USING
(
	VALUES {batchDataCsv}
) AS s ({sourceColumnCsv})
ON {onSql}
WHEN MATCHED THEN
	UPDATE SET {updateCsv}
WHEN NOT MATCHED BY TARGET THEN
	INSERT ({insertColumnCsv})
	VALUES ({insertValueCsv})");

			var outputSql = @this.Output.ToSql();
			if (!outputSql.IsBlank())
				sqlBuilder.AppendLine().Append(outputSql);

			return sqlBuilder.AppendLine(";").ToString();
		}

		public static string ToSql<T>(this BatchUpsert<T> @this) where T : class, new()
		{
			var inputProperties = @this.PropertyCache.Properties
				.GetValues(@this.Input.Columns)
				.If(property => property.GetMethod?.Public == true)
				.ToArray();
			var batch = new BatchUpsert
			{
				Input = new RowSet
				{
					Columns = @this.Input.Columns,
					Rows = @this.Input.Rows
						.To(row => inputProperties
							.To(property => property[row])
							.ToArray(inputProperties.Length))
						.ToArray(@this.Input.Rows.Length)
				},
				InsertColumns = @this.InsertColumns,
				UpdateColumns = @this.UpdateColumns,
				OnColumns = @this.OnColumns,
				Table = @this.Table
			};
			batch.Output.Deleted.Columns = @this.Output.Deleted.Columns;
			batch.Output.Inserted.Columns = @this.Output.Inserted.Columns;
			return batch.ToSql();
		}

		public static string ToSql(this Delete @this)
		{
			var sqlBuilder = new StringBuilder(@$"DELETE FROM {@this.Table.EscapeIdentifier()}");

			var whereSql = @this.Where.ToSql();
			if (!whereSql.IsBlank())
				sqlBuilder.AppendLine().Append($"WHERE {whereSql}");

			var outputColumnCsv = @this.Output.Columns.Join(SQL_DELIMETER, column => $"DELETED.[{column.EscapeIdentifier()}]");
			if (!outputColumnCsv.IsBlank())
				sqlBuilder.AppendLine().Append($"OUTPUT {outputColumnCsv}");

			return sqlBuilder.AppendLine(";").ToString();
		}

		public static string ToSql(this Output @this)
		{
			var outputDeletedColumnCsv = @this.Deleted.Columns.Join(SQL_DELIMETER, column => $"DELETED.[{column}]");
			var outputInsertedColumnCsv = @this.Inserted.Columns.Join(SQL_DELIMETER, column => $"INSERTED.[{column}]");

			if (!outputDeletedColumnCsv.IsBlank() && !outputInsertedColumnCsv.IsBlank())
				return $"OUTPUT {outputDeletedColumnCsv}{SQL_DELIMETER}{outputInsertedColumnCsv}";
			else if (!outputDeletedColumnCsv.IsBlank())
				return $"OUTPUT {outputDeletedColumnCsv}";
			else if (!outputInsertedColumnCsv.IsBlank())
				return $"OUTPUT {outputInsertedColumnCsv}";
			else
				return string.Empty;
		}

		public static string ToSql(this Update @this)
		{
			var updateCsv = @this.Set.Join(SQL_DELIMETER, item => $"[{item.Column.EscapeIdentifier()}] = {item.Value.ToSql()}");

			var sqlBuilder = new StringBuilder(@$"UPDATE {@this.Table.EscapeIdentifier()}
SET {updateCsv}");

			var whereSql = @this.Where.ToSql();
			if (!whereSql.IsBlank())
				sqlBuilder.AppendLine().Append($"WHERE {whereSql}");

			var outputSql = @this.Output.ToSql();
			if (!outputSql.IsBlank())
				sqlBuilder.AppendLine().Append(outputSql);

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

			if (@this.Value != null)
			{
				switch (@this.Operator)
				{
					case ComparisonOperator.Like: return $"{field} {@this.Operator.ToSql()} N'%{@this.Value.ToString().EscapeValue()}%'";
					case ComparisonOperator.StartWith: return $"{field} {@this.Operator.ToSql()} N'{@this.Value.ToString().EscapeValue()}%'";
					case ComparisonOperator.EndWith: return $"{field} {@this.Operator.ToSql()} N'%{@this.Value.ToString().EscapeValue()}'";
					case ComparisonOperator.NotLike: return $"{field} {@this.Operator.ToSql()} N'%{@this.Value.ToString().EscapeValue()}%'";
					case ComparisonOperator.NotStartWith: return $"{field} {@this.Operator.ToSql()} N'{@this.Value.ToString().EscapeValue()}%'";
					case ComparisonOperator.NotEndWith: return $"{field} {@this.Operator.ToSql()} N'%{@this.Value.ToString().EscapeValue()}'";
				}
			}

			switch (@this.Operator)
			{
				case ComparisonOperator.Equal: return $"{field} IS NULL";
				case ComparisonOperator.NotEqual: return $"{field} IS NOT NULL";
			}

			return $"{field} {@this.Operator.ToSql()} {@this.Value.ToSql()}";
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

		public static string ToSql(this object @this) => @this switch
		{
			null => "NULL",
			char text => text.Equals('\'') ? "N''''" : $"N'{text}'",
			string text => $"N'{text.EscapeValue()}'",
			DateTime dateTime => $"'{dateTime:o}'",
			DateTimeOffset dateTimeOffset => $"'{dateTimeOffset:o}'",
			TimeSpan time => $"'{time:c}'",
			Guid guid => $"'{guid:D}'",
			Enum token => token.Number(),
			IEnumerable _ => JsonSerializer.Serialize(@this, @this.GetType(), JsonOptions),
			_ => @this.ToString() ?? string.Empty
		};

		public static string ToSql(this Select @this)
		{
			var sqlBuilder = new StringBuilder()
				.AppendLine("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;")
				.AppendLine("SET NOCOUNT ON;")
				.AppendLine()
				.AppendLine($"SELECT {(@this.Output.Columns.Any() ? @this.Output.Columns.Join("\r\n, ", column => $"[{column.EscapeIdentifier()}]") : "*")}")
				.Append($"FROM [{@this.From.EscapeIdentifier()}]");

			if (@this.Where != null)
			{
				sqlBuilder.AppendLine().Append($"WHERE {@this.Where.ToSql()}");
			}

			if (@this.Having != null)
			{
				sqlBuilder.AppendLine().Append($"HAVING {@this.Having.ToSql()}");
			}

			if (!@this.OrderBy.Any())
			{
				sqlBuilder.AppendLine().Append($"ORDER BY {@this.OrderBy.Join("\r\n\t, ", orderBy => $"[{orderBy.Column.EscapeIdentifier()}] {orderBy.Sort.ToSql()}")}");
			}

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
