// Copyright (c) 2021 Samuel Abraham

using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TypeCache.Extensions;
using TypeCache.Mediation;
using static System.Net.Mime.MediaTypeNames;

namespace TypeCache.Web.Middleware;

public class SqlApiErrorHandlerMiddleware : WebMiddleware
{
	public SqlApiErrorHandlerMiddleware(IMediator mediator, JsonSerializerOptions? jsonSerializerOptions = null)
		: base(mediator, jsonSerializerOptions)
	{
	}

	public override async Task InvokeAsync(HttpContext context, RequestDelegate next)
	{
		try
		{
			await next.Invoke(context);
		}
		catch (AggregateException aggregateException)
		{
			context.Response.ContentType = Application.Json;
			context.Response.StatusCode = StatusCodes.Status500InternalServerError;
			var exception = aggregateException.InnerException ?? aggregateException;
			await JsonSerializer.SerializeAsync(context.Response.Body, new
			{
				ErrorType = exception.GetType().Name(),
				ErrorMessage = exception.Message,
				exception.StackTrace
			});
		}
		catch (Exception exception)
		{
			context.Response.ContentType = Application.Json;
			context.Response.StatusCode = StatusCodes.Status500InternalServerError;
			await JsonSerializer.SerializeAsync(context.Response.Body, new
			{
				ErrorType = exception.GetType().Name(),
				ErrorMessage = exception.Message,
				exception.StackTrace
			});
		}
	}
}
