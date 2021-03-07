// Copyright (c) 2021 Samuel Abraham

using System;
using System.Threading;
using System.Threading.Tasks;

namespace TypeCache.Data
{
	/// <summary>
	/// Mockable database access interface.
	/// </summary>
	public interface ISqlApi
	{
		string ObjectSchemaSQL { get; }

		ValueTask<ObjectSchema> CreateObjectSchema(string name);

		ObjectSchema GetObjectSchema(string name);

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
		ValueTask<int> TruncateTableAsync(string table, CancellationToken cancellationToken = default);

		/// <summary>
		/// UPDATE ... SET ... WHERE ...
		/// </summary>
		/// <returns><code>OUTPUT DELETED|INSERTED</code></returns>
		ValueTask<RowSet> UpdateAsync(UpdateRequest update, CancellationToken cancellationToken = default);
	}
}
