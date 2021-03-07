// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using TypeCache.Collections;
using TypeCache.Collections.Extensions;
using TypeCache.Data.Extensions;
using TypeCache.Extensions;

namespace TypeCache.Data
{
	internal sealed class SqlApi : ISqlApi
	{
		private static readonly IReadOnlyDictionary<string, ConcurrentDictionary<string, ObjectSchema>> SchemaCache =
			new LazyDictionary<string, ConcurrentDictionary<string, ObjectSchema>>(connectionString => new ConcurrentDictionary<string, ObjectSchema>(StringComparer.OrdinalIgnoreCase), StringComparer.OrdinalIgnoreCase);

		public const string OBJECT_NAME = "ObjectName";
		public const string DATA_SOURCE = "Data Source";
		public const string DATABASE = "Database";
		public const string INITIAL_CATALOG = "Initial Catalog";
		public const CommandBehavior SINGLE_RESULT = CommandBehavior.SingleResult | CommandBehavior.CloseConnection;

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

		private readonly DbProviderFactory _DbProviderFactory;
		private readonly string _ConnectionString;

		public SqlApi(string databaseProvider, string connectionString)
		{
			this._DbProviderFactory = DbProviderFactories.GetFactory(databaseProvider);
			this._ConnectionString = connectionString;
		}

		public string ObjectSchemaSQL { get; } = @$"
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

		public async ValueTask<ObjectSchema> CreateObjectSchema(string name)
		{
			if (name.EndsWith(')'))
				name = name.Left(name.LastIndexOf('('));

			var parts = name.Split('.', StringSplitOptions.RemoveEmptyEntries).To(part => part.TrimStart('[').TrimEnd(']')).ToArray();
			name = parts.Length switch
			{
				1 => parts[0],
				2 => parts[1],
				3 => parts[2],
				_ => throw new ArgumentException($"{nameof(SqlApi)}.{nameof(CreateObjectSchema)}: Invalid table source name: {name}", nameof(name))
			};

			await using var dbConnection = this._DbProviderFactory.CreateConnection(this._ConnectionString);
			await using var command = dbConnection.CreateSqlCommand(ObjectSchemaSQL);
			command.AddInputParameter(OBJECT_NAME, name, DbType.String);

			await using var transaction = await dbConnection.BeginTransactionAsync(IsolationLevel.ReadUncommitted);
			command.Transaction = transaction;

			await using var reader = await command.ExecuteReaderAsync();

			var tableRowSet = await reader.ReadRowSetAsync();

			ColumnSchema[]? columns = null;
			if (await reader.NextResultAsync())
			{
				var columnRowSet = await reader.ReadRowSetAsync();
				columns = columnRowSet.Map<ColumnSchema>();
			}

			ParameterSchema[]? parameters = null;
			if (await reader.NextResultAsync())
			{
				var parameterRowSet = await reader.ReadRowSetAsync();
				parameters = parameterRowSet.Map<ParameterSchema>();
			}

			return new ObjectSchema
			{
				Columns = columns.ToImmutable(),
				Id = (int)tableRowSet[0, nameof(ObjectSchema.Id)]!,
				DatabaseName = dbConnection.Database,
				ObjectName = tableRowSet[0, nameof(ObjectSchema.ObjectName)]!.ToString()!,
				Parameters = parameters.ToImmutable(),
				SchemaName = tableRowSet[0, nameof(ObjectSchema.SchemaName)]!.ToString()!,
				Type = (ObjectType)tableRowSet[0, nameof(ObjectSchema.Type)]!
			};
		}

		public ObjectSchema GetObjectSchema(string name)
		{
			var connectionStringBuilder = this._DbProviderFactory.CreateConnectionStringBuilder(this._ConnectionString);
			var server = connectionStringBuilder[DATA_SOURCE].ToString()!;
			var database = connectionStringBuilder.TryGetValue(DATABASE, out var value)
				|| connectionStringBuilder.TryGetValue(INITIAL_CATALOG, out value) ? value.ToString() : null;

			var parts = name.Split('.', StringSplitOptions.RemoveEmptyEntries).To(part => part.TrimStart('[').TrimEnd(']')).ToArray();
			var fullName = parts.Length switch
			{
				1 when !database.IsBlank() => $"[{database}]..[{parts[0]}]",
				1 => throw new ArgumentException($"{nameof(SqlApi)}.{nameof(GetObjectSchema)}: ConnectionString must have [{DATABASE}] or [{INITIAL_CATALOG}] specified for database object.", name),
				2 when name.Contains("..") => $"[{parts[0]}]..[{parts[1]}]",
				2 when !database.IsBlank() => $"[{database}].[{parts[0]}].[{parts[1]}]",
				2 => throw new ArgumentException($"{nameof(SqlApi)}.{nameof(GetObjectSchema)}: ConnectionString must have [{DATABASE}] or [{INITIAL_CATALOG}] specified for database object.", name),
				3 => $"[{parts[0]}].[{parts[1]}].[{parts[2]}]",
				_ => throw new ArgumentException($"{nameof(SqlApi)}.{nameof(GetObjectSchema)}: Invalid table source name.", name)
			};
			var tableSchemaCache = SchemaCache[server];
			return tableSchemaCache.GetOrAdd(fullName, key => this.CreateObjectSchema(name).AsTask().Result);
		}

