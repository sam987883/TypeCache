// Copyright (c) 2021 Samuel Abraham

using System;
using Microsoft.AspNetCore.Authorization;
using TypeCache.Collections;
using TypeCache.Collections.Extensions;
using TypeCache.Extensions;
using TypeCache.Web.Requirements;

namespace TypeCache.Web.Attributes
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
	public class RequireHeaderAttribute : AuthorizeAttribute
	{
		public string[] AllowedValues { get; set; }

		public string Key { get; set; }

		public RequireHeaderAttribute(string key, params string[] allowedValues) : base(nameof(HeaderAuthorizationRequirement))
		{
			key.AssertNotBlank(nameof(key));
			allowedValues.Do(allowedValue => allowedValue.AssertNotBlank(nameof(allowedValue)));

			this.Key = key;
			this.AllowedValues = allowedValues ?? Array<string>.Empty;
		}
	}
}
