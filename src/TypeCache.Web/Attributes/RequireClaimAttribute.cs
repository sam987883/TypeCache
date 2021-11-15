// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using TypeCache.Collections;
using TypeCache.Collections.Extensions;
using TypeCache.Extensions;
using TypeCache.Web.Requirements;

namespace TypeCache.Web.Attributes
{
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

			claims.AssertNotEmpty(nameof(claims));

			this.Claims = new Dictionary<string, string[]>(claims.Length, StringComparer.OrdinalIgnoreCase);
			claims.Do(claim =>
			{
				(claim.StartsWith(separator) || claim.EndsWith(separator) || claim.If(c => c == separator).Count() > 1).Assert(nameof(claim), false);

				if (claim.Contains('='))
				{
					(string? key, string? value, IEnumerable<string> _) = claim.Split(separator);
					var values = this.Claims.Get(key!);

					if (values is not null)
						this.Claims[key!] = values.And(value).ToArray();
					else
						this.Claims.Add(key!, new[] { value! });
				}
				else
					this.Claims.Add(claim, Array<string>.Empty);
			});
		}
	}
}
