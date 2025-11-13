// Copyright (c) 2021 Samuel Abraham

using Microsoft.AspNetCore.Authorization;
using TypeCache.Extensions;
using TypeCache.Web.Requirements;

namespace TypeCache.Web.Extensions;

public static class AuthorizationOptionsExtensions
{
	extension(AuthorizationOptions @this)
	{
		private AuthorizationPolicy AddAuthorizationPolicy(IAuthorizationRequirement requirement, string[]? authenticationSchemas)
		{
			var builder = new AuthorizationPolicyBuilder();
			if (authenticationSchemas?.Length > 0)
				builder.AddAuthenticationSchemes(authenticationSchemas);

			builder.RequireAuthenticatedUser();
			builder.AddRequirements(requirement);

			var policy = builder.Build();
			@this.AddPolicy(requirement.GetType().CodeName, policy);
			return policy;
		}

		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public AuthorizationPolicy AddClaimAuthorizationPolicy(string[]? authenticationSchemas)
			=> @this.AddAuthorizationPolicy(new ClaimAuthorizationRequirement(), authenticationSchemas);

		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public AuthorizationPolicy AddHeaderAuthorizationPolicy(string[]? authenticationSchemas)
			=> @this.AddAuthorizationPolicy(new HeaderAuthorizationRequirement(), authenticationSchemas);
	}
}
