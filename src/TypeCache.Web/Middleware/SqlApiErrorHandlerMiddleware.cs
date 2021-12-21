// Copyright (c) 2021 Samuel Abraham

using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using static System.Net.Mime.MediaTypeNames;

namespace TypeCache.Web.Middleware;

public class SqlApiErrorHandlerMiddleware
{
	private readonly RequestDelegate _Next;

	public SqlApiErrorHandlerMiddleware(RequestDelegate next)
	{
		this._Next = next;
	}

	public async Task Invoke(HttpContext httpContext)
	{
		try
		{
			await this._Next.Invoke(httpContext);
		}
		catch (Exception exception)
		{
			httpContext.Response.ContentType = Application.Json;
			httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
			await JsonSerializer.SerializeAsync(httpContext.Response.Body, new
			{
				ErrorType = exception.GetType().Name,
				ErrorMessage = exception.Message,
				exception.StackTrace
			});
		}
	}
}
