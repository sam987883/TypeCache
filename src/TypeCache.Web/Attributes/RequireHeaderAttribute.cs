// Copyright (c) 2021 Samuel Abraham

using Microsoft.AspNetCore.Authorization;
using TypeCache.Extensions;
using TypeCache.Utilities;
using TypeCache.Web.Requirements;

namespace TypeCache.Web.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
public class RequireHeaderAttribute : AuthorizeAttribute
{
	public string[] AllowedValues { get; set; }

	public string Key { get; set; }

	public RequireHeaderAttribute(string key, string[] allowedValues) : base(nameof(HeaderAuthorizationRequirement))
	{
		key.ThrowIfBlank();
		allowedValues?.ForEach(allowedValue => allowedValue.ThrowIfBlank());

		this.Key = key;
		this.AllowedValues = allowedValues ?? Array<string>.Empty;
	}
}
