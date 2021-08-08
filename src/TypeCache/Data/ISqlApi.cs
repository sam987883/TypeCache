// Copyright (c) 2021 Samuel Abraham

using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using TypeCache.Data.Requests;
using TypeCache.Data.Schema;

namespace TypeCache.Data
{
	/// <summary>
	/// Mockable database access interface.
	/// </summary>
	public interface ISqlApi
	{
		/// <summary>
		/// Gets a cached schema object that describes a table, view, table based function, or stored procedure.
		/// </summary>
		/// <param name="name">The database object name ie. Customers, dbo.Accounts, [CoreDb]..s_Customers</param>
		ObjectSchema GetObjectSchema(string dataSource, string name);

		/// <summary>
		/// All calls to <see cref="ISqlApiSession"/> within <paramref name="session"/> are perormed on the same <see cref="DbConnection"/>.
		/// </summary>
		ValueTask ExecuteSessionAsync(string dataSource, Func<ISqlApiSession, ValueTask> session, CancellationToken cancellationToken = default);

		/// <summary>
		/// All calls to <see cref="ISqlApiSession"/> within <paramref name="transaction"/> will be wrapped in a scoped async-enabled transaction.
		/// </summary>
		ValueTask ExecuteTransactionAsync(
			string dataSource
			, Func<ISqlApiSession, ValueTask> transaction
			, TransactionScopeOption option = TransactionScopeOption.Required
			, IsolationLevel isolationLevel = IsolationLevel.Unspecified
			, TimeSpan? commandTimeout = null
			, CancellationToken cancellationToken = default);

		/// <summary>
		/// <code>EXECUTE ...</code>
		/// </summary>
		ValueTask<RowSet[]> CallAsync(StoredProcedureRequest request, CancellationToken cancellationToken = default);

		/// <summary>
		/// <code>EXECUTE ...</code>
		/// </summary>
		ValueTask<RowSet[]> RunAsync(SqlRequest request, CancellationToken cancellationToken = default);

		/// <summary>
		/// <code>DELETE FROM ... WHERE ...</code>
		/// </summary>
		/// <returns><code>OUTPUT DELETED</code></returns>
		ValueTask<RowSet> DeleteAsync(DeleteRequest request, CancellationToken cancellationToken = default);

		/// <summary>
		/// <code>DELETE FROM ... VALUES ...</code>
		/// </summary>
		/// <returns><code>OUTPUT DELETED</code></returns>
		ValueTask<RowSet> DeleteDataAsync(DeleteDataRequest request, CancellationToken cancellationToken = default);

		/// <summary>
		/// <code>INSERT INTO ... SELECT ... FROM ... WHERE ... HAVING ... ORDER BY ...</code>
		/// </summary>
		/// <returns><code>OUTPUT INSERTED</code></returns>
		ValueTask<RowSet> InsertAsync(InsertRequest request, CancellationToken cancellationToken = default);

		/// <summary>
		/// <code>INSERT INTO ... VALUES ...</code>
		/// </summary>
		/// <returns><code>OUTPUT INSERTED</code></returns>
		ValueTask<RowSet> InsertDataAsync(InsertDataRequest request, CancellationToken cancellationToken = default);

		/// <summary>
		/// <code>SELECT ... FROM ... WHERE ... HAVING ... ORDER BY ...</code>
		/// </summary>
		ValueTask<RowSet> SelectAsync(SelectRequest request, CancellationToken cancellationToken = default);

		/// <summary>
		/// <code>TRUNCATE TABLE ...</code>
		/// </summary>
		ValueTask<int> TruncateTableAsync(string dataSource, string table, CancellationToken cancellationToken = default);

		/// <summary>
		/// <code>UPDATE ... SET ... OUTPUT ... WHERE ...</code>
		/// </summary>
		/// <returns><code>OUTPUT DELETED|INSERTED</code></returns>
		ValueTask<RowSet> UpdateAsync(UpdateRequest request, CancellationToken cancellationToken = default);

		/// <summary>
		/// <code>UPDATE ... SET ... OUTPUT ... VALUES ...</code>
		/// </summary>
		/// <returns><code>OUTPUT DELETED|INSERTED</code></returns>
		ValueTask<RowSet> UpdateDataAsync(UpdateDataRequest request, CancellationToken cancellationToken = default);
	}
}
