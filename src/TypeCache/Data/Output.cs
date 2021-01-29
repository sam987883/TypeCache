﻿// Copyright (c) 2021 Samuel Abraham

namespace TypeCache.Data
{
	/// <summary>
	/// JSON: <code>
	/// {<br />
	///		"Deleted": { "Columns": [ "Column1", "Column2", "Column3", ... ], "Rows": [ [ "Data", 123, null, ... ], [ ... ], ... ] },<br />
	///		"Inserted": { "Columns": [ "Column1", "Column2", "Column3", ... ], "Rows": [ [ "Data", 123, null, ... ], [ ... ], ... ] }<br />
	/// }
	/// </code>
	/// </summary>
	public readonly struct Output
	{
		/// <summary>
		/// JSON: <code>{ "Deleted": { "Columns": [ ... ], "Rows": [ [ ... ], [ ... ], [ ... ] ] } }</code>
		/// </summary>
		public RowSet Deleted { get; init; }

		/// <summary>
		/// JSON: <code>{ "Inserted": { "Columns": [ ... ], "Rows": [ [ ... ], [ ... ], [ ... ] ] } }</code>
		/// </summary>
		public RowSet Inserted { get; init; }
	}
}
