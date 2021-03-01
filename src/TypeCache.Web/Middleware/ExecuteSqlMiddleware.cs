// Copyright (c) 2021 Samuel Abraham

using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TypeCache.Business;
using TypeCache.Collections.Extensions;
using TypeCache.Data;
using TypeCache.Extensions;

namespace TypeCache.Web.Middleware
{
	public class ExecuteSqlMiddleware : DataMiddleware
	{
		public ExecuteSqlMiddleware(RequestDelegate _, string providerName, string connectionString, IMediator mediator)
			: base(providerName, connectionString, mediator)
		{
		}

		public async Task Invoke(HttpContext httpContext)
		{
			using var reader = new StreamReader(httpContext.Request.Body);
			var request = new SqlRequest
			{
				Parameters = httpContext.Request.Query
					.To(query => new Parameter(query.Key, query.Value.First()))
					.ToArray(httpContext.Request.Query.Count),
				SQL = reader.ReadToEnd()
			};

			await this.HandleRequest<SqlRequest, RowSet[]>(request, httpContext);
		}
	}
}
