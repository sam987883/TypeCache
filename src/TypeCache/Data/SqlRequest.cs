// Copyright (c) 2021 Samuel Abraham

using System.Text.Json.Serialization;
using TypeCache.Converters;

namespace TypeCache.Data
{
	/// <summary>
	/// Use Parameters to take in user input to avoid SQL Injection.
	/// </summary>
	public class SqlRequest
	{
		/// <summary>
		/// Raw SQL.
		/// </summary>
		public string SQL { get; set; } = string.Empty;

		/// <summary>
		/// JSON: <code>[ { "Parameter1": "Value1" }, { "Parameter2": null }, { "Parameter3": true } ]</code>
		/// SQL: <code>@Parameter1 = N'Value1', @Parameter2 = NULL, @Parameter3 = 1</code>
		/// </summary>
		[JsonConverter(typeof(ParameterArrayJsonConverter))]
		public Parameter[]? Parameters { get; set; }
	}
}
