// Copyright (c) 2021 Samuel Abraham

using Microsoft.AspNetCore.Http;
using System.Text.Json;
using System.Threading.Tasks;
using TypeCache.Business;
using TypeCache.Data;
using TypeCache.Extensions;

namespace TypeCache.Web.Middleware
{
	public class SchemaMiddleware : DataMiddleware
	{
		public SchemaMiddleware(RequestDelegate _, string providerName, string connectionString, IMediator mediator)
			: base(providerName, connectionString, mediator)
		{
		}

		public async Task Invoke(HttpContext httpContext, ISchemaStore schemaStore)
		{
			var name = httpContext.Request.Query["object"].First();
			if (!name.IsBlank())
			{
				await using var connection = this.CreateDbConnection();
				var schema = await schemaStore.GetObjectSchema(connection, name);
				await JsonSerializer.SerializeAsync(httpContext.Response.Body, schema);
			}
		}
	}
}
