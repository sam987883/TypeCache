// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
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

		public const string DATA_SOURCE = "Data Source";
		public const string DATABASE = "Database";
		public const string INITIAL_CATALOG = "Initial Catalog";
		public const string OBJECT_NAME = "ObjectName";
		public const string SCHEMA_NAME = "SchemaName";

		private static string HandleFunctionName(string name)
			=> name.Contains(')') ? name.Left(name.LastIndexOf('(')) : name;

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
	SELECT o.[object_id]
	FROM [sys].[objects] AS o
	INNER JOIN [sys].[schemas] AS s ON s.[schema_id] = o.[schema_id]
	WHERE o.[name] = @{OBJECT_NAME}
		AND s.[name] = ISNULL(@{SCHEMA_NAME}, SCHEMA_NAME())
		AND o.[type] IN ('U', 'V', 'TF', 'P')
);
DECLARE @SqlDbTypes AS TABLE
(
	[ID] INTEGER NOT NULL PRIMARY KEY
    , [Type] NVARCHAR(200) NOT NULL UNIQUE
);

INSERT INTO @SqlDbTypes ([ID], [Type])
VALUES ({SqlDbType.BigInt.Number()}, '{SqlDbType.BigInt.Name()}')
, ({SqlDbType.Binary.Number()}, '{SqlDbType.Binary.Name()}')
, ({SqlDbType.Bit.Number()}, '{SqlDbType.Bit.Name()}')
, ({SqlDbType.Char.Number()}, '{SqlDbType.Char.Name()}')
, ({SqlDbType.Date.Number()}, '{SqlDbType.Date.Name()}')
, ({SqlDbType.DateTime.Number()}, '{SqlDbType.DateTime.Name()}')
, ({SqlDbType.DateTime2.Number()}, '{SqlDbType.DateTime2.Name()}')
, ({SqlDbType.DateTimeOffset.Number()}, '{SqlDbType.DateTimeOffset.Name()}')
, ({SqlDbType.Decimal.Number()}, '{SqlDbType.Decimal.Name()}')
, ({SqlDbType.Float.Number()}, '{SqlDbType.Float.Name()}')
, ({SqlDbType.Image.Number()}, '{SqlDbType.Image.Name()}')
, ({SqlDbType.Int.Number()}, '{SqlDbType.Int.Name()}')
, ({SqlDbType.Money.Number()}, '{SqlDbType.Money.Name()}')
, ({SqlDbType.NChar.Number()}, '{SqlDbType.NChar.Name()}')
, ({SqlDbType.NText.Number()}, '{SqlDbType.NText.Name()}')
, ({SqlDbType.NVarChar.Number()}, '{SqlDbType.NVarChar.Name()}')
, ({SqlDbType.Real.Number()}, '{SqlDbType.Real.Name()}')
, ({SqlDbType.SmallDateTime.Number()}, '{SqlDbType.SmallDateTime.Name()}')
, ({SqlDbType.SmallInt.Number()}, '{SqlDbType.SmallInt.Name()}')
, ({SqlDbType.SmallMoney.Number()}, '{SqlDbType.SmallMoney.Name()}')
, ({SqlDbType.Structured.Number()}, '{SqlDbType.Structured.Name()}')
, ({SqlDbType.Text.Number()}, '{SqlDbType.Text.Name()}')
, ({SqlDbType.Time.Number()}, '{SqlDbType.Time.Name()}')
, ({SqlDbType.Timestamp.Number()}, '{SqlDbType.Timestamp.Name()}')
, ({SqlDbType.TinyInt.Number()}, '{SqlDbType.TinyInt.Name()}')
, ({SqlDbType.Udt.Number()}, '{SqlDbType.Udt.Name()}')
, ({SqlDbType.UniqueIdentifier.Number()}, '{SqlDbType.UniqueIdentifier.Name()}')
, ({SqlDbType.VarBinary.Number()}, '{SqlDbType.VarBinary.Name()}')
, ({SqlDbType.VarChar.Number()}, '{SqlDbType.VarChar.Name()}')
, ({SqlDbType.Xml.Number()}, '{SqlDbType.Xml.Name()}');

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
, ISNULL((SELECT [ID] FROM @SqlDbTypes WHERE [Type] = t.[name]), {SqlDbType.Variant.Number()}) AS [Type]
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
, ISNULL((SELECT [ID] FROM @SqlDbTypes WHERE [Type] = t.[name]), {SqlDbType.Variant.Number()}) AS [Type]
, p.[is_output] AS [Output]
, IIF(p.[is_output] = 1 AND p.[parameter_id] = 0, 1, 0) AS [Return]
FROM [sys].[parameters] AS p
INNER JOIN [sys].[types] AS t ON t.[user_type_id] = p.[user_type_id]
WHERE p.[object_id] = @ObjectId
ORDER BY p.[parameter_id] ASC;";

		public async ValueTask ExecuteTransactionAsync(Func<ISqlApi, CancellationToken, ValueTask> transaction, CancellationToken cancellationToken = default)
		{
			using var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
			await using var dbConnection = this._DbProviderFactory.CreateConnection(this._ConnectionString);
			await dbConnection.OpenAsync(cancellationToken);
			await transaction(this, cancellationToken);
			transactionScope.Complete();
			await dbConnection.CloseAsync();
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
				1 when !database.IsBlank() => $"[{database}]..[{HandleFunctionName(parts[0])}]",
				1 => throw new ArgumentException($"{nameof(SqlApi)}.{nameof(GetObjectSchema)}: ConnectionString must have [{DATABASE}] or [{INITIAL_CATALOG}] specified for database object.", name),
				2 when name.Contains("..") => $"[{parts[0]}]..[{HandleFunctionName(parts[1])}]",
				2 when !database.IsBlank() => $"[{database}].[{parts[0]}].[{HandleFunctionName(parts[1])}]",
				2 => throw new ArgumentException($"{nameof(SqlApi)}.{nameof(GetObjectSchema)}: ConnectionString must have [{DATABASE}] or [{INITIAL_CATALOG}] specified for database object.", name),
				3 => $"[{parts[0]}].[{parts[1]}].[{HandleFunctionName(parts[2])}]",
				_ => throw new ArgumentException($"{nameof(SqlApi)}.{nameof(GetObjectSchema)}: Invalid table source name.", name)
			};
			var tableSchemaCache = SchemaCache[server];
			return tableSchemaCache.GetOrAdd(fullName, this._GetObjectSchema);
		}

		private ObjectSchema _GetObjectSchema(string name)
		{
			using var dbConnection = this._DbProviderFactory.CreateConnection(this._ConnectionString);
			dbConnection.Open();

			var objectName = name.Split('.').Get(^1)!.TrimStart('[').TrimEnd(']');
			var schemaName = name.Contains("..") ? (object)DBNull.Value : name.Split('.').Get(^2)!.TrimStart('[').TrimEnd(']');
			var request = new SqlRequest
			{
				Parameters = new[]
				{
					new Parameter(OBJECT_NAME, objectName),
					new Parameter(SCHEMA_NAME, schemaName)
				},
				SQL = ObjectSchemaSQL
			};
			var rowSets = dbConnection.RunAsync(request).Result;
			dbConnection.Close();

			var table = rowSets[0];
			var columns = rowSets[1].Map<ColumnSchema>();
			var parameters = rowSets[2].Map<ParameterSchema>();

			return new ObjectSchema
			{
				Columns = columns.ToImmutable(),
				Id = (int)table[0, nameof(ObjectSchema.Id)]!,
				DatabaseName = dbConnection.Database,
				ObjectName = table[0, nameof(ObjectSchema.ObjectName)]!.ToString()!,
				Parameters = parameters.ToImmutable(),
				SchemaName = table[0, nameof(ObjectSchema.SchemaName)]!.ToString()!,
				Type = (ObjectType)table[0, nameof(ObjectSchema.Type)]!
			};
		}

		public async ValueTask<RowSet[]> CallAsync(StoredProcedureRequest procedure, CancellationToken cancellationToken = default)
		{
			await using var dbConnection = this._DbProviderFactory.CreateConnection(this._ConnectionString);
			await dbConnection.OpenAsync(cancellationToken);
			var results = await dbConnection.CallAsync(procedure, cancellationToken);
			await dbConnection.CloseAsync();
			return results;
		}

		public async ValueTask<RowSet[]> RunAsync(SqlRequest request, CancellationToken cancellationToken = default)
		{
			await using var dbConnection = this._DbProviderFactory.CreateConnection(this._ConnectionString);
			await dbConnection.OpenAsync(cancellationToken);
			var results = await dbConnection.RunAsync(request, cancellationToken);
			await dbConnection.CloseAsync();
			return results;
		}

		public async ValueTask<RowSet> DeleteAsync(DeleteRequest delete, CancellationToken cancellationToken = default)
		{
			await using var dbConnection = this._DbProviderFactory.CreateConnection(this._ConnectionString);
			await dbConnection.OpenAsync(cancellationToken);
			var result = await dbConnection.DeleteAsync(delete, cancellationToken);
			await dbConnection.CloseAsync();
			return result;
		}

		public async ValueTask<RowSet> InsertAsync(InsertRequest insert, CancellationToken cancellationToken = default)
		{
			await using var dbConnection = this._DbProviderFactory.CreateConnection(this._ConnectionString);
			await dbConnection.OpenAsync(cancellationToken);
			var result = await dbConnection.InsertAsync(insert, cancellationToken);
			await dbConnection.CloseAsync();
			return result;
		}

		public async ValueTask<RowSet> MergeAsync(BatchRequest batch, CancellationToken cancellationToken = default)
		{
			await using var dbConnection = this._DbProviderFactory.CreateConnection(this._ConnectionString);
			await dbConnection.OpenAsync(cancellationToken);
			var result = await dbConnection.MergeAsync(batch, cancellationToken);
			await dbConnection.CloseAsync();
			return result;
		}

		public async ValueTask<RowSet> SelectAsync(SelectRequest select, CancellationToken cancellationToken = default)
		{
			await using var dbConnection = this._DbProviderFactory.CreateConnection(this._ConnectionString);
			await dbConnection.OpenAsync(cancellationToken);
			var result = await dbConnection.SelectAsync(select, cancellationToken);
			await dbConnection.CloseAsync();
			return result;
		}

		public async ValueTask<int> TruncateTableAsync(string table, CancellationToken cancellationToken = default)
		{
			await using var dbConnection = this._DbProviderFactory.CreateConnection(this._ConnectionString);
			await dbConnection.OpenAsync(cancellationToken);
			var result = await dbConnection.TruncateTableAsync(table, cancellationToken);
			await dbConnection.CloseAsync();
			return result;
		}

		public async ValueTask<RowSet> UpdateAsync(UpdateRequest update, CancellationToken cancellationToken = default)
		{
			await using var dbConnection = this._DbProviderFactory.CreateConnection(this._ConnectionString);
			await dbConnection.OpenAsync(cancellationToken);
			var result = await dbConnection.UpdateAsync(update, cancellationToken);
			await dbConnection.CloseAsync();
			return result;
		}
	}
}
