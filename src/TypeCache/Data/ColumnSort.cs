// Copyright (c) 2021 Samuel Abraham

using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using TypeCache.Converters;

namespace TypeCache.Data
{
	/// <summary>
	/// JSON: <code>{ "Ascending": "SQL Expression" }</code>
	/// SQL: <code>[Column1] ASC</code>
	/// </summary>
	[JsonConverter(typeof(ColumnSortJsonConverter))]
	public readonly struct ColumnSort
	{
		[NotNull]
		public string Expression { get; init; }

		public Sort Sort { get; init; }
	}
}
