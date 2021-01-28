// Copyright (c) 2021 Samuel Abraham

using System.Diagnostics.CodeAnalysis;

namespace TypeCache.Data
{
	/// <summary>
	/// JSON: <code>{ "Alias 1": "SQL Expression", "Alias 2": "ColumnName" }</code>
	/// SQL: <code>NULLIF([Column1], 22) AS [Alias 1]</code>
	/// </summary>
	public readonly struct OutputExpression
	{
		/// <summary>
		/// SQL Expression: ie. <code>COALESCE(NULLIF(CONCAT([Column1], ', ', [Column2]), N'AAA'), [Column3], N'BBB')</code>
		/// </summary>
		[NotNull]
		public string Expression { get; init; }

		/// <summary>
		/// AS [Alias 1]
		/// </summary>
		public string? As { get; init; }
	}
}
