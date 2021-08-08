// Copyright (c) 2021 Samuel Abraham

using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TypeCache.Business;
using TypeCache.Collections.Extensions;
using TypeCache.Data.Extensions;
using TypeCache.Data.Schema;
using TypeCache.Extensions;
using static System.FormattableString;

namespace TypeCache.Web.Middleware
{
	public class SchemaSqlMiddleware : DataMiddleware
	{
		public SchemaSqlMiddleware(RequestDelegate _, IMediator mediator)
			: base(mediator)
		{
		}

		public async Task Invoke(HttpContext httpContext)
		{
			var name = httpContext.Request.Query["object"].First();
			await httpContext.Response.WriteAsync(!name.IsBlank()
				? Invariant($"DECLARE @ObjectName AS VARCHAR(8000) = '{name.EscapeValue()}';\r\n{ObjectSchema.SQL}")
				: ObjectSchema.SQL);
		}
	}
}
