// Copyright (c) 2020 Samuel Abraham

using Sam987883.Common.Models;
using System.Collections.Generic;

namespace Sam987883.Database.Models
{
	/// <summary>
	/// JSON: <code>
	/// {
	///		"Parameters": [ ... ],
	///		"Output":
	///		{
	///			"Columns": [ "Column1", "Column2", "Column3", ... ],
	///			"Rows": [ [ "Data", 123, null ], [ ... ], ... ]
	///		}
	///	}
	/// </code>
	/// </summary>
	public class StoredProcedureResponse
	{
		/// <summary>
		/// {
		///		"Columns": [ "Column1", "Column2", "Column3", ... ],
		///		"Rows": [ [ "Data", 123, null ], [ ... ], ... ]
		/// }
		/// </summary>
		public IEnumerable<RowSet> Output { get; set; } = new RowSet[0];

		/// <summary>
		/// JSON: <code>[ { "Parameter1": "Value1" }, { "Parameter2": NULL }, { "Parameter3": true } ]</code>
		/// SQL: <code>@Parameter1 = N'Value1' OUTPUT, @Parameter2 = NULL OUTPUT, @Parameter3 = 1 OUTPUT</code>
		/// </summary>
		public IDictionary<string, object?>? Parameters { get; set; } = null;
	}
}