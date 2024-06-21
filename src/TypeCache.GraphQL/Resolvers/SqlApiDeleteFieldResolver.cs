// Copyright (c) 2021 Samuel Abraham

using System.Data;
using GraphQL;
using Microsoft.Extensions.DependencyInjection;
using TypeCache.Data;
using TypeCache.Data.Extensions;
using TypeCache.Extensions;
using TypeCache.GraphQL.Extensions;
using TypeCache.GraphQL.SqlApi;
using TypeCache.Mediation;
using TypeCache.Utilities;
using static TypeCache.Data.DataSourceType;

namespace TypeCache.GraphQL.Resolvers;

public sealed class SqlApiDeleteFieldResolver : FieldResolver
{
	protected override async ValueTask<object?> ResolveAsync(IResolveFieldContext context)
	{
		var mediator = context.RequestServices!.GetRequiredService<IMediator>();
		var objectSchema = context.FieldDefinition.GetMetadata<ObjectSchema>(nameof(ObjectSchema));
		var selections = context.GetSelections().ToArray();
		var output = objectSchema.Columns
			.Where(column => selections.Any(_ => _.StartsWithIgnoreCase(Invariant($"output.{column.Name}"))))
			.Select(column => objectSchema.DataSource.Type switch
			{
				PostgreSql => column.Name.EscapeIdentifier(objectSchema.DataSource.Type),
				_ or SqlServer => Invariant($"DELETED.{column.Name.EscapeIdentifier(objectSchema.DataSource.Type)}")
			})
			.ToArray();
		var data = context.GetArgumentAsDataTable("data", objectSchema);
		var where = context.GetArgument<string>("where");
		var sql = data.Rows.Any<DataRow>() ? objectSchema.CreateDeleteSQL(data, output) : objectSchema.CreateDeleteSQL(where, output);
		var sqlCommand = objectSchema.DataSource.CreateSqlCommand(sql);

		context.GetArgument<Parameter[]>("parameters")?.ForEach(parameter => sqlCommand.Parameters[parameter.Name] = parameter.Value);

		var result = Array<DataRow>.Empty;
		if (output.Any())
			result = (await mediator.Map(sqlCommand.ToSqlDataTableRequest(), context.CancellationToken)).Select();
		else
			await mediator.Execute(sqlCommand.ToSqlExecuteRequest(), context.CancellationToken);

		return new OutputResponse<DataRow>()
		{
			TotalCount = sqlCommand.RecordsAffected,
			DataSource = objectSchema.DataSource.Name,
			Output = result,
			Sql = sql,
			Table = objectSchema.Name
		};
	}
}

public sealed class SqlApiDeleteFieldResolver<T> : FieldResolver
	where T : new()
{
	protected override async ValueTask<object?> ResolveAsync(IResolveFieldContext context)
	{
		var mediator = context.RequestServices!.GetRequiredService<IMediator>();
		var objectSchema = context.FieldDefinition.GetMetadata<ObjectSchema>(nameof(ObjectSchema));
		var selections = context.GetSelections().ToArray();
		var output = objectSchema.Columns
			.Where(column => selections.Any(_ => _.StartsWithIgnoreCase(Invariant($"output.{column.Name}"))))
			.Select(column => objectSchema.DataSource.Type switch
			{
				PostgreSql => column.Name.EscapeIdentifier(objectSchema.DataSource.Type),
				_ or SqlServer => Invariant($"DELETED.{column.Name.EscapeIdentifier(objectSchema.DataSource.Type)}")
			})
			.ToArray();
		var data = context.GetArgument<T[]>("data");
		var where = context.GetArgument<string>("where");
		var sql = data.Any() ? objectSchema.CreateDeleteSQL(data, output) : objectSchema.CreateDeleteSQL(where, output);
		var sqlCommand = objectSchema.DataSource.CreateSqlCommand(sql);

		context.GetArgument<Parameter[]>("parameters")?.ForEach(parameter => sqlCommand.Parameters[parameter.Name] = parameter.Value);

		var result = (IList<object>)Array<object>.Empty;
		if (output.Any())
			result = await mediator.Map(sqlCommand.ToSqlModelsRequest<T>(data.Length), context.CancellationToken);
		else
			await mediator.Execute(sqlCommand.ToSqlExecuteRequest(), context.CancellationToken);

		return new OutputResponse<T>()
		{
			TotalCount = sqlCommand.RecordsAffected,
			DataSource = objectSchema.DataSource.Name,
			Output = result.OfType<T>().ToArray(),
			Sql = sql,
			Table = objectSchema.Name
		};
	}
}
