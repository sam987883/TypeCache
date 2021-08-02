// Copyright (c) 2021 Samuel Abraham

using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
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
		ValueTask<RowSet[]> CallAsync(StoredProcedureRequest procedure, CancellationToken cancellationToken = default);

		/// <summary>
		/// <code>EXECUTE ...</code>
		/// </summary>
		ValueTask<RowSet[]> RunAsync(SqlRequest request, CancellationToken cancellationToken = default);

		/// <summary>
		/// <code>DELETE FROM ... WHERE ...</code>
		/// </summary>
		/// <returns><code>OUTPUT DELETED</code></returns>
		ValueTask<RowSet> DeleteAsync(DeleteRequest delete, CancellationToken cancellationToken = default);

		/// <summary>
		/// <code>SELECT ... FROM ... WHERE ... HAVING ... ORDER BY ...</code>
		/// </summary>
		/// <returns><code>OUTPUT INSERTED</code></returns>
		ValueTask<RowSet> InsertAsync(InsertRequest insert, CancellationToken cancellationToken = default);

		/// <summary>
		/// <code>
		/// MERGE ... USING ... ON ... WHEN MATCHED THEN UPDATE ... WHEN NOT MATCHED BY TARGET THEN INSERT ... WHEN NOT MATCHED BY SOURCE THEN DELETE ... OUTPUT ...;<br />
		/// MERGE ... USING ... ON ... WHEN NOT MATCHED BY TARGET THEN INSERT ... WHEN NOT MATCHED BY SOURCE THEN DELETE ... OUTPUT ...;<br />
		/// MERGE ... USING ... ON ... WHEN MATCHED THEN UPDATE ... WHEN NOT MATCHED BY SOURCE THEN DELETE ... OUTPUT ...;<br />
		/// MERGE ... USING ... ON ... WHEN MATCHED THEN DELETE ... OUTPUT ...;<br />
		/// INSERT INTO ... (...) OUTPUT ... VALUES ...;<br />
		/// </code>
		/// </summary>
		/// <returns><code>OUTPUT DELETED|INSERTED</code></returns>
		ValueTask<RowSet> MergeAsync(BatchRequest batch, CancellationToken cancellationToken = default);

		/// <summary>
		/// <code>SELECT ... FROM ... WHERE ... HAVING ... ORDER BY ...</code>
		/// </summary>
		ValueTask<RowSet> SelectAsync(SelectRequest select, CancellationToken cancellationToken = default);

		/// <summary>
		/// <code>TRUNCATE TABLE ...</code>
		/// </summary>
		ValueTask<int> TruncateTableAsync(string dataSource, string table, CancellationToken cancellationToken = default);

		/// <summary>
		/// UPDATE ... SET ... WHERE ...
		/// </summary>
		/// <returns><code>OUTPUT DELETED|INSERTED</code></returns>
		ValueTask<RowSet> UpdateAsync(UpdateRequest update, CancellationToken cancellationToken = default);
	}
}
