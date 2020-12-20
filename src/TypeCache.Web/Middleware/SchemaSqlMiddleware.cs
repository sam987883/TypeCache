// Copyright (c) 2021 Samuel Abraham

using Microsoft.AspNetCore.Http;
using TypeCache.Extensions;
using TypeCache.Data;
using System.Threading.Tasks;

namespace TypeCache.Web.Middleware
{
	public class SchemaSqlMiddleware
    {
        public SchemaSqlMiddleware(RequestDelegate _) { }

        public async Task Invoke(HttpContext httpContext, ISchemaFactory schemaFactory)
        {
            var name = httpContext.Request.Query["object"].First();
            await httpContext.Response.WriteAsync(!name.IsBlank()
                ? $"DECLARE @ObjectName AS VARCHAR(MAX) = '{name.EscapeValue()}';\r\n{schemaFactory.SQL}"
                : schemaFactory.SQL);
        }
    }
}
