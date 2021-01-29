// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Concurrent;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using TypeCache.Collections;
using TypeCache.Collections.Extensions;
using TypeCache.Extensions;

namespace TypeCache.Data.Extensions
{
	public static class DbConnectionExtensions
	{
		private static readonly LazyDictionary<string, ConcurrentDictionary<string, ObjectSchema>> SchemaCache =
			new LazyDictionary<string, ConcurrentDictionary<string, ObjectSchema>>(connectionString => new ConcurrentDictionary<string, ObjectSchema>(StringComparer.OrdinalIgnoreCase), StringComparer.OrdinalIgnoreCase);

		public const string OBJECT_NAME = "ObjectName";

		public static string ObjectSchemaSQL = @$"

DECLARE @ObjectId AS INTEGER =
(
	SELECT [object_id]
	FROM [sys].[objects]
	WHERE [name] = @{OBJECT_NAME}
		AND [type] IN ('U', 'V', 'TF', 'P')
);

SELECT o.[object_id] AS [Id]
, o.[name] AS [ObjectName]
, IIF(o.[type] = 'U', {ObjectType.Table.Number()}
	, IIF(o.[type] = 'V', {ObjectType.View.Number()}
	, IIF(o.[type] = 'TF', {ObjectType.Function.Number()}, {ObjectType.StoredProcedure.Number()}))) AS [Type]
, s.[name] AS [SchemaName]
FROM [sys].[objects] AS o
INNER JOIN [sys].[schemas] AS s ON s.[schema_id] = o.[schema_id]
WHERE o.[object_id] = @ObjectId;

SELECT c.[column_id] AS [Id]
, c.[name] AS [Name]
, UPPER(t.[name]) AS [Type]
, c.[is_hidden] AS [Hidden]
, c.[is_identity] AS [Identity]
, c.[is_nullable] AS [Nullable]
, IIF(i.[is_primary_key] = 1, 1, 0) AS [PrimaryKey]
, IIF(1 IN (c.[is_computed], c.[is_identity], c.[is_rowguidcol]), 1, 0) AS [Readonly]
, c.[max_length] AS [Length]
FROM [sys].[columns] AS c
INNER JOIN [sys].[types] AS t ON t.[user_type_id] = c.[user_type_id]
LEFT JOIN
(
	[sys].[index_columns] AS ic
	INNER JOIN [sys].[indexes] AS i ON i.[object_id] = ic.[object_id] AND i.[index_id] = ic.[index_id] AND i.[is_primary_key] = 1
) ON ic.[object_id] = c.[object_id] AND ic.[column_id] = c.[column_id]
WHERE c.[object_id] = @ObjectId
ORDER BY c.[column_id] ASC;

SELECT p.[parameter_id] AS [Id]
, p.[name] AS [Name]
, UPPER(t.[name]) AS [Type]
, p.[is_output] AS [Output]
, IIF(p.[is_output] = 1 AND p.[parameter_id] = 0, 1, 0) AS [Return]
FROM [sys].[parameters] AS p
INNER JOIN [sys].[types] AS t ON t.[user_type_id] = p.[user_type_id]
WHERE p.[object_id] = @ObjectId
ORDER BY p.[parameter_id] ASC;";

