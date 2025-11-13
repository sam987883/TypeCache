// Copyright (c) 2021 Samuel Abraham

using System.Data;
using GraphQL;
using Microsoft.Extensions.DependencyInjection;
using TypeCache.Data;
using TypeCache.Data.Mediation;
using TypeCache.Extensions;
using TypeCache.GraphQL.Data;
using TypeCache.GraphQL.Extensions;
using TypeCache.GraphQL.SqlApi;
using TypeCache.Mediation;
using static TypeCache.Data.DataSourceType;

namespace TypeCache.GraphQL.Resolvers;

public sealed class SqlApiSelectFieldResolver : FieldResolver
{
	protected override async ValueTask<object?> ResolveAsync(IResolveFieldContext context)
	{
		var mediator = context.RequestServices!.GetRequiredService<IMediator>();
		var objectSchema = context.FieldDefinition.GetMetadata<ObjectSchema>(nameof(ObjectSchema));
		var selections = context.GetSelections().ToArray();
		var top = context.GetArgument<string?>(nameof(SelectQuery.Top));
		var select = new SelectQuery
		{
			Distinct = context.GetArgument<bool>(nameof(SelectQuery.Distinct)),
			Fetch = context.GetArgument<uint>(nameof(SelectQuery.Fetch)),
			From = objectSchema.Name,
			Offset = context.GetArgument<uint>(nameof(SelectQuery.Offset)),
			OrderBy = context.GetArgument<string[]>(nameof(SelectQuery.OrderBy)),
			Select = objectSchema.Columns
				.Where(column => selections.Any(_ => _.EndsWithIgnoreCase(Invariant($"{nameof(Connection<>.Items)}.{column.Name}"))
					|| _.EndsWithIgnoreCase(Invariant($"{nameof(Connection<>.Edges)}.{nameof(Edge<>.Node)}.{column.Name}"))))
				.Select(column => column.Name)
				.ToArray(),
			TableHints = objectSchema.DataSource.Type is SqlServer ? "NOLOCK" : null,
			Top = top.IsNotBlank ? top.TrimEnd('%').Parse<uint>() : null,
			TopPercent = top?.EndsWith('%') is true,
			Where = context.GetArgument<string>(nameof(SelectQuery.Where))
		};
		var sql = objectSchema.CreateSelectSQL(select);
		var sqlCommand = objectSchema.DataSource.CreateSqlCommand(sql);

		context.GetArgument<Parameter[]>("parameters")?.ForEach(parameter => sqlCommand.Parameters[parameter.Name] = parameter.Value);

		if (selections.Any(_ => _.StartsWithIgnoreCase(Invariant($"{nameof(SelectResponse<>.Data)}.{nameof(Connection<>.Items)}."))
			|| _.StartsWithIgnoreCase(Invariant($"{nameof(SelectResponse<>.Data)}.{nameof(Connection<>.Edges)}.{nameof(Edge<>.Node)}."))))
		{
			var result = await mediator.Request<DataTable>().Send(sqlCommand, context.CancellationToken);
			var totalCount = result.Rows.Count;
			if (select.Fetch == totalCount)
			{
				var countSql = objectSchema.CreateCountSQL(null, select.Where);
				var countCommand = objectSchema.DataSource.CreateSqlCommand(countSql);
				totalCount = (int?)await mediator.Request<object?>().Send(countCommand, context.CancellationToken) ?? 0;
			}

			var rows = result.Select();
			return new SelectResponse<DataRow>(rows, totalCount, select.Offset)
			{
				DataSource = objectSchema.DataSource.Name,
				Sql = sql,
				Table = objectSchema.Name,
			};
		}
		else if (selections.Any(_ => _.EqualsIgnoreCase(Invariant($"{nameof(SelectResponse<>.Data)}.{nameof(Connection<>.TotalCount)}"))
			|| _.StartsWithIgnoreCase(Invariant($"{nameof(SelectResponse<>.Data)}.{nameof(Connection<>.PageInfo)}."))))
		{
			var countSql = objectSchema.CreateCountSQL(null, select.Where);
			var countCommand = objectSchema.DataSource.CreateSqlCommand(countSql);
			var totalCount = (int?)await mediator.Request<object?>().Send(countCommand, context.CancellationToken) ?? 0;
			return new SelectResponse<DataRow>()
			{
				Data = new()
				{
					PageInfo = new(select.Offset, select.Fetch, totalCount),
					TotalCount = totalCount
				},
				DataSource = objectSchema.DataSource.Name,
				Sql = countSql,
				Table = objectSchema.Name
			};
		}
		else
			return new SelectResponse<DataRow>()
			{
				DataSource = objectSchema.DataSource.Name,
				Table = objectSchema.Name
			};
	}
}

