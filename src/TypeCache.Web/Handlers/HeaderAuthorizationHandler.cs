// Copyright (c) 2021 Samuel Abraham

using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using TypeCache.Extensions;
using TypeCache.Web.Attributes;
using TypeCache.Web.Extensions;
using TypeCache.Web.Requirements;

namespace TypeCache.Web.Handlers;

public class HeaderAuthorizationHandler(IHttpContextAccessor httpContextAccessor) : AuthorizationHandler<HeaderAuthorizationRequirement>
{
	protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, HeaderAuthorizationRequirement requirement)
		=> context.GetControllerActionDescriptor() switch
		{
			null => Task.CompletedTask,
			var controller => Task.Run(() =>
			{
				var headers = httpContextAccessor.HttpContext!.Request.Headers;
				var attributes = controller.ControllerTypeInfo.GetCustomAttributes<RequireHeaderAttribute>()
					.Concat(controller.MethodInfo.GetCustomAttributes<RequireHeaderAttribute>());

				var success = attributes.All(_ => _.AllowedValues.Length is 0
					? headers.ContainsKey(_.Header)
					: headers.TryGetValue(_.Header, out var values) && values.Any(_.AllowedValues.ContainsIgnoreCase!));
				if (success)
					context.Succeed(requirement);
				else
					context.Fail();
			}, httpContextAccessor.HttpContext!.RequestAborted)
		};
}
