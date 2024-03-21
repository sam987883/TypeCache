// Copyright (c) 2021 Samuel Abraham

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using TypeCache.Data;
using TypeCache.Mediation;

namespace TypeCache.Web.Filters;

public sealed class SqlApiEndpointFilter
	: IEndpointFilter
{
	private const string DATASOURCE = "dataSource";
	private const string DATABASE = "database";
	private const string SCHEMA = "schema";
	private const string TABLE = "table";
	private const string VIEW = "view";
	private const string PROCEDURE = "procedure";

	private readonly IServiceProvider _ServiceProvider;
	private readonly IMediator _Mediator;

	public SqlApiEndpointFilter(IServiceProvider serviceProvider, IMediator mediator)
	{
		this._ServiceProvider = serviceProvider;
		this._Mediator = mediator;
	}

	public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
	{
		var routeValues = context.HttpContext.Request.RouteValues;
		var dataSourceName = (string)routeValues[DATASOURCE]!;

		await using var serviceScope = this._ServiceProvider.CreateAsyncScope();
		var dataSourceMap = serviceScope.ServiceProvider.GetService<IDictionary<string, IDataSource>>();
		IDataSource? dataSource = null;
		if (!dataSourceMap?.TryGetValue(dataSourceName, out dataSource) is not true)
			return Results.NotFound(Invariant($"{nameof(IDataSource)} [{dataSourceName}] was not found."));

		context.HttpContext.Items.Add(nameof(IDataSource), dataSource);

		var database = (string)routeValues[DATABASE]!;
		if (!dataSource!.Databases.Contains(database, StringComparer.OrdinalIgnoreCase))
			return Results.NotFound(Invariant($"Database {database} was not found in Data Source [{dataSourceName}]."));

		ObjectSchema? objectSchema = null;
		if (routeValues.TryGetValue(SCHEMA, out var schema))
		{
			if (routeValues.TryGetValue(TABLE, out var table))
			{
				var name = Invariant($"{database}.{schema}.{table}");
				objectSchema = dataSource[name];
				if (objectSchema is null)
					return Results.NotFound(Invariant($"{nameof(ObjectSchema)} for {name} was not found."));

				if (objectSchema.Type is not DatabaseObjectType.Table)
					return Results.BadRequest(Invariant($"[{table}] is not a table."));
			}
			else if(routeValues.TryGetValue(VIEW, out var view))
			{
				var name = Invariant($"{database}.{schema}.{view}");
				objectSchema = dataSource[name];
				if (objectSchema is null)
					return Results.NotFound(Invariant($"{nameof(ObjectSchema)} for {name} was not found."));

				if (objectSchema.Type is not DatabaseObjectType.View)
					return Results.BadRequest(Invariant($"[{view}] is not a view."));
			}
			else if (routeValues.TryGetValue(PROCEDURE, out var procedure))
			{
				var name = Invariant($"{database}.{schema}.{procedure}");
				objectSchema = dataSource[name];
				if (objectSchema is null)
					return Results.NotFound(Invariant($"{nameof(ObjectSchema)} for {name} was not found."));

				if (objectSchema.Type is not DatabaseObjectType.StoredProcedure)
					return Results.BadRequest(Invariant($"[{procedure}] is not a stored procedure."));
			}
		}

		if (objectSchema is not null)
			context.HttpContext.Items.Add(nameof(ObjectSchema), objectSchema);

		context.HttpContext.Items.Add(nameof(IMediator), this._Mediator);

		return await next(context);
	}
}
