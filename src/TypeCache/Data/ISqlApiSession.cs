// Copyright (c) 2021 Samuel Abraham

using System;
using System.Threading.Tasks;
using System.Transactions;
using TypeCache.Data.Requests;
using TypeCache.Data.Schema;

namespace TypeCache.Data
{
	/// <summary>
	/// Mockable database access interface.
	/// </summary>
	public interface ISqlApiSession
	{
		/// <summary>
		/// Gets a cached schema object that describes a table, view, table based function, or stored procedure.
		/// </summary>
		/// <param name="name">The database object name ie. Customers, dbo.Accounts, [Db]..s_Customers</param>
		ObjectSchema GetObjectSchema(string name);

		/// <summary>
		/// All calls to ISqlApi within parameter transaction will be wrapped in a scoped async-enabled transaction.
		/// </summary>
		ValueTask ExecuteTransactionAsync(
			string dataSource
			, Func<ISqlApiSession, ValueTask> transaction
			, TransactionScopeOption option = TransactionScopeOption.Required
			, IsolationLevel isolationLevel = IsolationLevel.Unspecified
			, TimeSpan? commandTimeout = null);

		/// <summary>
		/// <code>EXECUTE ...</code>
		/// </summary>
		ValueTask<RowSet[]> CallAsync(StoredProcedureRequest request);

		/// <summary>
		/// <code>EXECUTE ...</code>
		/// </summary>
		ValueTask<RowSet[]> RunAsync(SqlRequest request);

		/// <summary>
		/// <code>SELECT COUNT(1) FROM ... WHERE ...</code>
		/// </summary>
		ValueTask<long> CountAsync(CountRequest request);

		/// <summary>
		/// <code>DELETE FROM ... WHERE ...</code>
		/// </summary>
		/// <returns><code>OUTPUT DELETED</code></returns>
		ValueTask<RowSet> DeleteAsync(DeleteRequest request);

		/// <summary>
		/// <code>DELETE FROM ... VALUES ...</code>
		/// </summary>
		/// <returns><code>OUTPUT DELETED</code></returns>
		ValueTask<RowSet> DeleteDataAsync(DeleteDataRequest request);

		/// <summary>
		/// <code>SELECT ... FROM ... WHERE ... HAVING ... ORDER BY ...</code>
		/// </summary>
		/// <returns><code>OUTPUT INSERTED</code></returns>
		ValueTask<RowSet> InsertAsync(InsertRequest request);

		/// <summary>
		/// <code>SELECT ... FROM ... WHERE ... HAVING ... ORDER BY ...</code>
		/// </summary>
		ValueTask<RowSet> SelectAsync(SelectRequest request);

		/// <summary>
		/// <code>TRUNCATE TABLE ...</code>
		/// </summary>
		ValueTask<int> TruncateTableAsync(string table);

		/// <summary>
		/// <code>UPDATE ... SET ... OUTPUT ... WHERE ...</code>
		/// </summary>
		/// <returns><code>OUTPUT DELETED|INSERTED</code></returns>
		ValueTask<RowSet> UpdateAsync(UpdateRequest request);

		/// <summary>
		/// <code>UPDATE ... SET ... OUTPUT ... WHERE ...</code>
		/// </summary>
		/// <returns><code>OUTPUT DELETED|INSERTED</code></returns>
		ValueTask<RowSet> UpdateDataAsync(UpdateDataRequest request);
	}
}
