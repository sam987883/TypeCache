// Copyright (c) 2021 Samuel Abraham

using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TypeCache.Business;
using TypeCache.Data.Requests;

namespace TypeCache.Web.Middleware
{
	public class DeleteDataSqlMiddleware : DataMiddleware
	{
		public DeleteDataSqlMiddleware(RequestDelegate _, IMediator mediator)
			: base(mediator)
		{
		}

		public async Task Invoke(HttpContext httpContext)
			=> await this.HandleSqlRequest<DeleteDataRequest>(httpContext);
	}
}
