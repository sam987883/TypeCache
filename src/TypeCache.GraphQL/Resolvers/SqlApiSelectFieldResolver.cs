// Copyright (c) 2021 Samuel Abraham

using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Types.Relay.DataObjects;
using Microsoft.Extensions.DependencyInjection;
using TypeCache.Business;
using TypeCache.Collections.Extensions;
using TypeCache.Data;
using TypeCache.GraphQL.Extensions;
using TypeCache.GraphQL.SqlApi;
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
			OrderBy = context.GetArgument<OrderBy[]>(nameof(SelectQuery.OrderBy)).Map(_ => _.ToString()),
			Select = objectSchema.Columns
				.If(column => selections.AnyRight(Invariant($"{nameof(SelectResponse<DataRow>.Items)}.{column.Name}"))
					|| selections.AnyRight(Invariant($"{nameof(SelectResponse<DataRow>.Edges)}.{nameof(Edge<DataRow>.Node)}.{column.Name}")))
				.Map(column => column.Name),
			TableHints = objectSchema.DataSource.Type is SqlServer ? "WITH(NOLOCK)" : null,
			Top = context.GetArgument<string>(nameof(SelectQuery.Top)),
			Where = context.GetArgument<string>(nameof(SelectQuery.Where))
		};
		var sql = objectSchema.CreateSelectSQL(select);
		var sqlCommand = objectSchema.DataSource.CreateSqlCommand(sql);

		context.GetArgument<Parameter[]>("parameters")?.Do(parameter => sqlCommand.Parameters[parameter.Name] = parameter.Value);

		if (selections.AnyLeft(Invariant($"{nameof(SelectResponse<DataRow>.Items)}."))
			|| selections.AnyLeft(Invariant($"{nameof(SelectResponse<DataRow>.Edges)}.{nameof(Edge<DataRow>.Node)}.")))
		{
			var result = await mediator.ApplyRuleAsync<SqlCommand, DataTable>(sqlCommand, context.CancellationToken);
			var totalCount = result.Rows.Count;
			if (select.Fetch == totalCount)
			{
				var countSql = objectSchema.CreateCountSQL(null, select.Where);
				totalCount = await mediator.ApplyRuleAsync<SqlCommand, int>(sqlCommand, context.CancellationToken);
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
		else if (selections.Has(nameof(SelectResponse<DataRow>.TotalCount))
			|| selections.AnyLeft(Invariant($"{nameof(SelectResponse<DataRow>.PageInfo)}.")))
		{
			var countSql = objectSchema.CreateCountSQL(null, select.Where);
			var totalCount = await mediator.ApplyRuleAsync<SqlCommand, int>(sqlCommand, context.CancellationToken);
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
			OrderBy = context.GetArgument<OrderBy[]>(nameof(SelectQuery.OrderBy)).Map(_ => _.ToString()),
			Select = objectSchema.Columns
				.If(column => selections.AnyRight(Invariant($"{nameof(SelectResponse<T>.Items)}.{column.Name}"))
					|| selections.AnyRight(Invariant($"{nameof(SelectResponse<T>.Edges)}.{nameof(Edge<T>.Node)}.{column.Name}")))
				.Map(column => column.Name),
			TableHints = objectSchema.DataSource.Type is SqlServer ? "WITH(NOLOCK)" : null,
			Top = context.GetArgument<string>(nameof(SelectQuery.Top)),
			Where = context.GetArgument<string>(nameof(SelectQuery.Where))
		};
		var sql = objectSchema.CreateSelectSQL(select);
		var sqlCommand = objectSchema.DataSource.CreateSqlCommand(sql);

		context.GetArgument<Parameter[]>("parameters")?.Do(parameter => sqlCommand.Parameters[parameter.Name] = parameter.Value);

		if (selections.AnyLeft(Invariant($"{nameof(SelectResponse<T>.Items)}."))
			|| selections.AnyLeft(Invariant($"{nameof(SelectResponse<T>.Edges)}.{nameof(Edge<T>.Node)}.")))
		{
			var result = await mediator.ApplyRuleAsync<SqlCommand, IList<T>>(sqlCommand, context.CancellationToken);
			var totalCount = result.Count;
			if (select.Fetch == totalCount)
			{
				var countSql = objectSchema.CreateCountSQL(null, select.Where);
				totalCount = await mediator.ApplyRuleAsync<SqlCommand, int>(sqlCommand, context.CancellationToken);
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
		else if (selections.Has(nameof(SelectResponse<T>.TotalCount))
			|| selections.AnyLeft(Invariant($"{nameof(SelectResponse<T>.PageInfo)}.")))
		{
			var countSql = objectSchema.CreateCountSQL(null, select.Where);
			var totalCount = await mediator.ApplyRuleAsync<SqlCommand, int>(sqlCommand, context.CancellationToken);
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
