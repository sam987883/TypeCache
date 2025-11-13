// Copyright (c) 2021 Samuel Abraham

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using TypeCache.Data;
using TypeCache.Data.Extensions;
using TypeCache.Extensions;

namespace TypeCache.Web.Filters;

public sealed class SqlApiProcedureEndpointFilter
	: IEndpointFilter
{
	public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
	{
		var routeValues = context.HttpContext.Request.RouteValues;

		string source = (string)routeValues[nameof(source)]!;
		string database = (string)routeValues[nameof(database)]!;
		string schema = (string)routeValues[nameof(schema)]!;
		string procedure = (string)routeValues[nameof(procedure)]!;

		await using var serviceScope = context.HttpContext.RequestServices.CreateAsyncScope();
		var dataSource = serviceScope.ServiceProvider.GetKeyedService<IDataSource>(source);
		if (dataSource is null)
			return Results.NotFound(Invariant($"{nameof(IDataSource)} was not found: {source}."));

		if (!dataSource.Databases.ContainsIgnoreCase(database))
			return Results.NotFound(Invariant($"Database {database} was not found in Data Source: {source}."));

		var objectName = dataSource.Type switch
		{
			DataSourceType.MySql => Invariant($"{database.EscapeIdentifier(dataSource.Type)}.{procedure.EscapeIdentifier(dataSource.Type)}"),
			_ => Invariant($"{database.EscapeIdentifier(dataSource.Type)}.{schema.EscapeIdentifier(dataSource.Type)}.{procedure.EscapeIdentifier(dataSource.Type)}")
		};
		var objectSchema = dataSource[objectName];
		if (objectSchema is null)
			return Results.NotFound(Invariant($"{nameof(ObjectSchema)} for {objectName} was not found."));

		if (objectSchema.Type is not DatabaseObjectType.StoredProcedure)
			return Results.BadRequest(Invariant($"{objectName} is not a {DatabaseObjectType.StoredProcedure.Name}."));

		context.HttpContext.Items.Add(nameof(IDataSource), dataSource);
		context.HttpContext.Items.Add(nameof(ObjectSchema), objectSchema);

		return await next(context);
	}
}
