// Copyright (c) 2021 Samuel Abraham

using System.Data;
using System.Text;
using TypeCache.Collections.Extensions;
using TypeCache.Extensions;
using static System.FormattableString;

namespace TypeCache.Data.Schema;

public class ColumnSchema
{
	private static readonly string _SQL;

	private static readonly string _TypeSQL;

	static ColumnSchema()
	{
		var sqlBuilder = new StringBuilder("CASE t.[name]").AppendLine().Append('\t');

		EnumOf<SqlDbType>.Tokens.Do(token =>
			sqlBuilder.Append("WHEN").Append(' ').Append('\'').Append(token.Name.ToLowerInvariant()).Append('\'').Append(' ').Append("THEN").Append(' ').Append(token.Number),
			() => sqlBuilder.AppendLine().Append('\t'));

		_TypeSQL = sqlBuilder.AppendLine()
			.Append('\t').Append("ELSE").Append(' ').AppendLine(SqlDbType.Variant.Number())
			.Append('\t').Append("END").ToString();

		_SQL = Invariant(@$"
SELECT COUNT(1)
FROM [sys].[columns] AS c
INNER JOIN [sys].[types] AS t ON t.[user_type_id] = c.[user_type_id]
WHERE c.[object_id] = @ObjectId;

SELECT c.[column_id] AS [Id]
, c.[name] AS [Name]
, {_TypeSQL} AS [Type]
, c.[is_hidden] AS [Hidden]
, c.[is_identity] AS [Identity]
, c.[is_nullable] AS [Nullable]
, IIF(i.[is_primary_key] = 1, CAST(1 AS BIT), CAST(0 AS BIT)) AS [PrimaryKey]
, IIF(1 IN (c.[is_computed], c.[is_identity], c.[is_rowguidcol]), CAST(1 AS BIT), CAST(0 AS BIT)) AS [ReadOnly]
, CAST(c.[max_length] AS INTEGER) AS [Length]
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
	}

	public static string SQL => _SQL;

	public ColumnSchema(ColumnSchemaModel columnSchema)
	{
		this.Id = columnSchema.Id;
		this.Name = columnSchema.Name;
		this.Type = columnSchema.Type;
		this.Nullable = columnSchema.Nullable;
		this.ReadOnly = columnSchema.ReadOnly;
		this.Hidden = columnSchema.Hidden;
		this.Identity = columnSchema.Identity;
		this.PrimaryKey = columnSchema.PrimaryKey;
		this.Length = columnSchema.Length;
	}

	public int Id { get; }

	public string Name { get; }

	public SqlDbType Type { get; }

	public bool Nullable { get; }

	public bool ReadOnly { get; }

	public bool Hidden { get; }

	public bool Identity { get; }

	public bool PrimaryKey { get; }

	public int Length { get; }
}
