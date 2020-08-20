// Copyright (c) 2020 Samuel Abraham

using Sam987883.Database.Converters;
using Sam987883.Common.Models;
using System.Text.Json.Serialization;

namespace Sam987883.Database.Models
{
	/// <summary>
	/// JSON: <code>{ "From": ..., "Output": [ ... ], "Parameters": [ ... ], "Where": { ... }, "OrderBy": [ ... ] }</code>
	/// SQL: <code>SELECT ... FROM ... WHERE ... ORDER BY ...;</code>
	/// </summary>
	public class SelectRequest
	{
		/// <summary>
		/// JSON: <code>"Table1" or "dbo.Table1" or "[Database1].dbo.[Table1]"</code>
		/// SQL: <code>[Database1]..[Table1] or [Database1].[dbo].[Table1]</code>
		/// </summary>
		public string From { get; set; } = string.Empty;

		/// <summary>
		/// JSON: <code>
		///	{ "And":
		///	[
		///		{ "Or": [ { "Field": "Column1", "NotEqual": "Value2" }, { "Field": "Column2", "NotEqual": null } ] },
		///		{ "And": [ { "Field": "Column1", "Equal": "Value1" }, { "Field": "Column2", "Equal": null } ] },
		///		{ "Or": [ { "Field": "Column1", "StartsWith": "V" }, { "Field": "Column2", "MoreThan": 16 } ] }
		///	] }
		///	</code>
		/// SQL: <code>
		/// WHERE (([Column1] &lt;&gt; N'Value2' OR [Column2] IS NOT NULL)
		/// AND ([Column1] = N'Value1' AND [Column2] IS NULL)
		/// AND ([Column1] LIKE N'V%' OR [Column2] &gt; 16))
		/// </code>
		/// </summary>
		public ExpressionSet? Having { get; set; }

		/// <summary>
		/// JSON: <code>[ { "Ascending": "Column1" }, { "Descending": "Column2" }, ... ]</code>
		/// SQL: <code>ORDER BY [Column1] ASC, [Column2] DESC ...</code>
		/// </summary>
		public ColumnSort[] OrderBy { get; set; } = new ColumnSort[0];

		/// <summary>
		/// JSON: <code>{ "Column1": "Alias1", "Column2": "Alias2", "Column3": "Alias3", ... }</code>
		/// SQL: <code>SELECT [Column1] AS [Alias1], [Column2] AS [Alias2], [Column3] AS [Alias3], ...</code>
		/// </summary>
		public ColumnOutput[] Output { get; set; } = new ColumnOutput[0];

		/// <summary>
		/// JSON: <code>{ "ParameterName1": "ParameterValue1", "ParameterName2": null, "ParameterName3": 123 }</code>
		/// SQL:
		/// <code>
		/// SET @ParameterName1 = N'ParameterValue1';
		/// SET @ParameterName2 = NULL;
		/// SET @ParameterName3 = 123;
		/// </code>
		/// </summary>
		[JsonConverter(typeof(ParameterArrayJsonConverter))]
		public Parameter[] Parameters { get; set; } = new Parameter[0];

		/// <summary>
		/// JSON: <code>
		///	{ "And":
		///	[
		///		{ "Or": [ { "Field": "Column1", "NotEqual": "Value2" }, { "Field": "Column2", "NotEqual": null }, { "Field": "Column3", "Equal": [1, 2, 3] } ] },
		///		{ "And": [ { "Field": "Column1", "Equal": "Value1" }, { "Field": "Column2", "Equal": null } ] },
		///		{ "Or": [ { "Field": "Column1", "StartsWith": "V" }, { "Field": "Column2", "MoreThan": 16 }, { "Field": "Column3", "NotEqual": ["4", "5", "6"] } ] }
		///	] }
		///	</code>
		/// SQL: <code>
		/// WHERE (([Column1] &lt;&gt; N'Value2' OR [Column2] IS NOT NULL OR [Column3] IN (1, 2, 3))
		/// AND ([Column1] = N'Value1' AND [Column2] IS NULL)
		/// AND ([Column1] LIKE N'V%' OR [Column2] &gt; 16 OR [Column3] NOT IN (N'4', N'5', N'6')))
		/// </code>
		/// </summary>
		public ExpressionSet? Where { get; set; }
	}
}
