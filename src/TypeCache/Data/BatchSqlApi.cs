// Copyright (c) 2021 Samuel Abraham

using System;
using System.Data.Common;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using TypeCache.Collections.Extensions;
using TypeCache.Data.Extensions;
using TypeCache.Data.Schema;
using TypeCache.Extensions;

namespace TypeCache.Data
{
	internal sealed class BatchSqlApi : IBatchSqlApi
	{
		private static string HandleFunctionName(string name)
			=> name.Contains(')') ? name.Left(name.LastIndexOf('(')) : name;

		private readonly CancellationToken _CancellationToken;
		private readonly DbConnection _DbConnection;

		public BatchSqlApi(DbConnection dbConnection, CancellationToken cancellationToken = default)
		{
			this._CancellationToken = cancellationToken;
			this._DbConnection = dbConnection;
		}

		public async ValueTask ExecuteTransactionAsync(Func<IBatchSqlApi, ValueTask> transaction, TransactionScopeOption option = TransactionScopeOption.Required)
		{
			using var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
			await transaction(this);
			transactionScope.Complete();
		}

		public ObjectSchema GetObjectSchema(string name)
		{
			var server = this._DbConnection.DataSource;
			var database = this._DbConnection.Database;

			var parts = name.Split('.', StringSplitOptions.RemoveEmptyEntries).ToArray(part => part.TrimStart('[').TrimEnd(']'));
			var fullName = parts.Length switch
			{
				1 when !database.IsBlank() => $"[{database}]..[{HandleFunctionName(parts[0])}]",
				1 => throw new ArgumentException($"{nameof(SqlApi)}.{nameof(GetObjectSchema)}: ConnectionString must have [Database] or [Initial Catalog] specified for database object.", name),
				2 when name.Contains("..") => $"[{parts[0]}]..[{HandleFunctionName(parts[1])}]",
				2 when !database.IsBlank() => $"[{database}].[{parts[0]}].[{HandleFunctionName(parts[1])}]",
				2 => throw new ArgumentException($"{nameof(SqlApi)}.{nameof(GetObjectSchema)}: ConnectionString must have [Database] or [Initial Catalog] specified for database object.", name),
				3 => $"[{parts[0]}].[{parts[1]}].[{HandleFunctionName(parts[2])}]",
				_ => throw new ArgumentException($"{nameof(SqlApi)}.{nameof(GetObjectSchema)}: Invalid table source name.", name)
			};
			return ObjectSchema.Cache[server].GetOrAdd(fullName, name => this._DbConnection.GetObjectSchema(name).Result);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public async ValueTask<RowSet[]> CallAsync(StoredProcedureRequest procedure)
			=> await this._DbConnection.CallAsync(procedure, this._CancellationToken);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public async ValueTask<RowSet[]> RunAsync(SqlRequest request)
			=> await this._DbConnection.RunAsync(request, this._CancellationToken);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public async ValueTask<RowSet> DeleteAsync(DeleteRequest delete)
			=> await this._DbConnection.DeleteAsync(delete, this._CancellationToken);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public async ValueTask<RowSet> InsertAsync(InsertRequest insert)
			=> await this._DbConnection.InsertAsync(insert, this._CancellationToken);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public async ValueTask<RowSet> MergeAsync(BatchRequest batch)
			=> await this._DbConnection.MergeAsync(batch, this._CancellationToken);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public async ValueTask<RowSet> SelectAsync(SelectRequest select)
			=> await this._DbConnection.SelectAsync(select, this._CancellationToken);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public async ValueTask<int> TruncateTableAsync(string table)
			=> await this._DbConnection.TruncateTableAsync(table, this._CancellationToken);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public async ValueTask<RowSet> UpdateAsync(UpdateRequest update)
			=> await this._DbConnection.UpdateAsync(update, this._CancellationToken);
	}
}
