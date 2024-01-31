// Copyright (c) 2021 Samuel Abraham

using System.Collections.Generic;
using System.Threading.Tasks;
using GraphQL;
using Microsoft.Extensions.DependencyInjection;
using TypeCache.Data;
using TypeCache.Data.Extensions;
using TypeCache.Extensions;
using TypeCache.GraphQL.SqlApi;
using TypeCache.Mediation;

namespace TypeCache.GraphQL.Resolvers;

public sealed class SqlApiCallFieldResolver<T> : FieldResolver
{
	protected override async ValueTask<object?> ResolveAsync(IResolveFieldContext context)
	{
		var mediator = context.RequestServices!.GetRequiredService<IMediator>();
		var objectSchema = context.FieldDefinition.GetMetadata<ObjectSchema>(nameof(ObjectSchema));
		var sqlCommand = objectSchema.DataSource.CreateSqlCommand(objectSchema.Name);

		context.GetArgument<Parameter[]>("parameters")?.ForEach(parameter => sqlCommand.Parameters[parameter.Name] = parameter.Value);

		var result = (IList<T>)await mediator.Map(sqlCommand.ToSqlModelsRequest<T>(), context.CancellationToken);
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
