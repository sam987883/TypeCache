// Copyright (c) 2021 Samuel Abraham

using TypeCache.Collections;
using static TypeCache.Default;

namespace TypeCache.Data.Requests;

/// <summary>
/// JSON: <code>{ "From": "Table1", "Input": { ... }, "Output": [ ... ] }</code>
/// SQL: <code>DELETE FROM ... VALUES ... OUTPUT ...;</code>
/// </summary>
public class DeleteDataRequest
{
	/// <summary>
	/// The data source name that contains the connection string and database provider to use.
	/// </summary>
	public string DataSource { get; set; } = DATASOURCE;

	/// <summary>
	/// JSON: <code>"From": "Table1"</code>
	/// SQL: <code>DELETE FROM [Database1]..[Table1]</code>
	/// </summary>
	public string From { get; set; } = string.Empty;

	/// <summary>
	/// Batch of records to delete by Primary Key(s).
	/// </summary>
	public RowSet Input { get; set; } = new RowSet();

	/// <summary>
	/// JSON: <code>"Output": [ "NULLIF([Column1], 22) AS [Alias 1]", "INSERTED.ColumnName [Alias 2]", "DELETED.ColumnName" }</code>
	/// SQL: <code>OUTPUT NULLIF([Column1], 22) AS [Alias 1], INSERTED.[ColumnName] [Alias 2], DELETED.[ColumnName]</code>
	/// </summary>
	public string[] Output { get; set; } = Array<string>.Empty;
}