		private static Type GetColumnType(ColumnSchema column)
			=> column.Type switch
			{
				"BIGINT" when column.Nullable => typeof(long?),
				"BIGINT" => typeof(long),
				"BINARY" when column.Length > 1 => typeof(byte[]),
				"BINARY" when column.Nullable => typeof(byte?),
				"BINARY" => typeof(byte),
				"BIT" when column.Nullable => typeof(bool?),
				"BIT" => typeof(bool),
				"CHAR" when column.Length > 1 => typeof(string),
				"CHAR" when column.Nullable => typeof(char?),
				"CHAR" => typeof(char),
				"DATE" when column.Nullable => typeof(DateTime?),
				"DATE" => typeof(DateTime),
				"DATETIME" when column.Nullable => typeof(DateTime?),
				"DATETIME" => typeof(DateTime),
				"DATETIME2" when column.Nullable => typeof(DateTime?),
				"DATETIME2" => typeof(DateTime),
				"DATETIMEOFFSET" when column.Nullable => typeof(DateTimeOffset?),
				"DATETIMEOFFSET" => typeof(DateTimeOffset),
				"DECIMAL" when column.Nullable => typeof(decimal?),
				"DECIMAL" => typeof(decimal),
				"FLOAT" when column.Nullable => typeof(double?),
				"FLOAT" => typeof(double),
				"IMAGE" => typeof(byte[]),
				"INT" when column.Nullable => typeof(int?),
				"INT" => typeof(int),
				"MONEY" when column.Nullable => typeof(decimal?),
				"MONEY" => typeof(decimal),
				"NCHAR" when column.Length > 1 => typeof(string),
				"NCHAR" when column.Nullable => typeof(char?),
				"NCHAR" => typeof(char),
				"NTEXT" => typeof(string),
				"NUMERIC" when column.Nullable => typeof(decimal?),
				"NUMERIC" => typeof(decimal),
				"NVARCHAR" => typeof(string),
				"REAL" when column.Nullable => typeof(float?),
				"REAL" => typeof(float),
				"ROWVERSION" => typeof(byte[]),
				"SMALLDATETIME" when column.Nullable => typeof(DateTime?),
				"SMALLDATETIME" => typeof(DateTime),
				"SMALLINT" when column.Nullable => typeof(short?),
				"SMALLINT" => typeof(short),
				"SMALLMONEY" when column.Nullable => typeof(decimal?),
				"SMALLMONEY" => typeof(decimal),
				"SYSNAME" => typeof(string),
				"TEXT" => typeof(string),
				"TIME" when column.Nullable => typeof(TimeSpan?),
				"TIME" => typeof(TimeSpan),
				"TIMESTAMP" => typeof(byte[]),
				"TINYINT" => typeof(sbyte),
				"UNIQUEIDENTIFIER" when column.Nullable => typeof(Guid?),
				"UNIQUEIDENTIFIER" => typeof(Guid),
				"VARBINARY" => typeof(byte[]),
				"VARCHAR" => typeof(string),
				_ => typeof(object)
			};

		public static async ValueTask<ObjectSchema> CreateObjectSchema(this DbConnection @this, string name)
		{
			if (name.EndsWith(')'))
				name = name.Left(name.LastIndexOf('('));

			var parts = name.Split('.', StringSplitOptions.RemoveEmptyEntries).To(part => part.TrimStart('[').TrimEnd(']')).ToArray();
			name = parts.Length switch
			{
				1 => parts[0],
				2 => parts[1],
				3 => parts[2],
				_ => throw new ArgumentException($"{nameof(CreateObjectSchema)}: Invalid table source name: {name}", nameof(name))
			};

			var sql = $"USE {@this.Database.EscapeIdentifier()};{ObjectSchemaSQL}";
			await using var command = @this.CreateSqlCommand(sql);
			command.AddInputParameter(OBJECT_NAME, name, DbType.String);

			await using var transaction = await @this.BeginTransactionAsync(IsolationLevel.ReadUncommitted);
			command.Transaction = transaction;

			await using var reader = await command.ExecuteReaderAsync();

			var tableRowSet = await reader.ReadRowSetAsync();

			var objectSchema = tableRowSet.Map<ObjectSchema>().First();
			objectSchema.AssertNotNull($"{nameof(CreateObjectSchema)}: Database object not found: [{name}].  Must be a table, view, table-valued function or stored procedure.");
			objectSchema!.DatabaseName = @this.Database;

			if (await reader.NextResultAsync())
			{
				var columnRowSet = await reader.ReadRowSetAsync();
				objectSchema.Columns = columnRowSet.Map<ColumnSchema>().ToImmutable();
			}

			if (await reader.NextResultAsync())
			{
				var parameterRowSet = await reader.ReadRowSetAsync();
				objectSchema.Parameters = parameterRowSet.Map<ParameterSchema>().ToImmutable();
			}

			return objectSchema;
		}

