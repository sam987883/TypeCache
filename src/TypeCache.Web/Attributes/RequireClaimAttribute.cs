// Copyright (c) 2021 Samuel Abraham

using Microsoft.AspNetCore.Authorization;
using TypeCache.Web.Requirements;

namespace TypeCache.Web.Attributes;

/// <summary>
/// Require claim values for a controller or endpoint.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
public sealed class RequireClaimAttribute(string claim, string[] allowedValues) : AuthorizeAttribute(nameof(ClaimAuthorizationRequirement))
{
	public string Claim { get; } = claim;

	public string[] AllowedValues { get; } = allowedValues;
}
