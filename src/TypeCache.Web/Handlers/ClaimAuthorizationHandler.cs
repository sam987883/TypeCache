// Copyright (c) 2021 Samuel Abraham

using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using TypeCache.Extensions;
using TypeCache.Web.Attributes;
using TypeCache.Web.Extensions;
using TypeCache.Web.Requirements;

namespace TypeCache.Web.Handlers;

public class ClaimAuthorizationHandler : AuthorizationHandler<ClaimAuthorizationRequirement>
{
	protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, ClaimAuthorizationRequirement requirement)
	{
		var controller = context.GetControllerActionDescriptor();
		if (controller is not null)
		{
			var type = controller.ControllerTypeInfo.GetTypeMember();
			var method = type.GetMethod(controller.MethodInfo.MethodHandle)!;
			var success = type.Attributes
				.OfType<RequireClaimAttribute>()
				.Union(method.Attributes.OfType<RequireClaimAttribute>())
				.All(attribute => attribute!.Claims.Any(pair => context.User.Any(pair.Key, pair.Value)));

			if (success)
				context.Succeed(requirement);
			else
				context.Fail();
		}
		await Task.CompletedTask;
	}
}