		public static ObjectSchema GetObjectSchema(this DbConnection @this, string name)
		{
			var parts = name.Split('.', StringSplitOptions.RemoveEmptyEntries).To(part => part.TrimStart('[').TrimEnd(']')).ToArray();
			var fullName = parts.Length switch
			{
				1 => $"[{@this.Database}]..[{parts[0]}]",
				2 when name.Contains("..") => $"[{parts[0]}]..[{parts[1]}]",
				2 => $"[{@this.Database}].[{parts[0]}].[{parts[1]}]",
				3 => $"[{parts[0]}].[{parts[1]}].[{parts[2]}]",
				_ => throw new ArgumentException($"Invalid table source name: {name}", nameof(name))
			};
			var tableSchemaCache = SchemaCache[@this.ConnectionString];
			return tableSchemaCache.GetOrAdd(fullName, key => @this.CreateObjectSchema(name).AsTask().Result);
		}

		/// <summary>
		/// <code>command.CommandType = CommandType.StoredProcedure;</code>
		/// </summary>
		public static DbCommand CreateProcedureCommand(this DbConnection @this, string procedure)
		{
			var command = @this.CreateCommand();
			command.CommandType = CommandType.StoredProcedure;
			command.CommandText = procedure;
			return command;
		}

		/// <summary>
		/// <code>command.CommandType = CommandType.Text;</code>
		/// </summary>
		public static DbCommand CreateSqlCommand(this DbConnection @this, string sql)
		{
			var command = @this.CreateCommand();
			command.CommandType = CommandType.Text;
			command.CommandText = sql;
			return command;
		}

		/// <summary>
		/// EXECUTE ...
		/// </summary>
		public static async ValueTask<RowSet[]> CallAsync(this DbConnection @this, StoredProcedureRequest procedure, CancellationToken cancellationToken = default)
		{
			await using var command = @this.CreateProcedureCommand(procedure.Procedure);
			procedure.Parameters.Do(parameter => command.AddInputParameter(parameter.Name, parameter.Value));

			await using var reader = await command.ExecuteReaderAsync(cancellationToken);
			return (await reader.ReadRowSetsAsync(cancellationToken).ToListAsync(cancellationToken)).ToArray();
		}

		/// <summary>
		/// EXECUTE ...
		/// </summary>
		public static async ValueTask<RowSet[]> RunAsync(this DbConnection @this, SqlRequest request, CancellationToken cancellationToken = default)
		{
			await using var command = @this.CreateSqlCommand(request.SQL);
			request.Parameters.Do(parameter => command.AddInputParameter(parameter.Name, parameter.Value));

			await using var reader = await command.ExecuteReaderAsync(cancellationToken);
			return (await reader.ReadRowSetsAsync(cancellationToken).ToListAsync(cancellationToken)).ToArray();
		}

		/// <summary>
		/// DELETE FROM ... WHERE ...
		/// </summary>
		/// <returns>OUTPUT DELETED</returns>
		public static async ValueTask<RowSet> DeleteAsync(this DbConnection @this, DeleteRequest delete, CancellationToken cancellationToken = default)
		{
			await using var command = @this.CreateSqlCommand(delete.ToSql());
			delete.Parameters.Do(parameter => command.AddInputParameter(parameter.Name, parameter.Value));

			if (delete.Output.Any())
			{
				await using var reader = await command.ExecuteReaderAsync(cancellationToken);
				return await reader.ReadRowSetAsync(cancellationToken);
			}
			else
			{
				await command.ExecuteNonQueryAsync(cancellationToken);
				return default;
			}
		}

