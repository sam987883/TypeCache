// Copyright (c) 2021 Samuel Abraham

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using TypeCache.Collections.Extensions;

namespace TypeCache.Web.Extensions;

public static class AuthorizationHandlerContextExtensions
{
	public static ControllerActionDescriptor? GetControllerActionDescriptor(this AuthorizationHandlerContext @this)
		=> @this.Resource switch
		{
			AuthorizationFilterContext authorizationFilterContext => authorizationFilterContext.ActionDescriptor as ControllerActionDescriptor, // MVC
			Endpoint endpoint => endpoint.Metadata.If<ControllerActionDescriptor>().First(), // Web API
			_ => null
		};
}
