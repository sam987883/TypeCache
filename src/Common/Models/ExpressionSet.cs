// Copyright (c) 2020 Samuel Abraham

using Sam987883.Common.Converters;
using System.Text.Json.Serialization;

namespace Sam987883.Common.Models
{
	/// <summary>
	/// JSON: <code>{ "And": [ { "Or": [ ... ] }, { "AND": [ ... ] }, { "Or": [ ... ] } ] }</code>
	/// SQL: <code>( ... OR ... ) AND ( ... AND ... ) AND ( ... OR ... )</code>
	/// </summary>
	[JsonConverter(typeof(ExpressionSetJsonConverter))]
	public class ExpressionSet
	{
		/// <summary>
		/// JSON: <code>"And" or "Or"</code>
		/// SQL: <code>AND or OR</code>
		/// </summary>
		public LogicalOperator Operator { get; set; }

		/// <summary>
		/// JSON: <code>[{ "Field": "Column1", "Equal": "Value1" }, { "Field": "Column2", "NotEqual": null }, { "Field": "Column3", "Equal": [1, 2, 3] } ]</code>
		/// SQL: <code>([Column1] = N'Value1' AND [Column2] IS NOT NULL AND [Column3] IN (1, 2, 3))</code>
		/// </summary>
		public Expression[] Expressions { get; set; } = new Expression[0];

		/// <summary>
		/// JSON: <code>{ "And": [ { "Or": [ ... ] }, { "AND": [ ... ] }, { "Or": [ ... ] } ] }</code>
		/// SQL: <code>( ... OR ... ) AND ( ... AND ... ) AND ( ... OR ... )</code>
		/// </summary>
		public ExpressionSet[] ExpressionSets { get; set; } = new ExpressionSet[0];
	}
}
