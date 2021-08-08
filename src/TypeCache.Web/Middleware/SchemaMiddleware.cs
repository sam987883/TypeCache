// Copyright (c) 2021 Samuel Abraham

using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TypeCache.Business;
using TypeCache.Collections.Extensions;
using TypeCache.Data;
using TypeCache.Extensions;

namespace TypeCache.Web.Middleware
{
	public class SchemaMiddleware : DataMiddleware
	{
		private readonly ISqlApi _SqlApi;

		public SchemaMiddleware(RequestDelegate _, ISqlApi sqlApi, IMediator mediator)
			: base(mediator)
		{
			this._SqlApi = sqlApi;
		}

		public async Task Invoke(HttpContext httpContext)
		{
			var dataSource = httpContext.Request.Query["dataSource"].First();
			var name = httpContext.Request.Query["object"].First();
			if (!dataSource.IsBlank() && !name.IsBlank())
			{
				var schema = this._SqlApi.GetObjectSchema(dataSource, name);
				await JsonSerializer.SerializeAsync(httpContext.Response.Body, schema);
			}
		}
	}
}
