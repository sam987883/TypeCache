// Copyright (c) 2021 Samuel Abraham

using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Types.Relay.DataObjects;
using Microsoft.Extensions.DependencyInjection;
using TypeCache.Data;
using TypeCache.Data.Mediation;
using TypeCache.Extensions;
using TypeCache.GraphQL.Extensions;
using TypeCache.GraphQL.SqlApi;
using TypeCache.Mediation;
using static System.FormattableString;
using static TypeCache.Data.DataSourceType;

namespace TypeCache.GraphQL.Resolvers;

public sealed class SqlApiSelectFieldResolver : FieldResolver<SelectResponse<DataRow>>
{
	protected override async ValueTask<SelectResponse<DataRow>?> ResolveAsync(IResolveFieldContext context)
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
			OrderBy = context.GetArgument<OrderBy[]>(nameof(SelectQuery.OrderBy)).Select(_ => _.ToString()).ToArray(),
			Select = objectSchema.Columns
				.Where(column => selections.Any(_ => _.Right(Invariant($"{nameof(SelectResponse<DataRow>.Items)}.{column.Name}"))
					|| _.Right(Invariant($"{nameof(SelectResponse<DataRow>.Edges)}.{nameof(Edge<DataRow>.Node)}.{column.Name}"))))
				.Select(column => column.Name)
				.ToArray(),
			TableHints = objectSchema.DataSource.Type is SqlServer ? "WITH(NOLOCK)" : null,
			Top = context.GetArgument<string>(nameof(SelectQuery.Top)),
			Where = context.GetArgument<string>(nameof(SelectQuery.Where))
		};
		var sql = objectSchema.CreateSelectSQL(select);
		var sqlCommand = objectSchema.DataSource.CreateSqlCommand(sql);

		context.GetArgument<Parameter[]>("parameters")?.ForEach(parameter => sqlCommand.Parameters[parameter.Name] = parameter.Value);

		if (selections.Any(_ => _.Left(Invariant($"{nameof(SelectResponse<DataRow>.Items)}."))
			|| _.Left(Invariant($"{nameof(SelectResponse<DataRow>.Edges)}.{nameof(Edge<DataRow>.Node)}."))))
		{
			var request = new SqlDataTableRequest { Command = sqlCommand };
			var result = await mediator.MapAsync(request, context.CancellationToken);
			var totalCount = result.Rows.Count;
			if (select.Fetch == totalCount)
			{
				var countSql = objectSchema.CreateCountSQL(null, select.Where);
				var countCommand = objectSchema.DataSource.CreateSqlCommand(countSql);
				var countRequest = new SqlScalarRequest { Command = sqlCommand };
				totalCount = (int?)await mediator.MapAsync(countRequest, context.CancellationToken) ?? 0;
			}

			return new()
			{
				DataSource = objectSchema.DataSource.Name,
				Items = result.Select(),
				PageInfo = new PageInfo
				{
					StartCursor = (select.Offset + 1).ToString(),
					EndCursor = (select.Fetch + select.Offset).ToString(),
					HasNextPage = (select.Fetch + select.Offset) < result.Rows.Count,
					HasPreviousPage = select.Offset > 0
				},
				Sql = sql,
				Table = objectSchema.Name,
				TotalCount = totalCount
			};
		}
		else if (selections.Any(_ => _.Is(nameof(SelectResponse<DataRow>.TotalCount))
			|| _.Left(Invariant($"{nameof(SelectResponse<DataRow>.PageInfo)}."))))
		{
			var countSql = objectSchema.CreateCountSQL(null, select.Where);
			var countCommand = objectSchema.DataSource.CreateSqlCommand(countSql);
			var countRequest = new SqlScalarRequest { Command = sqlCommand };
			var totalCount = (int?)await mediator.MapAsync(countRequest, context.CancellationToken) ?? 0;
			return new()
			{
				DataSource = objectSchema.DataSource.Name,
				PageInfo = new PageInfo
				{
					StartCursor = (select.Offset + 1).ToString(),
					EndCursor = (select.Fetch + select.Offset).ToString(),
					HasNextPage = (select.Fetch + select.Offset) < totalCount,
					HasPreviousPage = select.Offset > 0
				},
				Sql = countSql,
				Table = objectSchema.Name,
				TotalCount = totalCount
			};
		}
		else
			return new()
			{
				DataSource = objectSchema.DataSource.Name,
				Table = objectSchema.Name
			};
	}
}

