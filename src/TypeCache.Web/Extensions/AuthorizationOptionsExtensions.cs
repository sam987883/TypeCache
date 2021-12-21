// Copyright (c) 2021 Samuel Abraham

using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Authorization;
using TypeCache.Collections.Extensions;
using TypeCache.Web.Requirements;
using static TypeCache.Default;

namespace TypeCache.Web.Extensions;

public static class AuthorizationOptionsExtensions
{
	private static AuthorizationPolicy AddAuthorizationPolicy(this AuthorizationOptions @this, IAuthorizationRequirement requirement, params string[]? authenticationSchemas)
	{
		var builder = new AuthorizationPolicyBuilder();
		if (authenticationSchemas.Any())
			builder.AddAuthenticationSchemes(authenticationSchemas);
		builder.RequireAuthenticatedUser();
		builder.AddRequirements(requirement);

		var policy = builder.Build();
		@this.AddPolicy(requirement.GetType().Name, policy);
		return policy;
	}

	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static AuthorizationPolicy AddClaimAuthorizationPolicy(this AuthorizationOptions @this, params string[]? authenticationSchemas)
		=> @this.AddAuthorizationPolicy(new ClaimAuthorizationRequirement(), authenticationSchemas);

	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static AuthorizationPolicy AddHeaderAuthorizationPolicy(this AuthorizationOptions @this, params string[]? authenticationSchemas)
		=> @this.AddAuthorizationPolicy(new HeaderAuthorizationRequirement(), authenticationSchemas);
}