public sealed class SqlApiSelectFieldResolver<T> : FieldResolver
	where T : notnull, new()
{
	protected override async ValueTask<object?> ResolveAsync(IResolveFieldContext context)
	{
		var mediator = context.RequestServices!.GetRequiredService<IMediator>();
		var objectSchema = context.FieldDefinition.GetMetadata<ObjectSchema>(nameof(ObjectSchema));
		var selections = context.GetSelections().ToArray();
		var top = context.GetArgument<string?>(nameof(SelectQuery.Top));
		var select = new SelectQuery
		{
			Distinct = context.GetArgument<bool>(nameof(SelectQuery.Distinct)),
			Fetch = context.GetArgument<uint>(nameof(SelectQuery.Fetch)),
			From = objectSchema.Name,
			Offset = context.GetArgument<uint>(nameof(SelectQuery.Offset)),
			OrderBy = context.GetArgument<string[]>(nameof(SelectQuery.OrderBy)),
			Select = objectSchema.Columns
				.Where(column => selections.Any(_ => _.EndsWithIgnoreCase(Invariant($"{nameof(Connection<>.Items)}.{column.Name}"))
					|| _.EndsWithIgnoreCase(Invariant($"{nameof(Connection<>.Edges)}.{nameof(Edge<>.Node)}.{column.Name}"))))
				.Select(column => column.Name)
				.ToArray(),
			TableHints = objectSchema.DataSource.Type is SqlServer ? "NOLOCK" : null,
			Top = top.IsNotBlank ? top.TrimEnd('%').Parse<uint>() : null,
			TopPercent = top?.EndsWith('%') is true,
			Where = context.GetArgument<string>(nameof(SelectQuery.Where))
		};
		var sql = objectSchema.CreateSelectSQL(select);
		var sqlCommand = objectSchema.DataSource.CreateSqlCommand(sql);

		context.GetArgument<Parameter[]>("parameters")?.ForEach(parameter => sqlCommand.Parameters[parameter.Name] = parameter.Value);

		if (selections.Any(_ => _.StartsWithIgnoreCase(Invariant($"{nameof(SelectResponse<>.Data)}.{nameof(Connection<>.Items)}."))
			|| _.StartsWithIgnoreCase(Invariant($"{nameof(SelectResponse<>.Data)}.{nameof(Connection<>.Edges)}.{nameof(Edge<>.Node)}."))))
		{
			var result = await mediator.Request<IList<T>>().Send(new SqlResultsRequest<T>(sqlCommand, (int)select.Fetch), context.CancellationToken);
			var totalCount = result.Count;
			if (select.Fetch == totalCount)
			{
				var countSql = objectSchema.CreateCountSQL(null, select.Where);
				var countCommand = objectSchema.DataSource.CreateSqlCommand(countSql);
				totalCount = (int?)await mediator.Request<object?>().Send(countCommand, context.CancellationToken) ?? 0;
			}

			var items = result.OfType<T>().ToArray();
			return new SelectResponse<T>(items, totalCount, select.Offset)
			{
				DataSource = objectSchema.DataSource.Name,
				Sql = sql,
				Table = objectSchema.Name
			};
		}
		else if (selections.Any(_ => _.EqualsIgnoreCase(Invariant($"{nameof(SelectResponse<>.Data)}.{nameof(Connection<>.TotalCount)}"))
			|| _.StartsWithIgnoreCase(Invariant($"{nameof(SelectResponse<>.Data)}.{nameof(Connection<>.PageInfo)}."))))
		{
			var countSql = objectSchema.CreateCountSQL(null, select.Where);
			var countCommand = objectSchema.DataSource.CreateSqlCommand(countSql);
			var totalCount = (int?)await mediator.Request<object?>().Send(countCommand, context.CancellationToken) ?? 0;
			return new SelectResponse<T>()
			{
				Data = new()
				{
					PageInfo = new(select.Offset, select.Fetch, totalCount),
					TotalCount = totalCount
				},
				DataSource = objectSchema.DataSource.Name,
				Sql = countSql,
				Table = objectSchema.Name
			};
		}
		else
			return new SelectResponse<T>()
			{
				DataSource = objectSchema.DataSource.Name,
				Table = objectSchema.Name
			};
	}
}
