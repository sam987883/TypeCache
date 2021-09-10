﻿// Copyright (c) 2021 Samuel Abraham

using System.Collections.Generic;
using System.Text.Json.Serialization;
using TypeCache.Collections;
using TypeCache.Converters;
using TypeCache.Data.Converters;
using TypeCache.Extensions;

namespace TypeCache.Data.Requests
{
	/// <summary>
	/// JSON: <code>{ "Into": ..., "Insert": [...]. "Output": [ ... ], ... }</code>
	/// SQL: <code>INSERT INTO ... (...) OUPUT ... SELECT ...;</code>
	/// </summary>
	public class InsertRequest
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
		/// JSON: <code>"Insert": [ "Column1", "Column2", "Column3" ]</code>
		/// SQL: <code>INSERT INTO ... ([Column1], Column2, [Column 3], ...)</code>
		/// </summary>
		public string[] Insert { get; set; } = Array<string>.Empty;

		/// <summary>
		/// JSON: <code>"Into": "Table"</code>
		/// SQL: <code>INSERT INTO [Database]..[Table]</code>
		/// </summary>
		public string Into { get; set; } = string.Empty;

		/// <summary>
		/// JSON: <code>[ { "Ascending": "Column1" }, { "Descending": "Column2" }, ... ]</code>
		/// SQL: <code>ORDER BY [Column1] ASC, [Column2] DESC ...</code>
		/// </summary>
		[JsonConverter(typeof(SortJsonConverter))]
		public (string, Sort)[] OrderBy { get; set; } = Array<(string, Sort)>.Empty;

		/// <summary>
		/// JSON: <code>"Output": { "Alias 1": "SQL Expression", "Alias 2": "INSERTED.ColumnName", "Alias 3": "DELETED.ColumnName" }</code>
		/// SQL: <code>OUTPUT SQL Expression AS [Alias 1], INSERTED.[ColumnName] AS [Alias 2], DELETED.[ColumnName] AS [Alias 3]</code>
		/// </summary>
		[JsonConverter(typeof(OutputJsonConverter))]
		public IDictionary<string, string> Output { get; set; } = new Dictionary<string, string>(Default.STRING_COMPARISON.ToStringComparer());

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