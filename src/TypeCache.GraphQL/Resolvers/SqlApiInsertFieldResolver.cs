﻿// Copyright (c) 2021 Samuel Abraham

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

public sealed class SqlApiInsertFieldResolver : FieldResolver
{
	protected override async ValueTask<object?> ResolveAsync(IResolveFieldContext context)
	{
		var mediator = context.RequestServices!.GetRequiredService<IMediator>();
		var objectSchema = context.FieldDefinition.GetMetadata<ObjectSchema>(nameof(ObjectSchema));
		var selections = context.GetSelections().ToArray();
		var select = context.GetArgument<string[]>(nameof(SelectQuery.Select));
		var output = objectSchema.Columns
			.Where(column => selections.Any(_ => _.StartsWithIgnoreCase(Invariant($"output.{column.Name}"))))
			.Select(column => objectSchema.DataSource.Type switch
			{
				PostgreSql => column.Name.EscapeIdentifier(objectSchema.DataSource.Type),
				_ or SqlServer => Invariant($"INSERTED.{column.Name.EscapeIdentifier(objectSchema.DataSource.Type)}")
			})
			.ToArray();
		var columns = context.GetArgument<string[]>("columns");
		var data = context.GetArgumentAsDataTable("data", objectSchema);

		string sql;
		if (data.Rows.Any<DataRow>())
			sql = objectSchema.CreateInsertSQL(data, output);
		else
		{
			var top = context.GetArgument<string?>(nameof(SelectQuery.Top));
			sql = objectSchema.CreateInsertSQL(columns, new SelectQuery
			{
				Distinct = context.GetArgument<bool>(nameof(SelectQuery.Distinct)),
				From = objectSchema.DataSource.Escape(context.GetArgument<string>(nameof(SelectQuery.From))),
				Fetch = context.GetArgument<uint>(nameof(SelectQuery.Fetch)),
				Offset = context.GetArgument<uint>(nameof(SelectQuery.Offset)),
				OrderBy = context.GetArgument<string[]>(nameof(SelectQuery.OrderBy)),
				Select = objectSchema.Columns
					.Where(column => select.Any(_ => _.EndsWithIgnoreCase(Invariant($"{nameof(SelectQuery.Select)}.{column.Name}"))))
					.Select(column => column.Name)
					.ToArray(),
				TableHints = objectSchema.DataSource.Type is SqlServer ? "NOLOCK" : null,
				Top = top.IsNotBlank() ? top.TrimEnd('%').Parse<uint>() : null,
				TopPercent = top?.EndsWith('%') is true,
				Where = context.GetArgument<string>(nameof(SelectQuery.Where))
			}, output);
		}

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

public sealed class SqlApiInsertFieldResolver<T> : FieldResolver
	where T : new()
{
	protected override async ValueTask<object?> ResolveAsync(IResolveFieldContext context)
	{
		var mediator = context.RequestServices!.GetRequiredService<IMediator>();
		var objectSchema = context.FieldDefinition.GetMetadata<ObjectSchema>(nameof(ObjectSchema));
		var selections = context.GetSelections().ToArray();
		var select = context.GetArgument<string[]>(nameof(SelectQuery.Select));
		var output = objectSchema.Columns
			.Where(column => selections.Any(_ => _.StartsWithIgnoreCase(Invariant($"output.{column.Name}"))))
			.Select(column => objectSchema.DataSource.Type switch
			{
				PostgreSql => column.Name.EscapeIdentifier(objectSchema.DataSource.Type),
				_ or SqlServer => Invariant($"INSERTED.{column.Name.EscapeIdentifier(objectSchema.DataSource.Type)}")
			})
			.ToArray();
		var columns = context.GetArgument<string[]>("columns");
		var data = context.GetArgument<T[]>("data");

		string sql;
		if (data.Any())
			sql = objectSchema.CreateInsertSQL(columns, data, output);
		else
		{
			var top = context.GetArgument<string?>(nameof(SelectQuery.Top));
			sql = objectSchema.CreateInsertSQL(columns, new SelectQuery
			{
				Distinct = context.GetArgument<bool>(nameof(SelectQuery.Distinct)),
				From = objectSchema.DataSource.Escape(context.GetArgument<string>(nameof(SelectQuery.From))),
				Fetch = context.GetArgument<uint>(nameof(SelectQuery.Fetch)),
				Offset = context.GetArgument<uint>(nameof(SelectQuery.Offset)),
				OrderBy = context.GetArgument<string[]>(nameof(SelectQuery.OrderBy)),
				Select = objectSchema.Columns
					.Where(column => select.Any(_ => _.EndsWithIgnoreCase(Invariant($"{nameof(SelectQuery.Select)}.{column.Name}"))))
					.Select(column => column.Name)
					.ToArray(),
				TableHints = objectSchema.DataSource.Type is SqlServer ? "NOLOCK" : null,
				Top = top.IsNotBlank() ? top.TrimEnd('%').Parse<uint>() : null,
				TopPercent = top?.EndsWith('%') is true,
				Where = context.GetArgument<string>(nameof(SelectQuery.Where))
			}, output);
		}

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
