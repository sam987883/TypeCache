// Copyright (c) 2021 Samuel Abraham

using System;
using TypeCache.Collections.Extensions;

namespace TypeCache.Data
{
	/// <summary>
	/// <code>
	/// {<br />
	///		"Columns": [ "Column1", "Column2", "Column3", ... ],<br />
	///		"Rows": [ [ "Data", 123, null ], [ ... ], ... ]<br />
	/// }
	/// </code>
	/// </summary>
	public record RowSet(string[] Columns, object?[][] Rows)
	{
		public static RowSet Empty { get; } = new RowSet(Array.Empty<string>(), Array.Empty<object[]>());

		public object? this[int row, string column] => this.Rows[row][this.Columns.ToIndex(column, false).FirstValue()!.Value];
	}
}
