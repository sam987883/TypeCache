// Copyright (c) 2021 Samuel Abraham

using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using TypeCache.Collections.Extensions;
using TypeCache.Reflection.Extensions;
using TypeCache.Web.Attributes;
using TypeCache.Web.Extensions;
using TypeCache.Web.Requirements;

namespace TypeCache.Web.Handlers
{
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
				var method = controller.MethodInfo.GetMethodMember();
				var headers = this._HttpContextAccessor.HttpContext!.Request.Headers;
				var success = type.Attributes.If<RequireHeaderAttribute>()
					.And(method.Attributes.If<RequireHeaderAttribute>())
					.All(attribue => headers.TryGetValue(attribue.Key, out var values)
						&& (!attribue.AllowedValues.Any() || attribue.AllowedValues.Any(values.Has)));

				if (success)
					context.Succeed(requirement);
				else
					context.Fail();
			}
			await Task.CompletedTask;
		}
	}
}
