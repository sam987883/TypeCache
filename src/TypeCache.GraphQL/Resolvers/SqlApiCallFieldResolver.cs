// Copyright (c) 2021 Samuel Abraham

using System.Collections.Generic;
using System.Threading.Tasks;
using GraphQL;
using Microsoft.Extensions.DependencyInjection;
using TypeCache.Data;
using TypeCache.Data.Mediation;
using TypeCache.Extensions;
using TypeCache.GraphQL.SqlApi;
using TypeCache.Mediation;

namespace TypeCache.GraphQL.Resolvers;

public sealed class SqlApiCallFieldResolver<T> : FieldResolver<OutputResponse<T>>
{
	protected override async ValueTask<OutputResponse<T>?> ResolveAsync(IResolveFieldContext context)
	{
		var mediator = context.RequestServices!.GetRequiredService<IMediator>();
		var objectSchema = context.FieldDefinition.GetMetadata<ObjectSchema>(nameof(ObjectSchema));
		var sqlCommand = objectSchema.DataSource.CreateSqlCommand(objectSchema.Name);

		context.GetArgument<Parameter[]>("parameters")?.ForEach(parameter => sqlCommand.Parameters[parameter.Name] = parameter.Value);

		var request = new SqlModelsRequest
		{
			Command = sqlCommand,
			ModelType = typeof(T)
		};
		var result = (IList<T>)await mediator.MapAsync(request, context.CancellationToken);

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
