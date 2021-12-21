// Copyright (c) 2021 Samuel Abraham

using System.Collections.Generic;
using System.Text.Json.Serialization;
using TypeCache.Collections;
using TypeCache.Converters;
using TypeCache.Extensions;
using static TypeCache.Default;

namespace TypeCache.Data.Requests;

/// <summary>
/// JSON: <code>{ "From": "Table1", "Where": { ... }, "Output": [ ... ] }</code>
/// SQL: <code>DELETE FROM ... OUTPUT ... WHERE ...;</code>
/// </summary>
public class DeleteRequest
{
	/// <summary>
	/// The data source name that contains the connection string and database provider to use.
	/// </summary>
	public string DataSource { get; set; } = DATASOURCE;

	/// <summary>
	/// JSON: <code>"From": "Table1"</code>
	/// SQL: <code>DELETE FROM [Database1]..[Table1]</code>
	/// </summary>
	public string From { get; set; } = string.Empty;

	/// <summary>
	/// JSON: <code>"Output": [ "NULLIF([Column1], 22) AS [Alias 1]", "INSERTED.ColumnName [Alias 2]", "DELETED.ColumnName" }</code>
	/// SQL: <code>OUTPUT NULLIF([Column1], 22) AS [Alias 1], INSERTED.[ColumnName] [Alias 2], DELETED.[ColumnName]</code>
	/// </summary>
	public string[] Output { get; set; } = Array<string>.Empty;

	/// <summary>
	/// JSON: <code>{ "ParameterName1": "ParameterValue1", "ParameterName2": null, "ParameterName3": 123 }</code>
	/// SQL:
	/// <code>
	/// SET @ParameterName1 = N'ParameterValue1';<br />
	/// SET @ParameterName2 = NULL;<br />
	/// SET @ParameterName3 = 123;
	/// </code>
	/// </summary>
	[JsonConverter(typeof(DictionaryJsonConverter))]
	public IDictionary<string, object?> Parameters { get; set; } = new Dictionary<string, object?>(STRING_COMPARISON.ToStringComparer());

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
	public string? Where { get; set; }
}
