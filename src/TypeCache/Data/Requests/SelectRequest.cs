// Copyright (c) 2021 Samuel Abraham

using System.Collections.Generic;
using System.Text.Json.Serialization;
using TypeCache.Collections;
using TypeCache.Converters;
using TypeCache.Data.Converters;
using TypeCache.Data.Schema;
using TypeCache.Extensions;

namespace TypeCache.Data.Requests
{
	/// <summary>
	/// JSON: <code>{ "From": ..., "Output": [ ... ], "Parameters": [ ... ], "Where": "...", "OrderBy": [ ... ] }</code>
	/// SQL: <code>SELECT ... FROM ... WHERE ... ORDER BY ...;</code>
	/// </summary>
	public class SelectRequest
	{
		/// <summary>
		/// The data source name that contains the connection string and database provider to use.
		/// </summary>
		public string DataSource { get; set; } = Default.DATASOURCE;

		/// <summary>
		/// JSON: <code>"Table1" or "dbo.Table1" or "[Database1].dbo.[Table1]"</code>
		/// SQL: <code>[Database1]..[Table1] or [Database1].[dbo].[Table1]</code>
		/// </summary>
		public string From { get; set; } = string.Empty;

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
		/// JSON: <code>[ { "Ascending": "Column1" }, { "Descending": "Column2" }, ... ]</code>
		/// SQL: <code>ORDER BY [Column1] ASC, [Column2] DESC ...</code>
		/// </summary>
		[JsonConverter(typeof(SortJsonConverter))]
		public (string, Sort)[] OrderBy { get; set; } = Array<(string, Sort)>.Empty;

		/// <summary>
		/// JSON: <code>{ "First": 100, "After": 0 }</code>
		/// SQL: <code>OFFSET 0 ROWS FETCH NEXT 100 ROWS ONLY</code>
		/// </summary>
		public Pager? Pager { get; set; }

		/// <summary>
		/// JSON: <code>{ "ParameterName1": "ParameterValue1", "ParameterName2": null, "ParameterName3": 123 }</code>
		/// SQL:
		/// <code>
		/// SET @ParameterName1 = N'ParameterValue1';
		/// SET @ParameterName2 = NULL;
		/// SET @ParameterName3 = 123;
		/// </code>
		/// </summary>
		[JsonConverter(typeof(DictionaryJsonConverter))]
		public IDictionary<string, object?> Parameters { get; set; } = new Dictionary<string, object?>(Default.STRING_COMPARISON.ToStringComparer());

		/// <summary>
		/// Set internally- used to build SQL.
		/// </summary>
		public ObjectSchema? Schema { get; set; }

		/// <summary>
		/// JSON: <code>"Output": { "Alias 1": "NULLIF([Column1], 22)", "Alias 2": "INSERTED.ColumnName", "Alias 3": "DELETED.ColumnName" }</code>
		/// SQL: <code>OUTPUT NULLIF([Column1], 22) AS [Alias 1], INSERTED.[ColumnName] AS [Alias 2], DELETED.[ColumnName] AS [Alias 3]</code>
		/// </summary>
		[JsonConverter(typeof(OutputJsonConverter))]
		public IDictionary<string, string> Select { get; set; } = new Dictionary<string, string>(Default.STRING_COMPARISON.ToStringComparer());

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
}
