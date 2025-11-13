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

namespace TypeCache.GraphQL.Web;

public sealed class GraphQLMiddleware(RequestDelegate next, PathString route, IConfigureSchema configureSchema)
{
	public async Task Invoke(HttpContext httpContext
		, IDocumentExecuter executer
		, IDocumentExecutionListener listener
		, IGraphQLSerializer graphQLSerializer)
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

		var requestId = Guid.CreateVersion7();
		var timeProvider = httpContext.RequestServices.GetTimeProvider();
		var requestTime = timeProvider.GetLocalNow().ToISO8601();
		var userContext = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase)
		{
			{ "RequestId", requestId },
			{ "RequestTime", requestTime },
			{ nameof(httpContext.User), httpContext.User }
		};
		var schema = new Schema(httpContext.RequestServices, [configureSchema])
		{
			Description = "GraphQL schema route: " + route
		};
		var options = new ExecutionOptions
		{
			CancellationToken = httpContext.RequestAborted,
			Variables = request.Variables is not null ? new Inputs(request.Variables!) : null,
			OperationName = request.OperationName,
			Query = request.Query,
			RequestServices = httpContext.RequestServices,
			Schema = schema,
			UserContext = userContext,
			ValidationRules = DocumentValidator.CoreRules
		};
		options.Listeners.Add(listener);

		var result = await executer.ExecuteAsync(options);
		result.Extensions ??= new(2, StringComparer.OrdinalIgnoreCase);
		result.Extensions["request-id"] = requestId;
		result.Extensions["request-time"] = requestTime;

		if (result.Errors?.Count > 0)
		{
			var logger = httpContext.RequestServices.GetLogger<GraphQLMiddleware>();
			Exception error = result.Errors.First();
			logger?.LogError(error.InnerMostException, error.InnerMostException.Message);
		}

		httpContext.Response.ContentType = Application.Json;
		httpContext.Response.StatusCode = StatusCodes.Status200OK;
		await graphQLSerializer.WriteAsync(httpContext.Response.Body, result, httpContext.RequestAborted);
	}
}
