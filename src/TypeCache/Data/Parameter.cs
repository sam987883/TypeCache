// Copyright (c) 2021 Samuel Abraham

using System.Diagnostics.CodeAnalysis;

namespace TypeCache.Data
{
	/// <summary>
	/// JSON: <code>{ "ParameterName": "ParameterValue" }</code>
	/// SQL: <code>SET @ParameterName = N'ParameterValue';</code>
	/// </summary>
	public class Parameter
	{
		/// <summary>
		/// JSON: <code>"ParameterName"</code>
		/// SQL: <code>@ParameterName</code>
		/// </summary>
		[NotNull]
		public string Name { get; set; }

		/// <summary>
		/// JSON: <code>"ParameterValue"</code>
		/// SQL: <code>N'ParameterValue'</code>
		/// </summary>
		public object? Value { get; set; }
	}
}
