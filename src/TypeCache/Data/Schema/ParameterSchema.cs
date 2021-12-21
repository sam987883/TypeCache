// Copyright (c) 2021 Samuel Abraham

using System.Data;
using TypeCache.Extensions;
using static System.FormattableString;

namespace TypeCache.Data.Schema;

public readonly struct ParameterSchema
{
	public static string SQL { get; } = Invariant($@"
SELECT p.[parameter_id] AS [Id]
, p.[name] AS [Name]
, ISNULL((SELECT [ID] FROM @SqlDbTypes WHERE [Type] = t.[name]), {SqlDbType.Variant.Number()}) AS [Type]
, p.[is_output] AS [Output]
, IIF(p.[is_output] = 1 AND p.[parameter_id] = 0, 1, 0) AS [Return]
FROM [sys].[parameters] AS p
INNER JOIN [sys].[types] AS t ON t.[user_type_id] = p.[user_type_id]
WHERE p.[object_id] = @ObjectId
ORDER BY [Id] ASC;
");

	public int Id { get; init; }

	public string Name { get; init; }

	public SqlDbType Type { get; init; }

	public bool Output { get; init; }

	public bool Return { get; init; }
}
