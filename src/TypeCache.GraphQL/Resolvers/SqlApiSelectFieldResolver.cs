// Copyright (c) 2021 Samuel Abraham

using System.Data;
using System.Linq;
using System.Threading.Tasks;
using GraphQL;
using Microsoft.Extensions.DependencyInjection;
using TypeCache.Data;
using TypeCache.Data.Extensions;
using TypeCache.Extensions;
using TypeCache.GraphQL.Data;
using TypeCache.GraphQL.Extensions;
using TypeCache.GraphQL.SqlApi;
using TypeCache.Mediation;
using static System.FormattableString;
using static TypeCache.Data.DataSourceType;

namespace TypeCache.GraphQL.Resolvers;

public sealed class SqlApiSelectFieldResolver : FieldResolver
{
	protected override async ValueTask<object?> ResolveAsync(IResolveFieldContext context)
	{
		var mediator = context.RequestServices!.GetRequiredService<IMediator>();
		var objectSchema = context.FieldDefinition.GetMetadata<ObjectSchema>(nameof(ObjectSchema));
		var selections = context.GetSelections().ToArray();
		var select = new SelectQuery
		{
			Distinct = context.GetArgument<bool>(nameof(SelectQuery.Distinct)),
			Fetch = context.GetArgument<uint>(nameof(SelectQuery.Fetch)),
			From = objectSchema.Name,
			Offset = context.GetArgument<uint>(nameof(SelectQuery.Offset)),
			OrderBy = context.GetArgument<string[]>(nameof(SelectQuery.OrderBy)),
			Select = objectSchema.Columns
				.Where(column => selections.Any(_ => _.Right(Invariant($"{nameof(Connection<DataRow>.Items)}.{column.Name}"))
					|| _.Right(Invariant($"{nameof(Connection<DataRow>.Edges)}.{nameof(Edge<DataRow>.Node)}.{column.Name}"))))
				.Select(column => column.Name)
				.ToArray(),
			TableHints = objectSchema.DataSource.Type is SqlServer ? "WITH(NOLOCK)" : null,
			Top = context.GetArgument<string>(nameof(SelectQuery.Top)),
			Where = context.GetArgument<string>(nameof(SelectQuery.Where))
		};
		var sql = objectSchema.CreateSelectSQL(select);
		var sqlCommand = objectSchema.DataSource.CreateSqlCommand(sql);

		context.GetArgument<Parameter[]>("parameters")?.ForEach(parameter => sqlCommand.Parameters[parameter.Name] = parameter.Value);

		if (selections.Any(_ => _.Left(Invariant($"{nameof(SelectResponse<DataRow>.Data)}.{nameof(Connection<DataRow>.Items)}."))
			|| _.Left(Invariant($"{nameof(SelectResponse<DataRow>.Data)}.{nameof(Connection<DataRow>.Edges)}.{nameof(Edge<DataRow>.Node)}."))))
		{
			var result = await mediator.MapAsync(sqlCommand.ToSqlDataTableRequest(), context.CancellationToken);
			var totalCount = result.Rows.Count;
			if (select.Fetch == totalCount)
			{
				var countSql = objectSchema.CreateCountSQL(null, select.Where);
				var countCommand = objectSchema.DataSource.CreateSqlCommand(countSql);
				totalCount = (int?)await mediator.MapAsync(countCommand.ToSqlScalarRequest(), context.CancellationToken) ?? 0;
			}

			var rows = result.Select();
			return new SelectResponse<DataRow>(rows, totalCount, select.Offset)
			{
				DataSource = objectSchema.DataSource.Name,
				Sql = sql,
				Table = objectSchema.Name,
			};
		}
		else if (selections.Any(_ => _.Is(Invariant($"{nameof(SelectResponse<DataRow>.Data)}.{nameof(Connection<DataRow>.TotalCount)}"))
			|| _.Left(Invariant($"{nameof(SelectResponse<DataRow>.Data)}.{nameof(Connection<DataRow>.PageInfo)}."))))
		{
			var countSql = objectSchema.CreateCountSQL(null, select.Where);
			var countCommand = objectSchema.DataSource.CreateSqlCommand(countSql);
			var totalCount = (int?)await mediator.MapAsync(countCommand.ToSqlScalarRequest(), context.CancellationToken) ?? 0;
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
		var select = new SelectQuery
		{
			Distinct = context.GetArgument<bool>(nameof(SelectQuery.Distinct)),
			Fetch = context.GetArgument<uint>(nameof(SelectQuery.Fetch)),
			From = objectSchema.Name,
			Offset = context.GetArgument<uint>(nameof(SelectQuery.Offset)),
			OrderBy = context.GetArgument<string[]>(nameof(SelectQuery.OrderBy)),
			Select = objectSchema.Columns
				.Where(column => selections.Any(_ => _.Right(Invariant($"{nameof(Connection<T>.Items)}.{column.Name}"))
					|| _.Right(Invariant($"{nameof(Connection<T>.Edges)}.{nameof(Edge<T>.Node)}.{column.Name}"))))
				.Select(column => column.Name)
				.ToArray(),
			TableHints = objectSchema.DataSource.Type is SqlServer ? "WITH(NOLOCK)" : null,
			Top = context.GetArgument<string>(nameof(SelectQuery.Top)),
			Where = context.GetArgument<string>(nameof(SelectQuery.Where))
		};
		var sql = objectSchema.CreateSelectSQL(select);
		var sqlCommand = objectSchema.DataSource.CreateSqlCommand(sql);

		context.GetArgument<Parameter[]>("parameters")?.ForEach(parameter => sqlCommand.Parameters[parameter.Name] = parameter.Value);

		if (selections.Any(_ => _.Left(Invariant($"{nameof(SelectResponse<T>.Data)}.{nameof(Connection<T>.Items)}."))
			|| _.Left(Invariant($"{nameof(SelectResponse<T>.Data)}.{nameof(Connection<T>.Edges)}.{nameof(Edge<T>.Node)}."))))
		{
			var result = await mediator.MapAsync(sqlCommand.ToSqlModelsRequest<T>((int)select.Fetch), context.CancellationToken);
			var totalCount = result.Count;
			if (select.Fetch == totalCount)
			{
				var countSql = objectSchema.CreateCountSQL(null, select.Where);
				var countCommand = objectSchema.DataSource.CreateSqlCommand(countSql);
				totalCount = (int?)await mediator.MapAsync(countCommand.ToSqlScalarRequest(), context.CancellationToken) ?? 0;
			}

			var items = result.OfType<T>().ToArray();
			return new SelectResponse<T>(items, totalCount, select.Offset)
			{
				DataSource = objectSchema.DataSource.Name,
				Sql = sql,
				Table = objectSchema.Name
			};
		}
		else if (selections.Any(_ => _.Is(Invariant($"{nameof(SelectResponse<T>.Data)}.{nameof(Connection<T>.TotalCount)}"))
			|| _.Left(Invariant($"{nameof(SelectResponse<T>.Data)}.{nameof(Connection<T>.PageInfo)}."))))
		{
			var countSql = objectSchema.CreateCountSQL(null, select.Where);
			var countCommand = objectSchema.DataSource.CreateSqlCommand(countSql);
			var totalCount = (int?)await mediator.MapAsync(countCommand.ToSqlScalarRequest(), context.CancellationToken) ?? 0;
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
