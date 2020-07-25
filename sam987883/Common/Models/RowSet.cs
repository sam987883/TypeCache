// Copyright (c) 2020 Samuel Abraham

namespace sam987883.Common.Models
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
