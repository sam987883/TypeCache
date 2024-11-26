// Copyright (c) 2021 Samuel Abraham

using GraphQL;
using GraphQL.DI;
using GraphQL.Execution;
using GraphQL.Types;
using GraphQL.Validation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using TypeCache.Extensions;
using static System.Net.Mime.MediaTypeNames;
using static System.StringSplitOptions;

namespace TypeCache.GraphQL.Web;

public sealed class GraphQLMiddleware(RequestDelegate next, PathString route, IConfigureSchema configureSchema)
{
	public async Task Invoke(HttpContext httpContext
		, IServiceProvider provider
		, IDocumentExecuter executer
		, IDocumentExecutionListener listener
		, IGraphQLSerializer graphQLSerializer
		, ILogger<GraphQLMiddleware> logger)
	{
		if (!httpContext.Request.Path.Equals(route))
		{
			await next.Invoke(httpContext);
			return;
		}

		var request = await graphQLSerializer.ReadAsync<GraphQLRequest>(httpContext.Request.Body, httpContext.RequestAborted);
		if (request is null)
		{
			await next.Invoke(httpContext);
			return;
		}

		var requestId = Guid.NewGuid();
		var timeProvider = provider.GetService(typeof(TimeProvider)) as TimeProvider ?? TimeProvider.System;
		var requestTime = timeProvider.GetLocalNow().ToISO8601();
		var userContext = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase)
		{
			{ "RequestId", requestId },
			{ "RequestTime", requestTime },
			{ nameof(httpContext.User), httpContext.User }
		};
		var schema = new Schema(provider, [configureSchema])
		{
			Description = "GraphQL schema route: " + route
		};
		var options = new ExecutionOptions
		{
			CancellationToken = httpContext.RequestAborted,
			Variables = request.Variables is not null ? new Inputs(request.Variables!) : null,
			OperationName = request.OperationName,
			Query = request.Query,
			RequestServices = provider,
			Schema = schema,
			UserContext = userContext,
			ValidationRules = DocumentValidator.CoreRules
		};
		options.Listeners.Add(listener);
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

			char[] separator = ['\r', '\n'];
			result.Extensions!["ErrorMessage"] = error.Message.Split(separator, RemoveEmptyEntries);
			result.Extensions["ErrorStackTrace"] = error.StackTrace?.Split(separator, RemoveEmptyEntries).Select(_ => _.Trim());
		}

		httpContext.Response.ContentType = Application.Json;
		httpContext.Response.StatusCode = StatusCodes.Status200OK;
		await graphQLSerializer.WriteAsync(httpContext.Response.Body, result, httpContext.RequestAborted);
	}
}
