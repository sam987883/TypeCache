// Copyright (c) 2021 Samuel Abraham

using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TypeCache.Business;
using TypeCache.Data;

namespace TypeCache.Web.Middleware
{
	public class UpdateMiddleware : DataMiddleware
	{
		public UpdateMiddleware(RequestDelegate _, string providerName, string connectionString, IMediator mediator)
			: base(providerName, connectionString, mediator)
		{
		}

		public async Task Invoke(HttpContext httpContext)
			=> await this.HandleRequest<UpdateRequest, RowSet>(httpContext);
	}
}
