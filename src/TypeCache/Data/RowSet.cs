// Copyright (c) 2021 Samuel Abraham

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
	public readonly struct RowSet
	{
		/// <summary>
		/// [ "Column1", "Column2", "Column3", ... ]
		/// </summary>
		public string[] Columns { get; init; }

		/// <summary>
		/// [ [ "Data", 123, null ], [ ... ], ... ]
		/// </summary>
		public object?[][] Rows { get; init; }
	}
}
