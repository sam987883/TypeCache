﻿// Copyright (c) 2020 Samuel Abraham

using sam987883.Database.Schemas;
using sam987883.Extensions;
using System;
using System.Collections.Concurrent;
using System.Data;

namespace sam987883.Database
{
	internal class SchemaStore : ISchemaStore
	{
		private readonly ConcurrentDictionary<string, ConcurrentDictionary<string, ObjectSchema>> _SchemaCache;
		private readonly ISchemaFactory _SchemaFactory;
		private readonly StringComparer _Comparer;

		public SchemaStore(ISchemaFactory schemaFactory)
		{
			this._Comparer = StringComparer.OrdinalIgnoreCase;
			this._SchemaFactory = schemaFactory;
			this._SchemaCache = new ConcurrentDictionary<string, ConcurrentDictionary<string, ObjectSchema>>(this._Comparer);
		}

		public ObjectSchema GetObjectSchema(IDbConnection connection, string name)
		{
			var parts = name.Split('.', StringSplitOptions.RemoveEmptyEntries)
				.To(part => part.TrimStart('[').TrimEnd(']'))
				.ToList();
			var fullName = parts.Count switch
			{
				1 => $"[{connection.Database}]..[{parts[0]}]",
				2 when name.Contains("..") => $"[{parts[0]}]..[{parts[1]}]",
				2 => $"[{connection.Database}].[{parts[0]}].[{parts[1]}]",
				3 => $"[{parts[0]}].[{parts[1]}].[{parts[2]}]",
				_ => throw new ArgumentException($"Invalid table source name: {name}", nameof(name))
			};
			var tableSchemaCache = this._SchemaCache.GetOrAdd(connection.ConnectionString, connectionString => new ConcurrentDictionary<string, ObjectSchema>(this._Comparer));
			return tableSchemaCache.GetOrAdd(fullName, key => this._SchemaFactory.LoadObjectSchema(connection, name));
		}
	}
}
