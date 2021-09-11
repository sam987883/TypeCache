// Copyright (c) 2021 Samuel Abraham

using System.Collections.Generic;
using System.Text.Json.Serialization;
using TypeCache.Converters;
using TypeCache.Extensions;
using static TypeCache.Default;

namespace TypeCache.Data.Requests
{
	/// <summary>
	/// Use Parameters to take in user input to avoid SQL Injection.
	/// </summary>
	public class SqlRequest
	{
		/// <summary>
		/// The data source name that contains the connection string and database provider to use.
		/// </summary>
		public string DataSource { get; set; } = DATASOURCE;

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
		public IDictionary<string, object?> Parameters { get; set; } = new Dictionary<string, object?>(STRING_COMPARISON.ToStringComparer());

		/// <summary>
		/// Raw SQL.
		/// </summary>
		public string SQL { get; set; } = string.Empty;
	}
}
