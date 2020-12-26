// Copyright (c) 2021 Samuel Abraham

using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using TypeCache.Business;
using TypeCache.Data;

namespace TypeCache.Web.Middleware
{
	public class InsertMiddleware : DataMiddleware
	{
		public InsertMiddleware(RequestDelegate _, string providerName, string connectionString, IMediator mediator)
			: base(providerName, connectionString, mediator)
		{
		}

		public async Task Invoke(HttpContext httpContext)
			=> await this.HandleRequest<InsertRequest, RowSet>(httpContext);
	}
}
