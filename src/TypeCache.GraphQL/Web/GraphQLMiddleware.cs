// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.DI;
using GraphQL.Types;
using GraphQL.Validation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using TypeCache.Extensions;
using TypeCache.GraphQL.Types;
using static System.FormattableString;
using static System.Net.Mime.MediaTypeNames;
using static System.StringSplitOptions;

namespace TypeCache.GraphQL.Web;

public sealed class GraphQLMiddleware
{
	private readonly JsonSerializerOptions _JsonOptions;
	private readonly RequestDelegate _Next;
	private readonly PathString _Route;
	private readonly ISchema _Schema;

	public GraphQLMiddleware(RequestDelegate next, PathString route, IConfigureSchema configureSchema, IServiceProvider provider)
	{
		this._Next = next;
		this._Route = route;
		this._JsonOptions = new()
		{
			PropertyNameCaseInsensitive = true,
			PropertyNamingPolicy = JsonNamingPolicy.CamelCase
		};
		this._Schema = new Schema(provider, new[] { configureSchema });
	}

	public async Task Invoke(HttpContext httpContext, IServiceProvider provider, IDocumentExecuter executer, IGraphQLSerializer graphQLSerializer, ILogger<GraphQLMiddleware> logger)
	{
		if (!httpContext.Request.Path.Equals(this._Route))
		{
			await this._Next.Invoke(httpContext);
			return;
		}

		var request = await JsonSerializer.DeserializeAsync<GraphQLRequest>(httpContext.Request.Body, this._JsonOptions, httpContext.RequestAborted);
		if (request is null)
		{
			await this._Next.Invoke(httpContext);
			return;
		}

		var requestId = Guid.NewGuid();
		var requestTime = DateTime.UtcNow;
		var userContext = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase)
		{
			{ "RequestId", requestId },
			{ "RequestTime", requestTime },
			{ nameof(httpContext.User), httpContext.User }
		};
		this._Schema.Description = Invariant($"GraphQL schema route: {this._Route}");
		var options = new ExecutionOptions
		{
			CancellationToken = httpContext.RequestAborted,
			Variables = request.Variables is not null ? new Inputs(request.Variables) : null,
			OperationName = request.OperationName,
			Query = request.Query,
			RequestServices = provider,
			Schema = this._Schema,
			UserContext = userContext,
			ValidationRules = DocumentValidator.CoreRules
		};
		var result = await executer.ExecuteAsync(options);
		result.Extensions ??= new(2, StringComparer.OrdinalIgnoreCase);

		var error = result.Errors?[0].InnerException;
		if (result.Extensions is not null)
		{
			result.Extensions["RequestId"] = requestId;
			result.Extensions["RequestTime"] = requestTime;
		}

		if (error is not null)
		{
			logger?.LogError(error, result.Errors?[0].Message);

			var separator = new[] { '\r', '\n' };
			result.Extensions!["ErrorMessage"] = error.Message.Split(separator, RemoveEmptyEntries);
			result.Extensions["ErrorStackTrace"] = error.StackTrace?.Split(separator, RemoveEmptyEntries).Select(_ => _.Trim());
		}

		httpContext.Response.ContentType = Application.Json;
		httpContext.Response.StatusCode = (int)HttpStatusCode.OK;
		await graphQLSerializer.WriteAsync(httpContext.Response.Body, result, httpContext.RequestAborted);
	}
}
