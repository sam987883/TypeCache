﻿// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using TypeCache.Data.Converters;

namespace TypeCache.Data.Requests
{
	/// <summary>
	/// JSON: <code>{ "Table": "Table1", "Input": { ... }, "OutputDeleted": [ ... ], "OutputInserted": [ ... ] }</code>
	/// SQL: <code>UPDATE ... OUTPUT ... VALUES ...;</code>
	/// </summary>
	public class UpdateDataRequest : IDataRequest
	{
		/// <inheritdoc/>
		public string DataSource { get; set; } = "Default";

		/// <summary>
		/// Batch of records to update based on Primary Key(s).
		/// </summary>
		public RowSet Input { get; set; } = new RowSet();

		/// <summary>
		/// JSON: <code>"Output": { "Alias 1": "NULLIF([Column1], 22)", "Alias 2": "INSERTED.ColumnName", "Alias 3": "DELETED.ColumnName" }</code>
		/// SQL: <code>OUTPUT NULLIF([Column1], 22) AS [Alias 1], INSERTED.[ColumnName] AS [Alias 2], DELETED.[ColumnName] AS [Alias 3]</code>
		/// </summary>
		[JsonConverter(typeof(OutputJsonConverter))]
		public IDictionary<string, string> Output { get; set; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

		/// <summary>
		/// JSON: <code>"Table1"</code>
		/// SQL: <code>UPDATE [Database1]..[Table1]</code>
		/// </summary>
		public string Table { get; set; } = string.Empty;
	}
}
