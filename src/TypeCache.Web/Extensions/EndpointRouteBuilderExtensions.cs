﻿// Copyright (c) 2021 Samuel Abraham

using System.Text;
using System.Text.Json;
using System.Xml.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using TypeCache.Converters;
using TypeCache.Extensions;
using TypeCache.Web.Filters;
using TypeCache.Web.Handlers;
using static System.Net.Mime.MediaTypeNames;

namespace TypeCache.Web.Extensions;

public static class EndpointRouteBuilderExtensions
{
	/// <summary>
	/// <c>/request/headers</c><br/><br/>
	/// Maps a GET endpoint that returns all request header values.<br/>
	/// Supported formats (<b>Accept</b> header):
	/// <list type="bullet">
	/// <item><c>application/json</c></item>
	/// <item><c>application/xml</c></item>
	/// <item><c>text/plain</c></item>
	/// <item><c>text/html</c></item>
	/// <item><c>text/xml</c></item>
	/// </list>
	/// </summary>
	public static IEndpointConventionBuilder MapGetRequestHeaders(this IEndpointRouteBuilder @this)
		=> @this.MapGet("/request/headers", async context =>
		{
			var acceptRequestHeader = context.Request.Headers.Accept.ToString();
			context.Response.StatusCode = context.Request.Headers.Any() ? StatusCodes.Status200OK : StatusCodes.Status204NoContent;
			context.Response.Headers.ContentType = acceptRequestHeader switch
			{
				Text.Plain or Text.Html or Application.Xml or Text.Xml or Application.Json => acceptRequestHeader,
				_ => Application.Json
			};
			var response = string.Empty;
			if (acceptRequestHeader.Equals(Text.Plain, StringComparison.OrdinalIgnoreCase))
			{
				var responseBuilder = new StringBuilder();
				foreach (var pair in context.Request.Headers)
				{
					responseBuilder.Append(Invariant($"{pair.Key}: {pair.Value}<br>"));
				}

				response = responseBuilder.ToString();
			}
			else if (acceptRequestHeader.Equals(Text.Html, StringComparison.OrdinalIgnoreCase))
			{
				var table = new XElement("table",
					new XElement("tr",
						new XElement("th", "Header"),
						new XElement("th", "Value")));
				foreach (var pair in context.Request.Headers)
				{
					table.Add(new XElement("tr",
						new XElement("td", pair.Key),
						new XElement("td", pair.Value.ToString())));
				}

				response = table.ToString();
			}
			else if (acceptRequestHeader.Equals(Application.Xml, StringComparison.OrdinalIgnoreCase)
				|| acceptRequestHeader.Equals(Text.Xml, StringComparison.OrdinalIgnoreCase))
			{
				var headers = new XElement("headers");
				foreach (var pair in context.Request.Headers)
				{
					headers.Add(new XElement(pair.Key.Replace(' ', '_'), pair.Value.ToString()));
				}

				response = new XDocument(headers).ToString();
			}
			else
			{
				response = JsonSerializer.Serialize(context.Request.Headers, CreateJsonSerializerOptions());
			}

			await context.Response.WriteAsync(response, context.RequestAborted);
		});

