// Copyright (c) 2021 Samuel Abraham

using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Authorization;
using TypeCache.Web.Requirements;
using static System.Runtime.CompilerServices.MethodImplOptions;

namespace TypeCache.Web.Extensions;

public static class AuthorizationOptionsExtensions
{
	private static AuthorizationPolicy AddAuthorizationPolicy(this AuthorizationOptions @this, IAuthorizationRequirement requirement, params string[]? authenticationSchemas)
	{
		var builder = new AuthorizationPolicyBuilder();
		if (authenticationSchemas?.Any() is true)
			builder.AddAuthenticationSchemes(authenticationSchemas);
		builder.RequireAuthenticatedUser();
		builder.AddRequirements(requirement);

		var policy = builder.Build();
		@this.AddPolicy(requirement.GetType().Name, policy);
		return policy;
	}

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static AuthorizationPolicy AddClaimAuthorizationPolicy(this AuthorizationOptions @this, params string[]? authenticationSchemas)
		=> @this.AddAuthorizationPolicy(new ClaimAuthorizationRequirement(), authenticationSchemas);

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static AuthorizationPolicy AddHeaderAuthorizationPolicy(this AuthorizationOptions @this, params string[]? authenticationSchemas)
		=> @this.AddAuthorizationPolicy(new HeaderAuthorizationRequirement(), authenticationSchemas);
}
