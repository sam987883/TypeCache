// Copyright (c) 2021 Samuel Abraham

using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TypeCache.Business;
using TypeCache.Extensions;

namespace TypeCache.Web.Middleware
{
	public class SchemaMiddleware : DataMiddleware
	{
		public SchemaMiddleware(RequestDelegate _, string providerName, string connectionString, IMediator mediator)
			: base(providerName, connectionString, mediator)
		{
		}

		public async Task Invoke(HttpContext httpContext)
		{
			var name = httpContext.Request.Query["object"].First();
			if (!name.IsBlank())
			{
				await using var dbConnection = this.CreateDbConnection();
				var schema = await dbConnection.GetObjectSchema(name);
				await JsonSerializer.SerializeAsync(httpContext.Response.Body, schema);
			}
		}
	}
}
