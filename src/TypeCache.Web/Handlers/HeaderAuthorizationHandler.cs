// Copyright (c) 2021 Samuel Abraham

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using TypeCache.Extensions;
using TypeCache.Reflection.Extensions;
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

	protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, HeaderAuthorizationRequirement requirement)
	{
		var controller = context.GetControllerActionDescriptor();
		if (controller is not null)
		{
			var type = controller.ControllerTypeInfo.GetTypeMember();
			var method = type.GetMethod(controller.MethodInfo.MethodHandle)!;
			var headers = this._HttpContextAccessor.HttpContext!.Request.Headers;
			var success = type
				.Attributes.OfType<RequireHeaderAttribute>()
				.Union(method.Attributes.OfType<RequireHeaderAttribute>())
				.All(attribue => headers.TryGetValue(attribue!.Key, out var values)
					&& (!attribue.AllowedValues.Any() || values.Any(value => attribue.AllowedValues.Contains(value, StringComparer.OrdinalIgnoreCase))));

			if (success)
				context.Succeed(requirement);
			else
				context.Fail();
		}
		await Task.CompletedTask;
	}
}
