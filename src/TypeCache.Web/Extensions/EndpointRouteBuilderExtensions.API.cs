// Copyright (c) 2021 Samuel Abraham

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using TypeCache.Web.Filters;
using TypeCache.Web.Handlers;
using static System.Net.Mime.MediaTypeNames;

namespace TypeCache.Web.Extensions;

public static partial class EndpointRouteBuilderExtensions
{
	private const HttpLoggingFields HTTP_LOGGING =
		HttpLoggingFields.RequestProtocol
		| HttpLoggingFields.RequestMethod
		| HttpLoggingFields.RequestPath
		| HttpLoggingFields.Duration
		| HttpLoggingFields.ResponseStatusCode;

	/// <summary>
	/// Maps endpoints that execute SQL commands either atomically or in a single local transaction.<br/>
	/// These calls are all POSTs for webhook compatibility.  Do <b>NOT</b> expose these publicly.
	/// <code>
	/// POST /api/execute/{dataSource}/{database}/{schema}/{procedure}<br/>
	/// POST /api/delete/{dataSource}/{database}/{schema}/{table}<br/>
	/// POST /api/delete-values/{dataSource}/{database}/{schema}/{table}<br/>
	/// POST /api/insert/{dataSource}/{database}/{schema}/{table}<br/>
	/// POST /api/insert-values/{dataSource}/{database}/{schema}/{table}<br/>
	/// POST /api/select/{dataSource}/{database}/{schema}/{table}<br/>
	/// POST /api/update/{dataSource}/{database}/{schema}/{table}<br/>
	/// POST /api/update-values/{dataSource}/{database}/{schema}/{table}<br/>
	/// </code>
	/// </summary>
	public static RouteGroupBuilder MapSqlApi(this IEndpointRouteBuilder @this, string route = Route.API)
	{
		var group = @this.MapGroup(route)
			.WithName("SqlApi")
			//.WithGroupName("SqlApi")
			.WithDisplayName("SQL API Endpoints")
			.WithDescription("Endpoints for performing database SQL-based actions.")
			.WithSummary("SQL API Endpoint")
			.WithTags("SQL API Endpoints");
		group.MapSqlApiExecuteProcedure();
		group.MapSqlApiDelete();
		group.MapSqlApiDeleteValues();
		group.MapSqlApiInsert();
		group.MapSqlApiInsertValues();
		group.MapSqlApiSelect();
		group.MapSqlApiUpdate();
		group.MapSqlApiUpdateValues();
		return group;
	}

	/// <summary>
	/// <c>POST /api/execute/{dataSource}/{database}/{schema}/{procedure}</c><br/><br/>
	/// <i><b>Requires calls to:</b></i>
	/// <code>
	/// MapGroup("/api")<br/>
	/// <see cref="TypeCache.Extensions.ServiceCollectionExtensions.AddSqlCommandRules(IServiceCollection)"/><br/>
	/// <see cref="ServiceCollectionExtensions.ConfigureSqlApi(IServiceCollection)"/><br/>
	/// </code>
	/// </summary>
	public static RouteHandlerBuilder MapSqlApiExecuteProcedure(this IEndpointRouteBuilder @this)
		=> @this.MapPost(Route.SqlApi.PROCEDURE, SqlApiHandler.ExecuteProcedure)
			.WithName(nameof(SqlApiHandler.ExecuteProcedure))
			.WithDisplayName("Execute SQL Procedure")
			.WithDescription("Executes a stored procedure returning any result(s).")
			.WithHttpLogging(HTTP_LOGGING)
			.AddEndpointFilter<SqlApiProcedureEndpointFilter>()
			.Produces(StatusCodes.Status200OK, contentType: Application.Json)
			.Produces(StatusCodes.Status204NoContent)
			.Produces(StatusCodes.Status404NotFound, contentType: Text.Plain);

