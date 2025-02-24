// Copyright (c) 2021 Samuel Abraham

using System.Text;
using System.Text.Json;
using System.Xml.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Primitives;
using TypeCache.Converters;
using TypeCache.Extensions;
using static System.Net.Mime.MediaTypeNames;

namespace TypeCache.Web.Extensions;

public static partial class EndpointRouteBuilderExtensions
{
	internal static class Route
	{
		[StringSyntax(nameof(Route))]
		public const string API = "/api";

		[StringSyntax(nameof(Route))]
		public const string PING = "/ping";

		[StringSyntax(nameof(Route))]
		public const string SQL = "/sql";

		[StringSyntax(nameof(Route))]
		public const string REQUEST_HEADERS = "/request/headers";

		internal static class SqlApi
		{
			[StringSyntax(nameof(Route))]
			public const string DATABASE_SCHEMA = "/schema/{dataSource}/{database}/{collection}";

			[StringSyntax(nameof(Route))]
			public const string TABLE = "/{dataSource}/{database}/{schema}/{table}";

			[StringSyntax(nameof(Route))]
			public const string DELETE = "/delete" + TABLE;

			[StringSyntax(nameof(Route))]
			public const string DELETE_VALUES = "/delete-values" + TABLE;

			[StringSyntax(nameof(Route))]
			public const string INSERT = "/insert" + TABLE;

			[StringSyntax(nameof(Route))]
			public const string INSERT_VALUES = "/insert-values" + TABLE;

			[StringSyntax(nameof(Route))]
			public const string PROCEDURE = "/execute/{dataSource}/{database}/{schema}/{procedure}";

			[StringSyntax(nameof(Route))]
			public const string SELECT = "/select" + TABLE;

			[StringSyntax(nameof(Route))]
			public const string UPDATE = "/update" + TABLE;

			[StringSyntax(nameof(Route))]
			public const string UPDATE_VALUES = "/update-values" + TABLE;
		}
	}

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
		=> @this.MapGet(Route.REQUEST_HEADERS, async context =>
		{
			var acceptRequestHeader = context.Request.Headers.Accept.ToString();
			context.Response.StatusCode = context.Request.Headers.Any() ? StatusCodes.Status200OK : StatusCodes.Status204NoContent;
			context.Response.Headers.ContentType = acceptRequestHeader switch
			{
				Text.Plain or Text.Html or Application.Xml or Text.Xml or Application.Json => acceptRequestHeader,
				_ => Application.Json
			};
			var response = string.Empty;
			if (acceptRequestHeader.EqualsIgnoreCase(Text.Plain))
			{
				var responseBuilder = new StringBuilder();
				foreach (var pair in context.Request.Headers)
					responseBuilder.AppendLine(Invariant($"{pair.Key}: {pair.Value}<br>"));

				response = responseBuilder.ToString();
				await context.Response.WriteAsync(response, context.RequestAborted);
				return;
			}
			else if (acceptRequestHeader.EqualsIgnoreCase(Text.Html))
			{
				var htmlBuilder = new StringBuilder("<table>").AppendLine()
					.AppendLine("<tr><th>Header</th><th>Value</th></tr>");
				foreach (var pair in context.Request.Headers)
					htmlBuilder.AppendLine(Invariant($"<tr><td>{pair.Key}</td><td>{pair.Value}</td></tr>"));

				htmlBuilder.AppendLine("</table>");
				response = htmlBuilder.ToString();
				await context.Response.WriteAsync(response, context.RequestAborted);
				return;
			}
			else if (acceptRequestHeader.EqualsIgnoreCase(Application.Xml)
				|| acceptRequestHeader.EqualsIgnoreCase(Text.Xml))
			{
				var headers = new XElement("headers");
				foreach (var pair in context.Request.Headers)
					headers.Add(new XElement("header", new XAttribute("name", pair.Key), pair.Value.ToString()));

				response = new XDocument(headers).ToString();
				await context.Response.WriteAsync(response, context.RequestAborted);
				return;
			}

			await context.Response.WriteAsJsonAsync(context.Request.Headers, CreateJsonSerializerOptions(), Application.Json, context.RequestAborted);
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
		=> @this.MapGet(Invariant($"{Route.REQUEST_HEADERS}/{(key.IsNotBlank() ? key : "{key}")}"), async context =>
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
				(StatusCodes.Status200OK, Text.Html) => Invariant($"<h1>{key}</h1><br><b>{value}<b/>"),
				(StatusCodes.Status200OK, Application.Xml or Text.Xml) => new XDocument(new XElement(key.Replace(' ', '_'), value.ToString())).ToString(),
				(StatusCodes.Status200OK, _) => JsonSerializer.Serialize(new Dictionary<string, string?>(1) { { key, value.ToString() } }, CreateJsonSerializerOptions()),
				(StatusCodes.Status204NoContent, Text.Plain) => Invariant($"{key}: "),
				(StatusCodes.Status204NoContent, Text.Html) => Invariant($"<h1>{key}</h1><br>"),
				(StatusCodes.Status204NoContent, Application.Xml or Text.Xml) => new XDocument(new XElement(key.Replace(' ', '_'))).ToString(),
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
		=> @this.MapGet(Invariant($"{Route.PING}/{name}"), async (HttpContext context, IHttpClientFactory factory) =>
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
