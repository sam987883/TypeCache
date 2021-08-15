// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using TypeCache.Converters;
using TypeCache.Data.Converters;

namespace TypeCache.Data.Requests
{
	/// <summary>
	/// JSON: <code>{ "Table": "Table1", "Set": [ ... ], "OutputDeleted": [ ... ], "OutputInserted": [ ... ], "Where": { ... } }</code>
	/// SQL: <code>UPDATE ... SET ... OUTPUT ... WHERE ...;</code>
	/// </summary>
	public class UpdateRequest
	{
		/// <summary>
		/// The data source name that contains the connection string and database provider to use.
		/// </summary>
		public string DataSource { get; set; } = "Default";

		/// <summary>
		/// JSON: <code>"Output": { "Alias 1": "NULLIF([Column1], 22)", "Alias 2": "INSERTED.ColumnName", "Alias 3": "DELETED.ColumnName" }</code>
		/// SQL: <code>OUTPUT NULLIF([Column1], 22) AS [Alias 1], INSERTED.[ColumnName] AS [Alias 2], DELETED.[ColumnName] AS [Alias 3]</code>
		/// </summary>
		[JsonConverter(typeof(OutputJsonConverter))]
		public IDictionary<string, string> Output { get; set; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

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
		public IDictionary<string, object?> Parameters { get; set; } = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);

		/// <summary>
		/// JSON: <code>"Table1"</code>
		/// SQL: <code>UPDATE [Database1]..[Table1]</code>
		/// </summary>
		public string Table { get; set; } = string.Empty;

		/// <summary>
		/// JSON: <code>[ { "Column1": "Value1" }, { "Column2": NULL }, { "Column3": false } ]</code>
		/// SQL: <code>SET [Column1] = N'Value1', [Column2] = NULL, [Column3] = 0</code>
		/// </summary>
		[JsonConverter(typeof(DictionaryJsonConverter))]
		public IDictionary<string, object?> Set { get; set; } = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);

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
