﻿// Copyright (c) 2021 Samuel Abraham

using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TypeCache.Business;
using TypeCache.Data;

namespace TypeCache.Web.Middleware
{
	public class UpdateMiddleware : DataMiddleware
	{
		public UpdateMiddleware(RequestDelegate _, ISqlApi sqlApi, IMediator mediator)
			: base(sqlApi, mediator)
		{
		}

		public async Task Invoke(HttpContext httpContext)
			=> await this.HandleRequest<UpdateRequest, RowSet>(httpContext);
	}
}
