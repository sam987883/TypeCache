// Copyright (c) 2021 Samuel Abraham

using System.Collections.Generic;
using System.Text.Json.Serialization;
using TypeCache.Converters;

namespace TypeCache.Data
{
	/// <summary>
	/// JSON: <code>
	/// {<br />
	///		"Parameters": [ ... ],<br />
	///		"Output":<br />
	///		{<br />
	///			"Columns": [ "Column1", "Column2", "Column3", ... ],<br />
	///			"Rows": [ [ "Data", 123, null ], [ ... ], ... ]<br />
	///		}<br />
	///	}
	/// </code>
	/// </summary>
	public record StoredProcedureResponse(IEnumerable<RowSet> Output)
	{
		/// <summary>
		/// JSON: <code>[ { "Parameter1": "Value1" }, { "Parameter2": NULL }, { "Parameter3": true } ]</code>
		/// SQL: <code>@Parameter1 = N'Value1' OUTPUT, @Parameter2 = NULL OUTPUT, @Parameter3 = 1 OUTPUT</code>
		/// </summary>
		[JsonConverter(typeof(ParameterArrayJsonConverter))]
		public Parameter[]? Parameters { get; init; }
	}
}
