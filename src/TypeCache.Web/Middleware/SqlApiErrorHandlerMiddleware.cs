// Copyright (c) 2021 Samuel Abraham

using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TypeCache.Extensions;
using static System.Net.Mime.MediaTypeNames;

namespace TypeCache.Web.Middleware;

public class SqlApiErrorHandlerMiddleware(ILogger<SqlApiErrorHandlerMiddleware>? logger = null
		, [FromKeyedServices(nameof(SqlApiErrorHandlerMiddleware))] JsonSerializerOptions? jsonSerializerOptions = null)
	: IMiddleware
{
	public async Task InvokeAsync(HttpContext context, RequestDelegate next)
	{
		try
		{
			context.Response.ContentType ??= Application.Json;
			await next.Invoke(context);
		}
		catch (AggregateException aggregateException)
		{
			logger?.LogAggregateException(aggregateException, "{Middleware} Error", [nameof(SqlApiErrorHandlerMiddleware)]);

			context.Response.StatusCode = StatusCodes.Status500InternalServerError;
			var message = aggregateException.InnerException is not null
				? aggregateException.InnerException.Message
				: string.Join(", ", aggregateException.InnerExceptions.Select(_ => _.Message));
			await JsonSerializer.SerializeAsync(context.Response.Body, new ProblemDetails
			{
				Detail = message,
				Status = context.Response.StatusCode,
				Title = nameof(SqlApiErrorHandlerMiddleware)
			}, jsonSerializerOptions, context.RequestAborted);
		}
		catch (Exception exception)
		{
			logger?.LogError(exception, "{Middleware} Error", [nameof(SqlApiErrorHandlerMiddleware)]);

			context.Response.StatusCode = StatusCodes.Status500InternalServerError;
			await JsonSerializer.SerializeAsync(context.Response.Body, new ProblemDetails
			{
				Detail = exception.Message,
				Status = context.Response.StatusCode,
				Title = nameof(SqlApiErrorHandlerMiddleware)
			}, jsonSerializerOptions, context.RequestAborted);
		}
	}
}
