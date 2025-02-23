// Copyright (c) 2021 Samuel Abraham

using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using TypeCache.Extensions;
using TypeCache.Web.Attributes;
using TypeCache.Web.Extensions;
using TypeCache.Web.Requirements;

namespace TypeCache.Web.Handlers;

public class ClaimAuthorizationHandler : AuthorizationHandler<ClaimAuthorizationRequirement>
{
	protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ClaimAuthorizationRequirement requirement)
		=> context.GetControllerActionDescriptor() switch
		{
			null => Task.CompletedTask,
			var controller => Task.Run(() =>
			{
				var attributes = controller.ControllerTypeInfo.GetCustomAttributes<RequireClaimAttribute>()
					.Concat(controller.MethodInfo.GetCustomAttributes<RequireClaimAttribute>());

				var success = attributes.All(_ => context.User.Any(_.Claim, _.AllowedValues));
				if (success)
					context.Succeed(requirement);
				else
					context.Fail();
			})
		};
}
