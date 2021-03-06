﻿// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using TypeCache.Data.Converters;

namespace TypeCache.Data
{
	/// <summary>
	/// JSON: <code>{ "Procedure": "[Database1]..[Procedure1]", "Parameters": { "Parameter1": "Value1", "Parameter2": null, "Parameter3": true } }</code>
	/// SQL: <code>EXECUTE [Database1]..[Procedure1] (@Parameter1 = N'Value1', @Parameter2 = NULL, @Parameter3 = 1);</code>
	/// </summary>
	public class StoredProcedureRequest
	{
		public StoredProcedureRequest()
		{
		}

		public StoredProcedureRequest(string procedure)
		{
			this.Procedure = procedure;
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
		[JsonConverter(typeof(ParameterJsonConverter))]
		public IDictionary<string, object?> Parameters { get; set; } = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);

		public string Procedure { get; set; } = string.Empty;
	}
}
