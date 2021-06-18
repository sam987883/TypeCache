// Copyright (c) 2021 Samuel Abraham

using System.Text.Json.Serialization;
using TypeCache.Converters;

namespace TypeCache.Data
{
	/// <summary>
	/// JSON: <code>{ "Procedure": "[Database1]..[Procedure1]", "Parameters": { "Parameter1": "Value1", "Parameter2": null, "Parameter3": true } }</code>
	/// SQL: <code>EXECUTE [Database1]..[Procedure1] (@Parameter1 = N'Value1', @Parameter2 = NULL, @Parameter3 = 1);</code>
	/// </summary>
	public class StoredProcedureRequest //(string Procedure, params Parameter[]? Parameters);
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

		public string Procedure { get; set; } = string.Empty;
	}
}
