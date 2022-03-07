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
using TypeCache.Converters;
using TypeCache.GraphQL.Types;
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
		this._JsonSerializerOptions.Converters.Add(new TypeJsonConverter());
	}

	public async Task Invoke(HttpContext httpContext, IServiceProvider provider, IDocumentExecuter executer)
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
			Inputs = request.Variables?.ToString().ToInputs(),
			OperationName = request.OperationName,
			Query = request.Query,
			RequestServices = provider,
			Schema = provider.GetRequiredService<T>(),
			UserContext = userContext,
			ValidationRules = DocumentValidator.CoreRules
		};
		var result = await executer.ExecuteAsync(options);
		var response = new GraphQLResponse
		{
			Data = result.Data,
			Errors = result.Errors?.Map(error => new GraphQLError
			{
				Data = error.Data,
				Extensions = result.Extensions,
				Locations = error.Locations,
				Message = error.Message,
				Path = error.Path
			}).ToArray()
		};
		response.Extensions["RequestId"] = requestId;
		response.Extensions["RequestTime"] = requestTime;

		var json = JsonSerializer.Serialize(response, this._JsonSerializerOptions);
		httpContext.Response.ContentType = Application.Json;
		httpContext.Response.StatusCode = (int)HttpStatusCode.OK;
		await JsonSerializer.SerializeAsync(httpContext.Response.Body, response, this._JsonSerializerOptions);

		await this._Next.Invoke(httpContext);
	}
}
