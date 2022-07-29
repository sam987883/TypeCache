// Copyright (c) 2021 Samuel Abraham

using System.Data;
using System.Text;
using TypeCache.Collections.Extensions;
using TypeCache.Extensions;
using static System.FormattableString;

namespace TypeCache.Data.Schema;

public class ParameterSchema
{
	private static readonly string _SQL;

	private static readonly string _TypeSQL;

	static ParameterSchema()
	{
		var sqlBuilder = new StringBuilder("CASE t.[name]").Append('\n').Append('\t');

		EnumOf<SqlDbType>.Tokens.Do(token =>
			sqlBuilder.Append("WHEN").Append(' ').Append('\'').Append(token.Name.ToLowerInvariant()).Append('\'').Append(' ').Append("THEN").Append(' ').Append(token.Number),
			() => sqlBuilder.Append('\n').Append('\t'));

		_TypeSQL = sqlBuilder.Append('\n').Append('\t')
			.Append("ELSE").Append(' ').Append(SqlDbType.Variant.Number()).Append('\n').Append('\t')
			.Append("END")
			.ToString();

		_SQL = Invariant($@"
SELECT COUNT(1)
FROM [sys].[parameters] AS p
INNER JOIN [sys].[types] AS t ON t.[user_type_id] = p.[user_type_id]
WHERE p.[object_id] = @ObjectId;

SELECT p.[parameter_id] AS [Id]
, p.[name] AS [Name]
, {_TypeSQL} AS [Type]
, p.[is_output] AS [Output]
FROM [sys].[parameters] AS p
INNER JOIN [sys].[types] AS t ON t.[user_type_id] = p.[user_type_id]
WHERE p.[object_id] = @ObjectId
ORDER BY [Id] ASC;
");
	}

	public static string SQL => _SQL;

	public ParameterSchema(ParameterSchemaModel parameterSchema)
	{
		this.Id = parameterSchema.Id;
		this.Name = parameterSchema.Name;
		this.Type = parameterSchema.Type;
		this.Direction = parameterSchema.Id switch
		{
			0 => ParameterDirection.ReturnValue,
			_ when parameterSchema.Output => ParameterDirection.InputOutput,
			_ => ParameterDirection.Input
		};
	}

	public ParameterDirection Direction { get; }

	public int Id { get; }

	public string Name { get; }

	public SqlDbType Type { get; }
}
