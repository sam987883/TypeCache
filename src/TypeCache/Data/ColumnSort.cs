﻿// Copyright (c) 2021 Samuel Abraham

using System.Text.Json.Serialization;
using TypeCache.Converters;

namespace TypeCache.Data
{
	/// <summary>
	/// JSON: <code>{ "Ascending": "SQL Expression" }</code>
	/// SQL: <code>[Column1] ASC</code>
	/// </summary>
	[JsonConverter(typeof(ColumnSortJsonConverter))]
	public record ColumnSort(string Expression, Sort Sort);
}
