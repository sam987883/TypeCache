// Copyright (c) 2021 Samuel Abraham

using Microsoft.AspNetCore.Authorization;
using TypeCache.Web.Requirements;

namespace TypeCache.Web.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
public sealed class RequireHeaderAttribute(string header, string[] allowedValues)
	: AuthorizeAttribute(nameof(HeaderAuthorizationRequirement))
{
	public string Header { get; set; } = header;

	public string[] AllowedValues { get; set; } = allowedValues;
}
