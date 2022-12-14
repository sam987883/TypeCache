// Copyright (c) 2021 Samuel Abraham

using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TypeCache.Mediation;

namespace TypeCache.Web.Middleware
{
	public abstract class WebMiddleware : IMiddleware
	{
		protected WebMiddleware(IMediator mediator, JsonSerializerOptions? jsonSerializerOptions = null)
		{
			this.JsonSerializerOptions = jsonSerializerOptions;
			this.Mediator = mediator;
		}

		protected JsonSerializerOptions? JsonSerializerOptions { get; }

		protected IMediator Mediator { get; }

		public abstract Task InvokeAsync(HttpContext context, RequestDelegate next);
	}
}
