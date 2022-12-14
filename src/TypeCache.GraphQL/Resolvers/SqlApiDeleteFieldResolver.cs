// Copyright (c) 2021 Samuel Abraham

using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using GraphQL;
using Microsoft.Extensions.DependencyInjection;
using TypeCache.Collections;
using TypeCache.Data;
using TypeCache.Data.Mediation;
using TypeCache.Extensions;
using TypeCache.GraphQL.Extensions;
using TypeCache.GraphQL.SqlApi;
using TypeCache.Mediation;
using static System.FormattableString;
using static TypeCache.Data.DataSourceType;

namespace TypeCache.GraphQL.Resolvers;

public sealed class SqlApiDeleteFieldResolver : FieldResolver<OutputResponse<DataRow>>
{
	protected override async ValueTask<OutputResponse<DataRow>?> ResolveAsync(IResolveFieldContext context)
	{
		var mediator = context.RequestServices!.GetRequiredService<IMediator>();
		var objectSchema = context.FieldDefinition.GetMetadata<ObjectSchema>(nameof(ObjectSchema));
		var selections = context.GetSelections().ToArray();
		var output = selections
			.Where(column => selections.Any(_ => _.Left(Invariant($"{nameof(OutputResponse<DataRow>.Output)}.{column}"))))
			.Select(column => objectSchema.DataSource.Type switch
			{
				PostgreSql => objectSchema.DataSource.EscapeIdentifier(column),
				_ or SqlServer => Invariant($"DELETED.{objectSchema.DataSource.EscapeIdentifier(column)}")
			})
			.ToArray();
		var data = context.GetArgumentAsDataTable("data", objectSchema);
		var where = context.GetArgument<string>("where");
		var sql = data.Rows.Any<DataRow>() ? objectSchema.CreateDeleteSQL(data, output) : objectSchema.CreateDeleteSQL(where, output);
		var sqlCommand = objectSchema.DataSource.CreateSqlCommand(sql);

		context.GetArgument<Parameter[]>("parameters")?.ForEach(parameter => sqlCommand.Parameters[parameter.Name] = parameter.Value);

		var result = Array<DataRow>.Empty;
		if (output.Any())
		{
			var request = new SqlDataTableRequest { Command = sqlCommand };
			result = (await mediator.MapAsync(request, context.CancellationToken)).Select();
		}
		else
		{
			var request = new SqlExecuteRequest { Command = sqlCommand };
			await mediator.ExecuteAsync(request, context.CancellationToken);
		}

		return new()
		{
			TotalCount = sqlCommand.RecordsAffected,
			DataSource = objectSchema.DataSource.Name,
			Output = result,
			Sql = sql,
			Table = objectSchema.Name
		};
	}
}

public sealed class SqlApiDeleteFieldResolver<T> : FieldResolver<OutputResponse<T>>
	where T : new()
{
	protected override async ValueTask<OutputResponse<T>?> ResolveAsync(IResolveFieldContext context)
	{
		var mediator = context.RequestServices!.GetRequiredService<IMediator>();
		var objectSchema = context.FieldDefinition.GetMetadata<ObjectSchema>(nameof(ObjectSchema));
		var selections = context.GetSelections().ToArray();
		var output = selections
			.Where(column => selections.Any(_ => _.Left(Invariant($"{nameof(OutputResponse<T>.Output)}.{column}"))))
			.Select(column => objectSchema.DataSource.Type switch
			{
				PostgreSql => objectSchema.DataSource.EscapeIdentifier(column),
				_ or SqlServer => Invariant($"DELETED.{objectSchema.DataSource.EscapeIdentifier(column)}")
			})
			.ToArray();
		var data = context.GetArgument<T[]>("data");
		var where = context.GetArgument<string>("where");
		var sql = data.Any() ? objectSchema.CreateDeleteSQL(data, output) : objectSchema.CreateDeleteSQL(where, output);
		var sqlCommand = objectSchema.DataSource.CreateSqlCommand(sql);

		context.GetArgument<Parameter[]>("parameters")?.ForEach(parameter => sqlCommand.Parameters[parameter.Name] = parameter.Value);

		var result = (IList<T>)Array<T>.Empty;
		if (output.Any())
		{
			var request = new SqlModelsRequest
			{
				Command = sqlCommand,
				ModelType = typeof(T)
			};
			result = (IList<T>)await mediator.MapAsync(request, context.CancellationToken);
		}
		else
		{
			var request = new SqlExecuteRequest { Command = sqlCommand };
			await mediator.ExecuteAsync(request, context.CancellationToken);
		}

		return new()
		{
			TotalCount = sqlCommand.RecordsAffected,
			DataSource = objectSchema.DataSource.Name,
			Output = result,
			Sql = sql,
			Table = objectSchema.Name
		};
	}
}