		/// <summary>
		/// SELECT ... FROM ... WHERE ... HAVING ... ORDER BY ...
		/// </summary>
		public static async ValueTask<RowSet> InsertAsync(this DbConnection @this, InsertRequest insert, CancellationToken cancellationToken = default)
		{
			await using var command = @this.CreateSqlCommand(insert.ToSql());
			insert.Parameters.Do(parameter => command.AddInputParameter(parameter.Name, parameter.Value));

			if (insert.Output.Any())
			{
				await using var reader = await command.ExecuteReaderAsync(cancellationToken);
				return await reader.ReadRowSetAsync(cancellationToken);
			}
			else
			{
				await command.ExecuteNonQueryAsync(cancellationToken);
				return default;
			}
		}

		/// <summary>
		/// <code>
		/// <list>
		/// <item>MERGE ... USING ... ON ... WHEN MATCHED THEN UPDATE ... WHEN NOT MATCHED BY TARGET THEN INSERT ... WHEN NOT MATCHED BY SOURCE THEN DELETE ... OUTPUT ...;</item>
		/// <item>MERGE ... USING ... ON ... WHEN NOT MATCHED BY TARGET THEN INSERT ... WHEN NOT MATCHED BY SOURCE THEN DELETE ... OUTPUT ...;</item>
		/// <item>MERGE ... USING ... ON ... WHEN MATCHED THEN UPDATE ... WHEN NOT MATCHED BY SOURCE THEN DELETE ... OUTPUT ...;</item>
		/// <item>MERGE ... USING ... ON ... WHEN MATCHED THEN DELETE ... OUTPUT ...;</item>
		/// <item>INSERT INTO ... (...) OUTPUT ... VALUES ...;</item>
		/// </list>
		/// </code>
		/// </summary>
		/// <returns>OUTPUT DELETED, INSERTED</returns>
		public static async ValueTask<RowSet> MergeAsync(this DbConnection @this, BatchRequest batch, CancellationToken cancellationToken = default)
		{
			await using var command = @this.CreateSqlCommand(batch.ToSql());
			if (batch.Output.Any())
			{
				await using var reader = await command.ExecuteReaderAsync(cancellationToken);
				return await reader.ReadRowSetAsync(cancellationToken);
			}
			else
			{
				await command.ExecuteNonQueryAsync(cancellationToken);
				return default;
			}
		}

		/// <summary>
		/// SELECT ... FROM ... WHERE ... HAVING ... ORDER BY ...
		/// </summary>
		public static async ValueTask<RowSet> SelectAsync(this DbConnection @this, SelectRequest select, CancellationToken cancellationToken = default)
		{
			await using var command = @this.CreateSqlCommand(select.ToSql());
			select.Parameters.Do(parameter => command.AddInputParameter(parameter.Name, parameter.Value));

			await using var transaction = await command.BeginTransactionAsync(IsolationLevel.ReadUncommitted, cancellationToken);
			await using var reader = await command.ExecuteReaderAsync(CommandBehavior.SingleResult, cancellationToken);
			return await reader.ReadRowSetAsync(cancellationToken);
		}

		/// <summary>
		/// TRUNCATE TABLE ...
		/// </summary>
		public static async ValueTask<int> TruncateTableAsync(this DbConnection @this, string table, CancellationToken cancellationToken = default)
		{
			await using var command = @this.CreateSqlCommand($"TRUNCATE TABLE {table.EscapeIdentifier()};");
			return await command.ExecuteNonQueryAsync(cancellationToken);
		}

		/// <summary>
		/// UPDATE ... SET ... WHERE ...
		/// </summary>
		/// <returns>OUTPUT DELETED, INSERTED</returns>
		public static async ValueTask<RowSet> UpdateAsync(this DbConnection @this, UpdateRequest update, CancellationToken cancellationToken = default)
		{
			await using var command = @this.CreateSqlCommand(update.ToSql());
			update.Parameters.Do(parameter => command.AddInputParameter(parameter.Name, parameter.Value));

			if (update.Output.Any())
			{
				await using var reader = await command.ExecuteReaderAsync(cancellationToken);
				return await reader.ReadRowSetAsync(cancellationToken);
			}
			else
			{
				await command.ExecuteNonQueryAsync(cancellationToken);
				return default;
			}
		}
	}
}
