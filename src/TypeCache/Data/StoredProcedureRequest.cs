﻿// Copyright (c) 2021 Samuel Abraham

using System.Text.Json.Serialization;
using TypeCache.Converters;

namespace TypeCache.Data
{
	/// <summary>
	/// JSON: <code>{ "Procedure": "Procedure1", "Parameters": [ { "Parameter1": "Value1" }, { "Parameter2": null }, { "Parameter3": true } ] }</code>
	/// SQL: <code>EXECUTE [Database1]..[Procedure1] (@Parameter1 = N'Value1', @Parameter2 = NULL, @Parameter3 = 1);</code>
	/// </summary>
	public class StoredProcedureRequest
	{
		/// <summary>
		/// JSON: <code>"Procedure1"</code>
		/// SQL: <code>[Database1]..[Procedure1]</code>
		/// </summary>
		public string Procedure { get; set; } = string.Empty;

		/// <summary>
		/// JSON: <code>[ { "Parameter1": "Value1" }, { "Parameter2": null }, { "Parameter3": true } ]</code>
		/// SQL: <code>@Parameter1 = N'Value1', @Parameter2 = NULL, @Parameter3 = 1</code>
		/// </summary>
		[JsonConverter(typeof(ParameterArrayJsonConverter))]
		public Parameter[]? Parameters { get; set; }
	}
}