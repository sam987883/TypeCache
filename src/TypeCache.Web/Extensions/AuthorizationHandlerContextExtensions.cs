﻿// Copyright (c) 2021 Samuel Abraham

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;

namespace TypeCache.Web.Extensions;

public static class AuthorizationHandlerContextExtensions
{
	public static ControllerActionDescriptor? GetControllerActionDescriptor(this AuthorizationHandlerContext @this)
		=> @this.Resource switch
		{
			Endpoint endpoint => endpoint.Metadata.OfType<ControllerActionDescriptor>().First(), // Web API
			AuthorizationFilterContext authorizationFilterContext => authorizationFilterContext.ActionDescriptor as ControllerActionDescriptor, // MVC
			_ => null
		};
}
