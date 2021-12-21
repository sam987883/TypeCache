// Copyright (c) 2021 Samuel Abraham

using System.Collections.Generic;
using System.Text.Json.Serialization;
using TypeCache.Converters;

namespace TypeCache.Data.Responses;

public class StoredProcedureResponse
{
	/// <summary>
	/// JSON: <code>
	/// {<br />
	///		"Output":<br />
	///		{<br />
	///			"Columns": [ "Column1", "Column2", "Column3", ... ],<br />
	///			"Rows": [ [ "Data", 123, null ], [ ... ], ... ]<br />
	///		}<br />
	///	}
	/// </code>
	/// </summary>
	public RowSet[]? Output { get; set; }

	/// <summary>
	/// JSON: <code>{ "ParameterName1": "ParameterValue1", "ParameterName2": null, "ParameterName3": 123 }</code>
	/// SQL:
	/// <code>
	/// SET @ParameterName1 = N'ParameterValue1';<br />
	/// SET @ParameterName2 = NULL;<br />
	/// SET @ParameterName3 = 123;
	/// </code>
	/// </summary>
	[JsonConverter(typeof(DictionaryJsonConverter))]
	public IDictionary<string, object?>? Parameters { get; set; }
}
