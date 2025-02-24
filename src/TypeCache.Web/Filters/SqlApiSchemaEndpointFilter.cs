// Copyright (c) 2021 Samuel Abraham

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using TypeCache.Data;
using TypeCache.Data.Extensions;
using TypeCache.Extensions;

namespace TypeCache.Web.Filters;

public sealed class SqlApiSchemaEndpointFilter
	: IEndpointFilter
{
	public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
	{
		var routeValues = context.HttpContext.Request.RouteValues;

		IDataSource? dataSource;
		var dataSourceName = (string)routeValues[nameof(dataSource)]!;
		string database = (string)routeValues[nameof(database)]!;
		string collection = (string)routeValues[nameof(collection)]!;

		await using var serviceScope = context.HttpContext.RequestServices.CreateAsyncScope();
		dataSource = serviceScope.ServiceProvider.GetKeyedService<IDataSource>(dataSourceName);
		if (dataSource is null)
			return Results.NotFound(Invariant($"{nameof(IDataSource)} was not found: {dataSourceName}."));

		if (!dataSource.Databases.Contains(database, StringComparer.OrdinalIgnoreCase))
			return Results.NotFound(Invariant($"Database {database} was not found in Data Source: {dataSourceName}."));

		if (!Enum.TryParse<SchemaCollection>(collection, out _))
			return Results.BadRequest(Invariant($"{collection} is not a {nameof(SchemaCollection)} value."));

		context.HttpContext.Items.Add(nameof(IDataSource), dataSource);

		return await next(context);
	}
}
