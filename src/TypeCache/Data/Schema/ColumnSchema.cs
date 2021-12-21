// Copyright (c) 2021 Samuel Abraham

using System.Data;
using TypeCache.Extensions;
using static System.FormattableString;

namespace TypeCache.Data.Schema;

public readonly struct ColumnSchema
{
	public static string SQL { get; } = Invariant(@$"
SELECT c.[column_id] AS [Id]
, c.[name] AS [Name]
, ISNULL((SELECT [ID] FROM @SqlDbTypes WHERE [Type] = t.[name]), {SqlDbType.Variant.Number()}) AS [Type]
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
");

	public int Id { get; init; }

	public string Name { get; init; }

	public SqlDbType Type { get; init; }

	public bool Nullable { get; init; }

	public bool ReadOnly { get; init; }

	public bool Hidden { get; init; }

	public bool Identity { get; init; }

	public bool PrimaryKey { get; init; }

	public int Length { get; init; }
}
