// Copyright (c) 2021 Samuel Abraham

using System.Text;
using TypeCache.Data.Extensions;
using TypeCache.Extensions;
using static System.FormattableString;

namespace TypeCache.Data.Domain;

/// <summary>
/// JSON: <code>{ "From": ..., "Parameters": [ ... ], "Where": "..." }</code>
/// SQL: <code>SELECT COUNT(1) FROM ... WHERE ...;</code>
/// </summary>
public class CountCommand : Command
{
	/// <summary>
	/// SQL: <code>SELECT COUNT_BIG(DISTINCT [...])</code>
	/// </summary>
	public string Distinct { get; set; } = string.Empty;

	/// <summary>
	/// JSON: <c>"Table1" or "dbo.Table1" or "Database1.dbo.Table1"</c><br/>
	/// SQL: <c>[Database1]..[Table1] or [Database1].[dbo].[Table1]</c>
	/// </summary>
	public string Table { get; set; } = string.Empty;

	/// <summary>
	/// JSON: <c>"WITH(NOLOCK)"</c><br/>
	/// SQL: <c>FROM [Database1]..[Table1] WITH(NOLOCK)</c>
	/// </summary>
	public string TableHints { get; set; } = "WITH(NOLOCK)";

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
			.AppendSQL("SELECT", this.Distinct.IsNotBlank() ? Invariant($"COUNT_BIG(DISTINCT {this.Distinct.EscapeIdentifier()})") : "COUNT_BIG(*)")
			.AppendSQL("FROM", this.Table, this.TableHints ?? "WITH(NOLOCK)")
			.AppendSQL("WHERE", this.Where)
			.AppendStatementEndSQL()
			.ToString();
}
