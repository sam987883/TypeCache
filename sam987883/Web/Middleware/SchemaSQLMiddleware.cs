// Copyright (c) 2020 Samuel Abraham

using Microsoft.AspNetCore.Http;
using sam987883.Common.Extensions;
using sam987883.Database;
using sam987883.Database.Extensions;
using System.Threading.Tasks;

namespace sam987883.Web.Middleware
{
	public class SchemaSQLMiddleware
    {
        public SchemaSQLMiddleware(RequestDelegate _) { }

        public async Task Invoke(HttpContext httpContext)
        {
            var name = httpContext.Request.Query["object"].First().Value;
            var sql = SchemaFactory.ObjectSchemaSQL;
            if (!name.IsBlank())
                sql = $"DECLARE @{SchemaFactory.OBJECT_NAME} AS VARCHAR(8000) = '{name.EscapeValue()}';\r\n{sql}";
            await httpContext.Response.WriteAsync(sql);
        }
    }
}