	/// <summary>
	/// <c>/request/headers/{key}</c><br/><br/>
	/// Maps a GET endpoint that returns the request header value for the specified parameter <c><paramref name="key"/></c>.<br/>
	/// If no parameter is specified, then the route will handle any key.<br/>
	/// Supported formats (<b>Accept</b> header):
	/// <list type="bullet">
	/// <item><c>application/json</c></item>
	/// <item><c>application/xml</c></item>
	/// <item><c>text/plain</c></item>
	/// <item><c>text/html</c></item>
	/// <item><c>text/xml</c></item>
	/// </list>
	/// </summary>
	public static IEndpointConventionBuilder MapGetRequestHeaderValue(this IEndpointRouteBuilder @this, string? key = null)
		=> @this.MapGet(key.IsNotBlank() ? Invariant($"/request/headers/{key}") : "/request/headers/{key}", async context =>
		{
			key ??= context.GetRouteValue(nameof(key))!.ToString()!;

			var response = string.Empty;
			var acceptRequestHeader = context.Request.Headers.Accept.ToString().ToLowerInvariant();
			context.Response.StatusCode = context.Request.Headers.TryGetValue(key, out var value) ? StatusCodes.Status200OK : StatusCodes.Status204NoContent;
			context.Response.Headers.ContentType = acceptRequestHeader switch
			{
				Text.Plain or Text.Html or Application.Xml or Text.Xml or Application.Json => acceptRequestHeader,
				_ => Application.Json
			};
			response = (context.Response.StatusCode, acceptRequestHeader) switch
			{
				(StatusCodes.Status200OK, Text.Plain) => Invariant($"{key}: {value}"),
				(StatusCodes.Status204NoContent, Text.Plain) => Invariant($"{key}: "),
				(StatusCodes.Status200OK, Text.Html) => Invariant($"<h1>{key}</h1><br><b>{value}<b/>"),
				(StatusCodes.Status204NoContent, Text.Html) => Invariant($"<h1>{key}</h1><br>"),
				(StatusCodes.Status200OK, Application.Xml or Text.Xml) => new XDocument(new XElement(key.Replace(' ', '_'), value.ToString())).ToString(),
				(StatusCodes.Status204NoContent, Application.Xml or Text.Xml) => new XDocument(new XElement(key.Replace(' ', '_'))).ToString(),
				(StatusCodes.Status200OK, _) => JsonSerializer.Serialize(new Dictionary<string, string?>(1) { { key, value.ToString() } }, CreateJsonSerializerOptions()),
				_ => JsonSerializer.Serialize(new Dictionary<string, string?>(1) { { key, null } }, CreateJsonSerializerOptions())
			};

			await context.Response.WriteAsync(response, context.RequestAborted);
		});

	/// <summary>
	/// <c>/ping/{<paramref name="name"/>}</c><br/><br/>
	/// Maps a GET endpoint that calls a simple GET endpoint registered with <c><see cref="IHttpClientFactory"/></c> to test connectivity.<br/>
	/// Headers are optionally propagated using the <c>Microsoft.AspNetCore.HeaderPropagation</c> package.<br/>
	/// Uses <c><paramref name="requestUri"/></c> as a relative <c><see cref="Uri"/></c> and propagates all query parameters.<br/>
	/// An example use of this would be to ping another system's endpoint to ensure that the firewall allows the call to be made,
	/// eliminating the need to install curl in the container or pod.
	/// </summary>
	public static IEndpointConventionBuilder MapGetPing(this IEndpointRouteBuilder @this, string name, Uri requestUri)
		=> @this.MapGet(Invariant($"/ping/{name}"), async (HttpContext context, IHttpClientFactory factory) =>
		{
			var httpClient = factory.CreateClient(name);

			using var responseMessage = await httpClient.GetAsync(requestUri, HttpCompletionOption.ResponseContentRead, context.RequestAborted);
			context.Response.StatusCode = (int)responseMessage.StatusCode;
			foreach (var pair in responseMessage.Headers)
			{
				context.Response.Headers[pair.Key] = new StringValues(pair.Value?.ToArray());
			}

			if (context.Response.SupportsTrailers())
			{
				foreach (var pair in responseMessage.TrailingHeaders)
				{
					context.Response.AppendTrailer(pair.Key, new StringValues(pair.Value?.ToArray()));
				}
			}

			await responseMessage.Content.CopyToAsync(context.Response.Body, context.RequestAborted);
		});

	/// <summary>
	/// Endpoints that return composed SQL.<br/>
	/// <code>
	/// {<br/>
	/// <see langword="    var"/> group = @<paramref name="this"/>.MapGroup(<paramref name="route"/>);<br/>
	/// <see langword="    "/>group.MapSqlApiGetDeleteSQL();<br/>
	/// <see langword="    "/>group.MapSqlApiGetDeleteBatchSQL();<br/>
	/// <see langword="    "/>group.MapSqlApiGetInsertSQL();<br/>
	/// <see langword="    "/>group.MapSqlApiGetInsertBatchSQL();<br/>
	/// <see langword="    "/>group.MapSqlApiGetSelectTableSQL();<br/>
	/// <see langword="    "/>group.MapSqlApiGetSelectViewSQL();<br/>
	/// <see langword="    "/>group.MapSqlApiGetUpdateSQL();<br/>
	/// <see langword="    "/>group.MapSqlApiGetUpdateBatchSQL();<br/>
	/// <see langword="    return"/> group.AddEndpointFilter&lt;<see cref="SqlApiEndpointFilter"/>&gt;();<br/>
	/// }
	/// </code>
	/// </summary>
	public static RouteGroupBuilder MapSqlGet(this IEndpointRouteBuilder @this, string route = "/sql")
	{
		var group = @this.MapGroup(route);
		group.MapSqlApiGetDeleteSQL();
		group.MapSqlApiGetDeleteBatchSQL();
		group.MapSqlApiGetInsertSQL();
		group.MapSqlApiGetInsertBatchSQL();
		group.MapSqlApiGetSelectTableSQL();
		group.MapSqlApiGetSelectViewSQL();
		group.MapSqlApiGetUpdateSQL();
		group.MapSqlApiGetUpdateBatchSQL();
		return group.AddEndpointFilter<SqlApiEndpointFilter>();
	}

