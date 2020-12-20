// Copyright (c) 2021 Samuel Abraham

using System;
using System.Data;
using TypeCache.Extensions;
using TypeCache.Reflection;

namespace TypeCache.Data.Schema
{
	internal class SchemaFactory : ISchemaFactory
	{
		public const string OBJECT_NAME = "ObjectName";

		public static string ObjectSchemaSQL = @$"

DECLARE @ObjectId AS INTEGER =
(
	SELECT [object_id]
	FROM [sys].[objects]
	WHERE [name] = @{OBJECT_NAME}
		AND [type] IN ('U', 'V', 'TF', 'P')
);

SELECT o.[object_id] AS [Id]
, o.[name] AS [ObjectName]
, IIF(o.[type] = 'U', {ObjectType.Table.Number()}
	, IIF(o.[type] = 'V', {ObjectType.View.Number()}
	, IIF(o.[type] = 'TF', {ObjectType.Function.Number()}, {ObjectType.StoredProcedure.Number()}))) AS [Type]
, s.[name] AS [SchemaName]
FROM [sys].[objects] AS o
INNER JOIN [sys].[schemas] AS s ON s.[schema_id] = o.[schema_id]
WHERE o.[object_id] = @ObjectId;

SELECT c.[column_id] AS [Id]
, c.[name] AS [Name]
, UPPER(t.[name]) AS [Type]
, c.[is_hidden] AS [Hidden]
, c.[is_identity] AS [Identity]
, c.[is_nullable] AS [Nullable]
, IIF(i.[is_primary_key] = 1, 1, 0) AS [PrimaryKey]
, IIF(1 IN (c.[is_computed], c.[is_identity], c.[is_rowguidcol]), 1, 0) AS [Readonly]
, c.[max_length] AS [Length]
FROM [sys].[columns] AS c
INNER JOIN [sys].[types] AS t ON t.[user_type_id] = c.[user_type_id]
LEFT JOIN
(
	[sys].[index_columns] AS ic
	INNER JOIN [sys].[indexes] AS i ON i.[object_id] = ic.[object_id] AND i.[index_id] = ic.[index_id] AND i.[is_primary_key] = 1
) ON ic.[object_id] = c.[object_id] AND ic.[column_id] = c.[column_id]
WHERE c.[object_id] = @ObjectId
ORDER BY c.[column_id] ASC;

SELECT p.[parameter_id] AS [Id]
, p.[name] AS [Name]
, UPPER(t.[name]) AS [Type]
, p.[is_output] AS [Output]
, IIF(p.[is_output] = 1 AND p.[parameter_id] = 0, 1, 0) AS [Return]
FROM [sys].[parameters] AS p
INNER JOIN [sys].[types] AS t ON t.[user_type_id] = p.[user_type_id]
WHERE p.[object_id] = @ObjectId
ORDER BY p.[parameter_id] ASC;";

		private readonly ITypeCache _TypeCache;

		public SchemaFactory(ITypeCache typeCache)
			=> this._TypeCache = typeCache;

		public string SQL { get; } = ObjectSchemaSQL;

		public ObjectSchema GetObjectSchema(IDbConnection connection, string name)
		{
			if (name.EndsWith(')'))
				name = name.Left(name.LastIndexOf('('));

			var parts = name.Split('.', StringSplitOptions.RemoveEmptyEntries)
				.To(part => part.TrimStart('[').TrimEnd(']'))
				.ToList();
			name = parts.Count switch
			{
				1 => parts[0],
				2 => parts[1],
				3 => parts[2],
				_ => throw new ArgumentException($"Invalid table source name: {name}", nameof(name))
			};

			var sql = $"USE {connection.Database.EscapeIdentifier()};{ObjectSchemaSQL}";
			using var command = connection.CreateSqlCommand(sql);
			command.AddInputParameter(OBJECT_NAME, name, DbType.String);

			using var transaction = connection.BeginTransaction(IsolationLevel.ReadUncommitted);
			command.Transaction = transaction;

			using var reader = command.ExecuteReader();

			var tableRowSet = reader.ReadRowSet();

			var objectSchema = this._TypeCache.Map<ObjectSchema>(tableRowSet).First();
			if (objectSchema != null)
			{
				objectSchema.DatabaseName = connection.Database;

				if (reader.NextResult())
				{
					var columnRowSet = reader.ReadRowSet();
					objectSchema.Columns = this._TypeCache.Map<ColumnSchema>(columnRowSet).ToImmutable();
				}

				if (reader.NextResult())
				{
					var parameterRowSet = reader.ReadRowSet();
					objectSchema.Parameters = this._TypeCache.Map<ParameterSchema>(parameterRowSet).ToImmutable();
				}

				return objectSchema;
			}

			throw new ArgumentException($"Database object not found: [{name}].  Must be a table, view, table-valued function or stored procedure.", nameof(name));
		}
	}
}
