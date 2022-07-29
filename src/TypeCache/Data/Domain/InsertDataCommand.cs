// Copyright (c) 2021 Samuel Abraham

using System.Text;
using TypeCache.Collections;
using TypeCache.Data.Extensions;

namespace TypeCache.Data.Domain;

/// <summary>
/// JSON: <code>{ "Into": ..., "Insert": [...]. "Output": [ ... ], ... }</code>
/// SQL: <code>INSERT INTO ... (...) OUPUT ... SELECT ...;</code>
/// </summary>
public class InsertDataCommand<T> : Command
{
	/// <summary>
	/// The columns being inserted.
	/// </summary>
	public string[] Columns { get; set; } = Array<string>.Empty;

	/// <summary>
	/// Batch of records to insert.
	/// </summary>
	public T[] Input { get; set; } = Array<T>.Empty;

	/// <summary>
	/// JSON: <c>"Output": [ "NULLIF([Column1], 22) AS [Alias 1]", "INSERTED.ColumnName [Alias 2]", "DELETED.ColumnName" ]</c><br/>
	/// SQL: <c>OUTPUT NULLIF([Column1], 22) AS [Alias 1], INSERTED.[ColumnName] [Alias 2], DELETED.[ColumnName]</c>
	/// </summary>
	public string[] Output { get; set; } = Array<string>.Empty;

	/// <summary>
	/// JSON: <code>"Into": "Table"</code>
	/// SQL: <code>INSERT INTO [Database]..[Table]</code>
	/// </summary>
	public string Table { get; set; } = string.Empty;

	/// <inheritdoc/>
	public override string ToSQL()
		=> new StringBuilder()
			.AppendInsertSQL(this.Table, this.Columns)
			.AppendSQL("OUTPUT", this.Output)
			.AppendValuesSQL(this.Columns, this.Input)
			.AppendStatementEndSQL()
			.ToString();
}
