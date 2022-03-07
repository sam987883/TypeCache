// Copyright (c) 2021 Samuel Abraham

using TypeCache.Collections;
using static TypeCache.Default;

namespace TypeCache.Data.Requests;

/// <summary>
/// JSON: <code>{ "Table": "Table1", "Input": { ... }, "OutputDeleted": [ ... ], "OutputInserted": [ ... ] }</code>
/// SQL: <code>UPDATE ... OUTPUT ... VALUES ...;</code>
/// </summary>
public class UpdateDataRequest
{
	/// <summary>
	/// The data source name that contains the connection string and database provider to use.
	/// </summary>
	public string DataSource { get; set; } = DATASOURCE;

	/// <summary>
	/// Batch of records to update based on Primary Key(s).
	/// </summary>
	public RowSet Input { get; set; } = new RowSet();

	/// <summary>
	/// JSON: <code>On: [ "ID1", "[ID2]" ]</code>
	/// SQL: <code>ON i.[ID1] = x.[ID1] AND i.[ID2] = x.[ID2]</code>
	/// </summary>
	public string[] On { get; set; } = Array<string>.Empty;

	/// <summary>
	/// JSON: <code>"Output": [ "NULLIF([Column1], 22) AS [Alias 1]", "INSERTED.ColumnName [Alias 2]", "DELETED.ColumnName" }</code>
	/// SQL: <code>OUTPUT NULLIF([Column1], 22) AS [Alias 1], INSERTED.[ColumnName] [Alias 2], DELETED.[ColumnName]</code>
	/// </summary>
	public string[] Output { get; set; } = Array<string>.Empty;

	/// <summary>
	/// JSON: <code>"Table1"</code>
	/// SQL: <code>UPDATE [Database1]..[Table1]</code>
	/// </summary>
	public string Table { get; set; } = string.Empty;

	/// <summary>
	/// JSON: <code>"WITH(UPDLOCK)"</code>
	/// SQL: <code>UPDATE [Database1]..[Table1] WITH(UPDLOCK)</code>
	/// </summary>
	public string TableHints { get; set; } = "WITH(UPDLOCK)";
}
