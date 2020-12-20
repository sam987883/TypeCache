// Copyright (c) 2021 Samuel Abraham

using System.Collections.Generic;
using System.Text.Json.Serialization;
using TypeCache.Converters;

namespace TypeCache.Data
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
		///		"Rows": [ [ "Data", 123, null, ... ], [ ... ], ... ]
		/// }
		/// </summary>
		public IEnumerable<RowSet> Output { get; set; } = new RowSet[0];

		/// <summary>
		/// JSON: <code>[ { "Parameter1": "Value1" }, { "Parameter2": NULL }, { "Parameter3": true } ]</code>
		/// SQL: <code>@Parameter1 = N'Value1' OUTPUT, @Parameter2 = NULL OUTPUT, @Parameter3 = 1 OUTPUT</code>
		/// </summary>
		[JsonConverter(typeof(ParameterArrayJsonConverter))]
		public Parameter[]? Parameters { get; set; }
	}
}