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
		/// JSON: <code>{ "ParameterName1": "ParameterValue1", "ParameterName2": null, "ParameterName3": 123 }</code>
		/// SQL:
		/// <code>
		/// SET @ParameterName1 = N'ParameterValue1';<br />
		/// SET @ParameterName2 = NULL;<br />
		/// SET @ParameterName3 = 123;
		/// </code>
		/// </summary>
		[JsonConverter(typeof(ParameterArrayJsonConverter))]
		public Parameter[]? Parameters { get; set; }

		public string SQL { get; set; } = string.Empty;
	}
}
