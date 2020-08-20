// Copyright (c) 2020 Samuel Abraham

namespace Sam987883.Common.Models
{
	/// <summary>
	/// JSON: <code>{ "Column": "Value" }</code>
	/// SQL: <code>[Column] = N'Value'</code>
	/// </summary>
	public struct ColumnValue
	{
		/// <summary>
		/// "Column"
		/// </summary>
		public string Column { get; set; }

		/// <summary>
		/// "Value"
		/// </summary>
		public object? Value { get; set; }
	}
}