	/// <summary>
	/// Endpoints that return composed SQL.<br/>
	/// <code>
	/// {<br/>
	/// <see langword="    var"/> group = @<paramref name="this"/>.MapGroup(<paramref name="route"/>);<br/>
	/// <see langword="    "/>group.MapSqlApiCallProcedure();<br/>
	/// <see langword="    "/>group.MapSqlApiDelete();<br/>
	/// <see langword="    "/>group.MapSqlApiDeleteBatch();<br/>
	/// <see langword="    "/>group.MapSqlApiInsert();<br/>
	/// <see langword="    "/>group.MapSqlApiInsertBatch();<br/>
	/// <see langword="    "/>group.MapSqlApiSelectTable();<br/>
	/// <see langword="    "/>group.MapSqlApiSelectView();<br/>
	/// <see langword="    "/>group.MapSqlApiUpdate();<br/>
	/// <see langword="    "/>group.MapSqlApiUpdateBatch();<br/>
	/// <see langword="    return"/> group.AddEndpointFilter&lt;<see cref="SqlApiEndpointFilter"/>&gt;();<br/>
	/// }
	/// </code>
	/// </summary>
	public static RouteGroupBuilder MapSqlApi(this IEndpointRouteBuilder @this, string route = "/api/sql")
	{
		var group = @this.MapGroup(route);
		group.MapSqlApiCallProcedure();
		group.MapSqlApiDelete();
		group.MapSqlApiDeleteBatch();
		group.MapSqlApiInsert();
		group.MapSqlApiInsertBatch();
		group.MapSqlApiSelectTable();
		group.MapSqlApiSelectView();
		group.MapSqlApiUpdate();
		group.MapSqlApiUpdateBatch();
		return group.AddEndpointFilter<SqlApiEndpointFilter>();
	}

	/// <summary>
	/// <c>GET|POST /{{dataSource:string}}/procedure/{{database:string}}/{{schema:string}}/{{procedure:string}}</c><br/><br/>
	/// <i><b>Requires calls to:</b></i>
	/// <code>
	/// <see cref="TypeCache.Extensions.ServiceCollectionExtensions.AddSqlCommandRules(IServiceCollection)"/><br/>
	/// <see cref="ServiceCollectionExtensions.ConfigureSqlApi(IServiceCollection)"/><br/>
	/// </code>
	/// </summary>
	public static RouteHandlerBuilder MapSqlApiCallProcedure(this IEndpointRouteBuilder @this)
		=> @this.MapMethods(Invariant($"/{{dataSource:string}}/procedure/{{database:string}}/{{schema:string}}/{{procedure:string}}"), [HttpMethods.Get, HttpMethods.Post], SqlApiHandler.CallProcedure)
			.AddEndpointFilter<SqlApiEndpointFilter>()
			.WithName("Call Procedure");

	/// <summary>
	/// <c>DELETE /{{dataSource:string}}/table/{{database:string}}/{{schema:string}}/{{table:string}}</c><br/><br/>
	/// <i><b>Requires calls to:</b></i>
	/// <code>
	/// <see cref="TypeCache.Extensions.ServiceCollectionExtensions.AddSqlCommandRules(IServiceCollection)"/><br/>
	/// <see cref="ServiceCollectionExtensions.ConfigureSqlApi(IServiceCollection)"/><br/>
	/// </code>
	/// </summary>
	public static RouteHandlerBuilder MapSqlApiDelete(this IEndpointRouteBuilder @this)
		=> @this.MapDelete(Invariant($"/{{dataSource:string}}/table/{{database:string}}/{{schema:string}}/{{table:string}}"), SqlApiHandler.DeleteTable)
			.AddEndpointFilter<SqlApiEndpointFilter>()
			.WithName("Delete");

