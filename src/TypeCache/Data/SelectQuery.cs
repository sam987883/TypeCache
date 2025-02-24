// Copyright (c) 2021 Samuel Abraham

using System.ComponentModel;

namespace TypeCache.Data;

/// <summary>
/// <code>SELECT ... FROM ... WHERE ... ORDER BY ...;</code>
/// </summary>
public class SelectQuery
{
	/// <summary>
	/// <c>SELECT <i><b>DISTINCT</b></i></c>
	/// </summary>
	[DefaultValue(false)]
	public bool Distinct { get; set; }

	/// <summary>
	/// <c>SELECT DISTINCT ON <i><b>("AccountType") AS "Type"</b></i></c>
	/// </summary>
	/// <remarks><i><b>* * * Postgre SQL only * * *</b></i></remarks>
	[DefaultValue(null)]
	public string? DistinctOn { get; set; }

	/// <summary>
	/// <c>FETCH NEXT <i><b>100</b></i> ROWS ONLY</c>
	/// </summary>
	[DefaultValue(0U)]
	public uint Fetch { get; set; }

	/// <summary>
	/// <c>FROM Table{ AS Alias}</c>
	/// </summary>
	[DefaultValue("")]
	public string? From { get; set; }

	/// <summary>
	/// <c>GROUP BY [Column1], TRIM([Column 2]), Column3, GROUPING SETS(CUBE(Column4), ROLLUP(Column5))</c>
	/// </summary>
	[DefaultValue("")]
	public string? GroupBy { get; set; }

	/// <summary>
	/// <code>
	/// WHERE (([Column1] &lt;&gt; N'Value2' OR [Column2] IS NOT NULL OR [Column3] IN (1, 2, 3))
	/// AND ([Column1] = N'Value1' AND [Column2] IS NULL)
	/// AND ([Column1] LIKE N'V%' OR [Column2] &gt; 16 OR [Column3] NOT IN (N'4', N'5', N'6')))
	/// </code>
	/// </summary>
	[DefaultValue("")]
	public string? Having { get; set; }

	/// <summary>
	/// <c>OFFSET 1000 ROWS</c>
	/// </summary>
	[DefaultValue(0U)]
	public uint Offset { get; set; }

	/// <summary>
	/// <c>ORDER BY [Column1] ASC, TRIM([Column 2]) DESC, Column3</c>
	/// </summary>
	public string[]? OrderBy { get; set; }

	/// <summary>
	/// <c>SELECT NULLIF([Column1], 22) AS [Alias 1], TRIM([Column 2]) [Alias 2], Column3</c>
	/// </summary>
	public string[]? Select { get; set; }

	/// <summary>
	/// <c>FROM [Database1]..[Table1] WITH(NOLOCK)</c>
	/// </summary>
	/// <remarks><i><b>* * * SQL Server only * * *</b></i></remarks>
	[DefaultValue(null)]
	public string? TableHints { get; set; }

	/// <summary>
	/// <c>SELECT TOP <i><b>100</b></i></c><br/>
	/// </summary>
	/// <remarks><i><b>* * * SQL Server only * * *</b></i></remarks>
	[DefaultValue(null)]
	public uint? Top { get; set; }

	/// <summary>
	/// <c>SELECT TOP 10 <i><b>PERCENT</b></i></c>
	/// </summary>
	/// <remarks><i><b>* * * SQL Server only * * *</b></i></remarks>
	[DefaultValue(false)]
	public bool TopPercent { get; set; }

	/// <summary>
	/// <code>
	/// WHERE (([Column1] &lt;&gt; N'Value2' OR [Column2] IS NOT NULL OR [Column3] IN (1, 2, 3))<br/>
	/// AND ([Column1] = N'Value1' AND [Column2] IS NULL)<br/>
	/// AND ([Column1] LIKE N'V%' OR [Column2] &gt; 16 OR [Column3] NOT IN (N'4', N'5', N'6')))
	/// </code>
	/// </summary>
	[DefaultValue("")]
	public string? Where { get; set; }
}
