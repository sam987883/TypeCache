// Copyright (c) 2021 Samuel Abraham

using System.Security.Claims;
using TypeCache.Extensions;

namespace TypeCache.Web.Extensions;

public static class ClaimsPrincipalExtensions
{
	extension(ClaimsPrincipal @this)
	{
		public bool Any(string claimType, string[] values)
		{
			claimType.ThrowIfBlank();

			var claim = @this.FindFirst(claimType); // OrdinalIgnoreCase
			if (claim is null)
				return false;

			return values.Length is 0 || values.Exists(value => value.EqualsIgnoreCase(claim.Value));
		}
	}
}
