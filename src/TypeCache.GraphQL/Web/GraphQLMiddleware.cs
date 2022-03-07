// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.SystemTextJson;
using GraphQL.Types;
using GraphQL.Validation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using TypeCache.Collections.Extensions;
using static System.Net.Mime.MediaTypeNames;

namespace TypeCache.GraphQL.Web;

public class GraphQLMiddleware<T>
	where T : ISchema
{
	private readonly RequestDelegate _Next;
	private readonly PathString _Route;
	private readonly JsonSerializerOptions _JsonSerializerOptions;

	public GraphQLMiddleware(RequestDelegate next, PathString route)
	{
		this._Next = next;
		this._Route = route;
		this._JsonSerializerOptions = new JsonSerializerOptions
		{
			PropertyNameCaseInsensitive = true,
			PropertyNamingPolicy = JsonNamingPolicy.CamelCase
		};
	}

	public async Task Invoke(HttpContext httpContext, IServiceProvider provider, IDocumentExecuter executer, IDocumentWriter writer)
	{
		if (!httpContext.Request.Path.Equals(this._Route))
		{
			await this._Next.Invoke(httpContext);
			return;
		}

		var request = await JsonSerializer.DeserializeAsync<GraphQLRequest>(httpContext.Request.Body, this._JsonSerializerOptions, httpContext.RequestAborted);
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
		var options = new ExecutionOptions
		{
			CancellationToken = httpContext.RequestAborted,
			Inputs = request.Variables?.ToString().ToInputs(),
			OperationName = request.OperationName,
			Query = request.Query,
			RequestServices = provider,
			Schema = provider.GetRequiredService<T>(),
			UserContext = userContext,
			ValidationRules = DocumentValidator.CoreRules
		};
		var result = await executer.ExecuteAsync(options);
		result.Extensions!["RequestId"] = requestId;
		result.Extensions["RequestTime"] = requestTime;

		httpContext.Response.ContentType = Application.Json;
		httpContext.Response.StatusCode = (int)HttpStatusCode.OK;
		await writer.WriteAsync(httpContext.Response.Body, result, httpContext.RequestAborted);
	}
}
