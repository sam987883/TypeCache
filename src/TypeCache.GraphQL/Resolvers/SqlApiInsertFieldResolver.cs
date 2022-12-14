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

public class SqlApiInsertFieldResolver : FieldResolver<OutputResponse<DataRow>>
{
	protected override async ValueTask<OutputResponse<DataRow>?> ResolveAsync(IResolveFieldContext context)
	{
		var mediator = context.RequestServices!.GetRequiredService<IMediator>();
		var objectSchema = context.FieldDefinition.GetMetadata<ObjectSchema>(nameof(ObjectSchema));
		var selections = context.GetSelections().ToArray();
		var select = context.GetArgument<string[]>(nameof(SelectQuery.Select));
		var output = selections
			.Where(column => selections.Any(_ => _.Left(Invariant($"{nameof(OutputResponse<DataRow>.Output)}.{column}"))))
			.Select(column => objectSchema.DataSource.Type switch
			{
				PostgreSql => objectSchema.DataSource.EscapeIdentifier(column),
				_ or SqlServer => Invariant($"INSERTED.{objectSchema.DataSource.EscapeIdentifier(column)}")
			})
			.ToArray();
		var columns = context.GetArgument<string[]>("columns");
		var data = context.GetArgumentAsDataTable("data", objectSchema);
		var sql = data.Rows.Any<DataRow>()
			? objectSchema.CreateInsertSQL(data, output)
			: objectSchema.CreateInsertSQL(columns, new SelectQuery
			{
				Distinct = context.GetArgument<bool>(nameof(SelectQuery.Distinct)),
				From = objectSchema.DataSource.CreateName(context.GetArgument<string>(nameof(SelectQuery.From))),
				Fetch = context.GetArgument<uint>(nameof(SelectQuery.Fetch)),
				Offset = context.GetArgument<uint>(nameof(SelectQuery.Offset)),
				OrderBy = context.GetArgument<OrderBy[]>(nameof(SelectQuery.OrderBy)).Select(_ => _.ToString()).ToArray(),
				Select = objectSchema.Columns
					.Where(column => select.Any(_ => _.Right(Invariant($"{nameof(SelectQuery.Select)}.{column.Name}"))))
					.Select(column => column.Name)
					.ToArray(),
				TableHints = objectSchema.DataSource.Type is SqlServer ? "WITH(NOLOCK)" : null,
				Top = context.GetArgument<string>(nameof(SelectQuery.Top)),
				Where = context.GetArgument<string>(nameof(SelectQuery.Where))
			}, output);
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

public class SqlApiInsertFieldResolver<T> : FieldResolver<OutputResponse<T>>
	where T : new()
{
	protected override async ValueTask<OutputResponse<T>?> ResolveAsync(IResolveFieldContext context)
	{
		var mediator = context.RequestServices!.GetRequiredService<IMediator>();
		var objectSchema = context.FieldDefinition.GetMetadata<ObjectSchema>(nameof(ObjectSchema));
		var selections = context.GetSelections().ToArray();
		var select = context.GetArgument<string[]>(nameof(SelectQuery.Select));
		var output = selections
			.Where(column => selections.Any(_ => _.Left(Invariant($"{nameof(OutputResponse<T>.Output)}.{column}"))))
			.Select(column => objectSchema.DataSource.Type switch
			{
				PostgreSql => objectSchema.DataSource.EscapeIdentifier(column),
				_ or SqlServer => Invariant($"INSERTED.{objectSchema.DataSource.EscapeIdentifier(column)}")
			})
			.ToArray();
		var columns = context.GetArgument<string[]>("columns");
		var data = context.GetArgument<T[]>("data");
		var sql = data.Any()
			? objectSchema.CreateInsertSQL(columns, data, output)
			: objectSchema.CreateInsertSQL(columns, new SelectQuery
			{
				Distinct = context.GetArgument<bool>(nameof(SelectQuery.Distinct)),
				From = objectSchema.DataSource.CreateName(context.GetArgument<string>(nameof(SelectQuery.From))),
				Fetch = context.GetArgument<uint>(nameof(SelectQuery.Fetch)),
				Offset = context.GetArgument<uint>(nameof(SelectQuery.Offset)),
				OrderBy = context.GetArgument<OrderBy[]>(nameof(SelectQuery.OrderBy)).Select(_ => _.ToString()).ToArray(),
				Select = objectSchema.Columns
					.Where(column => select.Any(_ => _.Right(Invariant($"{nameof(SelectQuery.Select)}.{column.Name}"))))
					.Select(column => column.Name)
					.ToArray(),
				TableHints = objectSchema.DataSource.Type is SqlServer ? "WITH(NOLOCK)" : null,
				Top = context.GetArgument<string>(nameof(SelectQuery.Top)),
				Where = context.GetArgument<string>(nameof(SelectQuery.Where))
			}, output);
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