	/// <summary>
	/// <c>DELETE /{{dataSource:string}}/table/{{database:string}}/{{schema:string}}/{{table:string}}</c><br/><br/>
	/// Body is an array of data whose property names match the primary keys of the table to delete from.<br/><br/>
	/// <i><b>Requires calls to:</b></i>
	/// <code>
	/// <see cref="TypeCache.Extensions.ServiceCollectionExtensions.AddSqlCommandRules(IServiceCollection)"/><br/>
	/// <see cref="ServiceCollectionExtensions.ConfigureSqlApi(IServiceCollection)"/><br/>
	/// </code>
	/// </summary>
	public static RouteHandlerBuilder MapSqlApiDeleteBatch(this IEndpointRouteBuilder @this)
		=> @this.MapDelete(Invariant($"/{{dataSource:string}}/table/{{database:string}}/{{schema:string}}/{{table:string}}/batch"), SqlApiHandler.DeleteTableBatch)
			.AddEndpointFilter<SqlApiEndpointFilter>()
			.WithName("Batch Delete");

	/// <summary>
	/// <c>GET /{{dataSource:string}}/table/{{database:string}}/{{schema:string}}/{{table:string}}/delete</c><br/><br/>
	/// Returns generated SQL statement.<br/><br/>
	/// <i><b>Requires calls to:</b></i>
	/// <code>
	/// <see cref="TypeCache.Extensions.ServiceCollectionExtensions.AddSqlCommandRules(IServiceCollection)"/><br/>
	/// <see cref="ServiceCollectionExtensions.ConfigureSqlApi(IServiceCollection)"/><br/>
	/// </code>
	/// </summary>
	public static RouteHandlerBuilder MapSqlApiGetDeleteSQL(this IEndpointRouteBuilder @this)
		=> @this.MapGet(Invariant($"/{{dataSource:string}}/table/{{database:string}}/{{schema:string}}/{{table:string}}/delete"), SqlApiHandler.GetDeleteSQL)
			.AddEndpointFilter<SqlApiEndpointFilter>()
			.WithName("Delete SQL");

	/// <summary>
	/// <c>POST /{{dataSource:string}}/table/{{database:string}}/{{schema:string}}/{{table:string}}/batch/delete</c><br/><br/>
	/// Body is an array of data whose property names match the primary keys of the table to delete from.<br/><br/>
	/// Returns generated SQL statement.<br/><br/>
	/// <i><b>Requires calls to:</b></i>
	/// <code>
	/// <see cref="TypeCache.Extensions.ServiceCollectionExtensions.AddSqlCommandRules(IServiceCollection)"/><br/>
	/// <see cref="ServiceCollectionExtensions.ConfigureSqlApi(IServiceCollection)"/><br/>
	/// </code>
	/// </summary>
	public static RouteHandlerBuilder MapSqlApiGetDeleteBatchSQL(this IEndpointRouteBuilder @this)
		=> @this.MapPost(Invariant($"/{{dataSource:string}}/table/{{database:string}}/{{schema:string}}/{{table:string}}/batch/delete"), SqlApiHandler.GetDeleteBatchSQL)
			.AddEndpointFilter<SqlApiEndpointFilter>()
			.WithName("Batch Delete SQL");

	/// <summary>
	/// <c>GET /{{dataSource:string}}/table/{{database:string}}/{{schema:string}}/{{table:string}}/insert</c><br/><br/>
	/// Returns generated SQL statement.<br/><br/>
	/// <i><b>Requires calls to:</b></i>
	/// <code>
	/// <see cref="TypeCache.Extensions.ServiceCollectionExtensions.AddSqlCommandRules(IServiceCollection)"/><br/>
	/// <see cref="ServiceCollectionExtensions.ConfigureSqlApi(IServiceCollection)"/><br/>
	/// </code>
	/// </summary>
	public static RouteHandlerBuilder MapSqlApiGetInsertSQL(this IEndpointRouteBuilder @this)
		=> @this.MapGet(Invariant($"/{{dataSource:string}}/table/{{database:string}}/{{schema:string}}/{{table:string}}/insert/sql"), SqlApiHandler.GetInsertSQL)
			.AddEndpointFilter<SqlApiEndpointFilter>()
			.WithName("Insert SQL");

