﻿// Copyright (c) 2021 Samuel Abraham

using Microsoft.AspNetCore.Authorization;
using TypeCache.Extensions;
using TypeCache.Utilities;
using TypeCache.Web.Requirements;

namespace TypeCache.Web.Attributes;

/// <summary>
/// 
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
public class RequireClaimAttribute : AuthorizeAttribute
{
	public IDictionary<string, string[]> Claims { get; }

	public RequireClaimAttribute(string[] claims) : base(nameof(ClaimAuthorizationRequirement))
	{
		const char separator = '=';

		claims.ThrowIfEmpty();

		this.Claims = new Dictionary<string, string[]>(claims.Length, StringComparer.OrdinalIgnoreCase);
		claims?.ForEach(claim =>
		{
			(claim.StartsWith(separator) || claim.EndsWith(separator) || claim.Count(c => c.Equals(separator)) > 1).ThrowIfTrue();

			if (claim.Contains(separator))
			{
				(string? key, string? value, IEnumerable<string> _) = claim.Split(separator);
				if (key.IsNotBlank())
				{
					if (this.Claims.TryGetValue(key, out var values))
						this.Claims[key] = [..values, value!];
					else
						this.Claims.Add(key!, [value!]);
				}
			}
			else
				this.Claims.Add(claim, Array<string>.Empty);
		});
	}
}
