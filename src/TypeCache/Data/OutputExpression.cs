// Copyright (c) 2021 Samuel Abraham

namespace TypeCache.Data
{
	/// <summary>
	/// JSON: <code>{ "Alias 1": "SQL Expression", "Alias 2": "ColumnName" }</code>
	/// SQL: <code>NULLIF([Column1], 22) AS [Alias 1]</code>
	/// </summary>
	public record OutputExpression(string Expression, string? As);
}
