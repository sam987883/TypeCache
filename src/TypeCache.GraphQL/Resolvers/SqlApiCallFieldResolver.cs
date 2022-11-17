// Copyright (c) 2021 Samuel Abraham

using System.Collections.Generic;
using System.Threading.Tasks;
using GraphQL;
using Microsoft.Extensions.DependencyInjection;
using TypeCache.Business;
using TypeCache.Collections.Extensions;
using TypeCache.Data;
using TypeCache.GraphQL.SqlApi;

namespace TypeCache.GraphQL.Resolvers;

public sealed class SqlApiCallFieldResolver<T> : FieldResolver<OutputResponse<T>>
{
	protected override async ValueTask<OutputResponse<T>?> ResolveAsync(IResolveFieldContext context)
	{
		var mediator = context.RequestServices!.GetRequiredService<IMediator>();
		var objectSchema = context.FieldDefinition.GetMetadata<ObjectSchema>(nameof(ObjectSchema));
		var sqlCommand = objectSchema.DataSource.CreateSqlCommand(objectSchema.Name);

		context.GetArgument<Parameter[]>("parameters")?.Do(parameter => sqlCommand.Parameters[parameter.Name] = parameter.Value);

		var result = await mediator.ApplyRuleAsync<SqlCommand, IList<T>>(sqlCommand, context.CancellationToken);

		return new()
		{
			TotalCount = sqlCommand.RecordsAffected > 0 ? sqlCommand.RecordsAffected : result.Count,
			DataSource = objectSchema.DataSource.Name,
			Output = result,
			Sql = sqlCommand.SQL,
			Table = objectSchema.Name
		};
	}
}
