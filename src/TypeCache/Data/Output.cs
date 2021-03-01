// Copyright (c) 2021 Samuel Abraham

namespace TypeCache.Data
{
	/// <summary>
	/// JSON: <code>
	/// {<br />
	///		"Deleted": { "Columns": [ "Column1", "Column2", "Column3", ... ], "Rows": [ [ "Data", 123, null, ... ], [ ... ], ... ] },<br />
	///		"Inserted": { "Columns": [ "Column1", "Column2", "Column3", ... ], "Rows": [ [ "Data", 123, null, ... ], [ ... ], ... ] }<br />
	/// }
	/// </code>
	/// SQL: <code>
	/// OUTPUT DELETED.[Column1], DELETED.[Column2], DELETED.[Column3] ...<br />
	/// OUTPUT INSERTED.[Column1], INSERTED.[Column2], INSERTED.[Column3] ...
	/// </code>
	/// </summary>
	public record Output(RowSet Deleted, RowSet Inserted);
}
