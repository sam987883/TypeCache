// Copyright (c) 2021 Samuel Abraham

namespace TypeCache.Data;

/// <summary>
/// <code>SELECT ... FROM ... WHERE ... ORDER BY ...;</code>
/// </summary>
public class SelectQuery
{
	/// <summary>
	/// <c>SELECT <i><b>DISTINCT</b></i></c>
	/// </summary>
	public bool Distinct { get; set; }

	/// <summary>
	/// <c>SELECT DISTINCT ON <i><b>("AccountType") AS "Type"</b></i></c>
	/// </summary>
	/// <remarks><i><b>* * * Postgre SQL only * * *</b></i></remarks>
	public string? DistinctOn { get; set; }

	/// <summary>
	/// <c>FETCH NEXT 100 ROWS ONLY</c>
	/// </summary>
	public uint Fetch { get; set; }

	/// <summary>
	/// <c>FROM [Table]</c>
	/// </summary>
	public DatabaseObject From { get; set; }

	/// <summary>
	/// <c>GROUP BY [Column1], TRIM([Column 2]), Column3</c>
	/// </summary>
	public string[]? GroupBy { get; set; }

	/// <summary>
	/// <c>GROUP BY {CUBE|ROLLUP} ([Column1], Column3)</c>
	/// </summary>
	/// <remarks><i><b>* * * Postgre SQL only * * *</b></i></remarks>
	public GroupBy GroupByOption { get; set; }

	/// <summary>
	/// <code>
	/// WHERE (([Column1] &lt;&gt; N'Value2' OR [Column2] IS NOT NULL OR [Column3] IN (1, 2, 3))
	/// AND ([Column1] = N'Value1' AND [Column2] IS NULL)
	/// AND ([Column1] LIKE N'V%' OR [Column2] &gt; 16 OR [Column3] NOT IN (N'4', N'5', N'6')))
	/// </code>
	/// </summary>
	public string? Having { get; set; }

	/// <summary>
	/// <c>OFFSET 1000 ROWS</c>
	/// </summary>
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
	public string? TableHints { get; set; }

	/// <summary>
	/// <c>SELECT TOP (100)</c>
	/// </summary>
	/// <remarks><i><b>* * * SQL Server only * * *</b></i></remarks>
	public string? Top { get; set; }

	/// <summary>
	/// <code>
	/// WHERE (([Column1] &lt;&gt; N'Value2' OR [Column2] IS NOT NULL OR [Column3] IN (1, 2, 3))<br/>
	/// AND ([Column1] = N'Value1' AND [Column2] IS NULL)<br/>
	/// AND ([Column1] LIKE N'V%' OR [Column2] &gt; 16 OR [Column3] NOT IN (N'4', N'5', N'6')))
	/// </code>
	/// </summary>
	public string? Where { get; set; }
}
