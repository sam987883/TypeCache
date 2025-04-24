// Copyright (c) 2021 Samuel Abraham

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using TypeCache.Web.Filters;
using TypeCache.Web.Handlers;
using static System.Net.Mime.MediaTypeNames;

namespace TypeCache.Web.Extensions;

public static partial class EndpointRouteBuilderExtensions
{
	/// <summary>
	/// Maps SQL API endpoints that return composed SQL <b><i>(for testing purposes only).</i></b>
	/// <code>
	/// GET /sql/schema/{source}/{database}/{collection}<br/>
	/// GET /sql/delete/{source}/{database}/{schema}/{table}<br/>
	/// GET /sql/delete-values/{source}/{database}/{schema}/{table}<br/>
	/// GET /sql/insert/{source}/{database}/{schema}/{table}<br/>
	/// GET /sql/insert-values/{source}/{database}/{schema}/{table}<br/>
	/// GET /sql/select/{source}/{database}/{schema}/{table}<br/>
	/// GET /sql/update/{source}/{database}/{schema}/{table}<br/>
	/// GET /sql/update-values/{source}/{database}/{schema}/{table}<br/>
	/// </code>
	/// </summary>
	public static RouteGroupBuilder MapSqlGet(this IEndpointRouteBuilder @this, string route = Route.SQL)
	{
		var group = @this.MapGroup(route)
			.WithName("GetSql")
			.WithDisplayName("Get SQL Endpoints")
			.WithDescription("Endpoints for retrieving prepared SQL statements.")
			.WithSummary("Get SQL Endpoint")
			.WithTags("Get generated SQL statement Endpoints");
		group.MapGetDatabaseSchema();
		group.MapGetDeleteSQL();
		group.MapGetDeleteValuesSQL();
		group.MapGetInsertSQL();
		group.MapGetInsertValuesSQL();
		group.MapGetSelectSQL();
		group.MapGetUpdateSQL();
		group.MapGetUpdateValuesSQL();
		return group;
	}

	/// <summary>
	/// <c>GET /sql/schema/{source}/{database}/{collection}</c><br/><br/>
	/// Gets database schema data.
	/// <i><b>Requires calls to:</b></i>
	/// <code>
	/// MapGroup("/sql")<br/>
	/// <see cref="TypeCache.Extensions.ServiceCollectionExtensions.AddSqlCommandRules(IServiceCollection)"/><br/>
	/// <see cref="ServiceCollectionExtensions.ConfigureSqlApi(IServiceCollection)"/><br/>
	/// </code>
	/// </summary>
	public static RouteHandlerBuilder MapGetDatabaseSchema(this IEndpointRouteBuilder @this)
		=> @this.MapGet(Route.SqlApi.DATABASE_SCHEMA, SqlHandler.GetDatabaseSchema)
			.WithName(nameof(SqlHandler.GetDatabaseSchema))
			.WithDisplayName("Get object schema")
			.WithDescription("Retrieves the schema of a database object.")
			.WithSummary("Retrieves the schema of a database object.")
			.AddEndpointFilter<SqlApiSchemaEndpointFilter>()
			.Produces(StatusCodes.Status200OK, null, Application.Json, Application.Xml, Text.Html)
			.Produces(StatusCodes.Status204NoContent)
			.Produces(StatusCodes.Status404NotFound, contentType: Text.Plain);

	/// <summary>
	/// <c>GET /sql/delete/{source}/{database}/{schema}/{table}</c><br/><br/>
	/// Returns generated SQL statement.<br/><br/>
	/// <i><b>Requires calls to:</b></i>
	/// <code>
	/// MapGroup("/sql")<br/>
	/// <see cref="TypeCache.Extensions.ServiceCollectionExtensions.AddSqlCommandRules(IServiceCollection)"/><br/>
	/// <see cref="ServiceCollectionExtensions.ConfigureSqlApi(IServiceCollection)"/><br/>
	/// </code>
	/// </summary>
	public static RouteHandlerBuilder MapGetDeleteSQL(this IEndpointRouteBuilder @this)
		=> @this.MapGet(Route.SqlApi.DELETE, SqlHandler.GetDeleteSQL)
			.WithName(nameof(SqlHandler.GetDeleteSQL))
			.WithDisplayName("Delete SQL command")
			.WithDescription("Retrieves the SQL for the DELETE command.")
			.WithSummary("Retrieves the SQL for the DELETE command.")
			.AddEndpointFilter<SqlApiTableEndpointFilter>()
			.CacheOutput(_ => _.Cache().Expire(TimeSpan.FromMinutes(1)))
			.Produces(StatusCodes.Status200OK, contentType: Text.Plain)
			.Produces(StatusCodes.Status204NoContent)
			.Produces(StatusCodes.Status404NotFound, contentType: Text.Plain);

