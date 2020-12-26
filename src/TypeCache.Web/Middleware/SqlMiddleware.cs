// Copyright (c) 2021 Samuel Abraham

using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;
using TypeCache.Business;
using TypeCache.Data;
using TypeCache.Extensions;

namespace TypeCache.Web.Middleware
{
	public class SqlMiddleware : DataMiddleware
	{
		public SqlMiddleware(RequestDelegate _, string providerName, string connectionString, IMediator mediator)
			: base(providerName, connectionString, mediator)
		{
		}

		public async Task Invoke(HttpContext httpContext)
		{
			using var reader = new StreamReader(httpContext.Request.Body);
			var request = new SqlRequest
			{
				Parameters = httpContext.Request.Query
					.To(query => new Parameter
					{
						Name = query.Key,
						Value = query.Value.First()
					}).ToArrayOf(httpContext.Request.Query.Count),
				SQL = reader.ReadToEnd()
			};

			await this.HandleRequest<SqlRequest, RowSet[]>(request, httpContext);
		}
	}
}
