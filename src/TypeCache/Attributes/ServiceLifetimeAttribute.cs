// Copyright (c) 2021 Samuel Abraham

using Microsoft.Extensions.DependencyInjection;

namespace TypeCache.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public sealed class ServiceLifetimeAttribute : Attribute
{
	public ServiceLifetimeAttribute(ServiceLifetime serviceLifetime)
	{
		this.ServiceLifetime = serviceLifetime;
	}

	public ServiceLifetime ServiceLifetime { get; }
}
