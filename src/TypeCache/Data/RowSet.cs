// Copyright (c) 2021 Samuel Abraham

using System.Text.Json.Serialization;
using TypeCache.Collections;
using TypeCache.Collections.Extensions;
using TypeCache.Data.Converters;

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
	public class RowSet
	{
		public string[] Columns { get; set; } = Array<string>.Empty;

		[JsonConverter(typeof(RowsJsonConverter))]
		public object?[][] Rows { get; set; } = Array<object[]>.Empty;

		public object? this[int row, string column] => this.Rows[row][this.Columns.ToIndex(column).FirstValue()!.Value];
	}
}
