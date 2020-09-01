// Copyright (c) 2020 Samuel Abraham

namespace Sam987883.Common.Models
{
	/// <summary>
	/// JSON: <code>{ "Column": "N'Expression 1'" }</code>
	/// SQL: <code>[Column] = N'Expression 1'</code>
	/// </summary>
	public struct ColumnSet
	{
		public string Column { get; set; }

		public string? Expression { get; set; }
	}
}