	/// <summary>
	/// <c>POST /{{dataSource:string}}/table/{{database:string}}/{{schema:string}}/{{table:string}}/batch/insert</c><br/><br/>
	/// Body is an array of data that would be inserted.<br/><br/>
	/// Returns generated SQL statement.<br/><br/>
	/// <i><b>Requires calls to:</b></i>
	/// <code>
	/// <see cref="TypeCache.Extensions.ServiceCollectionExtensions.AddSqlCommandRules(IServiceCollection)"/><br/>
	/// <see cref="ServiceCollectionExtensions.ConfigureSqlApi(IServiceCollection)"/><br/>
	/// </code>
	/// </summary>
	public static RouteHandlerBuilder MapSqlApiGetInsertBatchSQL(this IEndpointRouteBuilder @this)
		=> @this.MapPost(Invariant($"/{{dataSource:string}}/table/{{database:string}}/{{schema:string}}/{{table:string}}/batch/insert/sql"), SqlApiHandler.GetInsertBatchSQL)
			.AddEndpointFilter<SqlApiEndpointFilter>()
			.WithName("Batch Insert SQL");

	/// <summary>
	/// <c>POST /{{dataSource:string}}/table/{{database:string}}/{{schema:string}}/{{table:string}}/select</c><br/><br/>
	/// Returns generated SQL statement.<br/><br/>
	/// <i><b>Requires calls to:</b></i>
	/// <code>
	/// <see cref="TypeCache.Extensions.ServiceCollectionExtensions.AddSqlCommandRules(IServiceCollection)"/><br/>
	/// <see cref="ServiceCollectionExtensions.ConfigureSqlApi(IServiceCollection)"/><br/>
	/// </code>
	/// </summary>
	public static RouteHandlerBuilder MapSqlApiGetSelectTableSQL(this IEndpointRouteBuilder @this)
		=> @this.MapGet(Invariant($"/{{dataSource:string}}/table/{{database:string}}/{{schema:string}}/{{table:string}}/select"), SqlApiHandler.GetSelectSQL)
			.AddEndpointFilter<SqlApiEndpointFilter>()
			.WithName("Select Table SQL");

	/// <summary>
	/// <c>POST /{{dataSource:string}}/view/{{database:string}}/{{schema:string}}/{{view:string}}/select</c><br/><br/>
	/// Returns generated SQL statement.<br/><br/>
	/// <i><b>Requires calls to:</b></i>
	/// <code>
	/// <see cref="TypeCache.Extensions.ServiceCollectionExtensions.AddSqlCommandRules(IServiceCollection)"/><br/>
	/// <see cref="ServiceCollectionExtensions.ConfigureSqlApi(IServiceCollection)"/><br/>
	/// </code>
	/// </summary>
	public static RouteHandlerBuilder MapSqlApiGetSelectViewSQL(this IEndpointRouteBuilder @this)
		=> @this.MapGet(Invariant($"/{{dataSource:string}}/view/{{database:string}}/{{schema:string}}/{{view:string}}/select"), SqlApiHandler.GetSelectSQL)
			.AddEndpointFilter<SqlApiEndpointFilter>()
			.WithName("Select View SQL");

	/// <summary>
	/// <c>GET /{{dataSource:string}}/table/{{database:string}}/{{schema:string}}/{{table:string}}/update</c><br/><br/>
	/// Returns generated SQL statement.<br/><br/>
	/// <i><b>Requires calls to:</b></i>
	/// <code>
	/// <see cref="TypeCache.Extensions.ServiceCollectionExtensions.AddSqlCommandRules(IServiceCollection)"/><br/>
	/// <see cref="ServiceCollectionExtensions.ConfigureSqlApi(IServiceCollection)"/><br/>
	/// </code>
	/// </summary>
	public static RouteHandlerBuilder MapSqlApiGetUpdateSQL(this IEndpointRouteBuilder @this)
		=> @this.MapGet(Invariant($"/{{dataSource:string}}/table/{{database:string}}/{{schema:string}}/{{table:string}}/update"), SqlApiHandler.GetUpdateSQL)
			.AddEndpointFilter<SqlApiEndpointFilter>()
			.WithName("Update SQL");

