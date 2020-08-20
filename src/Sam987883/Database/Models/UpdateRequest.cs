// Copyright (c) 2020 Samuel Abraham

using Sam987883.Common.Models;

namespace Sam987883.Database.Models
{
	/// <summary>
	/// JSON: <code>{ "Table": "Table1", "Set": [ ... ], "OutputDeleted": [ ... ], "OutputInserted": [ ... ], "Where": { ... } }</code>
	/// SQL: <code>UPDATE ... SET ... OUTPUT ... WHERE ...;</code>
	/// </summary>
	public class UpdateRequest
	{
		/// <summary>
		/// JSON: <code>[ "Column1", "Column2", "Column3" ]</code>
		/// SQL: <code>OUTPUT DELETED.[Column1], DELETED.[Column2], DELETED.[Column3]</code>
		/// </summary>
		public string[] OutputDeleted { get; set; } = new string[0];

		/// <summary>
		/// JSON: <code>[ "Column1", "Column2", "Column3" ]</code>
		/// SQL: <code>OUTPUT INSERTED.[Column1], INSERTED.[Column2], INSERTED.[Column3]</code>
		/// </summary>
		public string[] OutputInserted { get; set; } = new string[0];

		/// <summary>
		/// JSON: <code>"Table1"</code>
		/// SQL: <code>UPDATE [Database1]..[Table1]</code>
		/// </summary>
		public string Table { get; set; } = string.Empty;

		/// <summary>
		/// JSON: <code>[ { "Column1": "Value1" }, { "Column2": NULL }, { "Column3": false } ]</code>
		/// SQL: <code>SET [Column1] = N'Value1', [Column2] = NULL, [Column3] = 0</code>
		/// </summary>
		public ColumnValue[] Set { get; set; } = new ColumnValue[0];

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
		public ExpressionSet Where { get; set; } = new ExpressionSet();
	}
}
