// Copyright (c) 2021 Samuel Abraham

using TypeCache.Collections;

namespace TypeCache.Data.Requests;

/// <summary>
/// JSON: <code>{ "Into": ..., "Insert": [...]. "Output": [ ... ], ... }</code>
/// SQL: <code>INSERT INTO ... (...) OUPUT ... SELECT ...;</code>
/// </summary>
public class InsertRequest : SelectRequest
{
	/// <summary>
	/// JSON: <code>"Insert": [ "Column1", "Column2", "Column3" ]</code>
	/// SQL: <code>INSERT INTO ... ([Column1], Column2, [Column 3], ...)</code>
	/// </summary>
	public string[] Insert { get; set; } = Array<string>.Empty;

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
