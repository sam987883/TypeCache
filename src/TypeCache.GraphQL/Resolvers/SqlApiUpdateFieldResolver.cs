// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using GraphQL;
using Microsoft.Extensions.DependencyInjection;
using TypeCache.Collections;
using TypeCache.Data;
using TypeCache.Data.Extensions;
using TypeCache.Extensions;
using TypeCache.GraphQL.Extensions;
using TypeCache.GraphQL.SqlApi;
using TypeCache.Mediation;
using static System.FormattableString;
using static TypeCache.Data.DataSourceType;

namespace TypeCache.GraphQL.Resolvers;

public sealed class SqlApiUpdateFieldResolver : FieldResolver
{
	protected override async ValueTask<object?> ResolveAsync(IResolveFieldContext context)
	{
		var mediator = context.RequestServices!.GetRequiredService<IMediator>();
		var objectSchema = context.FieldDefinition.GetMetadata<ObjectSchema>(nameof(ObjectSchema));
		var inputs = context.GetInputs().Keys.ToArray();
		var selections = context.GetSelections().ToArray();
		var output = objectSchema.Columns
			.Where(column => selections.Any(_ => _.Left(Invariant($"output.{column.Name}"))))
			.Select(column => objectSchema.DataSource.Type switch
			{
				PostgreSql => column.Name.EscapeIdentifier(objectSchema.DataSource.Type),
				_ or SqlServer => Invariant($"INSERTED.{column.Name.EscapeIdentifier(objectSchema.DataSource.Type)}")
			})
			.ToArray();
		var data = context.GetArgumentAsDataTable("data", objectSchema);
		var columns = objectSchema.Columns
			.Where(column => inputs.Any(_ => _.Right(Invariant($"{nameof(data)}.{column.Name}"))))
			.Select(column => column.Name);
		var set = context.GetArgument<string[]>("set");
		var where = context.GetArgument<string>("where");

		if (columns.Any())
			data.Columns
				.OfType<DataColumn>()
				.Where(column => !columns.Contains(column.ColumnName, StringComparer.OrdinalIgnoreCase))
				.ToArray()
				.ForEach(data.Columns.Remove);

		var sql = data.Rows.Any<DataRow>()
			? objectSchema.CreateUpdateSQL(data, output)
			: objectSchema.CreateUpdateSQL(set, where, output);
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

public sealed class SqlApiUpdateFieldResolver<T> : FieldResolver
	where T : new()
{
	protected override async ValueTask<object?> ResolveAsync(IResolveFieldContext context)
	{
		var mediator = context.RequestServices!.GetRequiredService<IMediator>();
		var objectSchema = context.FieldDefinition.GetMetadata<ObjectSchema>(nameof(ObjectSchema));
		var inputs = context.GetInputs().Keys.ToArray();
		var selections = context.GetSelections().ToArray();
		var output = objectSchema.Columns
			.Where(column => selections.Any(_ => _.Left(Invariant($"output.{column.Name}"))))
			.Select(column => objectSchema.DataSource.Type switch
			{
				PostgreSql => column.Name.EscapeIdentifier(objectSchema.DataSource.Type),
				_ or SqlServer => Invariant($"INSERTED.{column.Name.EscapeIdentifier(objectSchema.DataSource.Type)}")
			})
			.ToArray();
		var data = context.GetArgument<T[]>("data");
		var columns = objectSchema.Columns
			.Where(column => inputs.Any(_ => _.Right(Invariant($"{nameof(data)}.{column.Name}"))))
			.Select(column => column.Name)
			.ToArray();
		var set = context.GetArgument<string[]>("set");
		var where = context.GetArgument<string>("where");
		var sql = data.Any()
			? objectSchema.CreateUpdateSQL(columns, data, output)
			: objectSchema.CreateUpdateSQL(set, where, output);
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
