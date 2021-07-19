// Copyright (c) 2021 Samuel Abraham

using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using TypeCache.Collections.Extensions;
using TypeCache.Data.Extensions;
using TypeCache.Data.Schema;
using TypeCache.Extensions;

namespace TypeCache.Data
{
	internal sealed class SqlApi : ISqlApi
	{
		public const string DATA_SOURCE = "Data Source";
		public const string DATABASE = "Database";
		public const string INITIAL_CATALOG = "Initial Catalog";

		private static string HandleFunctionName(string name)
			=> name.Contains(')') ? name.Left(name.LastIndexOf('(')) : name;

		private readonly DbProviderFactory _DbProviderFactory;
		private readonly string _ConnectionString;

		public SqlApi(string databaseProvider, string connectionString)
		{
			this._DbProviderFactory = DbProviderFactories.GetFactory(databaseProvider);
			this._ConnectionString = connectionString;
		}

		private async ValueTask<ObjectSchema> _GetObjectSchema(string name)
		{
			await using var dbConnection = this._DbProviderFactory.CreateConnection(this._ConnectionString);
			await dbConnection.OpenAsync();
			var objectSchema = await dbConnection.GetObjectSchema(name);
			await dbConnection.CloseAsync();
			return objectSchema;
		}

		public ObjectSchema GetObjectSchema(string name)
		{
			var connectionStringBuilder = this._DbProviderFactory.CreateConnectionStringBuilder(this._ConnectionString);
			var server = connectionStringBuilder[DATA_SOURCE].ToString()!;
			var database = connectionStringBuilder.TryGetValue(DATABASE, out var value)
				|| connectionStringBuilder.TryGetValue(INITIAL_CATALOG, out value) ? value.ToString() : null;

			var parts = name.Split('.', StringSplitOptions.RemoveEmptyEntries).ToArray(part => part.TrimStart('[').TrimEnd(']'));
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
			var tableSchemaCache = ObjectSchema.Cache[server];
			return tableSchemaCache.GetOrAdd(fullName, name => _GetObjectSchema(name).Result);
		}

		public async ValueTask ExecuteTransactionAsync(Func<IBatchSqlApi, ValueTask> transaction
			, TransactionScopeOption option = TransactionScopeOption.Required
			, CancellationToken cancellationToken = default)
		{
			using var transactionScope = new TransactionScope(option, TransactionScopeAsyncFlowOption.Enabled);
			await using var dbConnection = this._DbProviderFactory.CreateConnection(this._ConnectionString);
			await dbConnection.OpenAsync(cancellationToken);
			await transaction(new BatchSqlApi(dbConnection, cancellationToken));
			transactionScope.Complete();
			await dbConnection.CloseAsync();
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
