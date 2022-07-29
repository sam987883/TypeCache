// Copyright (c) 2021 Samuel Abraham

using System.Text;
using TypeCache.Collections;
using TypeCache.Data.Extensions;

namespace TypeCache.Data.Domain;

/// <summary>
/// JSON: <code>{ "From": "Table1", "Where": { ... }, "Output": [ ... ] }</code>
/// SQL: <code>DELETE FROM ... OUTPUT ... WHERE ...;</code>
/// </summary>
public class DeleteCommand : Command
{
	/// <summary>
	/// JSON: <c>"Output": [ "NULLIF([Column1], 22) AS [Alias 1]", "INSERTED.ColumnName [Alias 2]", "DELETED.ColumnName" ]</c><br/>
	/// SQL: <c>OUTPUT NULLIF([Column1], 22) AS [Alias 1], INSERTED.[ColumnName] [Alias 2], DELETED.[ColumnName]</c>
	/// </summary>
	public string[] Output { get; set; } = Array<string>.Empty;

	/// <summary>
	/// JSON: <c>"Table": "Table1"</c><br/>
	/// SQL: <c>DELETE FROM [Database1]..[Table1]</c>
	/// </summary>
	public string Table { get; set; } = string.Empty;

	/// <summary>
	/// JSON: <code>
	/// "(([Column1] &lt;&gt; N'Value2' OR [Column2] IS NOT NULL OR [Column3] IN (1, 2, 3))<br/>
	/// AND ([Column1] = N'Value1' AND [Column2] IS NULL)<br/>
	/// AND ([Column1] LIKE N'V%' OR [Column2] &gt; 16 OR [Column3] NOT IN (N'4', N'5', N'6')))"
	/// </code>
	/// SQL: <code>
	/// WHERE (([Column1] &lt;&gt; N'Value2' OR [Column2] IS NOT NULL OR [Column3] IN (1, 2, 3))<br/>
	/// AND ([Column1] = N'Value1' AND [Column2] IS NULL)<br/>
	/// AND ([Column1] LIKE N'V%' OR [Column2] &gt; 16 OR [Column3] NOT IN (N'4', N'5', N'6')))
	/// </code>
	/// </summary>
	public string? Where { get; set; }

	/// <inheritdoc/>
	public override string ToSQL()
		=> new StringBuilder()
			.AppendSQL("DELETE", this.Table)
			.AppendSQL("OUTPUT", this.Output)
			.AppendSQL("WHERE", this.Where)
			.AppendStatementEndSQL()
			.ToString();
}
