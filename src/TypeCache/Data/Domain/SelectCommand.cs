// Copyright (c) 2021 Samuel Abraham

using System.Data;
using System.Text;
using TypeCache.Collections;
using TypeCache.Collections.Extensions;
using TypeCache.Data.Extensions;
using TypeCache.Extensions;
using static System.FormattableString;

namespace TypeCache.Data.Domain;

/// <summary>
/// JSON: <code>{ "From": ..., "Output": [ ... ], "Parameters": [ ... ], "Where": "...", "OrderBy": [ ... ] }</code>
/// SQL: <code>SELECT ... FROM ... WHERE ... ORDER BY ...;</code>
/// </summary>
public class SelectCommand : Command
{
	/// <summary>
	/// SQL: <c>SELECT DISTINCT</c>
	/// </summary>
	public bool Distinct { get; set; }

	/// <summary>
	/// JSON: <c>"Table1" or "dbo.Table1" or "[Database1].dbo.[Table1]"</c><br/>
	/// SQL: <c>[Database1]..[Table1] or [Database1].[dbo].[Table1]</c>
	/// </summary>
	public string From { get; set; } = string.Empty;

	/// <summary>
	/// JSON: <c>"GroupBy": [ "[Column1]", "TRIM([Column 2])", "Column3" ]</c><br/>
	/// SQL: <c>GROUP BY [Column1], TRIM([Column 2]), Column3</c>
	/// </summary>
	public string[]? GroupBy { get; set; }

	/// <summary>
	/// JSON: <code>
	/// "(([Column1] &lt;&gt; N'Value2' OR [Column2] IS NOT NULL OR [Column3] IN (1, 2, 3))
	/// AND ([Column1] = N'Value1' AND [Column2] IS NULL)
	/// AND ([Column1] LIKE N'V%' OR [Column2] &gt; 16 OR [Column3] NOT IN (N'4', N'5', N'6')))"
	/// </code>
	/// SQL: <code>
	/// WHERE (([Column1] &lt;&gt; N'Value2' OR [Column2] IS NOT NULL OR [Column3] IN (1, 2, 3))
	/// AND ([Column1] = N'Value1' AND [Column2] IS NULL)
	/// AND ([Column1] LIKE N'V%' OR [Column2] &gt; 16 OR [Column3] NOT IN (N'4', N'5', N'6')))
	/// </code>
	/// </summary>
	public string? Having { get; set; }

	/// <summary>
	/// JSON: <c>"OrderBy": [ "[Column1] ASC", "TRIM([Column 2]) DESC", "Column3" ]</c><br/>
	/// SQL: <c>ORDER BY [Column1] ASC, TRIM([Column 2]) DESC, Column3</c>
	/// </summary>
	public string[]? OrderBy { get; set; }

	/// <summary>
	/// JSON: <c>{ "First": 100, "After": 0 }</c><br/>
	/// SQL: <c>OFFSET 0 ROWS FETCH NEXT 100 ROWS ONLY</c>
	/// </summary>
	public Pager? Pager { get; set; }

	/// <summary>
	/// SQL: <c>SELECT TOP (100) PERCENT</c>
	/// </summary>
	public bool Percent { get; set; }

	/// <summary>
	/// JSON: <c>"Select": [ "NULLIF([Column1], 22) AS [Alias 1]", "TRIM([Column 2]) [Alias 2]", "Column3" ]</c><br/>
	/// SQL: <c>SELECT NULLIF([Column1], 22) AS [Alias 1], TRIM([Column 2]) [Alias 2], Column3</c>
	/// </summary>
	public string[] Select { get; set; } = Array<string>.Empty;

	/// <summary>
	/// JSON: <c>"WITH(NOLOCK)"</c><br/>
	/// SQL: <c>FROM [Database1]..[Table1] WITH(NOLOCK)</c>
	/// </summary>
	public string TableHints { get; set; } = string.Empty;

	/// <summary>
	/// SQL: <c>SELECT TOP (100)</c>
	/// </summary>
	public uint Top { get; set; }

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

	/// <summary>
	/// SQL: <c>SELECT TOP (100) WITH TIES</c>
	/// </summary>
	public bool WithTies { get; set; }

	/// <inheritdoc/>
	public override string ToSQL()
	{
		var sqlBuilder = new StringBuilder();

		if (this.Top == 0 && !(this.Pager?.First > 0))
		{
			sqlBuilder
				.AppendSQL("SELECT", "COUNT(*) AS [RowCount]")
				.AppendSQL("FROM", this.From, this.TableHints)
				.AppendSQL("WHERE", this.Where)
				.AppendStatementEndSQL()
				.AppendLine();
		}

		sqlBuilder.Append("SELECT");
		if (this.Distinct)
			sqlBuilder.Append(" DISTINCT");

		if (this.Top > 0)
		{
			sqlBuilder.Append(Invariant($" TOP ({this.Top})"));
			if (this.Percent)
				sqlBuilder.Append(" PERCENT");
			if (this.WithTies)
				sqlBuilder.Append(" WITH TIES");
		}

		sqlBuilder
			.AppendSQL(string.Empty, this.Select.Any() ? this.Select : new[] { "*" })
			.AppendSQL("FROM", this.From, this.TableHints)
			.AppendSQL("WHERE", this.Where)
			.AppendSQL("GROUP BY", this.GroupBy)
			.AppendSQL("HAVING", this.Having)
			.AppendSQL("ORDER BY", this.OrderBy)
			.AppendPagerSQL(this.Pager)
			.AppendStatementEndSQL();

		if (this.Pager.HasValue)
		{
			sqlBuilder.AppendLine()
				.AppendSQL("SELECT", "@RowCount = COUNT_BIG(1)")
				.AppendSQL("FROM", this.From, this.TableHints)
				.AppendSQL("WHERE", this.Where)
				.AppendStatementEndSQL();
		}

		return sqlBuilder.ToString();
	}
}
