// Copyright (c) 2020 Samuel Abraham

using sam987883.Database.Extensions;
using sam987883.Extensions;
using sam987883.Reflection;
using System;
using System.Data;
using static sam987883.Extensions.IEnumerableExtensions;
using static sam987883.Extensions.StringExtensions;

namespace sam987883.Database
{
	public class SchemaFactory : ISchemaFactory
	{
		private const string SCHEMA_SQL = @"SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
SET NOCOUNT ON;

DECLARE @ObjectId AS INTEGER = OBJECT_ID(@ObjectName);

SELECT o.[name] AS [ObjectName]
, s.[name] AS [SchemaName]
, o.[object_id] AS [ObjectId]
, IIF(o.[type] = 'U', N'Table', IIF(o.[type] = 'V', N'View', N'Function')) AS [Type]
, CAST(IIF(o.[type] = 'TF', 0, 1) AS BIT) AS [Inline]
FROM [sys].[objects] AS o
INNER JOIN [sys].[schemas] AS s ON s.[schema_id] = o.[schema_id]
WHERE o.[object_id] = @ObjectId AND o.[type] IN ('U', 'V', 'IF', 'TF');

SELECT c.[name] AS [ColumnName]
, CAST(c.[is_hidden] AS BIT) AS [Hidden]
, CAST(c.[is_identity] AS BIT) AS [Identity]
, CAST(c.[is_nullable] AS BIT) AS [Nullable]
, CAST(IIF(i.[is_primary_key] = 1, 1, 0) AS BIT) AS [PrimaryKey]
, CAST(IIF(1 IN (c.[is_computed], c.[is_identity], c.[is_rowguidcol]), 1, 0) AS BIT) AS [Readonly]
, UPPER(t.[name]) AS [Type]
, CAST(c.[max_length] AS INTEGER) AS [Length]
FROM [sys].[columns] AS c
INNER JOIN [sys].[types] AS t ON t.[user_type_id] = c.[user_type_id]
LEFT JOIN
(
	[sys].[index_columns] AS ic
	INNER JOIN [sys].[indexes] AS i ON i.[object_id] = ic.[object_id] AND i.[index_id] = ic.[index_id] AND i.[is_primary_key] = 1
) ON ic.[object_id] = c.[object_id] AND ic.[column_id] = c.[column_id]
WHERE c.[object_id] = @ObjectId
ORDER BY c.[column_id] ASC;";

		private readonly IPropertyCache<TableSchema> _TableSchemaPropertyCache;
		private readonly IPropertyCache<ColumnSchema> _ColumnSchemaPropertyCache;

		public SchemaFactory(IPropertyCache<TableSchema> tableSchemaPropertyCache, IPropertyCache<ColumnSchema> columnSchemaPropertyCache)
		{
			this._TableSchemaPropertyCache = tableSchemaPropertyCache;
			this._ColumnSchemaPropertyCache = columnSchemaPropertyCache;
		}

		public TableSchema LoadTableSchema(IDbConnection connection, string table)
		{
			table = table.TrimStart('[').TrimEnd(']');
			var tableSource = table.EndsWith(')') ? table.Left(table.LastIndexOf('(') + 1) : table;
			using var command = connection.CreateSqlCommand(SCHEMA_SQL);
			command.AddInputParameter("ObjectName", tableSource, DbType.String);

			using var reader = command.ExecuteReader();
			var (tableSchema, exists) = reader.Read(this._TableSchemaPropertyCache).First();
			if (exists)
			{
				tableSchema.DatabaseName = connection.Database;
				tableSchema.Name = $"[{tableSchema.DatabaseName}].[{tableSchema.SchemaName}].[{tableSchema.ObjectName}]";
				tableSchema.Columns = reader.Read(this._ColumnSchemaPropertyCache).ToArray();
				if (tableSchema.Columns.Any())
					return tableSchema;
			}

			throw new ArgumentException($"Table source name not found: {table}", nameof(table));
		}
	}
}
