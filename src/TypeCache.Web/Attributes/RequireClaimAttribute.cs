// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using TypeCache.Collections;
using TypeCache.Collections.Extensions;
using TypeCache.Extensions;
using TypeCache.Web.Requirements;

namespace TypeCache.Web.Attributes;

/// <summary>
/// 
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
public class RequireClaimAttribute : AuthorizeAttribute
{
	public IDictionary<string, string[]> Claims { get; }

	public RequireClaimAttribute(params string[] claims) : base(nameof(ClaimAuthorizationRequirement))
	{
		const char separator = '=';

		claims.AssertNotEmpty();

		this.Claims = new Dictionary<string, string[]>(claims.Length, StringComparer.OrdinalIgnoreCase);
		claims.Do(claim =>
		{
			(claim.StartsWith(separator) || claim.EndsWith(separator) || claim.CountOf(separator) > 1).AssertEquals(false);

			if (claim.Contains(separator))
			{
				(string? key, string? value, IEnumerable<string> _) = claim.Split(separator);

				if (this.Claims.Get(key!).IfFirst(out var values))
					this.Claims[key!] = values.Append(value).ToArray()!;
				else
					this.Claims.Add(key!, new[] { value! });
			}
			else
				this.Claims.Add(claim, Array<string>.Empty);
		});
	}
}
