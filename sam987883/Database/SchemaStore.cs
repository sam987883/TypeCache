// Copyright (c) 2020 Samuel Abraham

using System;
using System.Collections.Concurrent;
using System.Data;

namespace sam987883.Database
{
	public class SchemaStore : ISchemaStore
	{
		private readonly ConcurrentDictionary<string, ConcurrentDictionary<string, TableSchema>> _SchemaCache;
		private readonly ISchemaFactory _SchemaFactory;
		private readonly StringComparer _Comparer;

		public SchemaStore(ISchemaFactory schemaFactory)
		{
			this._Comparer = StringComparer.OrdinalIgnoreCase;
			this._SchemaFactory = schemaFactory;
			this._SchemaCache = new ConcurrentDictionary<string, ConcurrentDictionary<string, TableSchema>>(this._Comparer);
		}

		public TableSchema GetTableSchema(IDbConnection connection, string tableSource)
		{
			var parts = tableSource.Replace("[", string.Empty).Replace("]", string.Empty).Split('.', StringSplitOptions.RemoveEmptyEntries);
			var tableSourceKey = parts.Length switch
			{
				1 => $"[{connection.Database}]..[{parts[0]}]",
				2 => $"[{connection.Database}].[{parts[0]}].[{parts[1]}]",
				3 => $"[{parts[0]}].[{parts[1]}].[{parts[2]}]",
				_ => throw new ArgumentException($"Invalid table source name: {tableSource}", nameof(tableSource))
			};
			var tableSchemaCache = this._SchemaCache.GetOrAdd(connection.ConnectionString, connectionString => new ConcurrentDictionary<string, TableSchema>(this._Comparer));
			return tableSchemaCache.GetOrAdd(tableSourceKey, key => this._SchemaFactory.LoadTableSchema(connection, tableSource));
		}
	}
}