	/// <summary>
	/// <c>POST /api/delete/{dataSource}/{database}/{schema}/{table}</c><br/><br/>
	/// <i><b>Requires calls to:</b></i>
	/// <code>
	/// MapGroup("/api")<br/>
	/// <see cref="TypeCache.Extensions.ServiceCollectionExtensions.AddSqlCommandRules(IServiceCollection)"/><br/>
	/// <see cref="ServiceCollectionExtensions.ConfigureSqlApi(IServiceCollection)"/><br/>
	/// </code>
	/// </summary>
	public static RouteHandlerBuilder MapSqlApiDelete(this IEndpointRouteBuilder @this)
		=> @this.MapPost(Route.SqlApi.DELETE, SqlApiHandler.DeleteTable)
			.WithName(nameof(SqlApiHandler.DeleteTable))
			.WithDisplayName("Delete table data")
			.WithDescription("Executes an atomic SQL DELETE command against a table.")
			.WithHttpLogging(HTTP_LOGGING)
			.AddEndpointFilter<SqlApiTableEndpointFilter>()
			.Produces(StatusCodes.Status200OK, contentType: Application.Json)
			.Produces(StatusCodes.Status204NoContent)
			.Produces(StatusCodes.Status404NotFound, contentType: Text.Plain);

	/// <summary>
	/// <c>POST /api/delete-values/{dataSource}/{database}/{schema}/{table}</c><br/><br/>
	/// Body is an array of data whose property names match the primary keys of the table to delete from.<br/><br/>
	/// <i><b>Requires calls to:</b></i>
	/// <code>
	/// MapGroup("/api")<br/>
	/// <see cref="TypeCache.Extensions.ServiceCollectionExtensions.AddSqlCommandRules(IServiceCollection)"/><br/>
	/// <see cref="ServiceCollectionExtensions.ConfigureSqlApi(IServiceCollection)"/><br/>
	/// </code>
	/// </summary>
	public static RouteHandlerBuilder MapSqlApiDeleteValues(this IEndpointRouteBuilder @this)
		=> @this.MapPost(Route.SqlApi.DELETE_VALUES, SqlApiHandler.DeleteTableValues)
			.WithName(nameof(SqlApiHandler.DeleteTableValues))
			.WithDisplayName("Delete batch from table")
			.WithDescription("Executes a batch of SQL DELETE commands against a table.")
			.WithHttpLogging(HTTP_LOGGING)
			.AddEndpointFilter<SqlApiTableEndpointFilter>()
			.Produces(StatusCodes.Status200OK, contentType: Application.Json)
			.Produces(StatusCodes.Status204NoContent)
			.Produces(StatusCodes.Status404NotFound, contentType: Text.Plain);

	/// <summary>
	/// <c>POST /api/insert/{dataSource}/{database}/{schema}/{table}</c><br/><br/>
	/// <i><b>Requires calls to:</b></i>
	/// <code>
	/// MapGroup("/api")<br/>
	/// <see cref="TypeCache.Extensions.ServiceCollectionExtensions.AddSqlCommandRules(IServiceCollection)"/><br/>
	/// <see cref="ServiceCollectionExtensions.ConfigureSqlApi(IServiceCollection)"/><br/>
	/// </code>
	/// </summary>
	public static RouteHandlerBuilder MapSqlApiInsert(this IEndpointRouteBuilder @this)
		=> @this.MapPost(Route.SqlApi.INSERT, SqlApiHandler.InsertTable)
			.WithName(nameof(SqlApiHandler.InsertTable))
			.WithDisplayName("Insert table data")
			.WithDescription("Executes an atomic SQL INSERT command against a table.")
			.WithHttpLogging(HTTP_LOGGING)
			.AddEndpointFilter<SqlApiTableEndpointFilter>()
			.Produces(StatusCodes.Status200OK, contentType: Application.Json)
			.Produces(StatusCodes.Status204NoContent)
			.Produces(StatusCodes.Status404NotFound, contentType: Text.Plain);

	/// <summary>
	/// <c>POST /api/insert-values/{dataSource}/{database}/{schema}/{table}</c><br/><br/>
	/// Body is an array of data whose property names match the primary keys of the table to delete from.<br/><br/>
	/// <i><b>Requires calls to:</b></i>
	/// <code>
	/// MapGroup("/api")<br/>
	/// <see cref="TypeCache.Extensions.ServiceCollectionExtensions.AddSqlCommandRules(IServiceCollection)"/><br/>
	/// <see cref="ServiceCollectionExtensions.ConfigureSqlApi(IServiceCollection)"/><br/>
	/// </code>
	/// </summary>
	public static RouteHandlerBuilder MapSqlApiInsertValues(this IEndpointRouteBuilder @this)
		=> @this.MapPost(Route.SqlApi.INSERT_VALUES, SqlApiHandler.InsertTableValues)
			.WithName(nameof(SqlApiHandler.InsertTableValues))
			.WithDisplayName("Insert table batch data")
			.WithDescription("Executes batch of SQL INSERT commands against a table.")
			.WithHttpLogging(HTTP_LOGGING)
			.AddEndpointFilter<SqlApiTableEndpointFilter>()
			.Produces(StatusCodes.Status200OK, contentType: Application.Json)
			.Produces(StatusCodes.Status204NoContent)
			.Produces(StatusCodes.Status404NotFound, contentType: Text.Plain);

