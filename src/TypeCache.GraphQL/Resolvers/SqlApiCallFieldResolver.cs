// Copyright (c) 2021 Samuel Abraham

using GraphQL;
using TypeCache.Data;
using TypeCache.Data.Extensions;
using TypeCache.Extensions;
using TypeCache.GraphQL.SqlApi;

namespace TypeCache.GraphQL.Resolvers;

public sealed class SqlApiCallFieldResolver<T> : FieldResolver
	where T : notnull, new()
{
	protected override async ValueTask<object?> ResolveAsync(IResolveFieldContext context)
	{
		var objectSchema = context.FieldDefinition.GetMetadata<ObjectSchema>(nameof(ObjectSchema));
		var sqlCommand = objectSchema.DataSource.CreateSqlCommand(objectSchema.Name);

		context.GetArgument<Parameter[]>("parameters")?.ForEach(parameter => sqlCommand.Parameters[parameter.Name] = parameter.Value);

		await using var connection = sqlCommand.DataSource.CreateDbConnection();
		await connection.OpenAsync(context.CancellationToken);
		await using var dbCommand = connection.CreateCommand(sqlCommand);

		var result = await dbCommand.GetModelsAsync<T>(100, context.CancellationToken);
		sqlCommand.RecordsAffected = (int?)dbCommand.Parameters[nameof(sqlCommand.RecordsAffected)]?.Value ?? 0;

		if (sqlCommand.Parameters.Any())
			dbCommand.CopyOutputParameters(sqlCommand);

		return new OutputResponse<T>()
		{
			TotalCount = sqlCommand.RecordsAffected > 0 ? sqlCommand.RecordsAffected : result.Count,
			DataSource = objectSchema.DataSource.Name,
			Output = result,
			Sql = sqlCommand.SQL,
			Table = objectSchema.Name
		};
	}
}
