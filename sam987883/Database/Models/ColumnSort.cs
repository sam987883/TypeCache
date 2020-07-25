// Copyright (c) 2020 Samuel Abraham

using sam987883.Common.Models;
using sam987883.Database.Converters;
using System.Text.Json.Serialization;

namespace sam987883.Database.Models
{
	/// <summary>
	/// JSON: <code>{ "Ascending": "Column" }</code>
	/// SQL: <code>[Column1] ASC</code>
	/// </summary>
	[JsonConverter(typeof(ColumnSortJsonConverter))]
	public struct ColumnSort
	{
		public string Column { get; set; }

		public Sort Sort { get; set; }
	}
}
