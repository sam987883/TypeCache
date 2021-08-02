// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
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
		public const string DATABASE = "Database";

		public const string INITIAL_CATALOG = "Initial Catalog";

		private static string HandleFunctionName(string name)
			=> name.Contains(')') ? name.Left(name.LastIndexOf('(')) : name;

		private readonly IReadOnlyDictionary<string, DatabaseProvider> _DatabaseProviders;

		public SqlApi(params DataSource[] dataSources)
		{
			var databaseProviders = new Dictionary<string, DatabaseProvider>(dataSources.Length, StringComparer.OrdinalIgnoreCase);
			dataSources.Do(dataSource => databaseProviders.Add(dataSource.Name, new DatabaseProvider(dataSource)));
			this._DatabaseProviders = databaseProviders.ToReadOnly();
		}

		private DbConnection CreateConnection(IDataRequest request)
			=> this._DatabaseProviders[request.DataSource].CreateConnection();

		private async ValueTask<ObjectSchema> _GetObjectSchema(string dataSource, string name)
		{
			await using var dbConnection = this._DatabaseProviders[dataSource].CreateConnection();
			await dbConnection.OpenAsync();
			var objectSchema = await dbConnection.GetObjectSchema(name);
			await dbConnection.CloseAsync();
			return objectSchema;
		}

		public ObjectSchema GetObjectSchema(string dataSource, string name)
		{
			var connectionStringBuilder = this._DatabaseProviders[dataSource].CreateConnectionStringBuilder();
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
			return ObjectSchema.Cache[dataSource].GetOrAdd(fullName, name => _GetObjectSchema(dataSource, name).Result);
		}

		public async ValueTask ExecuteSessionAsync(string dataSource, Func<ISqlApiSession, ValueTask> session, CancellationToken cancellationToken = default)
		{
			await using var dbConnection = this._DatabaseProviders[dataSource].CreateConnection();
			await dbConnection.OpenAsync(cancellationToken);
			await session(new SqlApiSession(dataSource, dbConnection, cancellationToken));
			await dbConnection.CloseAsync();
		}

		public async ValueTask ExecuteTransactionAsync(
			string dataSource
			, Func<ISqlApiSession, ValueTask> transaction
			, TransactionScopeOption option = TransactionScopeOption.Required
			, IsolationLevel isolationLevel = IsolationLevel.Unspecified
			, TimeSpan? commandTimeout = null
			, CancellationToken cancellationToken = default)
		{
			var transactionOptions = new TransactionOptions
			{
				IsolationLevel = isolationLevel == IsolationLevel.Unspecified ? IsolationLevel.ReadCommitted : isolationLevel,
				Timeout = commandTimeout ?? TransactionManager.DefaultTimeout
			};
			using var transactionScope = new TransactionScope(option, transactionOptions, TransactionScopeAsyncFlowOption.Enabled);
			await using var dbConnection = this._DatabaseProviders[dataSource].CreateConnection();
			await dbConnection.OpenAsync(cancellationToken);
			await transaction(new SqlApiSession(dataSource, dbConnection, cancellationToken));
			transactionScope.Complete();
			await dbConnection.CloseAsync();
		}

		public async ValueTask<RowSet[]> CallAsync(StoredProcedureRequest procedure, CancellationToken cancellationToken = default)
		{
			await using var dbConnection = this.CreateConnection(procedure);
			await dbConnection.OpenAsync(cancellationToken);
			var results = await dbConnection.CallAsync(procedure, cancellationToken);
			await dbConnection.CloseAsync();
			return results;
		}

		public async ValueTask<RowSet[]> RunAsync(SqlRequest request, CancellationToken cancellationToken = default)
		{
			await using var dbConnection = this.CreateConnection(request);
			await dbConnection.OpenAsync(cancellationToken);
			var results = await dbConnection.RunAsync(request, cancellationToken);
			await dbConnection.CloseAsync();
			return results;
		}

		public async ValueTask<RowSet> DeleteAsync(DeleteRequest delete, CancellationToken cancellationToken = default)
		{
			await using var dbConnection = this.CreateConnection(delete);
			await dbConnection.OpenAsync(cancellationToken);
			var result = await dbConnection.DeleteAsync(delete, cancellationToken);
			await dbConnection.CloseAsync();
			return result;
		}

		public async ValueTask<RowSet> InsertAsync(InsertRequest insert, CancellationToken cancellationToken = default)
		{
			await using var dbConnection = this.CreateConnection(insert);
			await dbConnection.OpenAsync(cancellationToken);
			var result = await dbConnection.InsertAsync(insert, cancellationToken);
			await dbConnection.CloseAsync();
			return result;
		}

		public async ValueTask<RowSet> MergeAsync(BatchRequest batch, CancellationToken cancellationToken = default)
		{
			await using var dbConnection = this.CreateConnection(batch);
			await dbConnection.OpenAsync(cancellationToken);
			var result = await dbConnection.MergeAsync(batch, cancellationToken);
			await dbConnection.CloseAsync();
			return result;
		}

		public async ValueTask<RowSet> SelectAsync(SelectRequest select, CancellationToken cancellationToken = default)
		{
			await using var dbConnection = this.CreateConnection(select);
			await dbConnection.OpenAsync(cancellationToken);
			var result = await dbConnection.SelectAsync(select, cancellationToken);
			await dbConnection.CloseAsync();
			return result;
		}

		public async ValueTask<int> TruncateTableAsync(string dataSource, string table, CancellationToken cancellationToken = default)
		{
			await using var dbConnection = this._DatabaseProviders[dataSource].CreateConnection();
			await dbConnection.OpenAsync(cancellationToken);
			var result = await dbConnection.TruncateTableAsync(table, cancellationToken);
			await dbConnection.CloseAsync();
			return result;
		}

		public async ValueTask<RowSet> UpdateAsync(UpdateRequest update, CancellationToken cancellationToken = default)
		{
			await using var dbConnection = this.CreateConnection(update);
			await dbConnection.OpenAsync(cancellationToken);
			var result = await dbConnection.UpdateAsync(update, cancellationToken);
			await dbConnection.CloseAsync();
			return result;
		}
	}
}
