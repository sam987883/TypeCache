// Copyright (c) 2021 Samuel Abraham

using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TypeCache.Business;
using TypeCache.Converters;

namespace TypeCache.Web.Middleware
{
	public abstract class DataMiddleware
	{
		protected IMediator Mediator { get; }

		public DataMiddleware(IMediator mediator)
		{
			this.Mediator = mediator;
		}

		protected async ValueTask<T?> GetRequest<T>(HttpContext httpContext)
		{
			var jsonOptions = new JsonSerializerOptions();
			jsonOptions.Converters.Add(new ObjectJsonConverter());
			return await JsonSerializer.DeserializeAsync<T>(httpContext.Request.Body, jsonOptions);
		}

		protected async ValueTask HandleRequest<T, R>(HttpContext httpContext)
		{
			var request = await this.GetRequest<T>(httpContext);
			await this.HandleRequest<T, R>(request!, httpContext);
		}

		protected async ValueTask HandleRequest<T, R>(T request, HttpContext httpContext)
		{
			try
			{
				var response = await this.Mediator.ApplyRulesAsync<T, R>(request);
				httpContext.Response.ContentType = "application/json";
				await JsonSerializer.SerializeAsync(httpContext.Response.Body, response);
			}
			catch (Exception exception)
			{
				httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
				await JsonSerializer.SerializeAsync(httpContext.Response.Body, new[] { exception.Message, exception.StackTrace });
			}
		}

		protected async ValueTask HandleSqlRequest<T>(HttpContext httpContext)
		{
			var request = await this.GetRequest<T>(httpContext);
			try
			{
				var response = await this.Mediator.ApplyRulesAsync<T, string>(request!);
				httpContext.Response.ContentType = "text/plain";
				httpContext.Response.StatusCode = (int)HttpStatusCode.OK;
				await httpContext.Response.WriteAsync(response);
			}
			catch (Exception exception)
			{
				httpContext.Response.ContentType = "application/json";
				httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
				await JsonSerializer.SerializeAsync(httpContext.Response.Body, new[] { exception.Message, exception.StackTrace });
			}
		}
	}
}
