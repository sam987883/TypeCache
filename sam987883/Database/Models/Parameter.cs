﻿// Copyright (c) 2020 Samuel Abraham

namespace sam987883.Database.Models
{
	/// <summary>
	/// JSON: <code>{ "ParameterName": "ParameterValue" }</code>
	/// SQL: <code>SET @ParameterName = N'ParameterValue';</code>
	/// </summary>
	public struct Parameter
	{
		/// <summary>
		/// JSON: <code>"ParameterName"</code>
		/// SQL: <code>@ParameterName</code>
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// JSON: <code>"ParameterValue"</code>
		/// SQL: <code>N'ParameterValue'</code>
		/// </summary>
		public object? Value { get; set; }
	}
}