	/// <summary>
	/// <c>POST /sql/delete-values/{source}/{database}/{schema}/{table}</c><br/><br/>
	/// Body is an array of data whose property names match the primary keys of the table to delete from.<br/><br/>
	/// Returns generated SQL statement.<br/><br/>
	/// <i><b>Requires calls to:</b></i>
	/// <code>
	/// MapGroup("/sql")<br/>
	/// <see cref="TypeCache.Extensions.ServiceCollectionExtensions.AddSqlCommandRules(IServiceCollection)"/><br/>
	/// <see cref="ServiceCollectionExtensions.ConfigureSqlApi(IServiceCollection)"/><br/>
	/// </code>
	/// </summary>
	public static RouteHandlerBuilder MapGetDeleteValuesSQL(this IEndpointRouteBuilder @this)
		=> @this.MapPost(Route.SqlApi.DELETE_VALUES, SqlHandler.GetDeleteValuesSQL)
			.WithName(nameof(SqlHandler.GetDeleteValuesSQL))
			.WithDisplayName("Delete batch SQL command")
			.WithDescription("Retrieves the SQL for the batch DELETE commands.")
			.WithSummary("Retrieves the SQL for the batch DELETE commands.")
			.AddEndpointFilter<SqlApiTableEndpointFilter>()
			.Produces(StatusCodes.Status200OK, contentType: Text.Plain)
			.Produces(StatusCodes.Status204NoContent)
			.Produces(StatusCodes.Status404NotFound, contentType: Text.Plain);

	/// <summary>
	/// <c>GET /sql/insert/{source}/{database}/{schema}/{table}</c><br/><br/>
	/// Returns generated SQL statement.<br/><br/>
	/// <i><b>Requires calls to:</b></i>
	/// <code>
	/// MapGroup("/sql")<br/>
	/// <see cref="TypeCache.Extensions.ServiceCollectionExtensions.AddSqlCommandRules(IServiceCollection)"/><br/>
	/// <see cref="ServiceCollectionExtensions.ConfigureSqlApi(IServiceCollection)"/><br/>
	/// </code>
	/// </summary>
	public static RouteHandlerBuilder MapGetInsertSQL(this IEndpointRouteBuilder @this)
		=> @this.MapGet(Route.SqlApi.INSERT, SqlHandler.GetInsertSQL)
			.WithName(nameof(SqlHandler.GetInsertSQL))
			.WithDisplayName("Insert SQL command")
			.WithDescription("Retrieves the SQL for the INSERT command.")
			.WithSummary("Retrieves the SQL for the INSERT command.")
			.AddEndpointFilter<SqlApiTableEndpointFilter>()
			.Produces(StatusCodes.Status200OK, contentType: Text.Plain)
			.Produces(StatusCodes.Status204NoContent)
			.Produces(StatusCodes.Status404NotFound, contentType: Text.Plain);

	/// <summary>
	/// <c>POST /sql/insert-values/{source}/{database}/{schema}/{table}</c><br/><br/>
	/// Body is an array of data that would be inserted.<br/><br/>
	/// Returns generated SQL statement.<br/><br/>
	/// <i><b>Requires calls to:</b></i>
	/// <code>
	/// MapGroup("/sql")<br/>
	/// <see cref="TypeCache.Extensions.ServiceCollectionExtensions.AddSqlCommandRules(IServiceCollection)"/><br/>
	/// <see cref="ServiceCollectionExtensions.ConfigureSqlApi(IServiceCollection)"/><br/>
	/// </code>
	/// </summary>
	public static RouteHandlerBuilder MapGetInsertValuesSQL(this IEndpointRouteBuilder @this)
		=> @this.MapPost(Route.SqlApi.INSERT_VALUES, SqlHandler.GetInsertValuesSQL)
			.WithName(nameof(SqlHandler.GetInsertValuesSQL))
			.WithDisplayName("Insert batch SQL command")
			.WithDescription("Retrieves the SQL for the batch INSERT commands.")
			.WithSummary("Retrieves the SQL for the batch INSERT commands.")
			.AddEndpointFilter<SqlApiTableEndpointFilter>()
			.Produces(StatusCodes.Status200OK, contentType: Text.Plain)
			.Produces(StatusCodes.Status204NoContent)
			.Produces(StatusCodes.Status404NotFound, contentType: Text.Plain);