public sealed class SqlApiSelectFieldResolver<T> : FieldResolver<SelectResponse<T>>
	where T : new()
{
	protected override async ValueTask<SelectResponse<T>?> ResolveAsync(IResolveFieldContext context)
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
			OrderBy = context.GetArgument<OrderBy[]>(nameof(SelectQuery.OrderBy)).Select(_ => _.ToString()).ToArray(),
			Select = objectSchema.Columns
				.Where(column => selections.Any(_ => _.Right(Invariant($"{nameof(SelectResponse<T>.Items)}.{column.Name}"))
					|| _.Right(Invariant($"{nameof(SelectResponse<T>.Edges)}.{nameof(Edge<T>.Node)}.{column.Name}"))))
				.Select(column => column.Name)
				.ToArray(),
			TableHints = objectSchema.DataSource.Type is SqlServer ? "WITH(NOLOCK)" : null,
			Top = context.GetArgument<string>(nameof(SelectQuery.Top)),
			Where = context.GetArgument<string>(nameof(SelectQuery.Where))
		};
		var sql = objectSchema.CreateSelectSQL(select);
		var sqlCommand = objectSchema.DataSource.CreateSqlCommand(sql);

		context.GetArgument<Parameter[]>("parameters")?.ForEach(parameter => sqlCommand.Parameters[parameter.Name] = parameter.Value);

		if (selections.Any(_ => _.Left(Invariant($"{nameof(SelectResponse<T>.Items)}."))
			|| _.Left(Invariant($"{nameof(SelectResponse<T>.Edges)}.{nameof(Edge<T>.Node)}."))))
		{
			var request = new SqlDataTableRequest { Command = sqlCommand };
			var result = (IList<T>)await mediator.MapAsync(request, context.CancellationToken);
			var totalCount = result.Count;
			if (select.Fetch == totalCount)
			{
				var countSql = objectSchema.CreateCountSQL(null, select.Where);
				var countCommand = objectSchema.DataSource.CreateSqlCommand(countSql);
				var countRequest = new SqlScalarRequest { Command = sqlCommand };
				totalCount = (int?)await mediator.MapAsync(countRequest, context.CancellationToken) ?? 0;
			}

			return new()
			{
				DataSource = objectSchema.DataSource.Name,
				Items = result,
				PageInfo = new PageInfo
				{
					StartCursor = (select.Offset + 1).ToString(),
					EndCursor = (select.Fetch + select.Offset).ToString(),
					HasNextPage = (select.Fetch + select.Offset) < result.Count,
					HasPreviousPage = select.Offset > 0
				},
				Sql = sql,
				Table = objectSchema.Name,
				TotalCount = totalCount
			};
		}
		else if (selections.Any(_ => _.Is(nameof(SelectResponse<T>.TotalCount))
			|| _.Left(Invariant($"{nameof(SelectResponse<T>.PageInfo)}."))))
		{
			var countSql = objectSchema.CreateCountSQL(null, select.Where);
			var countCommand = objectSchema.DataSource.CreateSqlCommand(countSql);
			var countRequest = new SqlScalarRequest { Command = sqlCommand };
			var totalCount = (int?)await mediator.MapAsync(countRequest, context.CancellationToken) ?? 0;
			return new()
			{
				DataSource = objectSchema.DataSource.Name,
				PageInfo = new PageInfo
				{
					StartCursor = (select.Offset + 1).ToString(),
					EndCursor = (select.Fetch + select.Offset).ToString(),
					HasNextPage = (select.Fetch + select.Offset) < totalCount,
					HasPreviousPage = select.Offset > 0
				},
				Sql = countSql,
				Table = objectSchema.Name,
				TotalCount = totalCount
			};
		}
		else
			return new()
			{
				DataSource = objectSchema.DataSource.Name,
				Table = objectSchema.Name
			};
	}
}
