// Copyright (c) 2021 Samuel Abraham

using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using TypeCache.Extensions;
using TypeCache.Web.Attributes;
using TypeCache.Web.Extensions;
using TypeCache.Web.Requirements;

namespace TypeCache.Web.Handlers;

public class HeaderAuthorizationHandler : AuthorizationHandler<HeaderAuthorizationRequirement>
{
	private IHttpContextAccessor _HttpContextAccessor;

	public HeaderAuthorizationHandler(IHttpContextAccessor httpContextAccessor)
	{
		this._HttpContextAccessor = httpContextAccessor;
	}

	protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, HeaderAuthorizationRequirement requirement)
	{
		var controller = context.GetControllerActionDescriptor();
		if (controller is not null)
		{
			var type = controller.ControllerTypeInfo.GetType();
			var headers = this._HttpContextAccessor.HttpContext!.Request.Headers;
			var success = controller.ControllerTypeInfo
				.GetCustomAttributes<RequireHeaderAttribute>()
				.Append(controller.MethodInfo.GetCustomAttribute<RequireHeaderAttribute>())
				.All(attribute => headers.TryGetValue(attribute!.Key, out var values)
					&& (!attribute.AllowedValues.Any() || values.Any(value => attribute.AllowedValues.Contains(value, StringComparer.OrdinalIgnoreCase))));

			if (success)
				context.Succeed(requirement);
			else
				context.Fail();
		}
		return Task.CompletedTask;
	}
}
