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
		public SchemaMiddleware(RequestDelegate _, ISqlApi sqlApi, IMediator mediator)
			: base(sqlApi, mediator)
		{
		}

		public async Task Invoke(HttpContext httpContext)
		{
			var dataSource = httpContext.Request.Query["dataSource"].First();
			var name = httpContext.Request.Query["object"].First();
			if (!dataSource.IsBlank() && !name.IsBlank())
			{
				var schema = this.SqlApi.GetObjectSchema(dataSource, name);
				await JsonSerializer.SerializeAsync(httpContext.Response.Body, schema);
			}
		}
	}
}
