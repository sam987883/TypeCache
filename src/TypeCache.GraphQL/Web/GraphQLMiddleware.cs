﻿// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Validation;
using Microsoft.AspNetCore.Http;
using TypeCache.Extensions;
using TypeCache.GraphQL.Types;
using static System.FormattableString;
using static System.Net.Mime.MediaTypeNames;

namespace TypeCache.GraphQL.Web;

public sealed class GraphQLMiddleware
{
	private readonly RequestDelegate _Next;
	private readonly string _GraphQLSchemaName;
	private readonly PathString _Route;
	private readonly JsonSerializerOptions _JsonSerializerOptions;

	public GraphQLMiddleware(RequestDelegate next, string graphQLSchemaName, PathString route)
	{
		this._GraphQLSchemaName = graphQLSchemaName;
		this._Next = next;
		this._Route = route;
		this._JsonSerializerOptions = new JsonSerializerOptions
		{
			PropertyNameCaseInsensitive = true,
			PropertyNamingPolicy = JsonNamingPolicy.CamelCase
		};
	}

	public async Task Invoke(HttpContext httpContext
		, IServiceProvider provider
		, IDocumentExecuter executer
		, IGraphQLSerializer graphQLSerializer
		, IAccessor<GraphQLSchema> graphQLSchemaAccessor)
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
		var graphQLSchema = graphQLSchemaAccessor[this._GraphQLSchemaName];
		graphQLSchema.AssertNotNull();
        graphQLSchema.Description = Invariant($"GraphQL schema `{this._GraphQLSchemaName}` route: {this._Route}");
		var options = new ExecutionOptions
		{
			CancellationToken = httpContext.RequestAborted,
			Variables = request.Variables is not null ? new Inputs(request.Variables) : null,
			OperationName = request.OperationName,
			Query = request.Query,
			RequestServices = provider,
			Schema = graphQLSchema,
			UserContext = userContext,
			ValidationRules = DocumentValidator.CoreRules
		};
		var result = await executer.ExecuteAsync(options);
		var error = result.Errors?[0].InnerException;
		if (result.Extensions is not null)
		{
			result.Extensions["RequestId"] = requestId;
			result.Extensions["RequestTime"] = requestTime;
		}
		else
			result.Extensions = new Dictionary<string, object?>(2, StringComparer.OrdinalIgnoreCase);

		if (error is not null)
		{
			result.Extensions["ErrorMessage"] = error.Message.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
			result.Extensions["ErrorStackTrace"] = error.StackTrace?.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).Select(_ => _.Trim());
		}

		httpContext.Response.ContentType = Application.Json;
		httpContext.Response.StatusCode = (int)HttpStatusCode.OK;
		await graphQLSerializer.WriteAsync(httpContext.Response.Body, result, httpContext.RequestAborted);
	}
}
