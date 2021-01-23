// Copyright (c) 2021 Samuel Abraham

using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TypeCache.Extensions;

namespace TypeCache.Web.Middleware
{
	public class SchemaSqlMiddleware
	{
		public SchemaSqlMiddleware(RequestDelegate _) { }

		public async Task Invoke(HttpContext httpContext)
		{
			var name = httpContext.Request.Query["object"].First();
			await httpContext.Response.WriteAsync(!name.IsBlank()
				? $"DECLARE @ObjectName AS VARCHAR(MAX) = '{name.EscapeValue()}';\r\n{DbConnectionExtensions.ObjectSchemaSQL}"
				: DbConnectionExtensions.ObjectSchemaSQL);
		}
	}
}
