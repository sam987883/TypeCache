// Copyright (c) 2021 Samuel Abraham

using System;
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

namespace TypeCache.Data
{
	internal sealed class SqlApiSession : ISqlApiSession
	{
		private const string DATABASE = "Database";

		private const string INITIAL_CATALOG = "Initial Catalog";

		private static string HandleFunctionName(string name)
			=> name.Contains(')') ? name.Left(name.LastIndexOf('(')) : name;

		private readonly CancellationToken _CancellationToken;
		private readonly string _DataSource;
		private readonly DbConnection _DbConnection;

		public SqlApiSession(string dataSource, DbConnection dbConnection, CancellationToken cancellationToken = default)
		{
			this._CancellationToken = cancellationToken;
			this._DataSource = dataSource;
			this._DbConnection = dbConnection;
		}

		public async ValueTask ExecuteTransactionAsync(
			string dataSource
			, Func<ISqlApiSession, ValueTask> transaction
			, TransactionScopeOption option = TransactionScopeOption.Required
			, IsolationLevel isolationLevel = IsolationLevel.Unspecified
			, TimeSpan? commandTimeout = null)
		{
			var transactionOptions = new TransactionOptions
			{
				IsolationLevel = isolationLevel == IsolationLevel.Unspecified ? IsolationLevel.ReadCommitted : isolationLevel,
				Timeout = commandTimeout ?? TransactionManager.DefaultTimeout
			};
			using var transactionScope = new TransactionScope(option, transactionOptions, TransactionScopeAsyncFlowOption.Enabled);
			await transaction(this);
			transactionScope.Complete();
		}

		public ObjectSchema GetObjectSchema(string name)
		{
			var database = this._DbConnection.Database;

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
			return ObjectSchema.Cache[this._DataSource].GetOrAdd(fullName, name => this._DbConnection.GetObjectSchema(name).Result);
		}

		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public async ValueTask<RowSet[]> CallAsync(StoredProcedureRequest request)
			=> await this._DbConnection.CallAsync(request, this._CancellationToken);

		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public async ValueTask<RowSet[]> RunAsync(SqlRequest request)
			=> await this._DbConnection.RunAsync(request, this._CancellationToken);

		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public async ValueTask<long> CountAsync(CountRequest request)
			=> await this._DbConnection.CountAsync(request, this._CancellationToken);

		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public async ValueTask<RowSet> DeleteAsync(DeleteRequest request)
			=> await this._DbConnection.DeleteAsync(request, this._CancellationToken);

		public async ValueTask<RowSet> DeleteDataAsync(DeleteDataRequest request)
			=> await this._DbConnection.DeleteDataAsync(request, this._CancellationToken);

		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public async ValueTask<RowSet> InsertAsync(InsertRequest request)
			=> await this._DbConnection.InsertAsync(request, this._CancellationToken);

		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public async ValueTask<RowSet> InsertDataAsync(InsertDataRequest request)
			=> await this._DbConnection.InsertDataAsync(request, this._CancellationToken);

		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public async ValueTask<RowSet> SelectAsync(SelectRequest request)
			=> await this._DbConnection.SelectAsync(request, this._CancellationToken);

		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public async ValueTask<int> TruncateTableAsync(string table)
			=> await this._DbConnection.TruncateTableAsync(table, this._CancellationToken);

		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public async ValueTask<RowSet> UpdateAsync(UpdateRequest request)
			=> await this._DbConnection.UpdateAsync(request, this._CancellationToken);

		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public async ValueTask<RowSet> UpdateDataAsync(UpdateDataRequest request)
			=> await this._DbConnection.UpdateDataAsync(request, this._CancellationToken);
	}
}