	/// <summary>
	/// <c>POST /api/select/{dataSource}/{database}/{schema}/{table}</c><br/><br/>
	/// Selects, filters, sorts and pages data from a view.<br/><br/>
	/// <i><b>Requires calls to:</b></i>
	/// <code>
	/// MapGroup("/api")<br/>
	/// <see cref="TypeCache.Extensions.ServiceCollectionExtensions.AddSqlCommandRules(IServiceCollection)"/><br/>
	/// <see cref="ServiceCollectionExtensions.ConfigureSqlApi(IServiceCollection)"/><br/>
	/// </code>
	/// </summary>
	public static RouteHandlerBuilder MapSqlApiSelect(this IEndpointRouteBuilder @this)
		=> @this.MapPost(Route.SqlApi.SELECT, SqlApiHandler.SelectData)
			.WithName(nameof(SqlApiHandler.SelectData))
			.WithDisplayName("Select data")
			.WithDescription("Retrieves data from a table or view via a SELECT SQL command.")
			.WithHttpLogging(HTTP_LOGGING)
			.AddEndpointFilter<SqlApiTableEndpointFilter>()
			.Produces(StatusCodes.Status200OK, contentType: Application.Json)
			.Produces(StatusCodes.Status204NoContent)
			.Produces(StatusCodes.Status404NotFound, contentType: Text.Plain);

	/// <summary>
	/// <c>POST /api/update/{dataSource}/{database}/{schema}/{table}</c><br/><br/>
	/// Updates table data.<br/><br/>
	/// <i><b>Requires calls to:</b></i>
	/// <code>
	/// MapGroup("/api")<br/>
	/// <see cref="TypeCache.Extensions.ServiceCollectionExtensions.AddSqlCommandRules(IServiceCollection)"/><br/>
	/// <see cref="ServiceCollectionExtensions.ConfigureSqlApi(IServiceCollection)"/><br/>
	/// </code>
	/// </summary>
	public static RouteHandlerBuilder MapSqlApiUpdate(this IEndpointRouteBuilder @this)
		=> @this.MapPost(Route.SqlApi.UPDATE, SqlApiHandler.UpdateTable)
			.WithName(nameof(SqlApiHandler.UpdateTable))
			.WithDisplayName("Update table data")
			.WithDescription("Executes an atomic SQL UPDATE command against a table.")
			.WithHttpLogging(HTTP_LOGGING)
			.AddEndpointFilter<SqlApiTableEndpointFilter>()
			.Produces(StatusCodes.Status200OK, contentType: Application.Json)
			.Produces(StatusCodes.Status204NoContent)
			.Produces(StatusCodes.Status404NotFound, contentType: Text.Plain);

	/// <summary>
	/// <c>POST /api/update-values/{dataSource}/{database}/{schema}/{table}</c><br/><br/>
	/// Updates table data.<br/><br/>
	/// Body is an array of data that contains values to update in the table.<br/><br/>
	/// <i><b>Requires calls to:</b></i>
	/// <code>
	/// MapGroup("/api")<br/>
	/// <see cref="TypeCache.Extensions.ServiceCollectionExtensions.AddSqlCommandRules(IServiceCollection)"/><br/>
	/// <see cref="ServiceCollectionExtensions.ConfigureSqlApi(IServiceCollection)"/><br/>
	/// </code>
	/// </summary>
	public static RouteHandlerBuilder MapSqlApiUpdateValues(this IEndpointRouteBuilder @this)
		=> @this.MapPost(Route.SqlApi.UPDATE_VALUES, SqlApiHandler.UpdateTableValues)
			.WithName(nameof(SqlApiHandler.UpdateTableValues))
			.WithDisplayName("Update table with batch")
			.WithDescription("Executes an atomic SQL UPDATE command against a table.")
			.WithHttpLogging(HTTP_LOGGING)
			.AddEndpointFilter<SqlApiTableEndpointFilter>()
			.Produces(StatusCodes.Status200OK, contentType: Application.Json)
			.Produces(StatusCodes.Status204NoContent)
			.Produces(StatusCodes.Status404NotFound, contentType: Text.Plain);
}
