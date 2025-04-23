// Copyright (c) 2021 Samuel Abraham

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using TypeCache.Data;
using TypeCache.Extensions;

namespace TypeCache.Web.Filters;

public sealed class SqlApiSchemaEndpointFilter
	: IEndpointFilter
{
	public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
	{
		var routeValues = context.HttpContext.Request.RouteValues;

		string source = (string)routeValues[nameof(source)]!;
		string database = (string)routeValues[nameof(database)]!;
		string collection = (string)routeValues[nameof(collection)]!;

		await using var serviceScope = context.HttpContext.RequestServices.CreateAsyncScope();
		var dataSource = serviceScope.ServiceProvider.GetKeyedService<IDataSource>(source);
		if (dataSource is null)
			return Results.NotFound(Invariant($"{nameof(IDataSource)} was not found: {source}."));

		if (!dataSource.Databases.ContainsIgnoreCase(database))
			return Results.NotFound(Invariant($"Database {database} was not found in Data Source: {source}."));

		if (!Enum.TryParse<SchemaCollection>(collection, out _))
			return Results.BadRequest(Invariant($"{collection} is not a {nameof(SchemaCollection)} value."));

		context.HttpContext.Items.Add(nameof(IDataSource), dataSource);

		return await next(context);
	}
}
