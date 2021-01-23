// Copyright (c) 2021 Samuel Abraham

namespace TypeCache.Data
{
	/// <summary>
	/// JSON: <code>{ "Column": "N'Expression 1'" }</code>
	/// SQL: <code>[Column] = N'Expression 1'</code>
	/// </summary>
	public class ColumnSet
	{
		public string Column { get; set; }

		public object? Expression { get; set; }
	}
}
