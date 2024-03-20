// Copyright (c) 2021 Samuel Abraham

using Microsoft.AspNetCore.Authorization;
using TypeCache.Extensions;
using TypeCache.Web.Requirements;

namespace TypeCache.Web.Extensions;

public static class AuthorizationOptionsExtensions
{
	private static AuthorizationPolicy AddAuthorizationPolicy(this AuthorizationOptions @this, IAuthorizationRequirement requirement, string[]? authenticationSchemas)
	{
		var builder = new AuthorizationPolicyBuilder();
		if (authenticationSchemas?.Any() is true)
			builder.AddAuthenticationSchemes(authenticationSchemas);
		builder.RequireAuthenticatedUser();
		builder.AddRequirements(requirement);

		var policy = builder.Build();
		@this.AddPolicy(requirement.GetType().Name(), policy);
		return policy;
	}

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static AuthorizationPolicy AddClaimAuthorizationPolicy(this AuthorizationOptions @this, string[]? authenticationSchemas)
		=> @this.AddAuthorizationPolicy(new ClaimAuthorizationRequirement(), authenticationSchemas);

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static AuthorizationPolicy AddHeaderAuthorizationPolicy(this AuthorizationOptions @this, string[]? authenticationSchemas)
		=> @this.AddAuthorizationPolicy(new HeaderAuthorizationRequirement(), authenticationSchemas);
}
