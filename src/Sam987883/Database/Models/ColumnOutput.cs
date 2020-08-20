// Copyright (c) 2020 Samuel Abraham

using Sam987883.Database.Models;

namespace Sam987883.Common.Models
{
	/// <summary>
	/// JSON: <code>{ "Column": "Column1", "Alias": "Alias 1", "NullIf": 22 }</code>
	/// SQL: <code>NULLIF([Column1], 22) AS [Alias 1]</code>
	/// </summary>
	public struct ColumnOutput
	{
		/// <summary>
		/// [Column1]
		/// </summary>
		public string? Column { get; set; }

		/// <summary>
		/// NULLIF(..., N'Column or expression')
		/// </summary>
		public string? NullIf { get; set; }

		/// <summary>
		/// ISNULL(..., N'Column or expression')
		/// COALESCE(..., [Column1], N'expression', N'expression2', [Column2], ...)
		/// </summary>
		public string[]? Coalesce { get; set; }

		/// <summary>
		/// AVG, COUNT, MIN, MAX, ROW_NUMBER, STDEV, VAR
		/// </summary>
		public AggregateFunction Aggregate { get; set; }

		/// <summary>
		/// ... OVER (PARTITION BY [Column1])
		/// </summary>
		public string? PartitionBy { get; set; }

		/// <summary>
		/// ... OVER (ORDER BY [Column1] ASC|DESC)
		/// </summary>
		public ColumnSort? OrderBy { get; set; }

		/// <summary>
		/// Format specifier such as that for string.Format.
		/// FORMAT(..., "c")
		/// </summary>
		public string? Format { get; set; }

		/// <summary>
		/// [Alias 1]
		/// </summary>
		public string? Alias { get; set; }
	}
}
