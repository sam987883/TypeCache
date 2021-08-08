// Copyright (c) 2021 Samuel Abraham

using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TypeCache.Business;
using TypeCache.Data.Requests;

namespace TypeCache.Web.Middleware
{
	public class DeleteSqlMiddleware : DataMiddleware
	{
		public DeleteSqlMiddleware(RequestDelegate _, IMediator mediator)
			: base(mediator)
		{
		}

		public async Task Invoke(HttpContext httpContext)
			=> await this.HandleSqlRequest<DeleteRequest>(httpContext);
	}
}
