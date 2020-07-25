// Copyright (c) 2020 Samuel Abraham

namespace sam987883.Common.Models
{
	/// <summary>
	/// JSON: <code>{ "Column": "Alias" }</code>
	/// SQL: <code>[Column] AS [Alias]</code>
	/// </summary>
	public struct ColumnOutput
	{
		/// <summary>
		/// "Column"
		/// </summary>
		public string Column { get; set; }

		/// <summary>
		/// "Alias"
		/// </summary>
		public string? Alias { get; set; }
	}
}
