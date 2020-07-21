// Copyright (c) 2020 Samuel Abraham

namespace sam987883.Database.Requests
{
	/// <summary>
	/// Performs a SQL MERGE.
	/// When INSERT only uses INSERT statement.
	/// </summary>
	public sealed class BatchRequest
	{
		/// <summary>
		/// DELETE records WHEN NOT MATCHED BY SOURCE.
		/// </summary>
		public bool Delete { get; set; } = false;

		/// <summary>
		/// Batch input rows of data.
		/// </summary>
		public RowSet Input { get; set; } = new RowSet();

		/// <summary>
		/// Columns of the data to INSERT WHEN NOT MATCHED BY TARGET.
		/// </summary>
		public string[] Insert { get; set; } = new string[0];

		/// <summary>
		/// The columns to match on when performing a SQL MERGE operation for UPDATE. Not needed for INSERT only.
		/// </summary>
		public string[] On { get; set; } = new string[0];

		/// <summary>
		/// The columns of DELETED data to return, that is, the original data before being updated.
		/// </summary>
		public string[] OutputDeleted { get; set; } = new string[0];

		/// <summary>
		/// The columns of INSERTED data to return, that is, the new data after being updated.
		/// </summary>
		public string[] OutputInserted { get; set; } = new string[0];

		/// <summary>
		/// Target table to receive SQL MERGE operation.
		/// </summary>
		public string Table { get; set; } = string.Empty;

		/// <summary>
		/// Columns of the data to UPDATE WHEN MATCHED.
		/// </summary>
		public string[] Update { get; set; } = new string[0];
	}
}
