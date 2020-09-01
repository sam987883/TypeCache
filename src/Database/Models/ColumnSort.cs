// Copyright (c) 2020 Samuel Abraham

using Sam987883.Common.Models;
using Sam987883.Database.Converters;
using System.Text.Json.Serialization;

namespace Sam987883.Database.Models
{
	/// <summary>
	/// JSON: <code>{ "Ascending": "SQL Expression" }</code>
	/// SQL: <code>[Column1] ASC</code>
	/// </summary>
	[JsonConverter(typeof(ColumnSortJsonConverter))]
	public struct ColumnSort
	{
		public string Expression { get; set; }

		public Sort Sort { get; set; }
	}
}
