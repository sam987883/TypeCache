// Copyright (c) 2021 Samuel Abraham

using Microsoft.Extensions.DependencyInjection;
using TypeCache.Extensions;

namespace TypeCache.Attributes;

public static class Extensions
{
	extension(IReadOnlySet<Attribute> @this)
	{
		public ServiceLifetime? ServiceLifetime =>
			@this.FirstOrDefault<ServiceLifetimeAttribute>()?.ServiceLifetime;
	}
}
