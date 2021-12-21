// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using TypeCache.Collections.Extensions;
using TypeCache.Data.Extensions;
using TypeCache.Data.Requests;
using TypeCache.Data.Schema;
using TypeCache.Extensions;
using static TypeCache.Default;

namespace TypeCache.Data;

internal sealed class SqlApi : ISqlApi
{
	private const string DATABASE = "Database";

	private const string INITIAL_CATALOG = "Initial Catalog";

	private readonly IReadOnlyDictionary<string, DatabaseProvider> _DatabaseProviders;

	public SqlApi(params DataSource[] dataSources)
	{
		var databaseProviders = new Dictionary<string, DatabaseProvider>(dataSources.Length, STRING_COMPARISON.ToStringComparer());
		dataSources.Do(dataSource => databaseProviders.Add(dataSource.Name, new DatabaseProvider(dataSource)));
		this._DatabaseProviders = databaseProviders.ToReadOnly();
	}

	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public async ValueTask<RowSet[]> CallAsync(StoredProcedureRequest request, CancellationToken cancellationToken = default)
		=> await _GetResultAsync(request.DataSource, async _ => await _.CallAsync(request), cancellationToken);

	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public async ValueTask<long> CountAsync(CountRequest request, CancellationToken cancellationToken = default)
		=> await _GetResultAsync(request.DataSource, async _ => await _.CountAsync(request), cancellationToken);

	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public async ValueTask<RowSet> DeleteAsync(DeleteRequest request, CancellationToken cancellationToken = default)
		=> await _GetResultAsync(request.DataSource, async _ => await _.DeleteAsync(request), cancellationToken);

	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public async ValueTask<RowSet> DeleteDataAsync(DeleteDataRequest request, CancellationToken cancellationToken = default)
		=> await _GetResultAsync(request.DataSource, async _ => await _.DeleteDataAsync(request), cancellationToken);

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
		return ObjectSchema.Cache[dataSource].GetOrAdd(fullName, name => _GetResultAsync(dataSource, _ => _.GetObjectSchema(name)).Result);
	}

	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public async ValueTask<RowSet> InsertAsync(InsertRequest request, CancellationToken cancellationToken = default)
		=> await _GetResultAsync(request.DataSource, async _ => await _.InsertAsync(request), cancellationToken);

	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public async ValueTask<RowSet> InsertDataAsync(InsertDataRequest request, CancellationToken cancellationToken = default)
		=> await _GetResultAsync(request.DataSource, async _ => await _.InsertDataAsync(request), cancellationToken);

	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public async ValueTask<RowSet[]> RunAsync(SqlRequest request, CancellationToken cancellationToken = default)
		=> await _GetResultAsync(request.DataSource, async _ => await _.RunAsync(request), cancellationToken);

	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public async ValueTask<RowSet> SelectAsync(SelectRequest request, CancellationToken cancellationToken = default)
		=> await _GetResultAsync(request.DataSource, async _ => await _.SelectAsync(request), cancellationToken);

	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public async ValueTask<int> TruncateTableAsync(string dataSource, string table, CancellationToken cancellationToken = default)
		=> await _GetResultAsync(dataSource, async _ => await _.TruncateTableAsync(table), cancellationToken);

	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public async ValueTask<RowSet> UpdateAsync(UpdateRequest request, CancellationToken cancellationToken = default)
		=> await _GetResultAsync(request.DataSource, async _ => await _.UpdateAsync(request), cancellationToken);

	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public async ValueTask<RowSet> UpdateDataAsync(UpdateDataRequest request, CancellationToken cancellationToken = default)
		=> await _GetResultAsync(request.DataSource, async _ => await _.UpdateDataAsync(request), cancellationToken);

	private async ValueTask<T> _GetResultAsync<T>(string dataSource, Func<DbConnection, ValueTask<T>> session, CancellationToken cancellationToken = default)
	{
		await using var dbConnection = this._DatabaseProviders[dataSource].CreateConnection();
		await dbConnection.OpenAsync(cancellationToken);
		var result = await session(dbConnection);
		await dbConnection.CloseAsync();
		return result;
	}

	private static string HandleFunctionName(string name)
		=> name.Contains(')') ? name.Left(name.LastIndexOf('(')) : name;
}
