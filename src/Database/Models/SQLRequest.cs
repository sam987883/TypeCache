// Copyright (c) 2020 Samuel Abraham

using Sam987883.Database.Converters;
using System.Text.Json.Serialization;

namespace Sam987883.Database.Models
{
	/// <summary>
	/// Use Parameters to take in user input to avoid SQL Injection.
	/// </summary>
	public class SQLRequest
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