	/// <summary>
	/// <c>POST /{{dataSource:string}}/table/{{database:string}}/{{schema:string}}/{{table:string}}/batch/update</c><br/><br/>
	/// Body is an array of data with values tp use for update.<br/><br/>
	/// Returns generated SQL statement.<br/><br/>
	/// <i><b>Requires calls to:</b></i>
	/// <code>
	/// <see cref="TypeCache.Extensions.ServiceCollectionExtensions.AddSqlCommandRules(IServiceCollection)"/><br/>
	/// <see cref="ServiceCollectionExtensions.ConfigureSqlApi(IServiceCollection)"/><br/>
	/// </code>
	/// </summary>
	public static RouteHandlerBuilder MapSqlApiGetUpdateBatchSQL(this IEndpointRouteBuilder @this)
		=> @this.MapPost(Invariant($"/{{dataSource:string}}/table/{{database:string}}/{{schema:string}}/{{table:string}}/batch/update"), SqlApiHandler.GetUpdateBatchSQL)
			.AddEndpointFilter<SqlApiEndpointFilter>()
			.WithName("Batch Update SQL");

	/// <summary>
	/// <c>POST /{{dataSource:string}}/table/{{database:string}}/{{schema:string}}/{{table:string}}</c><br/><br/>
	/// <i><b>Requires calls to:</b></i>
	/// <code>
	/// <see cref="TypeCache.Extensions.ServiceCollectionExtensions.AddSqlCommandRules(IServiceCollection)"/><br/>
	/// <see cref="ServiceCollectionExtensions.ConfigureSqlApi(IServiceCollection)"/><br/>
	/// </code>
	/// </summary>
	public static RouteHandlerBuilder MapSqlApiInsert(this IEndpointRouteBuilder @this)
		=> @this.MapPost(Invariant($"/{{dataSource:string}}/table/{{database:string}}/{{schema:string}}/{{table:string}}"), SqlApiHandler.InsertTable)
			.AddEndpointFilter<SqlApiEndpointFilter>()
			.WithName("Insert");

	/// <summary>
	/// <c>POST /{{dataSource:string}}/table/{{database:string}}/{{schema:string}}/{{table:string}}</c><br/><br/>
	/// Body is an array of data whose property names match the primary keys of the table to delete from.<br/><br/>
	/// <i><b>Requires calls to:</b></i>
	/// <code>
	/// <see cref="TypeCache.Extensions.ServiceCollectionExtensions.AddSqlCommandRules(IServiceCollection)"/><br/>
	/// <see cref="ServiceCollectionExtensions.ConfigureSqlApi(IServiceCollection)"/><br/>
	/// </code>
	/// </summary>
	public static RouteHandlerBuilder MapSqlApiInsertBatch(this IEndpointRouteBuilder @this)
		=> @this.MapPost(Invariant($"/{{dataSource:string}}/table/{{database:string}}/{{schema:string}}/{{table:string}}/batch"), SqlApiHandler.InsertTableBatch)
			.AddEndpointFilter<SqlApiEndpointFilter>()
			.WithName("Batch Insert");

	/// <summary>
	/// <c>GET /sql-api/{{dataSource:string}}/schema/{{database:string}}</c><br/><br/>
	/// Gets database schema data.
	/// <i><b>Requires calls to:</b></i>
	/// <code>
	/// <see cref="TypeCache.Extensions.ServiceCollectionExtensions.AddSqlCommandRules(IServiceCollection)"/><br/>
	/// <see cref="ServiceCollectionExtensions.ConfigureSqlApi(IServiceCollection)"/><br/>
	/// </code>
	/// </summary>
	public static RouteHandlerBuilder MapSqlApiSchemaGet(this IEndpointRouteBuilder @this)
		=> @this.MapGet(Invariant($"/{{dataSource:string}}/schema/{{database:string}}"), SqlApiHandler.GetSchema)
			.AddEndpointFilter<SqlApiEndpointFilter>()
			.WithName("Schema");

