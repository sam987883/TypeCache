// Copyright (c) 2021 Samuel Abraham

namespace TypeCache.Data
{
	/// <summary>
	/// {
	///		"Columns": [ "Column1", "Column2", "Column3", ... ],
	///		"Rows": [ [ "Data", 123, null ], [ ... ], ... ]
	/// }
	/// </summary>
	public struct RowSet
	{
		/// <summary>
		/// [ "Column1", "Column2", "Column3", ... ]
		/// </summary>
		public string[] Columns { get; set; }

		/// <summary>
		/// [ [ "Data", 123, null ], [ ... ], ... ]
		/// </summary>
		public object?[][] Rows { get; set; }
	}
}
