// Copyright (c) 2021 Samuel Abraham

using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TypeCache.Business;
using TypeCache.Collections.Extensions;
using TypeCache.Data;
using TypeCache.Data.Requests;

namespace TypeCache.Web.Middleware
{
	public class ExecuteSqlMiddleware : DataMiddleware
	{
		public ExecuteSqlMiddleware(RequestDelegate _, IMediator mediator)
			: base(mediator)
		{
		}

		public async Task Invoke(HttpContext httpContext)
		{
			string dataSource = httpContext.Request.Query[nameof(dataSource)].First()!;
			using var reader = new StreamReader(httpContext.Request.Body);
			var request = new SqlRequest
			{
				DataSource = dataSource,
				SQL = reader.ReadToEnd()
			};
			httpContext.Request.Query.Do(query => request.Parameters[query.Key] = query.Value.First());

			await this.HandleRequest<SqlRequest, RowSet[]>(request, httpContext);
		}
	}
}
