// Copyright (c) 2021 Samuel Abraham

namespace TypeCache.Data
{
	/// <summary>
	/// JSON: <code>{ "Column": "N'Expression 1'" }</code>
	/// SQL: <code>[Column] = N'Expression 1'</code>
	/// </summary>
	public readonly struct ColumnSet
	{
		public string Column { get; init; }

		public object? Expression { get; init; }
	}
}
