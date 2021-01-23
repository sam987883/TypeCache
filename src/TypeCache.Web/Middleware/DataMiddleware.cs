// Copyright (c) 2021 Samuel Abraham

using System.Data.Common;
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
		private readonly string _ConnectionString;
		private readonly DbProviderFactory _DbProviderFactory;
		private readonly IMediator _Mediator;

		public DataMiddleware(string providerName, string connectionString, IMediator mediator)
		{
			this._ConnectionString = connectionString;
			this._DbProviderFactory = DbProviderFactories.GetFactory(providerName);
			this._Mediator = mediator;
		}

		protected DbConnection CreateDbConnection()
		{
			var connection = this._DbProviderFactory.CreateConnection();
			connection.ConnectionString = this._ConnectionString;
			return connection;
		}

		protected async ValueTask<T> GetRequest<T>(HttpContext httpContext)
		{
			var jsonOptions = new JsonSerializerOptions();
			jsonOptions.Converters.Add(new ObjectJsonConverter());
			return await JsonSerializer.DeserializeAsync<T>(httpContext.Request.Body, jsonOptions);
		}

		protected async ValueTask HandleRequest<T, R>(HttpContext httpContext)
		{
			var request = await this.GetRequest<T>(httpContext);
			await this.HandleRequest<T, R>(request, httpContext);
		}

		protected async ValueTask HandleRequest<T, R>(T request, HttpContext httpContext)
		{
			await using var dbConnection = this.CreateDbConnection();
			var response = await this._Mediator.ApplyRuleAsync<DbConnection, T, R>(dbConnection, request);

			httpContext.Response.ContentType = "application/json";
			if (!response.HasError)
				await JsonSerializer.SerializeAsync(httpContext.Response.Body, response.Result);
			else
			{
				httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
				await JsonSerializer.SerializeAsync(httpContext.Response.Body, response.Messages);
			}
		}

		protected async ValueTask HandleSqlRequest<T>(HttpContext httpContext)
		{
			var request = await this.GetRequest<T>(httpContext);

			await using var dbConnection = this.CreateDbConnection();
			var response = await this._Mediator.ApplyRuleAsync<DbConnection, T, string>(dbConnection, request);

			if (!response.HasError)
			{
				httpContext.Response.ContentType = "text/plain";
				httpContext.Response.StatusCode = (int)HttpStatusCode.OK;
				await httpContext.Response.WriteAsync(response.Result);
			}
			else
			{
				httpContext.Response.ContentType = "application/json";
				httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
				await JsonSerializer.SerializeAsync(httpContext.Response.Body, response.Messages);
			}
		}
	}
}