		public async ValueTask<RowSet[]> CallAsync(StoredProcedureRequest procedure, CancellationToken cancellationToken = default)
		{
			await using var dbConnection = this._DbProviderFactory.CreateConnection(this._ConnectionString);
			await dbConnection.OpenAsync(cancellationToken);
			await using var command = dbConnection.CreateProcedureCommand(procedure.Procedure);
			procedure.Parameters.Do(parameter => command.AddInputParameter(parameter.Name, parameter.Value));

			await using var reader = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection, cancellationToken);
			return (await reader.ReadRowSetsAsync(cancellationToken).ToListAsync(cancellationToken)).ToArray();
		}

		public async ValueTask<RowSet[]> RunAsync(SqlRequest request, CancellationToken cancellationToken = default)
		{
			await using var dbConnection = this._DbProviderFactory.CreateConnection(this._ConnectionString);
			await dbConnection.OpenAsync(cancellationToken);
			await using var command = dbConnection.CreateSqlCommand(request.SQL);
			request.Parameters.Do(parameter => command.AddInputParameter(parameter.Name, parameter.Value));

			await using var reader = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection, cancellationToken);
			return (await reader.ReadRowSetsAsync(cancellationToken).ToListAsync(cancellationToken)).ToArray();
		}

		public async ValueTask<RowSet> DeleteAsync(DeleteRequest delete, CancellationToken cancellationToken = default)
		{
			await using var dbConnection = this._DbProviderFactory.CreateConnection(this._ConnectionString);
			await dbConnection.OpenAsync(cancellationToken);
			await using var command = dbConnection.CreateSqlCommand(delete.ToSql());
			delete.Parameters.Do(parameter => command.AddInputParameter(parameter.Name, parameter.Value));

			if (delete.Output.Any())
			{
				await using var reader = await command.ExecuteReaderAsync(SINGLE_RESULT, cancellationToken);
				return await reader.ReadRowSetAsync(cancellationToken);
			}
			else
			{
				await command.ExecuteNonQueryAsync(cancellationToken);
				return RowSet.Empty;
			}
		}

		public async ValueTask<RowSet> InsertAsync(InsertRequest insert, CancellationToken cancellationToken = default)
		{
			await using var dbConnection = this._DbProviderFactory.CreateConnection(this._ConnectionString);
			await dbConnection.OpenAsync(cancellationToken);
			await using var command = dbConnection.CreateSqlCommand(insert.ToSql());
			insert.Parameters.Do(parameter => command.AddInputParameter(parameter.Name, parameter.Value));

			if (insert.Output.Any())
			{
				await using var reader = await command.ExecuteReaderAsync(SINGLE_RESULT, cancellationToken);
				return await reader.ReadRowSetAsync(cancellationToken);
			}
			else
			{
				await command.ExecuteNonQueryAsync(cancellationToken);
				return RowSet.Empty;
			}
		}

		public async ValueTask<RowSet> MergeAsync(BatchRequest batch, CancellationToken cancellationToken = default)
		{
			await using var dbConnection = this._DbProviderFactory.CreateConnection(this._ConnectionString);
			await dbConnection.OpenAsync(cancellationToken);
			await using var command = dbConnection.CreateSqlCommand(batch.ToSql());
			if (batch.Output.Any())
			{
				await using var reader = await command.ExecuteReaderAsync(SINGLE_RESULT, cancellationToken);
				return await reader.ReadRowSetAsync(cancellationToken);
			}
			else
			{
				await command.ExecuteNonQueryAsync(cancellationToken);
				return RowSet.Empty;
			}
		}

		public async ValueTask<RowSet> SelectAsync(SelectRequest select, CancellationToken cancellationToken = default)
		{
			await using var dbConnection = this._DbProviderFactory.CreateConnection(this._ConnectionString);
			await dbConnection.OpenAsync(cancellationToken);
			await using var command = dbConnection.CreateSqlCommand(select.ToSql());
			select.Parameters.Do(parameter => command.AddInputParameter(parameter.Name, parameter.Value));

			await using var transaction = await command.BeginTransactionAsync(IsolationLevel.ReadUncommitted, cancellationToken);
			await using var reader = await command.ExecuteReaderAsync(SINGLE_RESULT, cancellationToken);
			return await reader.ReadRowSetAsync(cancellationToken);
		}

		public async ValueTask<int> TruncateTableAsync(string table, CancellationToken cancellationToken = default)
		{
			await using var dbConnection = this._DbProviderFactory.CreateConnection(this._ConnectionString);
			await dbConnection.OpenAsync(cancellationToken);
			await using var command = dbConnection.CreateSqlCommand($"TRUNCATE TABLE {table.EscapeIdentifier()};");
			var result = await command.ExecuteNonQueryAsync(cancellationToken);
			await dbConnection.CloseAsync();
			return result;
		}

		public async ValueTask<RowSet> UpdateAsync(UpdateRequest update, CancellationToken cancellationToken = default)
		{
			await using var dbConnection = this._DbProviderFactory.CreateConnection(this._ConnectionString);
			await using var command = dbConnection.CreateSqlCommand(update.ToSql());
			update.Parameters.Do(parameter => command.AddInputParameter(parameter.Name, parameter.Value));

			if (update.Output.Any())
			{
				await using var reader = await command.ExecuteReaderAsync(SINGLE_RESULT, cancellationToken);
				return await reader.ReadRowSetAsync(cancellationToken);
			}
			else
			{
				await command.ExecuteNonQueryAsync(cancellationToken);
				return RowSet.Empty;
			}
		}
	}
}
