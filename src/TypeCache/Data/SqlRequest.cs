// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using TypeCache.Converters;

namespace TypeCache.Data
{
	/// <summary>
	/// Use Parameters to take in user input to avoid SQL Injection.
	/// </summary>
	public class SqlRequest : IDataRequest
	{
		/// <inheritdoc/>
		public string DataSource { get; set; } = "Default";

		public SqlRequest()
		{
		}

		public SqlRequest(string sql)
		{
			this.SQL = sql;
		}

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
		public IDictionary<string, object?> Parameters { get; set; } = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);

		public string SQL { get; set; } = string.Empty;
	}
}
