// Copyright (c) 2021 Samuel Abraham

using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TypeCache.Business;
using TypeCache.Converters;
using TypeCache.Data;

namespace TypeCache.Web.Middleware
{
	public abstract class DataMiddleware
	{
		protected IMediator Mediator { get; }
		protected ISqlApi SqlApi { get; }

		public DataMiddleware(ISqlApi sqlApi, IMediator mediator)
		{
			this.SqlApi = sqlApi;
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
			var response = await this.Mediator.ApplyRuleAsync<ISqlApi, T, R>(this.SqlApi, request);

			httpContext.Response.ContentType = "application/json";
			if (response.Exception == null)
				await JsonSerializer.SerializeAsync(httpContext.Response.Body, response.Result);
			else
			{
				httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
				await JsonSerializer.SerializeAsync(httpContext.Response.Body, new[] { response.Exception.Message, response.Exception.StackTrace });
			}
		}

		protected async ValueTask HandleSqlRequest<T>(HttpContext httpContext)
		{
			var request = await this.GetRequest<T>(httpContext);
			var response = await this.Mediator.ApplyRuleAsync<T, string>(request!);

			if (response.Exception == null)
			{
				httpContext.Response.ContentType = "text/plain";
				httpContext.Response.StatusCode = (int)HttpStatusCode.OK;
				await httpContext.Response.WriteAsync(response.Result);
			}
			else
			{
				httpContext.Response.ContentType = "application/json";
				httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
				await JsonSerializer.SerializeAsync(httpContext.Response.Body, new[] { response.Exception.Message, response.Exception.StackTrace });
			}
		}
	}
}
