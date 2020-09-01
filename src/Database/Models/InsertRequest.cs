// Copyright (c) 2020 Samuel Abraham

using Sam987883.Common.Models;

namespace Sam987883.Database.Models
{
	/// <summary>
	/// JSON: <code>{ "Into": ..., "Insert": [...]. "Output": [ ... ], ... }</code>
	/// SQL: <code>INSERT INTO ... (...) OUPUT ... SELECT ...;</code>
	/// </summary>
	public class InsertRequest : SelectRequest
	{
		/// <summary>
		/// JSON: <code>"Into": "Table"</code>
		/// SQL: <code>INSERT INTO [Database]..[Table]</code>
		/// </summary>
		public string Into { get; set; } = string.Empty;

		/// <summary>
		/// JSON: <code>"Insert": [ "Column1", "Column2", "Column3" ]</code>
		/// SQL: <code>INSERT INTO ... ([Column1], Column2, [Column 3], ...)</code>
		/// </summary>
		public string[] Insert { get; set; } = new string[0];

		/// <summary>
		/// JSON: <code>"Output": { "Alias 1": "SQL Expression", "Alias 2": "INSERTED.ColumnName", "Alias 3": "DELETED.ColumnName" }</code>
		/// SQL: <code>OUTPUT SQL Expression AS [Alias 1], INSERTED.[ColumnName] AS [Alias 2], DELETED.[ColumnName] AS [Alias 3]</code>
		/// </summary>
		public OutputExpression[]? Output { get; set; }
	}
}
