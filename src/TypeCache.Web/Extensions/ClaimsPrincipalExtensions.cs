// Copyright (c) 2021 Samuel Abraham

using System.Security.Claims;
using TypeCache.Collections.Extensions;
using TypeCache.Extensions;

namespace TypeCache.Web.Extensions;

public static class ClaimsPrincipalExtensions
{
	public static bool Any(this ClaimsPrincipal @this, string claimType, params string[] values)
	{
		claimType.AssertNotBlank();

		return values.Any()
			? values.Any(value => @this.HasClaim(claimType, value))
			: @this.Claims.Any(claim => claim.Type.Is(claimType));
	}
}