	/// <summary>
	/// <c>GET /{{dataSource:string}}/table/{{database:string}}/{{database:string}}/{{schema:string}}/{{table:string}}</c><br/><br/>
	/// Selects, filters, sorts and pages data from a table or view.<br/><br/>
	/// <i><b>Requires calls to:</b></i>
	/// <code>
	/// <see cref="TypeCache.Extensions.ServiceCollectionExtensions.AddSqlCommandRules(IServiceCollection)"/><br/>
	/// <see cref="ServiceCollectionExtensions.ConfigureSqlApi(IServiceCollection)"/><br/>
	/// </code>
	/// </summary>
	public static RouteHandlerBuilder MapSqlApiSelectTable(this IEndpointRouteBuilder @this)
		=> @this.MapGet(Invariant($"/{{dataSource:string}}/table/{{database:string}}/{{schema:string}}/{{table:string}}"), SqlApiHandler.Select)
			.AddEndpointFilter<SqlApiEndpointFilter>()
			.WithName("Select Table");

	/// <summary>
	/// <c>GET /{{dataSource:string}}/view/{{database:string}}/{{database:string}}/{{schema:string}}/{{view:string}}</c><br/><br/>
	/// Selects, filters, sorts and pages data from a view.<br/><br/>
	/// <i><b>Requires calls to:</b></i>
	/// <code>
	/// <see cref="TypeCache.Extensions.ServiceCollectionExtensions.AddSqlCommandRules(IServiceCollection)"/><br/>
	/// <see cref="ServiceCollectionExtensions.ConfigureSqlApi(IServiceCollection)"/><br/>
	/// </code>
	/// </summary>
	public static RouteHandlerBuilder MapSqlApiSelectView(this IEndpointRouteBuilder @this)
		=> @this.MapGet(Invariant($"/{{dataSource:string}}/view/{{database:string}}/{{schema:string}}/{{view:string}}"), SqlApiHandler.Select)
			.AddEndpointFilter<SqlApiEndpointFilter>()
			.WithName("Select View");

	/// <summary>
	/// <c>PUT /{{dataSource:string}}/table/{{database:string}}/{{database:string}}/{{schema:string}}/{{table:string}}</c><br/><br/>
	/// Updates table data.<br/><br/>
	/// <i><b>Requires calls to:</b></i>
	/// <code>
	/// <see cref="TypeCache.Extensions.ServiceCollectionExtensions.AddSqlCommandRules(IServiceCollection)"/><br/>
	/// <see cref="ServiceCollectionExtensions.ConfigureSqlApi(IServiceCollection)"/><br/>
	/// </code>
	/// </summary>
	public static RouteHandlerBuilder MapSqlApiUpdate(this IEndpointRouteBuilder @this)
		=> @this.MapPut(Invariant($"/{{dataSource:string}}/table/{{database:string}}/{{schema:string}}/{{table:string}}"), SqlApiHandler.UpdateTable)
			.AddEndpointFilter<SqlApiEndpointFilter>()
			.WithName("Update");

	/// <summary>
	/// <c>PUT /{{dataSource:string}}/table/{{database:string}}/{{database:string}}/{{schema:string}}/{{table:string}}</c><br/><br/>
	/// Updates table data.<br/><br/>
	/// Body is an array of data that contains values to update in the table.<br/><br/>
	/// <i><b>Requires calls to:</b></i>
	/// <code>
	/// <see cref="TypeCache.Extensions.ServiceCollectionExtensions.AddSqlCommandRules(IServiceCollection)"/><br/>
	/// <see cref="ServiceCollectionExtensions.ConfigureSqlApi(IServiceCollection)"/><br/>
	/// </code>
	/// </summary>
	public static RouteHandlerBuilder MapSqlApiUpdateBatch(this IEndpointRouteBuilder @this)
		=> @this.MapPut(Invariant($"/{{dataSource:string}}/table/{{database:string}}/{{schema:string}}/{{table:string}}/batch"), SqlApiHandler.UpdateTableBatch)
			.AddEndpointFilter<SqlApiEndpointFilter>()
			.WithName("Batch Update");

	private static JsonSerializerOptions CreateJsonSerializerOptions()
	{
		var options = new JsonSerializerOptions
		{
			PropertyNameCaseInsensitive = true,
			PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
			WriteIndented = true
		};
		options.Converters.Add(new StringValuesJsonConverter());
		return options;
	}
}
