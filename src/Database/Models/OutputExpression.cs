// Copyright (c) 2020 Samuel Abraham

namespace Sam987883.Common.Models
{
	/// <summary>
	/// JSON: <code>{ "Alias 1": "SQL Expression", "Alias 2": "ColumnName" }</code>
	/// SQL: <code>NULLIF([Column1], 22) AS [Alias 1]</code>
	/// </summary>
	public struct OutputExpression
	{
		/// <summary>
		/// SQL Expression: ie. <code>COALESCE(NULLIF(CONCAT([Column1], ', ', [Column2]), N'AAA'), [Column3], N'BBB')</code>
		/// </summary>
		public string Expression { get; set; }

		/// <summary>
		/// AS [Alias 1]
		/// </summary>
		public string? As { get; set; }
	}
}
