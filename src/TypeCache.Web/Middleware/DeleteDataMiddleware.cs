// Copyright (c) 2021 Samuel Abraham

using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TypeCache.Business;
using TypeCache.Data;
using TypeCache.Data.Requests;

namespace TypeCache.Web.Middleware
{
	public class DeleteDataMiddleware : DataMiddleware
	{
		public DeleteDataMiddleware(RequestDelegate _, IMediator mediator)
			: base(mediator)
		{
		}

		public async Task Invoke(HttpContext httpContext)
			=> await this.HandleRequest<DeleteDataRequest, RowSet>(httpContext);
	}
}
