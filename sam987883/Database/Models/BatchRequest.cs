// Copyright (c) 2020 Samuel Abraham

using sam987883.Common.Models;

namespace sam987883.Database.Models
{
	/// <summary>
	/// Performs a SQL MERGE.
	/// Insert only uses INSERT statement instead of MERGE statement.
	/// </summary>
	public sealed class BatchRequest
	{
		/// <summary>
		/// JSON: <code>true</code>
		/// SQL: <code>WHEN NOT MATCHED BY SOURCE THEN DELETE</code>
		/// </summary>
		public bool Delete { get; set; } = false;

		/// <summary>
		/// Batch input rows of data.
		/// </summary>
		public RowSet Input { get; set; } = new RowSet();

		/// <summary>
		/// JSON: <code>"Insert": [ "Column1", "Column2", "Column3" ]</code>
		/// SQL: <code>WHEN NOT MATCHED BY TARGET THEN INSERT ... </code>
		/// </summary>
		public string[] Insert { get; set; } = new string[0];

		/// <summary>
		/// The columns to match on when performing a SQL MERGE operation for UPDATE or DELETE.
		/// JSON: <code>"On": [ "PrimaryKey1", "PrimaryKey2" ]</code>
		/// SQL: <code>ON SOURCE.[PrimaryKey1] = TARGET.[PrimaryKey1] AND SOURCE.[PrimaryKey2] = TARGET.[PrimaryKey2]</code>
		/// </summary>
		public string[] On { get; set; } = new string[0];

		/// <summary>
		/// JSON: <code>[ "Column1", "Column2", "Column3" ]</code>
		/// SQL: <code>OUTPUT DELETED.[Column1], DELETED.[Column2], DELETED.[Column3]</code>
		/// </summary>
		public string[] OutputDeleted { get; set; } = new string[0];

		/// <summary>
		/// JSON: <code>[ "Column1", "Column2", "Column3" ]</code>
		/// SQL: <code>OUTPUT INSERTED.[Column1], INSERTED.[Column2], INSERTED.[Column3]</code>
		/// </summary>
		public string[] OutputInserted { get; set; } = new string[0];

		/// <summary>
		/// JSON: <code>"Table1"</code>
		/// SQL: <code>MERGE [Database1]..[Table1]</code>
		/// </summary>
		public string Table { get; set; } = string.Empty;

		/// <summary>
		/// JSON: <code>[ "Column1", "Column2", "Column3" ]</code>
		/// SQL: <code>WHEN MATCHED THEN UPDATE ... </code>
		/// </summary>
		public string[] Update { get; set; } = new string[0];
	}
}
