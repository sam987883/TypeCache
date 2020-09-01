// Copyright (c) 2020 Samuel Abraham

using Microsoft.AspNetCore.Http;
using Sam987883.Common.Extensions;
using Sam987883.Database;
using Sam987883.Database.Extensions;
using System.Threading.Tasks;

namespace Sam987883.Web.Middleware
{
	public class SchemaSQLMiddleware
    {
        public SchemaSQLMiddleware(RequestDelegate _) { }

        public async Task Invoke(HttpContext httpContext, ISchemaFactory schemaFactory)
        {
            var name = httpContext.Request.Query["object"].First().Value;
            await httpContext.Response.WriteAsync(!name.IsBlank()
                ? $"DECLARE @ObjectName AS VARCHAR(MAX) = '{name.EscapeValue()}';\r\n{schemaFactory.SQL}"
                : schemaFactory.SQL);
        }
    }
}
