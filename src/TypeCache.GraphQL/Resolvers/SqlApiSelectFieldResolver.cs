// Copyright (c) 2021 Samuel Abraham

using System.Data;
using GraphQL;
using Microsoft.Extensions.DependencyInjection;
using TypeCache.Data;
using TypeCache.Data.Extensions;
using TypeCache.Extensions;
using TypeCache.GraphQL.Data;
using TypeCache.GraphQL.Extensions;
using TypeCache.GraphQL.SqlApi;
using TypeCache.Mediation;
using static TypeCache.Data.DataSourceType;

namespace TypeCache.GraphQL.Resolvers;

public sealed class SqlApiSelectFieldResolver : FieldResolver
{
	private static readonly string DATA_ITEMS = Invariant($"{nameof(SelectResponse<>.Data)}.{nameof(Connection<>.Items)}.");
	private static readonly string DATA_EDGES_NODE = Invariant($"{nameof(SelectResponse<>.Data)}.{nameof(Connection<>.Edges)}.{nameof(Edge<>.Node)}.");
	private static readonly string DATA_TOTAL_COUNT = Invariant($"{nameof(SelectResponse<>.Data)}.{nameof(Connection<>.TotalCount)}");
	private static readonly string DATA_PAGE_INFO = Invariant($"{nameof(SelectResponse<>.Data)}.{nameof(Connection<>.PageInfo)}.");

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

		if (selections.Any(_ => _.StartsWithIgnoreCase(DATA_ITEMS) || _.StartsWithIgnoreCase(DATA_EDGES_NODE)))
		{
			var result = await mediator.Send(sqlCommand.Request.For<ValueTask<DataTable>>(), context.CancellationToken);

			if (selections.Any(_ => _.EqualsIgnoreCase(DATA_TOTAL_COUNT) || _.StartsWithIgnoreCase(DATA_PAGE_INFO)))
			{
				var countSql = objectSchema.CreateCountSQL(null, select.Where);
				var countCommand = objectSchema.DataSource.CreateSqlCommand(countSql);
				var totalCount = (int?)await mediator.Send(countCommand.Request.For<ValueTask<object?>>(), context.CancellationToken) ?? 0;

				return new SelectResponse<DataRow>()
				{
					Data = new(select.Offset, result.Select())
					{
						PageInfo = new(select.Offset, select.Fetch, totalCount),
						TotalCount = totalCount
					},
					DataSource = objectSchema.DataSource.Name,
					Sql = Invariant($"{sql}{Environment.NewLine}{Environment.NewLine}{countSql}"),
					Table = objectSchema.Name
				};
			}

			return new SelectResponse<DataRow>()
			{
				Data = new(select.Offset, result.Select()),
				DataSource = objectSchema.DataSource.Name,
				Sql = sql,
				Table = objectSchema.Name
			};
		}

		if (selections.Any(_ => _.EqualsIgnoreCase(DATA_TOTAL_COUNT) || _.StartsWithIgnoreCase(DATA_PAGE_INFO)))
		{
			var countSql = objectSchema.CreateCountSQL(null, select.Where);
			var countCommand = objectSchema.DataSource.CreateSqlCommand(countSql);
			var totalCount = (int?)await mediator.Send(countCommand.Request.For<ValueTask<object?>>(), context.CancellationToken) ?? 0;
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
	private static readonly string DATA_ITEMS = Invariant($"{nameof(SelectResponse<>.Data)}.{nameof(Connection<>.Items)}.");
	private static readonly string DATA_EDGES_NODE = Invariant($"{nameof(SelectResponse<>.Data)}.{nameof(Connection<>.Edges)}.{nameof(Edge<>.Node)}.");
	private static readonly string DATA_TOTAL_COUNT = Invariant($"{nameof(SelectResponse<>.Data)}.{nameof(Connection<>.TotalCount)}");
	private static readonly string DATA_PAGE_INFO = Invariant($"{nameof(SelectResponse<>.Data)}.{nameof(Connection<>.PageInfo)}.");

	protected override async ValueTask<object?> ResolveAsync(IResolveFieldContext context)
	{
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

		if (selections.Any(_ => _.StartsWithIgnoreCase(DATA_ITEMS) || _.StartsWithIgnoreCase(DATA_EDGES_NODE)))
		{
			var sql = objectSchema.CreateSelectSQL(select);
			var sqlCommand = objectSchema.DataSource.CreateSqlCommand(sql);

			context.GetArgument<Parameter[]>("parameters")?.ForEach(parameter => sqlCommand.Parameters[parameter.Name] = parameter.Value);

			await using var connection = sqlCommand.DataSource.CreateDbConnection();
			await connection.OpenAsync(context.CancellationToken);
			await using var dbCommand = connection.CreateCommand(sqlCommand);

			var result = await dbCommand.GetModelsAsync<T>((int)select.Fetch, context.CancellationToken);
			sqlCommand.RecordsAffected = (int?)dbCommand.Parameters[nameof(sqlCommand.RecordsAffected)]?.Value ?? 0;

			if (sqlCommand.Parameters.Any())
				dbCommand.CopyOutputParameters(sqlCommand);

			if (selections.Any(_ => _.EqualsIgnoreCase(DATA_TOTAL_COUNT) || _.StartsWithIgnoreCase(DATA_PAGE_INFO)))
			{
				var countSql = objectSchema.CreateCountSQL(null, select.Where);
				var countCommand = objectSchema.DataSource.CreateSqlCommand(countSql);

				await using var dbCountCommand = connection.CreateCommand(countCommand);
				var totalCount = await dbCountCommand.GetValueAsync<int>(context.CancellationToken) ?? 0;

				return new SelectResponse<T>()
				{
					Data = new(select.Offset, result.ToArray())
					{
						PageInfo = new(select.Offset, select.Fetch, totalCount),
						TotalCount = totalCount
					},
					DataSource = objectSchema.DataSource.Name,
					Sql = Invariant($"{sql}{Environment.NewLine}{Environment.NewLine}{countSql}"),
					Table = objectSchema.Name
				};
			}

			return new SelectResponse<T>()
			{
				Data = new(select.Offset, result.ToArray()),
				DataSource = objectSchema.DataSource.Name,
				Sql = sql,
				Table = objectSchema.Name
			};
		}

		if (selections.Any(_ => _.EqualsIgnoreCase(DATA_TOTAL_COUNT) || _.StartsWithIgnoreCase(DATA_PAGE_INFO)))
		{
			var countSql = objectSchema.CreateCountSQL(null, select.Where);
			var countCommand = objectSchema.DataSource.CreateSqlCommand(countSql);

			await using var connection = countCommand.DataSource.CreateDbConnection();
			await connection.OpenAsync(context.CancellationToken);
			await using var dbCountCommand = connection.CreateCommand(countCommand);
			var totalCount = await dbCountCommand.GetValueAsync<int>(context.CancellationToken) ?? 0;

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

		return new SelectResponse<T>()
		{
			DataSource = objectSchema.DataSource.Name,
			Table = objectSchema.Name
		};
	}
}
