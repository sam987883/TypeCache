// Copyright (c) 2021 Samuel Abraham

using TypeCache.Collections;
using static TypeCache.Default;

namespace TypeCache.Data.Requests
{
	/// <summary>
	/// JSON: <code>{ "Into": ..., "Insert": [...]. "Output": [ ... ], ... }</code>
	/// SQL: <code>INSERT INTO ... (...) OUPUT ... SELECT ...;</code>
	/// </summary>
	public class InsertDataRequest
	{
		/// <summary>
		/// The data source name that contains the connection string and database provider to use.
		/// </summary>
		public string DataSource { get; set; } = DATASOURCE;

		/// <summary>
		/// Batch of records to insert.
		/// </summary>
		public RowSet Input { get; set; } = new RowSet();

		/// <summary>
		/// JSON: <code>"Into": "Table"</code>
		/// SQL: <code>INSERT INTO [Database]..[Table]</code>
		/// </summary>
		public string Into { get; set; } = string.Empty;

		/// <summary>
		/// JSON: <code>"Output": [ "NULLIF([Column1], 22) AS [Alias 1]", "INSERTED.ColumnName [Alias 2]", "DELETED.ColumnName" }</code>
		/// SQL: <code>OUTPUT NULLIF([Column1], 22) AS [Alias 1], INSERTED.[ColumnName] [Alias 2], DELETED.[ColumnName]</code>
		/// </summary>
		public string[] Output { get; set; } = Array<string>.Empty;
	}
}
