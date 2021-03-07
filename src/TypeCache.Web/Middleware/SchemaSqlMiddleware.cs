// Copyright (c) 2021 Samuel Abraham

using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TypeCache.Business;
using TypeCache.Collections.Extensions;
using TypeCache.Data;
using TypeCache.Data.Extensions;
using TypeCache.Extensions;

namespace TypeCache.Web.Middleware
{
	public class SchemaSqlMiddleware : DataMiddleware
	{
		public SchemaSqlMiddleware(RequestDelegate _, ISqlApi sqlApi, IMediator mediator)
			: base(sqlApi, mediator)
		{
		}

		public async Task Invoke(HttpContext httpContext)
		{
			var name = httpContext.Request.Query["object"].First();
			await httpContext.Response.WriteAsync(!name.IsBlank()
				? $"DECLARE @ObjectName AS VARCHAR(MAX) = '{name.EscapeValue()}';\r\n{this.SqlApi.ObjectSchemaSQL}"
				: this.SqlApi.ObjectSchemaSQL);
		}
	}
}
