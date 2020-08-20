// Copyright (c) 2020 Samuel Abraham

using Sam987883.Common.Models;
using System.Text.Json.Serialization;

namespace Sam987883.Database.Models
{
	/// <summary>
	/// Performs a batch DELETE/INSERT/UPDATE or any combination thereof.
	/// <code>
	/// <list>
	/// <item>MERGE ... USING ... ON s.[PrimaryKey] = t.[PrimaryKey] WHEN MATCHED THEN UPDATE ... WHEN NOT MATCHED BY TARGET THEN INSERT ... WHEN NOT MATCHED BY SOURCE THEN DELETE ... OUTPUT ...;</item>
	/// <item>MERGE ... USING ... ON s.[PrimaryKey] = t.[PrimaryKey] WHEN NOT MATCHED BY TARGET THEN INSERT ... WHEN NOT MATCHED BY SOURCE THEN DELETE ... OUTPUT ...;</item>
	/// <item>MERGE ... USING ... ON s.[PrimaryKey] = t.[PrimaryKey] WHEN MATCHED THEN UPDATE ... WHEN NOT MATCHED BY SOURCE THEN DELETE ... OUTPUT ...;</item>
	/// <item>MERGE ... USING ... ON s.[PrimaryKey] = t.[PrimaryKey] WHEN MATCHED THEN DELETE ... OUTPUT ...;</item>
	/// <item>INSERT INTO ... (...) OUTPUT ... VALUES ...;</item>
	/// </list>
	/// </code>
	/// </summary>
	public sealed class BatchRequest
	{
		/// <summary>
		/// JSON: <code>true|false</code>
		/// SQL: For UPDATE/DELETE: <code>WHEN MATCHED THEN UPDATE ... WHEN NOT MATCHED BY SOURCE THEN DELETE</code>
		/// For DELETE only: <code>WHEN MATCHED THEN DELETE</code>
		/// </summary>
		public bool Delete { get; set; } = false;

		/// <summary>
		/// Batch input rows of data.
		/// </summary>
		public RowSet Input { get; set; } = new RowSet();

		/// <summary>
		/// JSON: <code>"Insert": [ "Column1", "Column2", "Column3" ]</code>
		/// SQL: For INSERT/[UPDATE|DELETE]: <code>WHEN NOT MATCHED BY TARGET THEN INSERT ...</code>
		/// For INSERT only: <code>INSERT INTO ... (...) OUTPUT ... VALUES ...;</code>
		/// </summary>
		public string[] Insert { get; set; } = new string[0];

		/// <summary>
		/// The columns to match on when performing a SQL MERGE operation for UPDATE or DELETE.
		/// JSON: <code>"On": [ "PrimaryKey1", "PrimaryKey2" ]</code>
		/// SQL: <code>ON SOURCE.[PrimaryKey1] = TARGET.[PrimaryKey1] AND SOURCE.[PrimaryKey2] = TARGET.[PrimaryKey2]</code>
		/// </summary>
		[JsonIgnore]
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
		/// <para>Table must have primary key(s) defined to be used for batch DELETE/UPDATE.</para>
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
