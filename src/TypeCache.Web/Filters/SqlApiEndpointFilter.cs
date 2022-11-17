// Copyright (c) 2021 Samuel Abraham

using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TypeCache.Business;
using TypeCache.Collections.Extensions;
using TypeCache.Data;
using TypeCache.Extensions;
using static System.FormattableString;

namespace TypeCache.Web.Filters;

public sealed class SqlApiEndpointFilter : IEndpointFilter
{
	private const string DATASOURCE = "dataSource";
	private const string DATABASE = "database";
	private const string SCHEMA = "schema";
	private const string TABLE = "table";
	private const string VIEW = "view";
	private const string PROCEDURE = "procedure";

	private readonly IAccessor<IDataSource> _DataSourceAccessor;
	private readonly IMediator _Mediator;

	public SqlApiEndpointFilter(IAccessor<IDataSource> dataSourceAccessor, IMediator mediator)
	{
		this._DataSourceAccessor = dataSourceAccessor;
		this._Mediator = mediator;
	}

	public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
	{
		var routeValues = context.HttpContext.Request.RouteValues;
		var dataSourceName = (string)routeValues[DATASOURCE]!;
		var dataSource = this._DataSourceAccessor[dataSourceName];
		if (dataSource is null)
			return Results.NotFound(Invariant($"{nameof(IDataSource)} [{dataSourceName}] was not found."));

		context.HttpContext.Items.Add(nameof(IDataSource), dataSource);

		var database = (string)routeValues[DATABASE]!;
		if (!dataSource.Databases.Has(database))
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

				if (objectSchema.Type is not ObjectType.Table)
					return Results.BadRequest(Invariant($"[{table}] is not a table."));
			}
			else if(routeValues.TryGetValue(VIEW, out var view))
			{
				var name = Invariant($"{database}.{schema}.{view}");
				objectSchema = dataSource[name];
				if (objectSchema is null)
					return Results.NotFound(Invariant($"{nameof(ObjectSchema)} for {name} was not found."));

				if (objectSchema.Type is not ObjectType.View)
					return Results.BadRequest(Invariant($"[{view}] is not a view."));
			}
			else if (routeValues.TryGetValue(PROCEDURE, out var procedure))
			{
				var name = Invariant($"{database}.{schema}.{procedure}");
				objectSchema = dataSource[name];
				if (objectSchema is null)
					return Results.NotFound(Invariant($"{nameof(ObjectSchema)} for {name} was not found."));

				if (objectSchema.Type is not ObjectType.StoredProcedure)
					return Results.BadRequest(Invariant($"[{procedure}] is not a stored procedure."));
			}
		}

		if (objectSchema is not null)
			context.HttpContext.Items.Add(nameof(ObjectSchema), objectSchema);

		context.HttpContext.Items.Add(nameof(IMediator), this._Mediator);

		return await next(context);
	}
}