	/// <summary>
	/// <c>GET /sql/select/{source}/{database}/{schema}/{table}</c><br/><br/>
	/// Returns generated SQL statement.<br/><br/>
	/// <i><b>Requires calls to:</b></i>
	/// <code>
	/// MapGroup("/sql")<br/>
	/// <see cref="TypeCache.Extensions.ServiceCollectionExtensions.AddSqlCommandRules(IServiceCollection)"/><br/>
	/// <see cref="ServiceCollectionExtensions.ConfigureSqlApi(IServiceCollection)"/><br/>
	/// </code>
	/// </summary>
	public static RouteHandlerBuilder MapGetSelectSQL(this IEndpointRouteBuilder @this)
		=> @this.MapGet(Route.SqlApi.SELECT, SqlHandler.GetSelectSQL)
			.WithName(nameof(SqlHandler.GetSelectSQL))
			.WithDisplayName("Select SQL command")
			.WithDescription("Retrieves the SQL for the SELECT command.")
			.WithSummary("Retrieves the SQL for the SELECT command.")
			.Produces(StatusCodes.Status200OK, contentType: Text.Plain)
			.Produces(StatusCodes.Status204NoContent)
			.Produces(StatusCodes.Status404NotFound, contentType: Text.Plain);

	/// <summary>
	/// <c>GET /sql/update/{source}/{database}/{schema}/{table}</c><br/><br/>
	/// Returns generated SQL statement.<br/><br/>
	/// <i><b>Requires calls to:</b></i>
	/// <code>
	/// MapGroup("/sql")<br/>
	/// <see cref="TypeCache.Extensions.ServiceCollectionExtensions.AddSqlCommandRules(IServiceCollection)"/><br/>
	/// <see cref="ServiceCollectionExtensions.ConfigureSqlApi(IServiceCollection)"/><br/>
	/// </code>
	/// </summary>
	public static RouteHandlerBuilder MapGetUpdateSQL(this IEndpointRouteBuilder @this)
		=> @this.MapGet(Route.SqlApi.UPDATE, SqlHandler.GetUpdateSQL)
			.WithName(nameof(SqlHandler.GetUpdateSQL))
			.WithDisplayName("Update SQL command")
			.WithDescription("Retrieves the SQL for the UPDATE command.")
			.WithSummary("Retrieves the SQL for the UPDATE command.")
			.Produces(StatusCodes.Status200OK, contentType: Text.Plain)
			.Produces(StatusCodes.Status204NoContent)
			.Produces(StatusCodes.Status404NotFound, contentType: Text.Plain);

	/// <summary>
	/// <c>POST /sql/update-values/{source}/{database}/{schema}/{table}</c><br/><br/>
	/// Body is an array of data with values tp use for update.<br/><br/>
	/// Returns generated SQL statement.<br/><br/>
	/// <i><b>Requires calls to:</b></i>
	/// <code>
	/// MapGroup("/sql")<br/>
	/// <see cref="TypeCache.Extensions.ServiceCollectionExtensions.AddSqlCommandRules(IServiceCollection)"/><br/>
	/// <see cref="ServiceCollectionExtensions.ConfigureSqlApi(IServiceCollection)"/><br/>
	/// </code>
	/// </summary>
	public static RouteHandlerBuilder MapGetUpdateValuesSQL(this IEndpointRouteBuilder @this)
		=> @this.MapPost(Route.SqlApi.UPDATE_VALUES, SqlHandler.GetUpdateValuesSQL)
			.WithName(nameof(SqlHandler.GetUpdateValuesSQL))
			.WithDisplayName("Update batch SQL command")
			.WithDescription("Retrieves the SQL for the batch UPDATE commands.")
			.WithSummary("Retrieves the SQL for the batch UPDATE commands.")
			.Produces(StatusCodes.Status200OK, contentType: Text.Plain)
			.Produces(StatusCodes.Status204NoContent)
			.Produces(StatusCodes.Status404NotFound, contentType: Text.Plain);
}
